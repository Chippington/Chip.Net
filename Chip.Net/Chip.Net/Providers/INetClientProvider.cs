using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers {
	public interface INetClientProvider : IDisposable {
		EventHandler<ProviderEventArgs> OnConnected { get; set; }
		EventHandler<ProviderEventArgs> OnDisconnected { get; set; }

		bool IsConnected { get; }

		void Connect(NetContext context);
		void Disconnect();
		void UpdateClient();
		IEnumerable<DataBuffer> GetIncomingMessages();
		void SendMessage(DataBuffer data);
	}
}