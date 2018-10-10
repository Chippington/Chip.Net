using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers {
	public interface INetClientProvider {
		ProviderEvent OnConnected { get; set; }
		ProviderEvent OnDisconnected { get; set; }

		bool IsConnected { get; }

		void StartClient(NetContext context);
		void StopClient();

		void Disconnect(User user);

		IEnumerable<DataBuffer> GetIncomingMessages();
		void SendMessage(Packet packet);
	}
}