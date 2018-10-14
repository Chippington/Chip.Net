using Chip.Net.Data;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
	public interface INetServer : IDisposable
	{
		PacketRouter Router { get; }
		NetEvent OnUserConnected { get; set; }
		NetEvent OnUserDisconnected { get; set; }
		NetEvent OnPacketReceived { get; set; }
		NetEvent OnPacketSent { get; set; }

		NetContext Context { get; }
		bool IsActive { get; }

		void InitializeServer(NetContext context);
		void StartServer(INetServerProvider provider);
		void StopServer();
		void UpdateServer();

		IEnumerable<NetUser> GetUsers();

		void SendPacket(Packet packet);
		void SendPacket(NetUser user, Packet packet);
	}
}
