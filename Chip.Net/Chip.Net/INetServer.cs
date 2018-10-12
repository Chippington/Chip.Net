using Chip.Net.Data;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
	public interface INetServer
	{
		void StartServer(NetContext context, INetServerProvider provider);
		void StopServer();
		void UpdateServer();

		void SendPacket(Packet packet);
		void SendPacket();
	}
}
