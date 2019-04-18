using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;

namespace Chip.Net.Controllers.Basic
{
	public class BasicClient : INetClientController {
		#region INetClient
		public EventHandler<NetEventArgs> OnConnected { get; set; }
		public EventHandler<NetEventArgs> OnDisconnected { get; set; }
		public EventHandler<NetEventArgs> OnPacketReceived { get; set; }
		public EventHandler<NetEventArgs> OnPacketSent { get; set; }

		public PacketRouter Router { get; protected set; }
		public NetContext Context { get; protected set; }
		public bool IsConnected { get; protected set; }
		public bool IsServer { get; set; } = false;
		public bool IsClient { get; set; } = true;

		private INetClientProvider provider;
		private bool disposed;

		public void InitializeClient(NetContext context, INetClientProvider provider) {
			this.provider = provider;
			this.IsConnected = false;
			this.Router = new PacketRouter(null, "");
			this.Context = context;
			disposed = false;

			context.LockContext(client: this);
		}

		public void StartClient() {
			provider.UserConnected += (s, i) => { IsConnected = true; OnConnected?.Invoke(this, new NetEventArgs()); };
			provider.UserDisconnected += (s, i) => { IsConnected = false; OnDisconnected?.Invoke(this, new NetEventArgs()); };

			provider.DataReceived += OnDataReceived;

			Context.Services.StartServices();
			provider.Connect(Context);
		}

		private void OnDataReceived(object sender, ProviderDataEventArgs e) {
			var msg = e.Data;
			if (msg.GetLength() == 0)
				return;

			var buffer = msg;
			var pid = buffer.ReadInt16();
			var router = Router.ReadHeader(buffer);

			var packet = Context.Packets.CreateFromId(pid);
			packet.ReadFrom(buffer);

			router.InvokeClient(packet);
			OnPacketReceived?.Invoke(this, new NetEventArgs() {
				Packet = packet,
			});
		}

		public void StopClient() {
			Context.Services.StopServices();
			provider.Disconnect();
			IsConnected = false;
		}

		public void UpdateClient() {
			Context.Services.UpdateServices();
			OutgoingMessage outgoing = null;
			PacketRouter router = null;

			while (((router, outgoing) = Router.Root.GetNextOutgoing()).Item1 != null) {
				var pid = Context.Packets.GetID(outgoing.Data.GetType());
				DataBuffer buffer = new DataBuffer();
				buffer.Write((Int16)pid);
				router.WriteHeader(buffer);
				outgoing.Data.WriteTo(buffer);
				provider.SendMessage(buffer);
			}

			provider.UpdateClient();
		}

		public void SendPacket(Packet packet) {
			Router.QueueOutgoing(new OutgoingMessage(packet));
		}

		public void Dispose() {
			if (IsConnected)
				StopClient();

			if(disposed == false) {
				disposed = true;
				Context.Services.Dispose();
			}
		}
		#endregion
	}
}
