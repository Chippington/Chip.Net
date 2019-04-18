using Chip.Net.Controllers;
using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests {
	public class TestNetService : NetService {
		public bool Started { get; private set; } = false;
		public bool Updated { get; private set; } = false;
		public bool Initialized { get; private set; } = false;
		public bool Stopped { get; private set; } = false;
		public bool Disposed { get; private set; } = false;
		public bool Received { get; private set; } = false;
		public string ReceivedData { get; private set; } = null;

		public bool Configured { get; set; }

		private Queue<Packet> outQueue;

		public override void Dispose() {
			base.Dispose();
			Disposed = true;
		}

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);
			outQueue = new Queue<Packet>();
			Initialized = true;

			context.Packets.Register<TestPacket>();
			if(Router != null)
				Router.Route<TestPacket>(i => {
					Received = true;
					ReceivedData = i.Data.data;
				});
		}

		public override void StartService() {
			base.StartService();
			outQueue.Clear();
			Started = true;
		}

		public override void StopService() {
			base.StopService();
			Stopped = true;
		}

		public override void UpdateService() {
			base.UpdateService();
			Updated = true;
		}

		public void Send(TestPacket p, NetUser recipient = null) {
			if (recipient != null) Router.QueueOutgoing(new OutgoingMessage(p, recipient));
			else Router.QueueOutgoing(new OutgoingMessage(p));
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
