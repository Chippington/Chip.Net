using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Packets;
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

		public RouterServer RouterController { get; set; }
		public ShardClient ShardController { get; set; }
		public UserClient UserController { get; set; }

		public PacketRouter Router { get => throw new Exception("Use server/client router"); }
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
				GlobalInitialize(context);
				Initialized = true;
			}
		}

		public enum RouteType {
			RouterShard,
			RouterUser,
			Passthrough,
		}

		public MessageChannel<T> RouteRouterShard<T>(string key = null) where T : Packet {
			if (IsShard) {
				return ShardController.Router.Route<T>(key);
			}

			if (IsRouter) {
				return RouterController.RouteShard<T>(key);
			}

			return null;
		}

		public MessageChannel<T> RouteRouterUser<T>(string key = null) where T : Packet {
			if (IsUser) {
				return UserController.Router.Route<T>(key);
			}

			if (IsRouter) {
				return RouterController.RouteShard<T>(key);
			}

			return null;
		}

		public DistributedChannel<T> CreatePassthrough<T>(string key = null) where T : Packet {
			if(IsClient) {
				return new DistributedChannel<T>(Client.Router.Route<PassthroughPacket<T>>(key));
			}

			if(IsServer) {
				RouterController.PassthroughRoute<T>(key);
			}

			return null;
		}

		public virtual void InitializeRouter() {
			IsServer = true;
			IsRouter = true;
			IsClient = false;
			IsShard = false;
			IsUser = false;
		}

		public virtual void InitializeShard() {
			IsServer = false;
			IsRouter = false;
			IsClient = true;
			IsShard = true;
			IsUser = false;
		}

		public virtual void InitializeUser() {
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
