using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers {
	public interface INetClientProvider : IDisposable {
		EventHandler<ProviderEventArgs> UserConnected { get; set; }
		EventHandler<ProviderEventArgs> UserDisconnected { get; set; }

		bool IsConnected { get; }

		void Connect(NetContext context);
		void Disconnect();
		void UpdateClient();
		IEnumerable<DataBuffer> GetIncomingMessages();
		void SendMessage(DataBuffer data);
	}
}