﻿using Chip.Net.Data;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
    public interface INetClient : IDisposable
    {
		NetEvent OnConnected { get; set; }
		NetEvent OnDisconnected { get; set; }
		NetEvent OnPacketReceived { get; set; }
		NetEvent OnPacketSent { get; set; }

		void StartClient(NetContext context, INetClientProvider provider);
		void StopClient();
		void UpdateClient();


		void SendPacket(Packet packet);
		void SendPacket(NetUser user, Packet packet);
    }
}
