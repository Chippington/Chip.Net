using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Packets;
using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services
{
	public class DistributedService<TRouter, TShard, TUser> : DistributedService
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {
		new public RouterServer<TRouter, TShard, TUser> RouterController { get; set; }
		new public ShardClient<TRouter, TShard, TUser> ShardController { get; set; }
		new public UserClient<TRouter, TShard, TUser> UserController { get; set; }
	}

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

		protected virtual void GlobalInitialize(NetContext context) { }

		protected virtual void InitializeDistributedService() { }

		protected virtual void InitializeContext(NetContext context) { }

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





		public class ShardChannel<T> where T : Packet {
			private MessageChannel<T> shardRawChannel;
			private MessageChannel<PassthroughPacket<T>> shardChannel;
			private MessageChannel<PassthroughPacket<T>> userChannel;

			public EventHandler<T> Receive { get; set; }

			public void Send(T data) {
				if (shardRawChannel != null) {
					if (userChannel != null) {
						//is router
						shardRawChannel.Send(new OutgoingMessage<T>(data));
					} else {
						//is shard
						shardRawChannel.Send(new OutgoingMessage<T>(data));
					}
				} else if (userChannel != null) {
					//is user
					userChannel.Send(new OutgoingMessage<PassthroughPacket<T>>(new PassthroughPacket<T>(data)));
				}
			}

			public void Send(T data, IShardModel recipient) {
				this.Send(data, recipient.Id);
			}

			private void Send(T data, int recipient) {
				if (shardRawChannel != null) {
					if (userChannel != null) {
						//is router
						shardRawChannel.Send(new OutgoingMessage<T>(data));
					} else {
						//is shard
						shardRawChannel.Send(new OutgoingMessage<T>(data));
					}
				} else if (userChannel != null) {
					//is user
					userChannel.Send(new OutgoingMessage<PassthroughPacket<T>>(new PassthroughPacket<T>(data)));
				}
			}

			public ShardChannel(MessageChannel<PassthroughPacket<T>> userChannel) {
				this.userChannel = userChannel;
				//is user, no receive
			}

			public ShardChannel(MessageChannel<T> shardRawChannel, MessageChannel<PassthroughPacket<T>> shardChannel) {
				this.shardRawChannel = shardRawChannel;
				this.shardChannel = shardChannel;
				//is shard

				this.shardRawChannel.Receive += (e) => {
					Receive?.Invoke(this, e.Data);
				};

				this.shardChannel.Receive += (e) => {
					Receive?.Invoke(this, e.Data.Data);
				};
			}

			public ShardChannel(Func<short, NetUser> netUserResolver, MessageChannel<T> shardRawChannel, MessageChannel<PassthroughPacket<T>> shardChannel, MessageChannel<PassthroughPacket<T>> userChannel) {
				this.shardRawChannel = shardRawChannel;
				this.shardChannel = shardChannel;
				this.userChannel = userChannel;

				//receive raw: resend to all
				this.shardRawChannel.Receive += (e) => {
					Send(e.Data);
				};

				this.shardChannel.Receive += (e) => {
					Send(e.Data.Data, e.Data.RecipientId);
				};

				this.userChannel.Receive += (e) => {
					Send(e.Data.Data, e.Data.RecipientId);
				};
			}
		}

		public ShardChannel<T> CreateShardChannel<T>(string key = null) where T : Packet {
			if(IsUser) {
				var userChannel = UserController.Router.Route<PassthroughPacket<T>>(key);
				return new ShardChannel<T>(
					userChannel);
			}

			if (IsShard) {
				var shardRawChannel = ShardController.Router.Route<T>(key);
				var shardChannel = ShardController.Router.Route<PassthroughPacket<T>>(key);

				return new ShardChannel<T>(
					shardRawChannel,
					shardChannel);
			}

			if(IsRouter) {
				var shardRawChannel = RouterController.ShardController.Router.Route<T>(key);
				var shardChannel = RouterController.ShardController.Router.Route<PassthroughPacket<T>>(key);
				var userChannel = RouterController.UserController.Router.Route<PassthroughPacket<T>>(key);

				return new ShardChannel<T>((id) => RouterController.GetNetUserFromShard(id),
					shardRawChannel,
					shardChannel,
					userChannel);
			}

			return null;
		}




		//public MessageChannel<T> CreateShardChannel<T>(string key = null) where T : Packet {
		//	if (IsShard) {
		//		return ShardController.Router.Route<T>(key);
		//	}

		//	if (IsRouter) {
		//		return RouterController.CreateShardChannel<T>(key);
		//	}

		//	return null;
		//}

		public MessageChannel<T> CreateUserChannel<T>(string key = null) where T : Packet {
			if (IsUser) {
				return UserController.Router.Route<T>(key);
			}

			if (IsRouter) {
				return RouterController.CreateUserChannel<T>(key);
			}

			return null;
		}

		public DistributedChannel<T> CreatePassthrough<T>(string key = null) where T : Packet {
			if(IsClient) {
				return new DistributedChannel<T>(Client.Router.Route<PassthroughPacket<T>>(key));
			}

			if(IsServer) {
				RouterController.CreatePassthrough<T>(key);
			}

			return null;
		}

		public virtual void InitializeRouter() {
			IsServer = true;
			IsRouter = true;
			IsClient = false;
			IsShard = false;
			IsUser = false;

			InitializeDistributedService();
		}

		public virtual void InitializeShard() {
			IsServer = false;
			IsRouter = false;
			IsClient = true;
			IsShard = true;
			IsUser = false;

			InitializeDistributedService();
		}

		public virtual void InitializeUser() {
			IsServer = false;
			IsRouter = false;
			IsClient = true;
			IsShard = false;
			IsUser = true;

			InitializeDistributedService();
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
