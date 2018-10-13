using Chip.Net.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.UnitTests.Services {
	[TestClass]
	public class NetServiceTests {
		[TestMethod]
		public void NetServiceCollection_RegisterService_HasService() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestNetService>();
			var result = c.Get<TestNetService>();

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void NetServiceCollection_RegisterServiceInstance_HasService() {
			NetServiceCollection c = new NetServiceCollection();
			var real = new TestNetService();
			var first = c.Register<TestNetService>(real);
			var second = c.Get<TestNetService>();

			Assert.IsNotNull(real);
			Assert.IsNotNull(first);
			Assert.IsNotNull(second);
			Assert.AreEqual(real, first);
			Assert.AreEqual(first, second);
		}

		[TestMethod]
		public void NetServiceCollection_UpdateServices_ServiceUpdated() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestNetService>();
			c.UpdateServices();

			Assert.IsTrue(c.Get<TestNetService>().Updated == true);
		}

		[TestMethod]
		public void NetServiceCollection_GetServices_HasServices() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestNetService>();

			Assert.IsTrue(c.Get().Count() == 1);
			Assert.IsTrue(c.Get().First() == c.Get<TestNetService>());
		}

		[TestMethod]
		public void NetServiceCollection_Initialized_ServiceInitialized() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestNetService>();
			c.InitializeServices(new NetContext());

			Assert.IsTrue(c.Get<TestNetService>().Initialized == true);
		}

		[TestMethod]
		public void NetServiceCollection_Disposed_ServiceDisposed() {
			NetServiceCollection c = new NetServiceCollection();
			var svc = c.Register<TestNetService>();
			c.Dispose();

			Assert.IsTrue(svc.Disposed == true);
		}
	}
}
