using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
	public interface INetServerController : INetService, IDisposable {
		EventHandler<NetEventArgs> UserConnected { get; set; }
		EventHandler<NetEventArgs> UserDisconnected { get; set; }
		EventHandler<NetEventArgs> PacketReceived { get; set; }
		EventHandler<NetEventArgs> PacketSent { get; set; }

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
