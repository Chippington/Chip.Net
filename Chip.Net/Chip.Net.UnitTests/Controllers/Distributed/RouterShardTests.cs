﻿using Chip.Net.Controllers.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
    public class RouterShardTests : BaseControllerTests<RouterServer<TestRouterModel, TestShardModel, TestUserModel>, ShardClient<TestRouterModel, TestShardModel, TestUserModel>> {
    }
}
