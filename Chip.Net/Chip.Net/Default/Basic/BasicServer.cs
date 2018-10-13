using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Default.Basic
{
	public class BasicServer : INetServer {
		public PacketRouter Router => throw new NotImplementedException();

		public NetEvent OnUserConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnUserDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public NetContext Context => throw new NotImplementedException();

		public bool IsActive => throw new NotImplementedException();

		public void Dispose() {
			throw new NotImplementedException();
		}

		public IEnumerable<NetUser> GetUsers() {
			throw new NotImplementedException();
		}

		public void InitializeServer(NetContext context) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void SendPacket() {
			throw new NotImplementedException();
		}

		public void StartServer(INetServerProvider provider) {
			throw new NotImplementedException();
		}

		public void StopServer() {
			throw new NotImplementedException();
		}

		public void UpdateServer() {
			throw new NotImplementedException();
		}
	}
}
