using Chip.Net.Controllers;
using Chip.Net.Controllers.Distributed;
using Chip.Net.Providers.Direct;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
    public class RouterShardTests : BaseDistributedTests<ShardClient<TestRouterModel, TestShardModel, TestUserModel>> {
		protected override ServerWrapper NewServer() {
			return null;
		}
	}
}
