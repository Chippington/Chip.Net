using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.TCP {
	public class TCPClientProvider : INetClientProvider {
		public ProviderEvent OnConnected { get; set; }
		public ProviderEvent OnDisconnected { get; set; }

		public bool IsConnected { get; private set; }

		public void Disconnect() {
			throw new NotImplementedException();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages() {
			throw new NotImplementedException();
		}

		public void SendMessage(Packet packet) {
			throw new NotImplementedException();
		}

		public void StartClient(NetContext context) {
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
