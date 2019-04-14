using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests {
	public class TestNetService : INetService {
		public PacketRouter Router { get; private set; }

		public NetContext Context { get; private set; }

		public bool Started { get; private set; } = false;
		public bool Updated { get; private set; } = false;
		public bool Initialized { get; private set; } = false;
		public bool Stopped { get; private set; } = false;
		public bool Disposed { get; private set; } = false;
		public bool Received { get; private set; } = false;
		public string ReceivedData { get; private set; } = null;
		public bool IsServer { get; set; }
		public bool IsClient { get; set; }

		private Queue<Packet> outQueue;

		public void Dispose() {
			Disposed = true;
		}

		public void InitializeService(NetContext context) {
			outQueue = new Queue<Packet>();
			Router = new PacketRouter();
			this.Context = context;
			Initialized = true;

			context.Packets.Register<TestPacket>();
			Router.Route<TestPacket>(i => {
				Received = true;
				ReceivedData = i.data;
			});
		}

		public void StartService() {
			outQueue.Clear();
			Started = true;
		}

		public void StopService() {
			Stopped = true;
		}

		public void UpdateService() {
			Updated = true;
		}

		public void Send(TestPacket p) {
			outQueue.Enqueue(p);
		}

		public Packet GetNextOutgoingPacket() {
			if (outQueue.Count == 0)
				return null;

			return outQueue.Dequeue();
		}
	}

	public class TestPacket : Packet {
		public string data { get; set; }

		public TestPacket() {
			data = "";
		}

		public override void WriteTo(DataBuffer buffer) {
			buffer.Write((string)data);
		}

		public override void ReadFrom(DataBuffer buffer) {
			data = buffer.ReadString();
		}
	}

	public static class Common {
		private static int port = 11111;
		public static int Port { get { return port++; } }
	}
}
