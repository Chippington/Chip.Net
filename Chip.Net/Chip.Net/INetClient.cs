using Chip.Net.Data;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net
{
	public class NetClientEventArgs
	{

	}


    public interface INetClient : IDisposable
    {
		void StartClient(NetContext context, INetClientProvider provider);
		void StopClient();
		void UpdateClient();

		void SendPacket(Packet packet);
    }
}
