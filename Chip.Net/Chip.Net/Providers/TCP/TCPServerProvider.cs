using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.TCP
{
	public class TCPServerProvider : INetServerProvider {
		public ProviderEvent OnUserConnected { get; set; }
		public ProviderEvent OnUserDisconnected { get; set; }

		public bool IsActive { get; private set; }

		public bool AcceptIncomingConnections { get; set; }

		public void DisconnectUser(User user) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetClients() {
			throw new NotImplementedException();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages() {
			throw new NotImplementedException();
		}

		public void SendMessage(Packet packet) {
			throw new NotImplementedException();
		}

		public void SendMessage(User recipient, Packet packet) {
			throw new NotImplementedException();
		}

		public void StartServer(NetContext context) {
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
