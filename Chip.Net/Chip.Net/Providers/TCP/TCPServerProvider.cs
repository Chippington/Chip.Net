using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.TCP
{
	public class TCPServerProvider : TCPProviderBase, INetServerProvider {
		public ProviderEvent OnUserConnected { get; set; }
		public ProviderEvent OnUserDisconnected { get; set; }
		public bool AcceptIncomingConnections { get; set; }
		public bool IsActive { get; private set; }

		private HashSet<TcpClient> connected;
		private Queue<Tuple<object, DataBuffer>> incoming;
		private List<TcpClient> clientList;
		private TcpListener clientListener;
		private byte[] message;

		public void StartServer(NetContext context) {
			clientListener = new TcpListener(IPAddress.Any, context.Port);
			clientListener.Start(context.MaxConnections);

			//clientList = new List<ClientInfo>();
			message = new byte[1024 * 512];
			clientList = new List<TcpClient>();
			incoming = new Queue<Tuple<object, DataBuffer>>();
			connected = new HashSet<TcpClient>();

			IsActive = true;
		}

		public void UpdateServer() {
			if (IsActive == false)
				return;

			//If any clients are waiting to connect
			if (clientListener.Pending()) {
				TcpClient newClient = clientListener.AcceptTcpClient();
				newClient.SendBufferSize = newClient.ReceiveBufferSize = 1024 * 256;
				newClient.NoDelay = false;

				connected.Add(newClient);
				clientList.Add(newClient);

				//Delegate
				OnUserConnected?.Invoke(new ProviderEventArgs() {
					UserKey = newClient,
				});
			}

			if (clientList == null)
				return;

			//Update each client, listening for any incoming data.
			for (int i = clientList.Count - 1; i >= 0; i--) {
				TcpClient tcpClient = clientList[i];

				//If the client is no longer connected, fire off a disconnection event delegate
				if (!tcpClient.Connected) {
					DisconnectUser(tcpClient);
					continue;
				}

				//Attempt to read any incoming data from this client
				int bytesRead = 0;
				int totalBytesRead = 0;
				NetworkStream messageStream = tcpClient.GetStream();
				MemoryStream memStream = new MemoryStream();

				if (messageStream.DataAvailable) {
					var buff = new DataBuffer();
					while (messageStream.DataAvailable) {
						bytesRead = messageStream.Read(message, 0, message.Length);
						byte[] data = new byte[bytesRead];
						Array.Copy(message, data, bytesRead);
						buff.Write((byte[])data, 0, data.Length);
						totalBytesRead += bytesRead;
					}

					buff.Seek(0);
					while (buff.GetPosition() < buff.GetLength()) {
						var size = buff.ReadInt16();
						byte[] d = new byte[size];
						buff.ReadBytes(d, 0, d.Length);

						d = Decompress(d);
						if (d.Length == 1 && d[0] == 0) {
							DisconnectUser(tcpClient);
							break;
						}

						DataBuffer msgBuffer = new DataBuffer(d);
						msgBuffer.Seek(0);

						//Delegate that other classes can attach to, mainly the network handler
						incoming.Enqueue(new Tuple<object, DataBuffer>(
							tcpClient, msgBuffer));
					}
				}
			}
		}

		public void StopServer() {
			Dispose();
		}

		public IEnumerable<object> GetClientKeys() {
			return clientList.AsReadOnly();
		}

		public void DisconnectUser(object userKey) {
			var tcpClient = userKey as TcpClient;
			if(tcpClient != null && connected.Contains(tcpClient)) {
				connected.Remove(tcpClient);
				byte[] arr = new byte[1] { 0 };
				try {
					SendMessage(tcpClient, new DataBuffer(arr));
				} catch { }

				OnUserDisconnected?.Invoke(new ProviderEventArgs() {
					UserKey = tcpClient,
				});

				tcpClient.Close();
				clientList.Remove(tcpClient);
			}
		}

		public IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages() {
			var ret = incoming;
			incoming = new Queue<Tuple<object, DataBuffer>>();
			return ret;
		}

		public void SendMessage(DataBuffer packet, object excludeKey = null) {
			for (int i = clientList.Count - 1; i >= 0; i--)
				if(clientList[i] != excludeKey)
					SendMessage(clientList[i], packet);
		}

		public void SendMessage(object recipientKey, DataBuffer buffer) {
			var msg = Compress(buffer.ToBytes());

			try {
				var tcpClient = recipientKey as TcpClient;
				tcpClient.GetStream().Write(BitConverter.GetBytes((Int16)msg.Length), 0, 2);
				tcpClient.GetStream().Write(msg, 0, msg.Length);
				tcpClient.GetStream().Flush();
			} catch(Exception ex) {
				if(connected.Contains(recipientKey as TcpClient))
					DisconnectUser(recipientKey);
			}
		}

		public void Dispose() {
			for (int i = clientList.Count - 1; i >= 0; i--) {
				DisconnectUser(clientList[i]);
			}

			clientListener.Stop();
			clientListener = null;
			clientList = null;
			connected = null;
			message = null;
			IsActive = false;
		}
	}
}
