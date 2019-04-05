using Chip.Net.Controllers.Distributed;
using Chip.Net.Controllers.Distributed.Services;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
	public class DistributedServiceInitializationTests {
		public RouterServer Router { get; set; }
		public ShardClient Shard { get; set; }
		public UserClient User { get; set; }


		[TestInitialize]
		public void Initialize() {
			Router = new RouterServer();
			Shard = new ShardClient();
			User = new UserClient();
		}

		#region Router Server

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsRouterIsTrue() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsShardIsFalse() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsUserIsFalse() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsServerIsTrue() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsClientIsFalse() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToUser_PacketReceived() {

		}
		#endregion

		#region Shard Client

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsRouterIsFalse() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsShardIsTrue() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsUserIsFalse() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsServerIsFalse() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsClientIsTrue() {

		}

		#endregion

		#region User Client

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsRouterIsFalse() {

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsShardIsFalse() {

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsUserIsTrue() {

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsServerIsFalse() {

		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsClientIsTrue() {

		}

		#endregion
	}

	[TestClass]
	public class DistributedServiceIntegrationTests {
		public RouterServer Router { get; set; }
		public List<ShardClient> Shards { get; set; }
		public List<UserClient> Users { get; set; }

		public int ShardCount { get; set; } = 3;
		public int UserCount { get; set; } = 3;

		public NetContext GetContext(int Port) {
			NetContext ctx = new NetContext();
			ctx.IPAddress = "localhost";
			ctx.Port = Port;

			return ctx;
		}

		[TestInitialize]
		public void Initialize() {
			var port = Common.Port;

			Router = new RouterServer();
			Shards = new List<ShardClient>();
			Users = new List<UserClient>();

			for(int i = 0; i < ShardCount; i++) {
				Shards.Add(new ShardClient());
			}

			for(int i = 0; i < UserCount; i++) {
				Users.Add(new UserClient());
			}

			Router.InitializeServer(GetContext(port));
			foreach (var shard in Shards) {
				shard.InitializeClient(GetContext(port));
			}
			foreach (var user in Users) {
				user.InitializeClient(GetContext(port));
			}

			Router.StartServer(new TCPServerProvider());
			foreach (var shard in Shards) {
				shard.StartClient(new TCPClientProvider());
			}
			foreach (var user in Users) {
				user.StartClient(new TCPClientProvider());
			}
		}

		#region Router Server

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_ExcludingOne_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllUsers_ExcludingMany_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToShard_PacketReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_ExcludingOne_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_SendToAllShards_ExcludingMany_PacketsReceived() {

		}

		#endregion

		#region Shard Client
		
		[TestMethod]
		public void DistributedService_ShardClient_SendToRouter_PacketReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToUser_PacketReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_ExcludingOne_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllUsers_ExcludingMany_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToShard_PacketReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_ExcludingOne_PacketsReceived() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_SendToAllShards_ExcludingMany_PacketsReceived() {

		}

		#endregion

		#region User Client
		
		[TestMethod]
		public void DistributedService_UserClient_SendToRouter_ReceivesPacket() {

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToShard_ReceivesPacket() {

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ReceivesPackets() {

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ExcludingOne_ReceivesPackets() {

		}

		[TestMethod]
		public void DistributedService_UserClient_SendToAllShards_ExcludingMany_ReceivesPackets() {

		}

		#endregion
	}
}
