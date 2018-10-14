using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;

namespace Chip.Net.Default.Basic
{
	public class BasicClient : INetClient, INetService {
		#region INetClient
		public NetEvent OnConnected { get; set; }
		public NetEvent OnDisconnected { get; set; }
		public NetEvent OnPacketReceived { get; set; }
		public NetEvent OnPacketSent { get; set; }

		public PacketRouter Router { get; protected set; }
		public NetContext Context { get; protected set; }
		public bool IsConnected { get; protected set; }

		private INetClientProvider provider;
		private bool disposed;

		public void InitializeClient(NetContext context) {
			this.IsConnected = false;
			this.Router = new PacketRouter();
			this.Context = context;
			disposed = false;

			context.Services.Register<INetService>(this);
			context.Services.InitializeServices(context);
			context.LockContext();
		}

		public void StartClient(INetClientProvider provider) {
			this.provider = provider;
			provider.OnConnected += i => { OnConnected?.Invoke(new NetEventArgs()); };
			provider.OnDisconnected += i => { OnDisconnected?.Invoke(new NetEventArgs()); };

			provider.Connect(Context);
			Context.Services.StartServices();
			IsConnected = provider.IsConnected;
		}

		public void StopClient() {
			Context.Services.StopServices();
			provider.Disconnect();
			IsConnected = false;
		}

		public void UpdateClient() {
			Context.Services.UpdateServices();
			foreach(var svc in Context.Services.Get()) {
				var outgoing = svc.GetOutgoingClientPackets();
				if (outgoing == null)
					continue;

				var sid = Context.Services.GetServiceId(svc);

				foreach(var p in outgoing) {
					var pid = Context.Packets.GetID(p.GetType());
					DataBuffer buffer = new DataBuffer();
					buffer.Write((Int16)pid);
					buffer.Write((byte)sid);
					p.WriteTo(buffer);
					provider.SendMessage(buffer);
				}
			}

			provider.UpdateClient();
			var incoming = provider.GetIncomingMessages();
			foreach (var msg in incoming) {
				var buffer = msg;
				var pid = buffer.ReadInt16();
				var sid = buffer.ReadByte();
				var service = Context.Services.GetServiceFromId(sid);
				var packet = Context.Packets.CreateFromId(pid);
				packet.ReadFrom(buffer);

				service.Router.InvokeClient(packet);
				OnPacketReceived?.Invoke(new NetEventArgs() {
					Packet = packet,
				});
			}
		}

		public void SendPacket(Packet packet) {
			packetQueue.Enqueue(packet);
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

		#region INetService 
		private Queue<Packet> packetQueue;

		public void InitializeService(NetContext context) {
			packetQueue = new Queue<Packet>();
		}

		public void StartService() {
			packetQueue.Clear();
		}

		public void StopService() {

		}

		public void UpdateService() {

		}

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			var ret = packetQueue;
			packetQueue = new Queue<Packet>();
			return ret;
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			return null;
		}
		#endregion
	}
}
