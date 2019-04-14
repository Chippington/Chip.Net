using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed {
	public class UserClient<TRouter, TShard, TUser> : INetClientController
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {

		public EventHandler<NetEventArgs> OnConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public EventHandler<TUser> UserConfiguredEvent { get; set; }
		public EventHandler<TShard> AssignedToShardEvent { get; set; }
		public EventHandler<TShard> UnassignedFromShardEvent { get; set; }
		public EventHandler<TRouter> ConnectedToRouterEvent { get; set; }
		public EventHandler<TRouter> DisconnectedFromRouterEvent { get; set; }

		public TRouter CurrentRouter { get; private set; }
		public IReadOnlyList<TShard> Shards { get; private set; }

		public NetContext Context => throw new NotImplementedException();

		public bool IsConnected => throw new NotImplementedException();

		public PacketRouter Router => throw new NotImplementedException();

		public bool IsServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Dispose() {
			throw new NotImplementedException();
		}

		public void InitializeClient(NetContext context) {
			throw new NotImplementedException();
		}

		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void StartClient(INetClientProvider provider) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopClient() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateClient() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}

		public void SendToRouter(Packet Pack) {

		}

		public void SendToShard(TShard Shard, Packet Pack) {

		}

		public void SendToShards(Packet Pack) {

		}

		public void SendToShards(Packet Pack, TShard Exclude) {

		}

		public void SendToShards(Packet Pack, IEnumerable<TShard> Exclude) {

		}

		public Packet GetNextOutgoingPacket() {
			throw new NotImplementedException();
		}
	}
}