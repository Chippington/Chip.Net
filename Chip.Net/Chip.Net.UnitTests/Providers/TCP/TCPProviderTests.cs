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
		public void Server_ClientConnected_EventInvoked() {

		}

		[TestMethod]
		public void Server_ClientDisconnected_EventInvoked() {

		}

		[TestMethod]
		public void Client_ConnectedToServer_EventInvoked() {

		}

		[TestMethod]
		public void Client_DisconnectedFromServer_EventInvoked() {

		}
	}
}
