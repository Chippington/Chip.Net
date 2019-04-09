using Chip.Net.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chip.Net.UnitTests.Providers
{
    public abstract class ProviderTests<TServer, TClient> : ProviderTests
		where TServer : INetServerProvider
		where TClient : INetClientProvider
    {
		public abstract void Server_StartServer_IsActive();
		public abstract void Server_StopServer_IsNotActive();
		public abstract void Server_Dispose_ClientDisconnected();
		public abstract void Server_DisconnectClient_ClientDisconnected();
		public abstract void Server_ClientConnected_EventInvoked();
		public abstract void Server_ClientDisconnected_EventInvoked();
		public abstract void Server_SendToClient_ClientReceivesData();
		public abstract void Server_SendToOneClient_ClientReceivesData();
		public abstract void Server_SendToAllClients_ClientReceivesData();

		public abstract void Client_Connect_IsConnected();
		public abstract void Client_Disconnect_IsNotConnected();
		public abstract void Client_ConnectedToServer_EventInvoked();
		public abstract void Client_SendToServer_ServerReceivesData();
		public abstract void Client_DisconnectedFromServer_EventInvoked();
	}

	public class ProviderTests {
		public static void Wait(Func<bool> func, int ms = 1000) {
			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			while (stopwatch.ElapsedMilliseconds <= ms && func() == false)
				System.Threading.Thread.Sleep(10);
			stopwatch.Stop();
		}
	}

	[TestClass]
	public class ProviderTestsTests {
		private Stopwatch stopwatch;

		[TestInitialize]
		public void Initialize() {
			stopwatch = new Stopwatch();
		}

		[TestMethod]
		public void Wait_WaitXMilliseconds_XMillisecondsElapsed() {
			for(int i = 1; i < 4; i++) {
				stopwatch.Start();
				ProviderTests.Wait(() => false, i * 250);
				stopwatch.Stop();
				Assert.IsTrue(stopwatch.ElapsedMilliseconds > i * 250);
				stopwatch.Reset();
			}
		}

		[TestMethod]
		public void Wait_WaitReturnTrue_EarlyEscape() {
			stopwatch.Start();
			ProviderTests.Wait(() => true, 250);
			Assert.IsTrue(stopwatch.ElapsedMilliseconds < 250);
			stopwatch.Reset();
		}
	}
}
