using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.Direct
{
	public class DirectClientProvider : INetClientProvider
	{
		public ProviderEvent OnConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ProviderEvent OnDisconnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool IsConnected => throw new NotImplementedException();

		public void Connect(NetContext context)
		{
			throw new NotImplementedException();
		}

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages()
		{
			throw new NotImplementedException();
		}

		public void SendMessage(DataBuffer data)
		{
			throw new NotImplementedException();
		}

		public void UpdateClient()
		{
			throw new NotImplementedException();
		}
	}
}
