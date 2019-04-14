﻿using Chip.Net.Controllers.Basic;
using Chip.Net.Providers.Direct;
using Chip.Net.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chip.Net.UnitTests.Services
{
    public class NetServiceBaseTests<TService>
		where TService : INetService
    {
		public BasicServer Server { get; set; }
		public BasicClient Client { get; set; }
		public TService ServerService { get; set; }
		public TService ClientService { get; set; }

		public virtual NetContext GetContext(string ip, int port) {
			NetContext ret = new NetContext();
			ret.Services.Register<TService>();

			ret.IPAddress = "localhost";
			ret.Port = port;
			return ret;
		}

		[TestInitialize]
		public virtual void Initialize() {
			Server = new BasicServer();
			Client = new BasicClient();

			string ip = Guid.NewGuid().ToString();
			int port = Common.Port;
			Server.InitializeServer(GetContext(ip, port));
			Client.InitializeClient(GetContext(ip, port));

			Server.StartServer(new DirectServerProvider());
			var sv = DirectServerProvider.Servers;

			Client.StartClient(new DirectClientProvider());

			ServerService = Server.Context.Services.Get<TService>();
			ClientService = Client.Context.Services.Get<TService>();

			Assert.IsNotNull(ServerService);
			Assert.IsNotNull(ClientService);
		}

		public void Wait(Func<bool> func, int ms = 1000, int runoff = 0) {
			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			while (stopwatch.ElapsedMilliseconds <= ms && func() == false)
				System.Threading.Thread.Sleep(10);

			var m = stopwatch.ElapsedMilliseconds;
			if(runoff != 0)
				while (stopwatch.ElapsedMilliseconds < m + runoff)
					func();

			stopwatch.Stop();
		}
	}
}