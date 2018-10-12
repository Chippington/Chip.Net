using Chip.Net.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.UnitTests.Services {
	public class TestService : INetService {
		public bool initialized = false;
		public bool disposed = false;
		public bool updated = false;

		public void InitializeService(NetContext context) {
			initialized = true;
		}

		public void Dispose() {
			disposed = true;
		}

		public void UpdateService() {
			updated = true;
		}
	}

	[TestClass]
	public class NetServiceTests {
		[TestMethod]
		public void NetServiceCollection_RegisterService_HasService() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestService>();
			var result = c.Get<TestService>();

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void NetServiceCollection_RegisterServiceInstance_HasService() {
			NetServiceCollection c = new NetServiceCollection();
			var real = new TestService();
			var first = c.Register<TestService>(real);
			var second = c.Get<TestService>();

			Assert.IsNotNull(real);
			Assert.IsNotNull(first);
			Assert.IsNotNull(second);
			Assert.AreEqual(real, first);
			Assert.AreEqual(first, second);
		}

		[TestMethod]
		public void NetServiceCollection_UpdateServices_ServiceUpdated() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestService>();
			c.UpdateServices();

			Assert.IsTrue(c.Get<TestService>().updated == true);
		}

		[TestMethod]
		public void NetServiceCollection_GetServices_HasServices() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestService>();

			Assert.IsTrue(c.Get().Count() == 1);
			Assert.IsTrue(c.Get().First() == c.Get<TestService>());
		}

		[TestMethod]
		public void NetServiceCollection_Initialized_ServiceInitialized() {
			NetServiceCollection c = new NetServiceCollection();
			c.Register<TestService>();
			c.InitializeServices(new NetContext());

			Assert.IsTrue(c.Get<TestService>().initialized == true);
		}

		[TestMethod]
		public void NetServiceCollection_Disposed_ServiceDisposed() {
			NetServiceCollection c = new NetServiceCollection();
			var svc = c.Register<TestService>();
			c.Dispose();

			Assert.IsTrue(svc.disposed == true);
		}
	}
}
