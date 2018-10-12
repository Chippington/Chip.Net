using Chip.Net.Data;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
	public interface INetServer : IDisposable
	{
		NetEvent OnUserConnected { get; set; }
		NetEvent OnUserDisconnected { get; set; }
		NetEvent OnPacketReceived { get; set; }
		NetEvent OnPacketSent { get; set; }

		void StartServer(NetContext context, INetServerProvider provider);
		void StopServer();
		void UpdateServer();

		IEnumerable<NetUser> GetUsers();

		void SendPacket(Packet packet);
		void SendPacket();
	}
}
