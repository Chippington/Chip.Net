using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Basic;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Packets;
using Chip.Net.Controllers.Distributed.Services;
using Chip.Net.Controllers.Distributed.Services.ModelTracking;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed {
	public class UserClient : BasicClient {

	}

	public class UserClient<TRouter, TShard, TUser> : UserClient, INetClientController
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {

		public TUser Model { get; private set; }

		public override void InitializeClient(NetContext context, INetClientProvider provider) {
			context.Packets.Register<SetShardModelPacket<TShard>>();
			context.Packets.Register<SetUserModelPacket<TUser>>();
			context.Packets.Register<SendToShardPacket>();
			context.Packets.Register<SendToUserPacket>();

			context.Services.Register<ModelTrackerService<TShard>>();
			context.Services.Register<ModelTrackerService<TUser>>();

			base.InitializeClient(context, provider);
			foreach (var svc in Context.Services.ServiceList)
				if (typeof(IDistributedService).IsAssignableFrom(svc.GetType())) {
					(svc as IDistributedService).IsClient = true;
					(svc as IDistributedService).IsUser = true;
					(svc as IDistributedService).UserController = this;
					(svc as IDistributedService).InitializeUser();
				}
		}

		private void SetUserModel(IncomingMessage<SetUserModelPacket<TUser>> obj) {
			Model = obj.Data.Model;
			
		}

		public MessageChannel<T> RouteRouter<T>(string key = null) where T : Packet {
			return Router.Route<T>(key);
		}

		public DistributedChannel<T> RouteShard<T>(string key = null) where T : Packet {
			return new DistributedChannel<T>(Router.Route<PassthroughPacket<T>>(key));
		}
	}
}