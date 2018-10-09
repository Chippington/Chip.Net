using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net {
	public class NetContext {
		public string ApplicationName { get; set; }
		public string IPAddress { get; set; }
		public int MaxConnections { get; set; }
		public int Port { get; set; }

		#region Providers
		private Func<NetContext, INetClientProvider> clientFactory { get; set; }
		private Func<NetContext, INetClientProvider> serverFactory { get; set; }
		public void UseProvider<TProvider>() where TProvider : INetClientProvider {
			clientFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};
		}

		public void UseProvider<TServer, TClient>() 
			where TServer : INetClientProvider 
			where TClient : INetClientProvider {

			clientFactory = (ctx) => {
				return Activator.CreateInstance<TClient>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TServer>();
			};
		}

		public void UseProvider(Func<NetContext, INetClientProvider> providerFactory) {
			clientFactory = providerFactory;
			serverFactory = providerFactory;
		}

		public void UseProvider(Func<NetContext, INetClientProvider> clientFactory, Func<NetContext, INetClientProvider> serverFactory) {
			this.serverFactory = serverFactory;
			this.clientFactory = clientFactory;
		}
		#endregion
	}
}
