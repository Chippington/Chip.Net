using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.Direct
{
	public class DirectClientProvider : INetClientProvider
	{
		public EventHandler<ProviderEventArgs> OnConnected { get; set; }
		public EventHandler<ProviderEventArgs> OnDisconnected { get; set; }

		private Queue<DataBuffer> Incoming { get; set; }

		public bool IsConnected { get; private set; }
		public DirectServerProvider ActiveServer { get; set; }

		public void AcceptConnection(DirectServerProvider Server) {
			IsConnected = true;
			OnConnected?.Invoke(this, new ProviderEventArgs());
			ActiveServer = Server;
		}

		public void RejectConnection(DirectServerProvider Server) {
			IsConnected = false;
			throw new Exception("Server not accepting new connections");
		}

		public void DisconnectFrom(DirectServerProvider Server) {
			IsConnected = false;
			OnDisconnected?.Invoke(this, new ProviderEventArgs());
		}

		public void ReceiveMessage(DataBuffer buffer) {
			DataBuffer b = new DataBuffer(buffer.ToBytes());
			Incoming.Enqueue(b);
		}

		public void Connect(NetContext context)
		{
			Incoming = new Queue<DataBuffer>();

			var server = DirectServerProvider.GetServer(context.IPAddress, context.Port);
			if (server == null) {
				long tick = Environment.TickCount;
				while (Environment.TickCount < tick + 50 && server == null) {
					server = DirectServerProvider.GetServer(context.IPAddress, context.Port);
					System.Threading.Thread.Sleep(1);
				}

				if(server == null)
					throw new Exception("Could not connect to server");
			}
			server.TryConnectClient(this);
		}

		public void Disconnect()
		{
			if(IsConnected) {
				IsConnected = false;
				ActiveServer.DisconnectClient(this);
				OnDisconnected?.Invoke(this, new ProviderEventArgs());

				Incoming.Clear();
				Incoming = null;
			}
		}

		public void Dispose()
		{
			if (IsConnected)
				Disconnect();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages()
		{
			if (Incoming == null)
				return new DataBuffer[0];

			var inc = Incoming;
			Incoming = new Queue<DataBuffer>();
			return inc;
		}

		public void SendMessage(DataBuffer data)
		{
			ActiveServer.ReceiveMessage(this, data);
		}

		public void UpdateClient()
		{
		}
	}
}
