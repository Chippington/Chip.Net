using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Providers.Direct;

namespace Chip.Net.Controllers.Distributed
{
	public class RouterServer<TRouter, TShard, TUser> : INetServerController
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

		public bool IsServer { get; set; }
		public bool IsClient { get; set; }

		private INetServerProvider ShardProvider;
		private INetServerProvider UserProvider;

		private Dictionary<object, NetUser> netUserMap;
		private Dictionary<NetUser, TShard> shardMap;
		private Dictionary<NetUser, TUser> userMap;
		private Dictionary<uint, TShard> shardIdMap;
		private Dictionary<uint, TUser> userIdMap;

		private List<NetUser> netUserList;
		private List<TShard> shardList;
		private List<TUser> userList;

		private uint nextNetUserId;
		private uint nextShardId;
		private uint nextUserId;

		private Queue<uint> availableShardIds;
		private Queue<uint> availableUserIds;

		private string modelKey = "RouterModel";

		public void InitializeServer(NetContext context) {
			availableShardIds = new Queue<uint>();
			availableUserIds = new Queue<uint>();
			nextShardId = 1;
			nextUserId = 1;

			netUserMap = new Dictionary<object, NetUser>();
			shardIdMap = new Dictionary<uint, TShard>();
			userIdMap = new Dictionary<uint, TUser>();
			shardMap = new Dictionary<NetUser, TShard>();
			userMap = new Dictionary<NetUser, TUser>();

			netUserList = new List<NetUser>();
			shardList = new List<TShard>();
			userList = new List<TUser>();

			Shards = shardList.AsReadOnly();
			Users = userList.AsReadOnly();
		}

		public void UpdateServer() {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void SendPacket(NetUser user, Packet packet) {
			throw new NotImplementedException();
		}

		public void StartServer(INetServerProvider provider) {
			throw new Exception("Router server cannot be started normally. Use StartShardServer and StartUserServer instead.");
		}

		public void StartShardServer(INetServerProvider shardProvider) {
			this.ShardProvider = shardProvider;
			ShardProvider.StartServer(Context);

			ShardProvider.UserConnected += OnShardConnected;
			ShardProvider.UserDisconnected += OnShardDisconnected;
			ShardProvider.DataReceived += OnShardDataReceived;
		}

		private void OnShardDataReceived(object sender, ProviderDataEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnShardConnected(object sender, ProviderUserEventArgs e) {
			var shard = Activator.CreateInstance<TShard>();
			if(availableShardIds.Count != 0) {
				shard.Id = availableShardIds.Dequeue();
			} else {
				shard.Id = nextShardId++;
			}

			var netUser = new NetUser(e.UserKey, (int)nextNetUserId++);
			netUserList.Add(netUser);
			netUserMap.Add(e.UserKey, netUser);
			NetUserConnected?.Invoke(this, new NetEventArgs() { User = netUser });

			shardList.Add(shard);
			shardMap.Add(netUser, shard);
			shardIdMap.Add(shard.Id, shard);
			ShardConnectedEvent?.Invoke(this, shard);
		}

		private void OnShardDisconnected(object sender, ProviderUserEventArgs e) {
			var netUser = netUserMap[e.UserKey];
			var shard = shardMap[netUser];
			shardList.Remove(shard);
			shardMap.Remove(netUser);
			shardIdMap.Remove(shard.Id);

			ShardDisconnectedEvent?.Invoke(this, shard);
			NetUserDisconnected?.Invoke(this, new NetEventArgs() { User = netUser });
		}

		public void StartUserServer(INetServerProvider userProvider) {
			this.UserProvider = userProvider;
			UserProvider.StartServer(Context);

			UserProvider.UserConnected += OnUserConnected;
			UserProvider.UserDisconnected += OnUserDisconnected;
			UserProvider.DataReceived += OnUserDataReceived;
		}

		private void OnUserDataReceived(object sender, ProviderDataEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnUserConnected(object sender, ProviderUserEventArgs e) {
			var user = Activator.CreateInstance<TUser>();
			if (availableShardIds.Count != 0) {
				user.Id = availableShardIds.Dequeue();
			} else {
				user.Id = nextShardId++;
			}

			var netUser = new NetUser(e.UserKey, (int)nextNetUserId++);
			netUserList.Add(netUser);
			netUserMap.Add(e.UserKey, netUser);
			NetUserConnected?.Invoke(this, new NetEventArgs() { User = netUser });

			userList.Add(user);
			userMap.Add(netUser, user);
			userIdMap.Add(user.Id, user);
			UserConnectedEvent?.Invoke(this, user);
		}

		private void OnUserDisconnected(object sender, ProviderUserEventArgs e) {
			var netUser = netUserMap[e.UserKey];
			var user = userMap[netUser];
			userList.Remove(user);
			userMap.Remove(netUser);
			userIdMap.Remove(user.Id);

			UserDisconnectedEvent?.Invoke(this, user);
			NetUserDisconnected?.Invoke(this, new NetEventArgs() { User = netUser });
		}

		public IEnumerable<NetUser> GetUsers() {
			throw new Exception("Invalid method on Router server.");
			return netUserList.AsReadOnly();
		}

		public void StopServer() {
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
		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}

		public Packet GetNextOutgoingPacket() {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}
	}
}
