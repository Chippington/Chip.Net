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
			NetContext ctx = new NetContext();
			ctx.Serializer = Serializer.Clone();
			ctx.Services = Services.Clone();
			ctx.Packets = Packets.Clone();

			ctx.ApplicationName = ApplicationName;
			ctx.MaxConnections = MaxConnections;
			ctx.IPAddress = IPAddress;
			ctx.Port = Port;
			ctx.IsLocked = IsLocked;
			return ctx;
		}

		public TServer CreateServer<TServer, TProvider>(Action<NetContext> config = null)
			where TServer : INetServerController
			where TProvider : INetServerProvider {

			TProvider provider = Activator.CreateInstance<TProvider>();
			return CreateServer<TServer>(provider, config);
		}

		public TClient CreateClient<TClient, TProvider>(Action<NetContext> config = null)
			where TClient : INetClientController
			where TProvider : INetClientProvider {

			TProvider provider = Activator.CreateInstance<TProvider>();
			return CreateClient<TClient>(provider, config);
		}

		public TServer CreateServer<TServer>(INetServerProvider provider, Action<NetContext> config = null)
			where TServer : INetServerController {

			var ctx = this.Clone();
			if (config != null)
				config.Invoke(ctx);

			TServer server = Activator.CreateInstance<TServer>();
			server.InitializeServer(ctx, provider);
			return server;
		}

		public TClient CreateClient<TClient>(INetClientProvider provider, Action<NetContext> config = null)
			where TClient : INetClientController {

			var ctx = this.Clone();
			if (config != null)
				config.Invoke(ctx);

			TClient client = Activator.CreateInstance<TClient>();
			client.InitializeClient(ctx, provider);
			return client;
		}
	}
}
