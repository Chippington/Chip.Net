using Chip.Net.Providers;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net {
	public class NetContext {
		public string ApplicationName { get; set; }
		public string IPAddress { get; set; }
		public int MaxConnections { get; set; }
		public int Port { get; set; }
		public bool Initialized { get; private set; }

		public NetServiceCollection Services { get; private set; }

		public NetContext() {
			Services = new NetServiceCollection();
			Initialized = false;
		}

		public void InitializeContext() {
			if (Initialized == true)
				throw new Exception("NetContext can only be initialized once.");

			Services.InitializeServices(this);
			Initialized = true;
		}

		#region Providers
		private Func<NetContext, INetClientProvider> clientFactory { get; set; }
		private Func<NetContext, INetServerProvider> serverFactory { get; set; }
		public void UseProvider<TProvider>() 
			where TProvider : INetClientProvider, INetServerProvider {
			clientFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};
		}

		public void UseProvider<TServer, TClient>() 
			where TServer : INetServerProvider 
			where TClient : INetClientProvider {

			clientFactory = (ctx) => {
				return Activator.CreateInstance<TClient>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TServer>();
			};
		}

		public void UseProvider(Func<NetContext, INetClientProvider> clientFactory, Func<NetContext, INetServerProvider> serverFactory) {
			this.serverFactory = serverFactory;
			this.clientFactory = clientFactory;
		}
		#endregion
	}
}
