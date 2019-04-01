using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Shards.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Shards {
	public class RouterServer<TUserModel, TShardModel> : IDisposable
		where TUserModel : IUserModel
		where TShardModel : IShardModel {

		public delegate void RouterUserEvent(TUserModel User);
		public delegate void RouterShardEvent(TShardModel Shard);

		public RouterUserEvent UserConnectedEvent { get; set; }
		public RouterUserEvent UserDisconnectedEvent { get; set; }

		public RouterShardEvent ShardConnectedEvent { get; set; }
		public RouterShardEvent ShardDisconnectedEvent { get; set; }

		public INetServer ShardServer { get; set; }
		public INetServer UserServer { get; set; }

		private Dictionary<NetUser, TUserModel> UserMap;
		private Dictionary<NetUser, TShardModel> ShardMap;

		public ModelCollection<TUserModel> Users;
		public ModelCollection<TShardModel> Shards;

		public NetContext Context { get; private set; }

		public bool IsActive { get; private set; }

		public RouterServer(NetContext context, INetServer shardServer, INetServer userServer) {
			this.Context = context;

			ShardMap = new Dictionary<NetUser, TShardModel>();
			UserMap = new Dictionary<NetUser, TUserModel>();

			this.ShardServer = shardServer;
			this.UserServer = userServer;
		}

		public void StartRouter(INetServerProvider shardProvider, INetServerProvider userProvider) {
			if (ShardServer.IsActive)
				throw new ArgumentException("Shard Server is already active.");

			if (UserServer.IsActive)
				throw new ArgumentException("User Server is already active.");

			ShardServer.InitializeServer(Context);
			UserServer.InitializeServer(Context);

			ShardServer.StartServer(shardProvider);
			UserServer.StartServer(userProvider);

			UserServer.OnUserConnected += OnUserConnected;
			UserServer.OnUserDisconnected += OnUserDisconnected;

			ShardServer.OnUserConnected += OnShardConnected;
			ShardServer.OnUserDisconnected += OnShardDisconnected;
		}

		private void OnShardConnected(NetEventArgs args) {
			var model = Activator.CreateInstance<TShardModel>();
			Shards.Add(model);
			ShardMap.Add(args.User, model);

			ShardConnectedEvent?.Invoke(model);
		}

		private void OnShardDisconnected(NetEventArgs args) {
			var model = ShardMap[args.User];
			ShardMap.Remove(args.User);
			Shards.Remove(model);

			ShardDisconnectedEvent?.Invoke(model);
		}

		private void OnUserConnected(NetEventArgs args) {
			var model = Activator.CreateInstance<TUserModel>();
			Users.Add(model);
			UserMap.Add(args.User, model);

			UserConnectedEvent?.Invoke(model);
		}

		private void OnUserDisconnected(NetEventArgs args) {
			var model = UserMap[args.User];
			UserMap.Remove(args.User);
			Users.Remove(model);

			UserDisconnectedEvent?.Invoke(model);
		}

		public void Shutdown() {

		}

		public void Dispose() {
			if (IsActive)
				Shutdown();
		}
	}
}