using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services
{
	public class DistributedService : IDistributedService {
		public bool IsShard { get; set; }
		public bool IsUser { get; set; }
		public bool IsRouter { get; set; }

		public PacketRouter Router { get; private set; }
		public INetServerController Server { get; set; }
		public INetClientController Client { get; set; }

		public bool IsServer { get; set; }
		public bool IsClient { get; set; }

		public bool Initialized { get; private set; }
		public bool Disposed { get; private set; }

		public virtual void GlobalInitialize(NetContext context) { }

		public virtual void InitializeContext(NetContext context) { }

		public void InitializeService(NetContext context) {
			InitializeContext(context);
			if (Initialized == false) {
				Router = new PacketRouter(null, "");

				GlobalInitialize(context);
				Initialized = true;
			}
		}

		public virtual void InitializeRouter(IRouterModel Model) {
			IsServer = true;
			IsRouter = true;
			IsClient = false;
			IsShard = false;
			IsUser = false;
		}

		public virtual void InitializeShard(IShardModel Model) {
			IsServer = false;
			IsRouter = false;
			IsClient = true;
			IsShard = true;
			IsUser = false;
		}

		public virtual void InitializeUser(IUserModel Model) {
			IsServer = false;
			IsRouter = false;
			IsClient = true;
			IsShard = false;
			IsUser = true;
		}

		public virtual void StartService() {
		}

		public virtual void StopService() {
		}

		public virtual void UpdateService() {
		}

		public void Dispose() {
			Disposed = true;
		}
	}
}
