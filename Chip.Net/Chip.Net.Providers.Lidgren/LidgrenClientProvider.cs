﻿using Chip.Net.Data;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers.Lidgren {
	public class LidgrenClientProvider : INetClientProvider {
		public EventHandler<ProviderUserEventArgs> UserConnected { get; set; }
		public EventHandler<ProviderUserEventArgs> UserDisconnected { get; set; }

		public EventHandler<ProviderDataEventArgs> DataSent { get; set; }
		public EventHandler<ProviderDataEventArgs> DataReceived { get; set; }

		public bool IsConnected => client == null ? false : client.ConnectionStatus == NetConnectionStatus.Connected;

		private NetClient client;

		public void Connect(INetContext context) {
			NetPeerConfiguration config = new NetPeerConfiguration(context.ApplicationName);
			client = new NetClient(config);
			client.Start();
			var nc = client.Connect(context.IPAddress, context.Port);
		}

		public void Dispose() {
			if (client != null)
				Disconnect();
		}

		public void SendMessage(DataBuffer data) {
			if (client != null) {
				NetOutgoingMessage outmsg = client.CreateMessage();
				outmsg.Write(data.ToBytes());
				client.SendMessage(outmsg, NetDeliveryMethod.ReliableSequenced);

				DataSent?.Invoke(this, new ProviderDataEventArgs(null, false, data, data.GetLength()));
			}
		}

		public void UpdateClient() {
			NetIncomingMessage inc = null;
			while (client != null && (inc = client.ReadMessage()) != null) {
				switch (inc.MessageType) {
					case NetIncomingMessageType.StatusChanged:
						switch (inc.SenderConnection.Status) {
							case NetConnectionStatus.Connected:
								UserConnected?.Invoke(this, new ProviderUserEventArgs());
								break;

							case NetConnectionStatus.Disconnected:
								UserDisconnected?.Invoke(this, new ProviderUserEventArgs());
								break;
						}
						break;

					case NetIncomingMessageType.Data:
						var bytes = inc.ReadBytes(inc.LengthBytes);
						DataReceived?.Invoke(this, new ProviderDataEventArgs(null, true, new DataBuffer(bytes), inc.LengthBytes));
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
				client = null;

				this.UserDisconnected?.Invoke(this, new ProviderUserEventArgs());
			}
		}
	}
}
