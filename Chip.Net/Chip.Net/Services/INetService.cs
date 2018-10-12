using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services {
	public interface INetService : IDisposable {
		PacketRouter Router { get; }

		void InitializeService(NetContext context);
		void StartService();
		void StopService();
		void UpdateService();

		IEnumerable<Packet> GetOutgoingClientPackets();
		IEnumerable<Packet> GetOutgoingServerPackets();
	}
}