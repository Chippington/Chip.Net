﻿using Chip.Net.Controllers.Distributed;
using Chip.Net.Controllers.Distributed.Services;
using Chip.Net.Data;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	[TestClass]
	public class DistributedServiceInitializationTests {
		public RouterServer<TestRouterModel, TestShardModel, TestUserModel> Router { get; set; }
		public ShardClient<TestRouterModel, TestShardModel, TestUserModel> Shard { get; set; }
		public UserClient<TestRouterModel, TestShardModel, TestUserModel> User { get; set; }

		public class TestService : DistributedService {

		}


		[TestInitialize]
		public void Initialize() {
			Router = new RouterServer<TestRouterModel, TestShardModel, TestUserModel>();
			Shard = new ShardClient<TestRouterModel, TestShardModel, TestUserModel>();
			User = new UserClient<TestRouterModel, TestShardModel, TestUserModel>();
		}

		private NetContext GetContext() {
			NetContext ctx = new NetContext();
			ctx.Services.Register<TestService>();
			return ctx;
		}

		#region Router Server

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsRouterIsTrue() {
			Router.InitializeServer(GetContext());
			Assert.IsTrue(Router.Context.Services.Get<TestService>().IsRouter);
		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsShardIsFalse() {
			Router.InitializeServer(GetContext());
			Assert.IsFalse(Router.Context.Services.Get<TestService>().IsShard);
		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsUserIsFalse() {
			Router.InitializeServer(GetContext());
			Assert.IsFalse(Router.Context.Services.Get<TestService>().IsUser);
		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsServerIsTrue() {
			Router.InitializeServer(GetContext());
			Assert.IsTrue(Router.Context.Services.Get<TestService>().IsServer);
		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_IsClientIsFalse() {
			Router.InitializeServer(GetContext());
			Assert.IsFalse(Router.Context.Services.Get<TestService>().IsClient);
		}

		[TestMethod]
		public void DistributedService_RouterServer_Initialize_ConfiguredEventInvoked() {
			bool configured = false;
			Router.RouterConfiguredEvent += (s, e) => { configured = true; };
			Router.InitializeServer(GetContext());

			Assert.IsTrue(configured);
		}

		#endregion

		#region Shard Client

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsRouterIsFalse() {
			Shard.InitializeClient(GetContext());
			Assert.IsFalse(Shard.Context.Services.Get<TestService>().IsRouter);
		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsShardIsTrue() {
			Shard.InitializeClient(GetContext());
			Assert.IsTrue(Shard.Context.Services.Get<TestService>().IsShard);
		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsUserIsFalse() {
			Shard.InitializeClient(GetContext());
			Assert.IsFalse(Shard.Context.Services.Get<TestService>().IsUser);
		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsServerIsFalse() {
			Shard.InitializeClient(GetContext());
			Assert.IsFalse(Shard.Context.Services.Get<TestService>().IsServer);
		}

		[TestMethod]
		public void DistributedService_ShardClient_Initialize_IsClientIsTrue() {
			Shard.InitializeClient(GetContext());
			Assert.IsTrue(Shard.Context.Services.Get<TestService>().IsClient);
		}

		#endregion

		#region User Client

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsRouterIsFalse() {
			User.InitializeClient(GetContext());
			Assert.IsFalse(User.Context.Services.Get<TestService>().IsRouter);
		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsShardIsFalse() {
			User.InitializeClient(GetContext());
			Assert.IsFalse(User.Context.Services.Get<TestService>().IsShard);
		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsUserIsTrue() {
			User.InitializeClient(GetContext());
			Assert.IsTrue(User.Context.Services.Get<TestService>().IsUser);
		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsServerIsFalse() {
			User.InitializeClient(GetContext());
			Assert.IsFalse(User.Context.Services.Get<TestService>().IsServer);
		}

		[TestMethod]
		public void DistributedService_UserClient_Initialize_IsClientIsTrue() {
			User.InitializeClient(GetContext());
			Assert.IsTrue(User.Context.Services.Get<TestService>().IsClient);
		}

		#endregion
	}

	[TestClass]
	public class DistributedServiceIntegrationTests {
		public class TestPacket : Packet {
			public string Data { get; set; }

			public TestPacket() {
				Data = "";
			}

			public override void WriteTo(DataBuffer buffer) {
				buffer.Write((string)Data);
			}

			public override void ReadFrom(DataBuffer buffer) {
				base.ReadFrom(buffer);
				Data = buffer.ReadString();
			}
		}


		public RouterServer<TestRouterModel, TestShardModel, TestUserModel> Router { get; set; }
		public List<ShardClient<TestRouterModel, TestShardModel, TestUserModel>> Shards { get; set; }
		public List<UserClient<TestRouterModel, TestShardModel, TestUserModel>> Users { get; set; }

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

			Router = new RouterServer<TestRouterModel, TestShardModel, TestUserModel>();
			Shards = new List<ShardClient<TestRouterModel, TestShardModel, TestUserModel>>();
			Users = new List<UserClient<TestRouterModel, TestShardModel, TestUserModel>>();

			for(int i = 0; i < ShardCount; i++) {
				Shards.Add(new ShardClient<TestRouterModel, TestShardModel, TestUserModel>());
			}

			for(int i = 0; i < UserCount; i++) {
				Users.Add(new UserClient<TestRouterModel, TestShardModel, TestUserModel>());
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

		[TestMethod]
		public void DistributedService_RouterServer_SendToUser_PacketReceived() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_UserConnected_EventInvoked() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_UserDisconnected_EventInvoked() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_ShardConnected_EventInvoked() {

		}

		[TestMethod]
		public void DistributedService_RouterServer_ShardDisconnected_EventInvoked() {

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

		[TestMethod]
		public void DistributedService_ShardClient_ConnectedToRouter_ConfiguredEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_UserAssigned_EventInvoked() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_UserUnassigned_EventInvoked() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_ConnectedToRouter_ConnectedEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_ShardClient_DisconnectedFromRouter_DisconnectedEventInvoked() {

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

		[TestMethod]
		public void DistributedService_UserClient_ConnectedToRouter_UserConfiguredEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_UserClient_ConnectedToRouter_ConnectedEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_UserClient_DisconnectedFromRouter_DisconnectedEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_UserClient_RouterAssignUserToShard_AssignedToShardEventInvoked() {

		}

		[TestMethod]
		public void DistributedService_UserClient_RouterUnassignUserFromShard_UnassignedToShardEventInvoked() {

		}

		#endregion
	}
}
