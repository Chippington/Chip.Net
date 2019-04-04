using Chip.Net.Controllers.Distributed;
using Chip.Net.Controllers.Distributed.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	public class DistributedServiceBaseTests<TService>
		where TService : IDistributedService
	{
		public RouterServer Router { get; set; }
		public IEnumerable<ShardClient> Shards { get; set; }
		public IEnumerable<UserClient> Users { get; set; }

		[TestInitialize]
		public void Initialize()
		{

		}

		#region Router Server
		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsRouterIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsShardIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsUserIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsServerIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsClientIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToUser_PacketReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_ExcludingOne_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_ExcludingMany_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToShard_PacketReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_ExcludingOne_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_ExcludingMany_PacketsReceived()
		{

		}

		#endregion

		#region Shard Client
		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsRouterIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsShardIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsUserIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsServerIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsClientIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToRouter_PacketReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToUser_PacketReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_ExcludingOne_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_ExcludingMany_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToShard_PacketReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_ExcludingOne_PacketsReceived()
		{

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_ExcludingMany_PacketsReceived()
		{

		}

		#endregion

		#region User Client
		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsRouterIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsShardIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsUserIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsServerIsFalse()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsClientIsTrue()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToRouter_ReceivesPacket()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToShard_ReceivesPacket()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ReceivesPackets()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ExcludingOne_ReceivesPackets()
		{

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ExcludingMany_ReceivesPackets()
		{

		}

		#endregion
	}
}