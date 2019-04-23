using Chip.Net.Controllers;
using Chip.Net.Controllers.Distributed;
using Chip.Net.Providers.Direct;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
    public class BaseDistributedTests<TClient> : BaseControllerTests<INetServerController, TClient> where TClient : class, INetClientController
    {
		private static Dictionary<string, RouterServer<TestRouterModel, TestShardModel, TestUserModel>> routerMap = new Dictionary<string, RouterServer<TestRouterModel, TestShardModel, TestUserModel>>();
		private RouterServer<TestRouterModel, TestShardModel, TestUserModel> Router;

		string guid = null;
		protected virtual NetContext CreateContext() {
			if (guid == null) guid = Guid.NewGuid().ToString();

			NetContext ctx = this.Context;
			ctx.ApplicationName = guid;
			ctx.IPAddress = guid;
			ctx.Port = 0;

			return ctx;
		}

		protected override ServerWrapper StartNewServer() {
			var ctx = CreateContext();
			Router = new RouterServer<TestRouterModel, TestShardModel, TestUserModel>();
			Router.InitializeServer(ctx, new DirectServerProvider(), 0, new	DirectServerProvider(), 1);

			Router.StartShardServer();
			Router.StartUserServer();

			lock (routerMap) {
				routerMap.Add(Router.ShardContext.ApplicationName + "SHARD", Router);
				routerMap.Add(Router.UserContext.ApplicationName + "USER", Router);
			}

			return null;
		}

		protected override ServerWrapper NewServer() {
			return default(ServerWrapper);
		}

		protected override ClientWrapper StartNewClient() {
			var cl = NewClient();
			return null;
		}

		protected override ClientWrapper NewClient() {
			var cl = Activator.CreateInstance<TClient>();
			cl.InitializeClient(CreateContext(), new DirectClientProvider());
			return null;
		}

		protected override void UpdateClient(TClient client) {
			base.UpdateClient(client);
		}

		protected override void UpdateServer(INetServerController server) {
			lock (routerMap) {
				routerMap[server.Context.ApplicationName + "SHARD"].UpdateServer();
				routerMap[server.Context.ApplicationName + "USER"].UpdateServer();
			}
		}
	}
}
