using Chip.Net.Providers;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Providers.TCP {
	[TestClass]
	public class TCPProviderTests {
		public NetContext context { get; set; }
		public INetClientProvider client { get; set; }
		public INetServerProvider server { get; set; }

		[TestInitialize]
		public void Initialize() {
			context = new NetContext();
			context.IPAddress = "127.0.0.1";
			context.Port = 11111;

			client = new TCPClientProvider();
			server = new TCPServerProvider();
		}

		[TestMethod]
		public void Server_DisconnectClient_ClientDisconnected() {
			server.StartServer(context);
			int connections = 0;
			server.OnUserConnected += i => {
				server.DisconnectUser(i.UserData);
			};

			server.OnUserDisconnected += i => {
				connections++;
			};

			client.StartClient(context);
			int tick = Environment.TickCount;
			while (connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}

		[TestMethod]
		public void Server_ClientConnected_EventInvoked() {
			server.StartServer(context);
			int connections = 0;
			server.OnUserConnected += i => {
				connections++;
			};

			client.StartClient(context);
			int tick = Environment.TickCount;
			while(connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}

		[TestMethod]
		public void Server_ClientDisconnected_EventInvoked() {
			server.StartServer(context);
			int connections = 0;
			server.OnUserConnected += i => {
				client.Disconnect();
			};

			server.OnUserDisconnected += i => {
				connections++;
			};

			client.StartClient(context);
			int tick = Environment.TickCount;
			while (connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}

		[TestMethod]
		public void Client_ConnectedToServer_EventInvoked() {
			server.StartServer(context);
			int connections = 0;
			client.OnConnected += i => {
				connections++;
			};

			client.StartClient(context);
			int tick = Environment.TickCount;
			while (connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}

		[TestMethod]
		public void Client_DisconnectedFromServer_EventInvoked() {
			server.StartServer(context);
			int connections = 0;
			client.OnConnected += i => {
				client.Disconnect();
			};

			client.OnDisconnected += i => {
				connections++;
			};

			client.StartClient(context);
			int tick = Environment.TickCount;
			while (connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}
	}
}
