﻿using Chip.Net.Controllers.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
    public class RouterUserTests : BaseControllerTests<RouterServer<RouterModel, ShardModel, UserModel>, UserClient<RouterModel, ShardModel, UserModel>> {
	}
}
