using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers {
	public interface INetServerProvider : IDisposable {
		ProviderEvent OnUserConnected { get; set; }
		ProviderEvent OnUserDisconnected { get; set; }

		bool IsActive { get; }
		bool AcceptIncomingConnections { get; set; }

		void StartServer(NetContext context);
		void StopServer();

		void UpdateServer();

		IEnumerable<User> GetClients();
		void DisconnectUser(User user);

		IEnumerable<DataBuffer> GetIncomingMessages();

		void SendMessage(Packet packet);
		void SendMessage(User recipient, Packet packet);
	}
}
