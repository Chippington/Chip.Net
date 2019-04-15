using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Providers.Direct;

namespace Chip.Net.Controllers.Distributed
{
	public class RouterServer<TRouter, TShard, TUser>
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {

		public EventHandler<NetEventArgs> NetUserConnected { get; set; }
		public EventHandler<NetEventArgs> NetUserDisconnected { get; set; }

		public EventHandler<NetEventArgs> PacketReceived { get; set; }
		public EventHandler<NetEventArgs> PacketSent { get; set; }

		public EventHandler<TRouter> RouterConfiguredEvent { get; set; }
		public EventHandler<TShard> ShardDisconnectedEvent { get; set; }
		public EventHandler<TShard> ShardConnectedEvent { get; set; }
		public EventHandler<TUser> UserDisconnectedEvent { get; set; }
		public EventHandler<TUser> UserConnectedEvent { get; set; }

		public IReadOnlyList<TShard> Shards { get; private set; }
		public IReadOnlyList<TUser> Users { get; private set; }

		public PacketRouter Router { get; private set; }
		public NetContext Context { get; private set; }
		public bool IsActive { get; private set; }

		public INetServerController ShardController { get; private set; }
		public INetServerController UserController { get; private set; }

		public void InitializeServer(NetContext context) {

		}

		public void StartShardServer(int port, INetServerProvider shardProvider) {
			
		}

		public void StartUserServer(int port, INetServerProvider userProvider) {

		}

		public void UpdateServer()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<NetUser> GetUsers() {
			throw new NotImplementedException();
		}

		public void Shutdown() {
			throw new NotImplementedException();
		}

		public void SendToShard(TShard Shard, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, TShard Exclude) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, IEnumerable<TShard> Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUser(TUser User, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, TUser Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, IEnumerable<TUser> Exclude) {
			throw new NotImplementedException();
		}
		public void Dispose() {
			throw new NotImplementedException();
		}
	}
}
