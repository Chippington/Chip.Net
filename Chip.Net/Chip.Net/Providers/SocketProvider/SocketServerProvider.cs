using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Chip.Net.Data;
using System.Linq;

namespace Chip.Net.Providers.SocketProvider {
    public class SocketServerProvider : SocketProviderBase, INetServerProvider {
		public ProviderEvent OnUserConnected { get; set; }
		public ProviderEvent OnUserDisconnected { get; set; }

		public bool IsActive { get; }
		public bool AcceptIncomingConnections { get; set; }

		private Socket Listener;

		private Queue<Tuple<object, DataBuffer>> Incoming;
		private List<Socket> Clients;

		public void DisconnectUser(object userKey) {
			var client = userKey as Socket;
			client.Shutdown(SocketShutdown.Both);
			client.Close();

			Clients.Remove(client);
			OnUserDisconnected?.Invoke(new ProviderEventArgs() {
				UserKey = client,
			});
		}

		public void Dispose() {
			if (Listener != null)
				StopServer();
		}

		public IEnumerable<object> GetClientKeys() {
			return Clients.AsReadOnly();
		}

		public IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages() {
			var t = new Queue<Tuple<object, DataBuffer>>(Incoming);
			Incoming = new Queue<Tuple<object, DataBuffer>>();
			return t;
		}

		public void SendMessage(DataBuffer data, object excludeKey = null) {
			foreach (var cl in Clients)
				if (cl != excludeKey)
					SendMessage(cl, null, data);
		}

		public void SendMessage(object recipientKey, object excludeKey, DataBuffer data) {
			if (recipientKey == excludeKey)
				return;

			var client = recipientKey as Socket;
			var dataArr = data.ToBytes();
			client.BeginSend(dataArr, 0, dataArr.Length, 0, OnSend, client);
		}

		private void OnSend(IAsyncResult ar) {

		}

		public void StartServer(NetContext context) {
			IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, context.Port);

			Clients = new List<Socket>();
			Incoming = new Queue<Tuple<object, DataBuffer>>();
			Listener = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			Listener.Bind(localEndPoint);
			Listener.Listen(context.MaxConnections > 0 ? context.MaxConnections : 10);
			Listener.BeginAccept(OnSocketBeginAccept, Listener);
		}

		private void OnSocketBeginAccept(IAsyncResult ar) {
			var server = ar.AsyncState as Socket;
			var client = server.EndAccept(ar);
			Listener.BeginAccept(OnSocketBeginAccept, Listener);

			Clients.Add(client);
			OnUserConnected?.Invoke(new ProviderEventArgs() {
				UserKey = client,
			});

			StateObject state = new StateObject();
			state.socket = client;
			client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnSocketRead, state);
		}

		private void OnSocketRead(IAsyncResult ar) {
			var state = ar.AsyncState as StateObject;
			var client = state.socket;

			int bytesRead = client.EndReceive(ar);
			client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnSocketRead, state);

			if(bytesRead > 0) {
				byte[] arr = new byte[bytesRead];
				Array.Copy(state.buffer, arr, bytesRead);

				DataBuffer buffer = new DataBuffer(arr);
				Incoming.Enqueue(new Tuple<object, DataBuffer>(client, buffer));
			}
		}

		public void StopServer() {
			if (Listener != null) {
				Listener.Shutdown(SocketShutdown.Both);
				Listener.Close();
			}

			Listener = null;
		}

		public void UpdateServer() {

		}
	}
}
