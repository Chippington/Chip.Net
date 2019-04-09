using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed
{
	public class RouterServer : INetServerController {
		public NetEvent OnUserConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnUserDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public EventHandler<RouterModel> RouterConfiguredEvent { get; set; }
		public EventHandler<UserModel> UserConnectedEvent { get; set; }
		public EventHandler<UserModel> UserDisconnectedEvent { get; set; }
		public EventHandler<ShardModel> ShardConnectedEvent { get; set; }
		public EventHandler<ShardModel> ShardDisconnectedEvent { get; set; }

		public NetContext Context => throw new NotImplementedException();

		public bool IsActive => throw new NotImplementedException();

		public PacketRouter Router => throw new NotImplementedException();

		public bool IsServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Dispose() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			throw new NotImplementedException();
		}

		public IEnumerable<NetUser> GetUsers() {
			throw new NotImplementedException();
		}

		public void InitializeServer(NetContext context) {
			throw new NotImplementedException();
		}

		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void SendPacket(NetUser user, Packet packet) {
			throw new NotImplementedException();
		}

		public void StartServer(INetServerProvider provider) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopServer() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateServer() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}

		public void SendToShard(ShardModel Shard, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, ShardModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, IEnumerable<ShardModel> Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUser(UserModel User, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, UserModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, IEnumerable<UserModel> Exclude) {
			throw new NotImplementedException();
		}
	}
}
