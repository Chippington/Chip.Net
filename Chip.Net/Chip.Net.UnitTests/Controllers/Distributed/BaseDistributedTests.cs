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

		protected virtual NetContext CreateContext() {
			NetContext ctx = new NetContext();
			ctx.ApplicationName = Guid.NewGuid().ToString();
			ctx.IPAddress = "localhost";
			ctx.Port = 0;

			return ctx;
		}

		protected override INetServerController StartNewServer() {
			var ctx = CreateContext();
			var sv = NewServer() as RouterServer<TestRouterModel, TestShardModel, TestUserModel>;
			sv.InitializeServer(0, 1, ctx);

			sv.StartShardServer(new DirectServerProvider());
			sv.StartUserServer(new DirectServerProvider());

			lock (routerMap)
				routerMap.Add(sv.Context.ApplicationName, sv);

			return sv.ShardController;
		}

		protected override INetClientController StartNewClient() {
			var cl = NewClient();
			cl.InitializeClient(CreateContext());
			cl.StartClient(new DirectClientProvider());
			return cl;
		}

		protected override INetClientController NewClient() {
			return base.NewClient();
		}

		protected override void UpdateClient(TClient client) {
			base.UpdateClient(client);
		}

		protected override void UpdateServer(INetServerController server) {
			lock (routerMap)
				routerMap[server.Context.ApplicationName].UpdateServer();
		}
	}
}
