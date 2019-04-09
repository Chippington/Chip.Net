using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.Direct
{
	public class DirectServerProvider : INetServerProvider
	{
		public ProviderEvent OnUserConnected { get; set; }
		public ProviderEvent OnUserDisconnected { get; set; }

		public bool IsActive { get; private set; }

		public bool AcceptIncomingConnections { get; set; }

		public void StartServer(NetContext context)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<object> GetClientKeys()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages()
		{
			throw new NotImplementedException();
		}

		public void SendMessage(DataBuffer data, object excludeKey = null)
		{
			throw new NotImplementedException();
		}

		public void SendMessage(object recipientKey, DataBuffer data)
		{
			throw new NotImplementedException();
		}

		public void UpdateServer()
		{
			throw new NotImplementedException();
		}

		public void StopServer()
		{
			throw new NotImplementedException();
		}

		public void DisconnectUser(object userKey)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
