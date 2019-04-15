using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Controllers;
using Chip.Net.Data;

namespace Chip.Net.Services
{
	public class NetService : INetService {
		public PacketRouter Router { get; private set; }

		public bool IsServer { get; set; } = false;
		public bool IsClient { get; set; } = false;

		public INetServerController Server { get; set; }
		public INetClientController Client { get; set; }

		protected NetContext Context { get; private set; }

		private object LockObject = new object();
		private Queue<Packet> outQueue;
		private List<Tuple<DateTime, Action>> scheduledEvents;

		public virtual void InitializeService(NetContext context) {
			Context = context;
			Router = new PacketRouter();
			outQueue = new Queue<Packet>();
			scheduledEvents = new List<Tuple<DateTime, Action>>();
		}

		public virtual void StartService() {
			outQueue.Clear();
			scheduledEvents.Clear();
		}

		public virtual void UpdateService() {
			for(int i = scheduledEvents.Count - 1; i >= 0; i--) {
				var time = scheduledEvents[i].Item1;
				var ev = scheduledEvents[i].Item2;

				if(DateTime.Now > time) {
					ev.Invoke();
					scheduledEvents.RemoveAt(i);
				}
			}
		}

		public virtual void StopService() { }

		public void SendPacketToClients(Packet packet) {
			if (packet == null)
				throw new Exception("Packet is null");

			packet.Recipient = null;

			lock(LockObject)
				outQueue.Enqueue(packet);
		}

		public void SendPacketToClient(NetUser user, Packet packet) {
			if (packet == null)
				throw new Exception("Packet is null");

			packet.Recipient = user;

			lock (LockObject)
				outQueue.Enqueue(packet);
		}

		public void SendPacketToServer(Packet packet) {
			if (packet == null)
				throw new Exception("Packet is null");

			lock(LockObject)
				outQueue.Enqueue(packet);
		}

		public void SendPacket(Packet packet) {
			if (packet == null)
				throw new Exception("Packet is null");

			lock (LockObject) {
				outQueue.Enqueue(packet);
			}
		}

		public void SendPacket(NetUser user, Packet packet) {
			if (packet == null)
				throw new Exception("Packet is null");

			packet.Recipient = user;
			lock (LockObject) {
				outQueue.Enqueue(packet);
			}
		}

		public void Dispose() {
			if (outQueue != null) outQueue.Clear();
		}

		public void ScheduleEvent(TimeSpan time, Action action) {
			var endt = DateTime.Now.Add(time);
			scheduledEvents.Add(new Tuple<DateTime, Action>(endt, action));
		}

		public void ScheduleEvent(int milliseconds, Action action) {
			var endt = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, milliseconds));
			scheduledEvents.Add(new Tuple<DateTime, Action>(endt, action));
		}

		public Packet GetNextOutgoingPacket() {
			if (outQueue.Count == 0)
				return null;

			return outQueue.Dequeue();
		}
	}
}
