﻿using Chip.Net.Controllers.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
    public class RouterShardTests : BaseControllerTests<RouterServer<RouterModel, ShardModel, UserModel>, ShardClient<RouterModel, ShardModel, UserModel>> {
    }
}
