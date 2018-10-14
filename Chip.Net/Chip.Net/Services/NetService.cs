using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Services
{
	public class NetService : INetService {
		public PacketRouter Router { get; private set; }

		private Queue<Packet> clientOutQueue;
		private Queue<Packet> serverOutQueue;

		public void InitializeService(NetContext context) {
			Router = new PacketRouter();
			clientOutQueue = new Queue<Packet>();
			serverOutQueue = new Queue<Packet>();
		}

		public virtual void StartService() {
			clientOutQueue.Clear();
			serverOutQueue.Clear();
		}

		public virtual void UpdateService() { }

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

		public void Dispose() {
			if (clientOutQueue != null) clientOutQueue.Clear();
			if (serverOutQueue != null) serverOutQueue.Clear();
		}
	}
}
