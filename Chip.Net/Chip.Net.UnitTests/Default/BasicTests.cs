using Chip.Net.Data;
using Chip.Net.Default.Basic;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.UnitTests.Default
{
	[TestClass]
    public class BasicTests
    {
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
				return _context;
			}
		}

		INetServer StartNewServer() {
			var sv = NewServer();
			sv.StartServer(new TCPServerProvider());
			return sv;
		}

		INetServer NewServer() {
			var sv = new BasicServer();
			sv.InitializeServer(Context);
			return sv;
		}

		INetClient StartNewClient() {
			var cl = NewClient();
			cl.StartClient(new TCPClientProvider());
			return cl;
		}

		INetClient NewClient() {
			var cl = new BasicClient();
			cl.InitializeClient(Context);
			return cl;
		}

		void Wait(Func<bool> func) {
			var tick = Environment.TickCount;
			while (Environment.TickCount - tick < 1000 && func() == false)
				System.Threading.Thread.Sleep(10);
		}

		[TestMethod]
		public void Server_StartServer_RouterNotNull() {
			var sv = StartNewServer();
			Assert.IsNotNull(sv.Router);
		}

		[TestMethod]
		public void Client_StartClient_RouterNotNull() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			Assert.IsNotNull(cl.Router);
		}

		[TestMethod]
		public void Server_StartServer_IsActive() {
			var sv = StartNewServer();
			Assert.IsTrue(sv.IsActive);
		}

		[TestMethod]
		public void Client_StartClient_IsConnected() {
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
		public void Server_ClientConnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			NetUser user = null;
			sv.OnUserConnected += i => { user = i.User; eventInvoked = true; };


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
		public void Server_ClientDisconnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			NetUser user1 = null;
			NetUser user2 = null;
			sv.OnUserConnected += i => { user1 = i.User; };
			sv.OnUserDisconnected += i => { user2 = i.User; eventInvoked = true; };

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
			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public void Client_ClientConnects_EventInvoked() {
			var sv = StartNewServer();
			var cl = NewClient();
			bool eventInvoked = false;
			cl.OnConnected += i => { eventInvoked = true; };

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
		public void Client_ClientDisconnects_EventInvoked() {
			var sv = StartNewServer();
			var cl = NewClient();
			bool eventInvoked = false;
			cl.OnDisconnected += i => { eventInvoked = true; };

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

			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public void Server_ClientConnected_HasUser() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.OnUserConnected += i => {
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
		public void Server_InitializeServer_HasContext() {
			var sv = NewServer();
			Assert.IsNotNull(sv.Context);
		}

		[TestMethod]
		public void Client_InitializeClient_HasContext() {
			var cl = NewClient();
			Assert.IsNotNull(cl.Context);
		}

		[TestMethod]
		public void Server_InitializeServer_ContextInitialized() {
			var sv = NewServer();
			Assert.IsTrue(Context.Initialized);
		}

		[TestMethod]
		public void Server_StartServer_ServicesStarted() {
			var sv = NewServer();
			sv.StartServer(new TCPServerProvider());
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Started);
		}

		[TestMethod]
		public void Server_UpdateServer_ServicesUpdated() {
			var sv = NewServer();
			sv.StartServer(new TCPServerProvider());
			sv.UpdateServer();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Updated);

		}

		[TestMethod]
		public void Server_StopServer_ServicesStopped() {
			var sv = NewServer();
			sv.StartServer(new TCPServerProvider());
			sv.UpdateServer();
			sv.StopServer();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Stopped);
		}

		[TestMethod]
		public void Server_DisposeServer_ServicesDisposed() {
			var sv = NewServer();
			sv.StartServer(new TCPServerProvider());
			sv.UpdateServer();
			sv.StopServer();
			sv.Dispose();
			Assert.IsTrue(sv.Context.Services.Get<TestNetService>().Disposed);
		}

		[TestMethod]
		public void Client_InitializeClient_ContextInitialized() {
			var cl = NewClient();
			Assert.IsTrue(cl.Context.Initialized);
		}

		[TestMethod]
		public void Client_StartClient_ServicesStarted() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Started);
		}

		[TestMethod]
		public void Client_UpdateClient_ServicesUpdated() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			cl.UpdateClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Updated);
		}

		[TestMethod]
		public void Client_StopClient_ServicesStopped() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			cl.UpdateClient();
			cl.StopClient();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Stopped);
		}

		[TestMethod]
		public void Client_DisposeClient_ServicesDisposed() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			cl.UpdateClient();
			cl.StopClient();
			cl.Dispose();
			Assert.IsTrue(cl.Context.Services.Get<TestNetService>().Disposed);
		}

		[TestMethod]
		public void Server_SendPacket_ClientReceivesPacket() {
			var sv = StartNewServer();
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

			sv.SendPacket(new TestPacket());

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return received;
			});

			Assert.IsTrue(received);
		}

		[TestMethod]
		public void Server_SendPacketToUser_ClientReceivesPacket() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.OnUserConnected += i => {
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
		public void Client_SendPacket_ServerReceivesPacket() {
			var sv = StartNewServer();
			var cl = StartNewClient();
			bool received = false;

			sv.Router.RouteClient<TestPacket>(i => {
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
		public void Client_SendPacket_PacketHasRecipient() {
			var sv = StartNewServer();
			NetUser user = null;
			sv.OnUserConnected += i => {
				user = i.User;
			};

			var cl = StartNewClient();
			bool received = false;
			NetUser user2 = null;

			sv.Router.RouteClient<TestPacket>(i => {
				received = true;
				user2 = i.Sender;
			});

			Wait(() => {
				sv.UpdateServer();
				cl.UpdateClient();
				return cl.IsConnected;
			});

			sv.SendPacket(new TestPacket());

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
		public void Server_ServiceSendPacket_ClientServiceReceivesPacket() {

		}

		[TestMethod]
		public void Server_ServiceSendPacketToUser_ClientServiceReceivesPacket() {

		}

		[TestMethod]
		public void Client_ServiceSendsPacket_ServerServiceReceivesPacket() {

		}
	}
}
