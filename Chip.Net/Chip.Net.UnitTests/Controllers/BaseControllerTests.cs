﻿using Chip.Net.Data;
using Chip.Net.Providers.Direct;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.UnitTests.Controllers {
	public abstract class BaseControllerTests<TServer, TClient>
		where TServer : INetServerController
		where TClient : INetClientController {

		private int _port = 0;
		private int Port {
			get {
				if (_port == 0)
					_port = Common.Port;

				return _port;
			}
		}

		private NetContext _context;
		public NetContext Context {
			get {
				_context = new NetContext();
				_context.IPAddress = "127.0.0.1";
				_context.Port = Port;
				_context.Services.Register<TestNetService>();
				_context.Packets.Register<TestPacket>();
				return _context;
			}
		}

		protected virtual INetServerController StartNewServer() {
			var sv = NewServer();
			sv.StartServer(new DirectServerProvider());
			return sv;
		}

		protected virtual INetServerController NewServer() {
			var sv = Activator.CreateInstance<TServer>();
			sv.InitializeServer(Context);
			return sv;
		}

		protected virtual INetClientController StartNewClient() {
			var cl = NewClient();
			cl.StartClient(new DirectClientProvider());
			return cl;
		}

		protected virtual INetClientController NewClient() {
			var cl = Activator.CreateInstance<TClient>();
			cl.InitializeClient(Context);
			return cl;
		}

		protected virtual void Wait(Func<bool> func, int ticks = 1000) {
			var tick = Environment.TickCount;
			while (Environment.TickCount - tick < ticks && func() == false)
				System.Threading.Thread.Sleep(10);
		}

		[TestMethod]
		public virtual void Server_StartServer_RouterNotNull() {
			var sv = StartNewServer();
			Assert.IsNotNull(sv.Router);
		}

		[TestMethod]
		public virtual void Client_StartClient_RouterNotNull() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			Assert.IsNotNull(cl.Router);
		}

		[TestMethod]
		public virtual void Server_StartServer_IsActive() {
			var sv = StartNewServer();
			Assert.IsTrue(sv.IsActive);
		}

		[TestMethod]
		public virtual void Client_StartClient_IsConnected() {
			var sv = StartNewServer();
			var cl = StartNewClient();

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Assert.IsTrue(cl.IsConnected);
		}

		[TestMethod]
		public virtual void Server_ClientConnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			NetUser user = null;
			sv.UserConnected += (s, i) => { user = i.User; eventInvoked = true; };


			var cl = StartNewClient();
			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return eventInvoked;
			});

			Assert.IsNotNull(user);
			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public virtual void Server_ClientDisconnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			NetUser user1 = null;
			NetUser user2 = null;
			sv.UserConnected += (s, i) => { user1 = i.User; };
			sv.UserDisconnected += (s, i) => { user2 = i.User; eventInvoked = true; };

			var cl = StartNewClient();
			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			cl.StopClient();

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return eventInvoked;
			});

			Assert.IsNotNull(user1);
			Assert.AreEqual(user1, user2);
			Assert.IsTrue(cl.IsConnected == false);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public virtual void Client_ClientConnects_EventInvoked() {
			var sv = StartNewServer();
			var cl = NewClient();
			bool eventInvoked = false;
			cl.OnConnected += (s, i) => { eventInvoked = true; };
			cl.StartClient(new DirectClientProvider());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return eventInvoked;
			});

			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public virtual void Client_ClientDisconnects_EventInvoked() {
			var sv = StartNewServer();
			var cl = NewClient();
			bool eventInvoked = false;
			cl.OnDisconnected += (s, i) => { eventInvoked = true; };
			cl.StartClient(new DirectClientProvider());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			cl.StopClient();

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return eventInvoked;
			});

			Assert.IsTrue(cl.IsConnected == false);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public virtual void Server_ClientConnected_HasUser() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.UserConnected += (s, i) => {
				user = i.User;
			};

			var cl = StartNewClient();

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Assert.IsNotNull(user);
			Assert.IsTrue(sv.GetUsers().Count() == 1);
			Assert.AreEqual(sv.GetUsers().First(), user);
		}

		[TestMethod]
		public virtual void Server_InitializeServer_HasContext() {
			var sv = NewServer();
			Assert.IsNotNull(sv.Context);
		}

		[TestMethod]
		public virtual void Client_InitializeClient_HasContext() {
			var cl = NewClient();
			Assert.IsNotNull(cl.Context);
		}

		[TestMethod]
		public virtual void Server_InitializeServer_ContextLocked() {
			var sv = NewServer();
			Assert.IsTrue(sv.Context.IsLocked);
		}

		[TestMethod]
		public virtual void Server_StartServer_ServicesStarted() {
			var sv = NewServer();
			sv.StartServer(new DirectServerProvider());
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Started);
		}

		[TestMethod]
		public virtual void Server_UpdateServer_ServicesUpdated() {
			var sv = NewServer();
			sv.StartServer(new DirectServerProvider());
			sv.UpdateServer();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Updated);

		}

		[TestMethod]
		public virtual void Server_StopServer_ServicesStopped() {
			var sv = NewServer();
			sv.StartServer(new DirectServerProvider());
			sv.UpdateServer();
			sv.StopServer();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Stopped);
		}

		[TestMethod]
		public virtual void Server_DisposeServer_ServicesDisposed() {
			var sv = NewServer();
			var svc = sv.Context.Services.Get<TestNetService>();
			sv.StartServer(new DirectServerProvider());
			sv.UpdateServer();
			sv.StopServer();
			sv.Dispose();
			Assert.IsTrue(svc.Disposed);
		}

		[TestMethod]
		public virtual void Client_InitializeClient_ContextLocked() {
			var cl = NewClient();
			Assert.IsTrue(cl.Context.IsLocked);
		}

		[TestMethod]
		public virtual void Server_InitializeServer_ServicesInitialized() {
			var sv = NewServer();
			var cl = NewClient();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Initialized);
		}

		[TestMethod]
		public virtual void Client_InitializeClient_ServicesInitialized() {
			var sv = NewServer();
			var cl = NewClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Initialized);
		}

		[TestMethod]
		public virtual void Client_StartClient_ServicesStarted() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Started);
		}

		[TestMethod]
		public virtual void Client_UpdateClient_ServicesUpdated() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			cl.UpdateClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Updated);
		}

		[TestMethod]
		public virtual void Client_StopClient_ServicesStopped() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			cl.UpdateClient();
			cl.StopClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Stopped);
		}

		[TestMethod]
		public virtual void Client_DisposeClient_ServicesDisposed() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			var svc = cl.Context.Services.Get<TestNetService>();
			cl.UpdateClient();
			cl.StopClient();
			cl.Dispose();
			Assert.IsTrue(svc.Disposed);
		}

		[TestMethod]
		public virtual void Server_SendPacket_ClientReceivesPacket() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.UserConnected += (s, i) => {
				user = i.User;
			};

			var cl = StartNewClient();
			bool received = false;

			cl.Router.RouteClient<TestPacket>(i => {
				received = true;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Assert.IsNotNull(user);
			sv.SendPacket(new TestPacket());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return received;
			});

			Assert.IsTrue(received);
		}

		[TestMethod]
		public virtual void Server_SendPacketToUser_ClientReceivesPacket() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.UserConnected += (s, i) => {
				user = i.User;
			};

			var cl = StartNewClient();
			bool received = false;

			cl.Router.RouteClient<TestPacket>(i => {
				received = true;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			Assert.IsNotNull(user);
			sv.SendPacket(user, new TestPacket());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return received;
			});

			Assert.IsTrue(received);
		}

		[TestMethod]
		public virtual void Client_SendPacket_ServerReceivesPacket() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			bool received = false;

			sv.Router.RouteServer<TestPacket>(i => {
				received = true;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			cl.SendPacket(new TestPacket());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return received;
			});

			Assert.IsTrue(received);
		}

		[TestMethod]
		public virtual void Client_SendPacket_PacketHasRecipient() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.UserConnected += (s, i) => {
				user = i.User;
			};

			var cl = StartNewClient();
			bool received = false;
			NetUser user2 = null;

			sv.Router.RouteServer<TestPacket>(i => {
				received = true;
				user2 = i.Sender;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			cl.SendPacket(new TestPacket());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return received;
			});

			Assert.IsTrue(received);
			Assert.IsNotNull(user);
			Assert.AreEqual(user, user2);
		}

		[TestMethod]
		public virtual void Server_ServiceSendPacket_ClientServiceReceivesPacket() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			string data = "Hello world" + Port;

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			var cl_svc = cl.Context.Services.Get<TestNetService>();
			var sv_svc = sv.Context.Services.Get<TestNetService>();
			sv_svc.Send(new TestPacket() {
				data = data,
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl_svc.Received;
			});

			Assert.IsTrue(cl_svc.Received);
			Assert.AreEqual(cl_svc.ReceivedData, data);
		}

		[TestMethod]
		public virtual void Server_ServiceSendPacketToUser_ClientServiceReceivesPacket() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			string data = "Hello world" + Port;

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			var cl_svc = cl.Context.Services.Get<TestNetService>();
			var sv_svc = sv.Context.Services.Get<TestNetService>();
			sv_svc.Send(new TestPacket() {
				data = data,
				Recipient = sv.GetUsers().First(),
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl_svc.Received;
			});

			Assert.IsTrue(cl_svc.Received);
			Assert.AreEqual(cl_svc.ReceivedData, data);
		}

		[TestMethod]
		public virtual void Client_ServiceSendsPacket_ServerServiceReceivesPacket() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			string data = "Hello world" + Port;

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			var cl_svc = cl.Context.Services.Get<TestNetService>();
			var sv_svc = sv.Context.Services.Get<TestNetService>();
			cl_svc.Send(new TestPacket() {
				data = data,
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return sv_svc.Received;
			});

			Assert.IsTrue(sv_svc.Received);
			Assert.AreEqual(sv_svc.ReceivedData, data);
		}

		[TestMethod]
		public virtual void Server_InitializeServer_ServiceIsServerTrue() {
			var sv = NewServer();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().IsServer);
			Assert.IsFalse(sv.Context.Services.Get<TestNetService>().IsClient);
		}

		[TestMethod]
		public virtual void Client_InitializeClient_ServiceIsServerFalse() {
			var cl = NewClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().IsClient);
			Assert.IsFalse(cl.Context.Services.Get<TestNetService>().IsServer);
		}
	}
}
