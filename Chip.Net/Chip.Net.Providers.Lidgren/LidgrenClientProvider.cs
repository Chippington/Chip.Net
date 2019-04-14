using Chip.Net.Data;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers.Lidgren {
	public class LidgrenClientProvider : INetClientProvider {
		public EventHandler<ProviderEventArgs> OnConnected { get; set; }
		public EventHandler<ProviderEventArgs> OnDisconnected { get; set; }

		public bool IsConnected => client == null ? false : client.ConnectionStatus == NetConnectionStatus.Connected;

		private NetClient client;
		private Queue<DataBuffer> incoming;

		public void Connect(NetContext context) {
			incoming = new Queue<DataBuffer>();

			NetPeerConfiguration config = new NetPeerConfiguration(context.ApplicationName);
			client = new NetClient(config);
			client.Start();
			client.Connect(context.IPAddress, context.Port);
		}

		public void Dispose() {
			if (client != null)
				Disconnect();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages() {
			var ret = incoming;
			incoming = new Queue<DataBuffer>();
			return ret;
		}

		public void SendMessage(DataBuffer data) {
			if (client != null) {
				NetOutgoingMessage outmsg = client.CreateMessage();
				outmsg.Write(data.ToBytes());
				client.SendMessage(outmsg, NetDeliveryMethod.ReliableSequenced);
			}
		}

		public void UpdateClient() {
			NetIncomingMessage inc = null;
			while (client != null && (inc = client.ReadMessage()) != null) {
				switch (inc.MessageType) {
					case NetIncomingMessageType.StatusChanged:
						switch (inc.SenderConnection.Status) {
							case NetConnectionStatus.Connected:
								OnConnected?.Invoke(this, new ProviderEventArgs());
								break;

							case NetConnectionStatus.Disconnected:
								OnDisconnected?.Invoke(this, new ProviderEventArgs());
								break;
						}
						break;

					case NetIncomingMessageType.Data:
						var bytes = inc.ReadBytes(inc.LengthBytes);
						incoming.Enqueue(new DataBuffer(bytes));
						break;

					case NetIncomingMessageType.Error:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						Console.WriteLine(DateTime.Now.ToShortTimeString() + "Lidgren Client" + inc.ReadString());
						break;
				}
			}
		}

		public void Disconnect() {
			if (client != null) {
				client.Disconnect("Disconnecting");
				client.Shutdown("Disconnecting");
				while (IsConnected) {
					UpdateClient();
					System.Threading.Thread.Sleep(10);
				}
				//client = null;
			}
		}
	}
}
