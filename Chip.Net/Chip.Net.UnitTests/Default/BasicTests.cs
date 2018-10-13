using Chip.Net.Default.Basic;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Default
{
	[TestClass]
    public class BasicTests
    {
		private NetContext _context;
		public NetContext Context {
			get {
				if (_context == null) {
					_context = new NetContext();
					_context.IPAddress = "127.0.0.1";
					_context.Port = Common.Port;
				}

				return _context;
			}
		}

		INetServer StartNewServer() {
			var sv = new BasicServer();
			sv.InitializeServer(Context);
			sv.StartServer(new TCPServerProvider());
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
				return cl.IsConnected;
			});

			Assert.IsTrue(cl.IsConnected);
		}

		[TestMethod]
		public void Server_ClientConnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			sv.OnUserConnected += i => { eventInvoked = true; };

			var cl = StartNewClient();
			Wait(() => {
				return cl.IsConnected;
			});

			Wait(() => {
				return eventInvoked;
			});

			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}

		[TestMethod]
		public void Server_ClientDisconnects_EventInvoked() {
			var sv = StartNewServer();
			bool eventInvoked = false;
			sv.OnUserConnected += i => { eventInvoked = true; };

			var cl = StartNewClient();
			Wait(() => {
				return cl.IsConnected;
			});

			cl.StopClient();

			Wait(() => {
				return eventInvoked;
			});

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
				return cl.IsConnected;
			});

			Wait(() => {
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
				return cl.IsConnected;
			});

			cl.StopClient();

			Wait(() => {
				return eventInvoked;
			});

			Assert.IsTrue(cl.IsConnected);
			Assert.IsTrue(eventInvoked);
		}
	}
}
