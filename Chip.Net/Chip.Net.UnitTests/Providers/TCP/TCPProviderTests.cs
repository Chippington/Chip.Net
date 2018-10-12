using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Providers.TCP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.UnitTests.Providers.TCP {
	[TestClass]
	public class TCPProviderTests {
		public static int portDiff = 0;
		public NetContext context { get; set; }
		public INetClientProvider client { get; set; }
		public INetServerProvider server { get; set; }

		[TestInitialize]
		public void Initialize() {
			context = new NetContext();
			context.IPAddress = "127.0.0.1";
			context.Port = 11111 + portDiff++;
			context.MaxConnections = 10;

			client = new TCPClientProvider();
			server = new TCPServerProvider();
		}

		[TestMethod]
		public void Server_DisconnectClient_ClientDisconnected() {
			server.StartServer(context);
			int connections = 0;
			server.OnUserConnected += i => {
				server.DisconnectUser(i.UserKey);
			};

			server.OnUserDisconnected += i => {
				connections++;
			};

			client.Connect(context);
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

			client.Connect(context);
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

			client.Connect(context);
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

			client.Connect(context);
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

			client.Connect(context);
			int tick = Environment.TickCount;
			while (connections != 1 && Environment.TickCount - tick < 1000) {
				System.Threading.Thread.Sleep(10);
				server.UpdateServer();
				client.UpdateClient();
			}

			Assert.IsTrue(connections == 1);
		}

		[TestMethod]
		public void Server_SendToClient_ClientReceivesData() {
			byte[] data = new byte[3] { 1, 3, 7 };
			byte[] received = null;

			server.StartServer(context);
			client.Connect(context);

			server.OnUserConnected += i => {
				client.SendMessage(new DataBuffer(data));
			};

			int tick = Environment.TickCount;
			while(Environment.TickCount - tick < 1000) {
				server.UpdateServer();
				client.UpdateClient();

				var incoming = server.GetIncomingMessages();
				if(incoming.Any()) {
					received = incoming.First().Item2.ToBytes();
					break;
				}
			}

			Assert.IsNotNull(received);
			Assert.AreEqual(data.Length, received.Length);
			for(int i = 0; i < data.Length; i++) {
				Assert.AreEqual(data[i], received[i]);
			}
		}

		[TestMethod]
		public void Client_SendToServer_ServerReceivesData() {
			byte[] data = new byte[3] { 1, 3, 7 };
			byte[] received = null;

			server.StartServer(context);
			client.Connect(context);

			server.OnUserConnected += i => {
				server.SendMessage(new DataBuffer(data));
			};

			int tick = Environment.TickCount;
			while (Environment.TickCount - tick < 1000) {
				server.UpdateServer();
				client.UpdateClient();

				var incoming = client.GetIncomingMessages();
				if (incoming.Any()) {
					received = incoming.First().ToBytes();
					break;
				}
			}

			Assert.IsNotNull(received);
			Assert.AreEqual(data.Length, received.Length);
			for (int i = 0; i < data.Length; i++) {
				Assert.AreEqual(data[i], received[i]);
			}
		}

		[TestMethod]
		public void Server_SendToOneClient_ClientReceivesData() {
			byte[] data = new byte[3] { 1, 3, 7 };
			byte[] received = null;

			server.StartServer(context);

			var clientTwo = new TCPClientProvider();

			client.OnConnected += i => {
				clientTwo.Connect(context);
			};

			server.OnUserConnected += i => {
				if (server.GetClientKeys().Count() == 2) {
					var keys = server.GetClientKeys();
					server.SendMessage(keys.Last(), new DataBuffer(data));
				}
			};

			client.Connect(context);
			int tick = Environment.TickCount;
			while (Environment.TickCount - tick < 1000) {
				server.UpdateServer();
				client.UpdateClient();
				clientTwo.UpdateClient();

				if (clientTwo.IsConnected == false)
					continue;

				var incoming = clientTwo.GetIncomingMessages();
				if (incoming.Any()) {
					received = incoming.First().ToBytes();
					break;
				}

				incoming = client.GetIncomingMessages();
				if (incoming.Any()) {
					received = incoming.First().ToBytes();
					break;
				}
			}

			Assert.IsNotNull(received);
			Assert.AreEqual(data.Length, received.Length);
			for (int i = 0; i < data.Length; i++) {
				Assert.AreEqual(data[i], received[i]);
			}
		}

		[TestMethod]
		public void Server_SendToAllClients_ClientReceivesData() {
			byte[] data = new byte[3] { 1, 3, 7 };
			byte[] received1 = null;
			byte[] received2 = null;

			server.StartServer(context);

			var clientTwo = new TCPClientProvider();

			client.OnConnected += i => {
				clientTwo.Connect(context);
			};

			server.OnUserConnected += i => {
				if (server.GetClientKeys().Count() == 2) {
					var keys = server.GetClientKeys();
					server.SendMessage(new DataBuffer(data));
				}
			};

			client.Connect(context);
			int tick = Environment.TickCount;
			while (Environment.TickCount - tick < 100000) {
				server.UpdateServer();
				client.UpdateClient();
				clientTwo.UpdateClient();

				if (clientTwo.IsConnected == false)
					continue;

				var incoming = clientTwo.GetIncomingMessages();
				if (incoming.Any()) {
					received1 = incoming.First().ToBytes();

					if(received2 != null)
						break;
				}

				incoming = client.GetIncomingMessages();
				if (incoming.Any()) {
					received2 = incoming.First().ToBytes();

					if(received1 != null)
						break;
				}
			}

			Assert.IsNotNull(received1);
			Assert.AreEqual(data.Length, received1.Length);
			for (int i = 0; i < data.Length; i++) {
				Assert.AreEqual(data[i], received1[i]);
			}

			Assert.IsNotNull(received2);
			Assert.AreEqual(data.Length, received2.Length);
			for (int i = 0; i < data.Length; i++) {
				Assert.AreEqual(data[i], received2[i]);
			}
		}
	}
}
