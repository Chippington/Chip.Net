using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Basic;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.ModelTracking;
using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Providers.Direct;

namespace Chip.Net.Controllers.Distributed
{
	public class RouterServer<TRouter, TShard, TUser>
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {

		public EventHandler<TRouter> RouterConfiguredEvent { get; set; }
		public EventHandler<TShard> ShardDisconnectedEvent { get; set; }
		public EventHandler<TShard> ShardConnectedEvent { get; set; }
		public EventHandler<TUser> UserDisconnectedEvent { get; set; }
		public EventHandler<TUser> UserConnectedEvent { get; set; }

		public EventHandler<ShardDataEventArgs> ShardDataReceivedEvent { get; set; }
		public EventHandler<UserDataEventArgs> UserDataReceivedEvent { get; set; }
		public EventHandler<ShardDataEventArgs> ShardDataSentEvent { get; set; }
		public EventHandler<UserDataEventArgs> UserDataSentEvent { get; set; }

		public struct ShardDataEventArgs {
			public TShard Shard { get; set; }
			public Packet Data { get; set; }
		}

		public struct UserDataEventArgs {
			public TUser Shard { get; set; }
			public Packet Data { get; set; }
		}

		public IReadOnlyList<TShard> Shards { get; private set; }
		public IReadOnlyList<TUser> Users { get; private set; }

		public PacketRouter Router { get; private set; }
		public NetContext Context { get; private set; }
		public bool IsActive { get; private set; }

		public BasicServer ShardController { get; private set; }
		public BasicServer UserController { get; private set; }

		public NetContext ShardContext { get; private set; }
		public NetContext UserContext { get; private set; }

		private ModelTrackerCollection<TShard> shards;

		private List<TShard> shardList;
		private List<TUser> userList;

		private Dictionary<uint, TShard> shardMap;
		private Dictionary<uint, TUser> userMap;

		public void InitializeServer(int shardPort, int userPort, NetContext context) {
			ShardContext = context.Clone();
			UserContext = context.Clone();

			ShardContext.Port = shardPort;
			UserContext.Port = userPort;

			ShardController = new BasicServer();
			ShardController.InitializeServer(ShardContext);
			ShardController.NetUserConnected += OnShardConnected;
			ShardController.NetUserDisconnected += OnShardDisconnected;
			ShardController.PacketReceived += OnShardDataReceived;

			UserController = new BasicServer();
			UserController.InitializeServer(UserContext);
			UserController.NetUserConnected += OnUserConnected;
			UserController.NetUserDisconnected += OnUserDisconnected;
			UserController.PacketReceived += OnUserDataReceived;

			shardList = new List<TShard>();
			userList = new List<TUser>();

			Shards = shardList.AsReadOnly();
			Users = userList.AsReadOnly();

			shardMap = new Dictionary<uint, TShard>();
			userMap = new Dictionary<uint, TUser>();
		}

		#region Event Handling
		private void OnShardConnected(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnShardDisconnected(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnShardDataReceived(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnUserConnected(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnUserDisconnected(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnUserDataReceived(object sender, NetEventArgs e) {
			throw new NotImplementedException();
		}
		#endregion

		public void StartShardServer(INetServerProvider shardProvider) {
			ShardController.StartServer(shardProvider);
		}

		public void StartUserServer(INetServerProvider userProvider) {
			UserController.StartServer(userProvider);
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
