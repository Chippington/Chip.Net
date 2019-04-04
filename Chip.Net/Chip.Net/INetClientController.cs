﻿using Chip.Net.Data;
using Chip.Net.Providers;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
    public interface INetClientController : INetService, IDisposable {
		NetEvent OnConnected { get; set; }
		NetEvent OnDisconnected { get; set; }
		NetEvent OnPacketReceived { get; set; }
		NetEvent OnPacketSent { get; set; }

		NetContext Context { get; }
		bool IsConnected { get; }

		void InitializeClient(NetContext context);
		void StartClient(INetClientProvider provider);
		void StopClient();
		void UpdateClient();


		void SendPacket(Packet packet);
    }
}