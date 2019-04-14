using Chip.Net.Controllers.Distributed;
using Chip.Net.Providers.Direct;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
    public class RouterShardTests : BaseControllerTests<RouterServer<TestRouterModel, TestShardModel, TestUserModel>, ShardClient<TestRouterModel, TestShardModel, TestUserModel>> {
		protected override INetServerController StartNewServer() {
			var sv = NewServer() as RouterServer<TestRouterModel, TestShardModel, TestUserModel>;
			sv.StartShardServer(new DirectServerProvider());
			return sv;
		}
	}
}
