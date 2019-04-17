using Chip.Net.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chip.Net.UnitTests.Data
{
	public class TestPacket : Packet {

	}

    [TestClass]
    public class PacketRouterTests
    {
		[TestMethod]
		public void RouteClient_InvokeClient_CallbackInvoked() {
			PacketRouter router = new PacketRouter();
			int test = 0;
			router.RouteClient<TestPacket>(i => {
				test++;
			});

			router.RouteServer<TestPacket>(i => {
				test--;
			});

			router.InvokeClient(new TestPacket());
			Assert.IsTrue(test == 1);
		}

		[TestMethod]
		public void RouteServer_InvokeServer_CallbackInvoked() {
			PacketRouter router = new PacketRouter();
			int test = 0;
			router.RouteServer<TestPacket>(i => {
				test++;
			});

			router.RouteClient<TestPacket>(i => {
				test--;
			});

			router.InvokeServer(new TestPacket(), null);
			Assert.IsTrue(test == 1);
		}

		[TestMethod]
		public void Route_InvokeClient_CallbackInvoked() {
			PacketRouter router = new PacketRouter();
			int test = 0;
			router.Route<TestPacket>(i => {
				test++;
			});

			router.InvokeClient(new TestPacket());
			Assert.IsTrue(test == 1);
		}

		[TestMethod]
		public void Route_InvokeServer_CallbackInvoked() {
			PacketRouter router = new PacketRouter();
			int test = 0;
			router.Route<TestPacket>(i => {
				test++;
			});

			router.InvokeServer(new TestPacket(), null);
			Assert.IsTrue(test == 1);
		}
	}
}
