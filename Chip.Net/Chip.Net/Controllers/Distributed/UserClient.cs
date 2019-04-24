﻿using System;
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
	public class UserClient<TRouter, TShard, TUser> : BasicClient, INetClientController
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
				}
		}

		private void SetUserModel(IncomingMessage<SetUserModelPacket<TUser>> obj) {
			Model = obj.Data.Model;
			foreach (var svc in Context.Services.ServiceList)
				if (typeof(IDistributedService).IsAssignableFrom(svc.GetType()))
					(svc as IDistributedService).InitializeUser(Model);
		}

		public MessageChannel<T> RouteRouter<T>(string key = null) where T : Packet {
			return Router.Route<T>(key);
		}

		public MessageChannel<PassthroughPacket<T>> RouteShard<T>(string key = null) where T : Packet {
			return Router.Route<PassthroughPacket<T>>(key);
		}
	}
}