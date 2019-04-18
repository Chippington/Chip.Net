using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;

namespace Chip.Net.Controllers.Basic
{
	public class BasicServer : INetServerController {
		#region INetServer
		public EventHandler<NetEventArgs> NetUserConnected { get; set; }
		public EventHandler<NetEventArgs> NetUserDisconnected { get; set; }
		public EventHandler<NetEventArgs> PacketReceived { get; set; }
		public EventHandler<NetEventArgs> PacketSent { get; set; }

		public NetContext Context { get; protected set; }
		public PacketRouter Router { get; protected set; }

		public bool IsActive { get; protected set; }
		public bool IsServer { get; set; } = true;
		public bool IsClient { get; set; } = false;

		private INetServerProvider provider;
		private Dictionary<object, NetUser> userMap;
		private Queue<Packet> packetQueue;
		private List<NetUser> userList;
		private int nextUserId;
		private bool disposed;

		public virtual void InitializeServer(NetContext context, INetServerProvider provider) {
			this.provider = provider;
			this.Router = new PacketRouter(null, "");
			this.Context = context;
			this.IsActive = false;

			nextUserId = 0;
			disposed = false;
			userMap = new Dictionary<object, NetUser>();
			userList = new List<NetUser>();
			packetQueue = new Queue<Packet>();

			Context.LockContext(server: this);
		}

		public virtual void StartServer() {
			userMap.Clear();
			userList.Clear();
			packetQueue.Clear();

			provider.UserConnected += OnProviderUserConnected;
			provider.UserDisconnected += OnProviderUserDisconnected;

			provider.DataReceived += OnDataReceived;

			provider.StartServer(Context);
			Context.Services.StartServices();
			IsActive = true;
		}

		private void OnDataReceived(object sender, ProviderDataEventArgs e) {
			var buffer = e.Data;
			if (buffer.GetLength() == 0)
				return;

			var user = userMap[e.UserKey];
			var pid = buffer.ReadInt16();
			var router = Router.ReadHeader(buffer);

			var packet = Context.Packets.CreateFromId(pid);
			packet.ReadFrom(buffer);
			packet.Sender = user;

			router.InvokeServer(packet, user);
			PacketReceived?.Invoke(this, new NetEventArgs() {
				User = user,
				Packet = packet,
			});
		}

		private void OnProviderUserConnected(object sender, ProviderUserEventArgs args) {
			var user = new NetUser(args.UserKey, nextUserId);
			userMap[user.UserKey] = user;
			userList.Add(user);

			NetUserConnected?.Invoke(this, new NetEventArgs() {
				User = user,
			});
		}

		private void OnProviderUserDisconnected(object sender, ProviderUserEventArgs args) {
			var user = userMap[args.UserKey];
			NetUserDisconnected?.Invoke(this, new NetEventArgs() {
				User = user,
			});

			userMap.Remove(user.UserKey);
			userList.Remove(user);
		}

		public virtual void UpdateServer() {
			if (IsActive == false)
				return;

			Context.Services.UpdateServices();
			OutgoingMessage outgoing = null;
			PacketRouter router = null;
			foreach (var svc in Context.Services.ServiceList) {
				while (((router, outgoing) = svc.Router.GetNextOutgoing()).Item1 != null) {
					var pid = Context.Packets.GetID(outgoing.Data.GetType());
					DataBuffer buffer = new DataBuffer();
					buffer.Write((Int16)pid);
					router.WriteHeader(buffer);
					outgoing.Data.WriteTo(buffer);

					if (outgoing.Data.Recipient == null) {
						foreach (var user in userList)
							if (user != outgoing.Data.Exclude?.UserKey)
								provider.SendMessage(user.UserKey, buffer);
					} else {
						if (outgoing.Data.Recipient != outgoing.Data.Exclude?.UserKey)
							provider.SendMessage(outgoing.Data.Recipient?.UserKey, buffer);
					}
				}
			}

			Packet p = null;
			while (packetQueue.Count != 0 && (p = packetQueue.Dequeue()) != null) {
				var pid = Context.Packets.GetID(p.GetType());
				DataBuffer buffer = new DataBuffer();
				buffer.Write((Int16)pid);
				Router.WriteHeader(buffer);
				p.WriteTo(buffer);

				if (p.Recipient == null)
				{
					foreach (var user in userList)
						if (user != p.Exclude?.UserKey)
							provider.SendMessage(user.UserKey, buffer);
				}
				else
				{
					if (p.Recipient != p.Exclude?.UserKey)
						provider.SendMessage(p.Recipient?.UserKey, buffer);
				}
			}

			provider.UpdateServer();
		}

		public void StopServer() {
			if (IsActive == false)
				return;

			Context.Services.StopServices();
			provider.StopServer();
			IsActive = false;
		}

		public void Dispose() {
			if (IsActive) {
				StopServer();
			}

			if(disposed == false) {
				disposed = true;
				Context.Services.Dispose();
			}
		}

		public IEnumerable<NetUser> GetUsers() {
			return userList.AsReadOnly();
		}

		public void SendPacket(Packet packet) {
			foreach (var user in userList)
				SendPacket(user, packet);
		}

		public void SendPacket(NetUser user, Packet packet) {
			packet.Recipient = user;
			packetQueue.Enqueue(packet);
		}
		#endregion
	}
}
