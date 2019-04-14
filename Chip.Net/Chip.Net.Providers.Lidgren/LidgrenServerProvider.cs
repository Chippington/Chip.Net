using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Lidgren.Network;

namespace Chip.Net.Providers.Lidgren {
	public class LidgrenServerProvider : INetServerProvider {
		public EventHandler<ProviderUserEventArgs> UserConnected { get; set; }
		public EventHandler<ProviderUserEventArgs> UserDisconnected { get; set; }


		public EventHandler<ProviderDataEventArgs> DataSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<ProviderDataEventArgs> DataReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool AcceptIncomingConnections { get; set; }
		public bool IsActive { get; private set; }

		private NetServer server;
		private List<NetConnection> connections;
		private Queue<Tuple<object, DataBuffer>> incoming;

		public void StartServer(NetContext context) {
			incoming = new Queue<Tuple<object, DataBuffer>>();

			NetPeerConfiguration config = new NetPeerConfiguration(context.ApplicationName);
			config.Port = context.Port;
			config.MaximumConnections = context.MaxConnections;

			connections = new List<NetConnection>();
			server = new NetServer(config);
			server.Start();
			IsActive = true;
		}

		public void UpdateServer() {
			NetIncomingMessage inc = null;
			while (server != null && (inc = server.ReadMessage()) != null) {
				switch (inc.MessageType) {
					case NetIncomingMessageType.StatusChanged:
						Console.WriteLine(inc.SenderEndPoint.ToString() + " Status: " + inc.SenderConnection.Status.ToString());
						switch (inc.SenderConnection.Status) {
							case NetConnectionStatus.Connected:
								connections.Add(inc.SenderConnection);
								UserConnected?.Invoke(this, new ProviderUserEventArgs() {
									UserKey = inc.SenderConnection,
								});
								break;

							case NetConnectionStatus.Disconnected:
								connections.Remove(inc.SenderConnection);
								UserDisconnected?.Invoke(this, new ProviderUserEventArgs() {
									UserKey = inc.SenderConnection,
								});
								break;
						}
						break;

					case NetIncomingMessageType.Data:
						var bytes = inc.ReadBytes(inc.LengthBytes);
						incoming.Enqueue(new Tuple<object, DataBuffer>(inc.SenderConnection, new DataBuffer(bytes)));
						break;

					case NetIncomingMessageType.Error:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						Console.WriteLine(DateTime.Now.ToShortTimeString() + "Lidgren Server" + inc.ReadString());
						break;
				}
			}
		}

		public IEnumerable<object> GetClientKeys() {
			return connections;
		}

		public IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages() {
			var ret = incoming;
			incoming = new Queue<Tuple<object, DataBuffer>>();
			return ret;
		}

		public void SendMessage(DataBuffer data, object excludeKey = null) {
			if (server == null)
				return;

			var bytes = data.ToBytes();
			foreach (var client in connections) {
				if (client == excludeKey)
					continue;

				NetOutgoingMessage outmsg = server.CreateMessage();
				outmsg.Write(bytes);
				client.SendMessage(outmsg, NetDeliveryMethod.ReliableSequenced, 0);
			}
		}

		public void SendMessage(object recipientKey, DataBuffer data) {
			if (server == null)
				return;

			var client = recipientKey as NetConnection;
			if (client == null) {
				throw new ArgumentException("Recipient key cannot be null");
			}

			var bytes = data.ToBytes();
			NetOutgoingMessage outmsg = server.CreateMessage();
			outmsg.Write(bytes);
			client.SendMessage(outmsg, NetDeliveryMethod.ReliableSequenced, 0);
		}

		public void DisconnectUser(object userKey) {
			var connection = userKey as NetConnection;
			if (connection != null) {
				connection.Disconnect("Disconnected by server");
			}
		}

		public void StopServer() {
			if (server != null) {
				foreach(var connection in connections) {
					UserDisconnected?.Invoke(this, new ProviderUserEventArgs() {
						UserKey = connection,
					});
				}

				server.Shutdown("Server shutting down");
				IsActive = false;
				server = null;
			}
		}

		public void Dispose() {
			if (server != null)
				StopServer();
		}
	}
}
