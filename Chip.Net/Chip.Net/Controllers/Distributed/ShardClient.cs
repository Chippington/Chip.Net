using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed
{
	public class ShardClient<TRouter, TShard, TUser> : INetClientController
		where TRouter : IRouterModel
		where TShard : IShardModel
		where TUser : IUserModel {
		public PacketRouter Router => throw new NotImplementedException();

		public EventHandler<NetEventArgs> OnConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public EventHandler<NetEventArgs> OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public NetContext Context => throw new NotImplementedException();

		public bool IsConnected => throw new NotImplementedException();

		public void Dispose() {
			throw new NotImplementedException();
		}

		public void InitializeClient(NetContext context, INetClientProvider provider) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void StartClient() {
			throw new NotImplementedException();
		}

		public void StopClient() {
			throw new NotImplementedException();
		}

		public void UpdateClient() {
			throw new NotImplementedException();
		}
	}
}
