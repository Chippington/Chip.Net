using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Services
{
	public class NetService : INetService {
		public PacketRouter Router { get; private set; }

		public bool IsServer { get; set; } = false;
		public bool IsClient { get; set; } = false;

		protected NetContext Context { get; private set; }

		private Queue<Packet> clientOutQueue;
		private Queue<Packet> serverOutQueue;
		private List<Tuple<DateTime, Action>> scheduledEvents;

		public virtual void InitializeService(NetContext context) {
			Context = context;
			Router = new PacketRouter();
			clientOutQueue = new Queue<Packet>();
			serverOutQueue = new Queue<Packet>();
			scheduledEvents = new List<Tuple<DateTime, Action>>();
		}

		public virtual void StartService() {
			clientOutQueue.Clear();
			serverOutQueue.Clear();
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

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			var ret = clientOutQueue;
			clientOutQueue = new Queue<Packet>();
			return ret;
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			var ret = serverOutQueue;
			serverOutQueue = new Queue<Packet>();
			return ret;
		}

		public void SendPacketToClients(Packet packet) {
			packet.Recipient = null;
			clientOutQueue.Enqueue(packet);
		}

		public void SendPacketToClient(NetUser user, Packet packet) {
			packet.Recipient = user;
			clientOutQueue.Enqueue(packet);
		}

		public void SendPacketToServer(Packet packet) {
			serverOutQueue.Enqueue(packet);
		}

		public void SendPacket(Packet packet) {
			if (IsServer) serverOutQueue.Enqueue(packet);
			if (IsClient) clientOutQueue.Enqueue(packet);
		}

		public void SendPacket(NetUser user, Packet packet) {
			packet.Recipient = user;
			if (IsServer) serverOutQueue.Enqueue(packet);
			if (IsClient) clientOutQueue.Enqueue(packet);
		}

		public void Dispose() {
			if (clientOutQueue != null) clientOutQueue.Clear();
			if (serverOutQueue != null) serverOutQueue.Clear();
		}

		public void ScheduleEvent(TimeSpan time, Action action) {
			var endt = DateTime.Now.Add(time);
			scheduledEvents.Add(new Tuple<DateTime, Action>(endt, action));
		}
	}
}
