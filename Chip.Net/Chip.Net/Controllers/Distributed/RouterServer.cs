using System;
using System.Collections.Generic;
using System.Linq;
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
			public TUser User { get; set; }
			public Packet Data { get; set; }
		}

		public PacketRouter Router { get; private set; }
		public NetContext Context { get; private set; }
		public bool IsActive { get; private set; }

		public BasicServer ShardController { get; private set; }
		public BasicServer UserController { get; private set; }

		public NetContext ShardContext { get; private set; }
		public NetContext UserContext { get; private set; }

		public ModelTrackerCollection<TShard> Shards { get; private set; }
		public ModelTrackerCollection<TUser> Users { get; private set; }

		private readonly string UK_Shard = "Router-ShardConnection";
		private readonly string UK_User = "Router-UserConnection";

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

			Shards = new ModelTrackerCollection<TShard>();
			Users = new ModelTrackerCollection<TUser>();
		}

		#region Event Handling
		private void OnShardConnected(object sender, NetEventArgs e) {
			var shard = Activator.CreateInstance<TShard>();
			Shards.Add(shard);

			SetShard(e.User, shard);
			ShardConnectedEvent?.Invoke(this, shard);
		}

		private void OnShardDisconnected(object sender, NetEventArgs e) {
			var shard = GetShard(e.User);
			Shards.Remove(shard);

			ShardDisconnectedEvent?.Invoke(this, shard);
		}

		private void OnShardDataReceived(object sender, NetEventArgs e) {
			var shard = GetShard(e.User);

			ShardDataReceivedEvent?.Invoke(this, new ShardDataEventArgs()
			{
				Data = e.Packet,
				Shard = shard
			});
		}

		private void OnUserConnected(object sender, NetEventArgs e) {
			var user = Activator.CreateInstance<TUser>();
			Users.Add(user);

			SetUserModel(e.User, user);
			UserConnectedEvent?.Invoke(this, user);
		}

		private void OnUserDisconnected(object sender, NetEventArgs e) {
			var user = GetUserModel(e.User);
			Users.Remove(user);

			UserDisconnectedEvent?.Invoke(this, user);
		}

		private void OnUserDataReceived(object sender, NetEventArgs e) {
			var user = GetUserModel(e.User);

			UserDataReceivedEvent?.Invoke(this, new UserDataEventArgs()
			{
				Data = e.Packet,
				User = user,
			});
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
			if (ShardController != null && ShardController.IsActive) ShardController.UpdateServer();
			if (UserController != null && UserController.IsActive) UserController.UpdateServer();
		}

		private void SetShard(NetUser user, TShard shardModel)
		{
			user.SetLocal<TShard>(UK_Shard, shardModel);
		}

		public TShard GetShard(NetUser user)
		{
			return user.GetLocal<TShard>(UK_Shard);
		}

		private void SetUserModel(NetUser user, TUser userModel)
		{
			user.SetLocal<TUser>(UK_User, userModel);
		}

		public TUser GetUserModel(NetUser user)
		{
			return user.GetLocal<TUser>(UK_User);
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

		public void Shutdown()
		{
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}
	}
}
