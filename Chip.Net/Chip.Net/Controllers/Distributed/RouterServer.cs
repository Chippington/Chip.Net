﻿using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Providers;

namespace Chip.Net.Controllers.Distributed
{
	public class RouterServer : INetServerController {
		public NetEvent OnUserConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnUserDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public NetEvent OnPacketSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public NetContext Context => throw new NotImplementedException();

		public bool IsActive => throw new NotImplementedException();

		public PacketRouter Router => throw new NotImplementedException();

		public bool IsServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Dispose() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			throw new NotImplementedException();
		}

		public IEnumerable<NetUser> GetUsers() {
			throw new NotImplementedException();
		}

		public void InitializeServer(NetContext context) {
			throw new NotImplementedException();
		}

		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void SendPacket(Packet packet) {
			throw new NotImplementedException();
		}

		public void SendPacket(NetUser user, Packet packet) {
			throw new NotImplementedException();
		}

		public void StartServer(INetServerProvider provider) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopServer() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateServer() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}
	}
}
