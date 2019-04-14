﻿using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;

namespace Chip.Net.Controllers.Basic
{
	public class BasicServer : INetServerController {
		#region INetServer
		public EventHandler<NetEventArgs> OnUserConnected { get; set; }
		public EventHandler<NetEventArgs> OnUserDisconnected { get; set; }
		public EventHandler<NetEventArgs> OnPacketReceived { get; set; }
		public EventHandler<NetEventArgs> OnPacketSent { get; set; }

		public NetContext Context { get; protected set; }
		public PacketRouter Router { get; protected set; }

		public bool IsActive { get; protected set; }
		public bool IsServer { get; set; } = true;
		public bool IsClient { get; set; } = false;

		private INetServerProvider provider;
		private Dictionary<object, NetUser> userMap;
		private List<NetUser> userList;
		private int nextUserId;
		private bool disposed;

		public virtual void InitializeServer(NetContext context) {
			this.Router = new PacketRouter();
			this.Context = context;
			this.IsActive = false;

			nextUserId = 0;
			disposed = false;
			userMap = new Dictionary<object, NetUser>();
			userList = new List<NetUser>();

			Context.Services.Register<INetServerController>(this);
			Context.Services.InitializeServices(Context);
			Context.LockContext();
		}

		public virtual void StartServer(INetServerProvider provider) {
			this.provider = provider;
			userMap.Clear();
			userList.Clear();

			provider.OnUserConnected += OnProviderUserConnected;
			provider.OnUserDisconnected += OnProviderUserDisconnected;

			provider.StartServer(Context);
			Context.Services.StartServices();
			IsActive = true;
		}

		private void OnProviderUserConnected(ProviderEventArgs args) {
			var user = new NetUser(args.UserKey, nextUserId);
			userMap[user.UserKey] = user;
			userList.Add(user);

			OnUserConnected?.Invoke(this, new NetEventArgs() {
				User = user,
			});
		}

		private void OnProviderUserDisconnected(ProviderEventArgs args) {
			var user = userMap[args.UserKey];
			OnUserDisconnected?.Invoke(this, new NetEventArgs() {
				User = user,
			});

			userMap.Remove(user.UserKey);
			userList.Remove(user);
		}

		public virtual void UpdateServer() {
			if (IsActive == false)
				return;

			Context.Services.UpdateServices();

			foreach (var svc in Context.Services.Get()) {
				var outgoing = svc.GetOutgoingServerPackets();
				if (outgoing == null)
					continue;

				var sid = Context.Services.GetServiceId(svc);

				foreach (var p in outgoing) {
					var pid = Context.Packets.GetID(p.GetType());
					DataBuffer buffer = new DataBuffer();
					buffer.Write((Int16)pid);
					buffer.Write((byte)sid);
					p.WriteTo(buffer);

					if(p.Recipient == null) {
						foreach(var user in userList)
							if(user != p.Exclude?.UserKey)
								provider.SendMessage(user.UserKey, buffer);
					} else {
						if(p.Recipient != p.Exclude?.UserKey)
							provider.SendMessage(p.Recipient?.UserKey, buffer);
					}
				}
			}

			provider.UpdateServer();
			var incoming = provider.GetIncomingMessages();
			foreach(var msg in incoming) {
				if (msg.Item2.GetLength() == 0)
					continue;

				var user = userMap[msg.Item1];
				var buffer = msg.Item2;
				var pid = buffer.ReadInt16();
				var sid = buffer.ReadByte();
				var service = Context.Services.GetServiceFromId(sid);
				var packet = Context.Packets.CreateFromId(pid);
				packet.ReadFrom(buffer);

				packet.Sender = user;
				service.Router.InvokeServer(packet);
				OnPacketReceived?.Invoke(this, new NetEventArgs() {
					User = user,
					Packet = packet,
				});
			}
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
			return null;
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			var ret = packetQueue;
			packetQueue = new Queue<Packet>();
			return ret;
		}
		#endregion
	}
}
