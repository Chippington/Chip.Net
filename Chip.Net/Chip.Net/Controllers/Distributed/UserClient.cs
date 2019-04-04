using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed {
	public class UserClient : INetClientController {
		public NetEvent OnConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public NetContext Context => throw new NotImplementedException();

		public bool IsConnected => throw new NotImplementedException();

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

		public void InitializeClient(NetContext context) {
			throw new NotImplementedException();
		}

		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void StartClient(INetClientProvider provider) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopClient() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateClient() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}
	}
}