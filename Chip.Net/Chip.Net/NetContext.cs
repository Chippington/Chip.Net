using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net {
	public class NetContext {
		public string ApplicationName { get; set; }
		public string IPAddress { get; set; }
		public int Port { get; set; }

		#region Providers
		private Func<NetContext, INetProvider> clientFactory { get; set; }
		private Func<NetContext, INetProvider> serverFactory { get; set; }
		public void UseProvider<TProvider>() where TProvider : INetProvider {
			clientFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TProvider>();
			};
		}

		public void UseProvider<TServer, TClient>() 
			where TServer : INetProvider 
			where TClient : INetProvider {

			clientFactory = (ctx) => {
				return Activator.CreateInstance<TClient>();
			};

			serverFactory = (ctx) => {
				return Activator.CreateInstance<TServer>();
			};
		}

		public void UseProvider(Func<NetContext, INetProvider> providerFactory) {
			clientFactory = providerFactory;
			serverFactory = providerFactory;
		}

		public void UseProvider(Func<NetContext, INetProvider> clientFactory, Func<NetContext, INetProvider> serverFactory) {
			this.serverFactory = serverFactory;
			this.clientFactory = clientFactory;
		}
		#endregion
	}
}
