using Chip.Net.Controllers;
using Chip.Net.Data;
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
		public bool IsLocked { get; private set; }

		public DynamicSerializer Serializer { get; private set; }
		public NetServiceCollection Services { get; private set; }
		public PacketRegistry Packets { get; private set; }

		public NetContext() {
			Services = new NetServiceCollection();
			Serializer = new DynamicSerializer();
			Packets = new PacketRegistry();
			IsLocked = false;
		}

		public void LockContext(INetServerController server = null, INetClientController client = null) {
			if (IsLocked == true)
				throw new Exception("NetContext can only be initialized once.");

			Services.LockServices();
			IsLocked = true;

			var isServer = server != null;
			var isClient = client != null;
			foreach (var svc in Services.ServiceList) {
				svc.IsClient = isClient;
				svc.IsServer = isServer;

				svc.Server = server;
				svc.Client = client;
			}

			Services.InitializeServices(this);
			Packets.LockPackets();
		}

		public NetContext Clone()
		{
			return null;
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
