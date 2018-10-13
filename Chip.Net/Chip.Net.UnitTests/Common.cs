using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests {
	public class TestNetService : INetService {
		public PacketRouter Router => throw new NotImplementedException();

		public NetContext Context { get; private set; }

		public bool Started { get; private set; } = false;
		public bool Updated { get; private set; } = false;
		public bool Initialized { get; private set; } = false;
		public bool Stopped { get; private set; } = false;
		public bool Disposed { get; private set; } = false;

		public void Dispose() {
			Disposed = true;
		}

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			return null;
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			return null;
		}

		public void InitializeService(NetContext context) {
			this.Context = context;
			Initialized = true;
		}

		public void StartService() {
			Started = true;
		}

		public void StopService() {
			Stopped = true;
		}

		public void UpdateService() {
			Updated = true;
		}
	}

	public static class Common {
		private static int port = 11111;
		public static int Port { get { return port++; } }
	}
}
