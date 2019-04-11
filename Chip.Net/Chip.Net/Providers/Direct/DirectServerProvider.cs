using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.Direct
{
	public class DirectServerProvider : INetServerProvider
	{
		public static ConcurrentDictionary<string, DirectServerProvider> Servers { get; private set; } 
			= new ConcurrentDictionary<string, DirectServerProvider>();

		public static DirectServerProvider GetServer(string ip, int port) {
			DirectServerProvider server = null;
			Servers.TryGetValue(ip + ":" + port, out server);

			return server;
		}

		public List<DirectClientProvider> Clients { get; private set; }
		private Queue<Tuple<object, DataBuffer>> Incoming { get; set; }

		public ProviderEvent OnUserConnected { get; set; }
		public ProviderEvent OnUserDisconnected { get; set; }

		public bool IsActive { get; private set; }

		public bool AcceptIncomingConnections { get; set; }

		public void StartServer(NetContext context)
		{
			var key = context.IPAddress + ":" + context.Port;
			if (Servers.TryAdd(key, this) == false)
				throw new Exception("Server already exists with that IP:Port endpoint");

			Incoming = new Queue<Tuple<object, DataBuffer>>();
			Clients = new List<DirectClientProvider>();
			AcceptIncomingConnections = true;
			IsActive = true;
		}

		public void TryConnectClient(DirectClientProvider client) {
			if(AcceptIncomingConnections) {
				Clients.Add(client);
				client.AcceptConnection(this);

				OnUserConnected?.Invoke(new ProviderEventArgs() {
					UserKey = client,
				});
			} else {
				client.RejectConnection(this);
			}
		}

		public void DisconnectClient(DirectClientProvider client) {
			Clients.Remove(client);
			OnUserDisconnected?.Invoke(new ProviderEventArgs() {
				UserKey = client,
			});
		}

		public void ReceiveMessage(DirectClientProvider client, DataBuffer data) {
			DataBuffer d = new DataBuffer(data.ToBytes());
			Incoming.Enqueue(new Tuple<object, DataBuffer>(client, d));
		}

		public IEnumerable<object> GetClientKeys()
		{
			return Clients;
		}

		public IEnumerable<Tuple<object, DataBuffer>> GetIncomingMessages()
		{
			var inc = Incoming;
			Incoming = new Queue<Tuple<object, DataBuffer>>();
			return inc;
		}

		public void SendMessage(DataBuffer data, object excludeKey = null)
		{
			foreach (var client in Clients)
				if (client != excludeKey)
					client.ReceiveMessage(data);
		}

		public void SendMessage(object recipientKey, DataBuffer data)
		{
			(recipientKey as DirectClientProvider).ReceiveMessage(data);
		}

		public void UpdateServer()
		{
		}

		public void StopServer()
		{
			IsActive = false;

			if (Clients != null) {
				foreach (var client in Clients)
					client.DisconnectFrom(this);

				Clients.Clear();
			}

			if (Incoming != null)
				Incoming.Clear();

			Clients = null;
			Incoming = null;
		}

		public void DisconnectUser(object userKey)
		{
			Clients.Remove(userKey as DirectClientProvider);
			(userKey as DirectClientProvider).DisconnectFrom(this);

			OnUserDisconnected?.Invoke(new ProviderEventArgs() {
				UserKey = userKey,
			});
		}

		public void Dispose()
		{
			if (IsActive)
				StopServer();
		}
	}
}
