using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers {
	public interface INetServerProvider : IDisposable {
		EventHandler<ProviderEventArgs> OnUserConnected { get; set; }
		EventHandler<ProviderEventArgs> OnUserDisconnected { get; set; }

		bool IsActive { get; }
		bool AcceptIncomingConnections { get; set; }

		void StartServer(NetContext context);
		void StopServer();

		void UpdateServer();

		IEnumerable<object> GetClientKeys();
		void DisconnectUser(object userKey);

		IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages();

		void SendMessage(DataBuffer data, object excludeKey = null);
		void SendMessage(object recipientKey, DataBuffer data);
	}
}
