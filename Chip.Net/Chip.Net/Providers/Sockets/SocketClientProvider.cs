using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Chip.Net.Data;

namespace Chip.Net.Providers.Sockets
{
	public class SocketClientProvider : SocketProviderBase, INetClientProvider {
		public EventHandler<ProviderEventArgs> OnConnected { get; set; }
		public EventHandler<ProviderEventArgs> OnDisconnected { get; set; }

		public bool IsConnected { get; }

		private object LockObject = new object();
		private Socket Connection;
		private Queue<DataBuffer> Incoming;

		public void Connect(NetContext context) {
			IPHostEntry ipHostInfo = Dns.GetHostEntry(context.IPAddress);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, context.Port);

			Incoming = new Queue<DataBuffer>();
			Connection = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			//Connection.Connect(ipAddress, context.Port);
			Connection.BeginConnect(localEndPoint, OnConnect, Connection);
		}

		private void OnConnect(IAsyncResult ar) {
			var conn = ar.AsyncState as Socket;
			conn.EndConnect(ar);

			if (conn.Connected) {
				OnConnected?.Invoke(this, new ProviderEventArgs() { UserKey = conn });

				StateObject state = new StateObject();
				state.socket = conn;
				conn.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);
			}
		}

		private void OnReceive(IAsyncResult ar) {
			var state = ar.AsyncState as StateObject;
			var conn = state.socket;

			int bytesRead = conn.EndReceive(ar);
			conn.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, OnReceive, state);

			if (bytesRead > 0) {
				byte[] arr = new byte[bytesRead];
				Array.Copy(state.buffer, arr, bytesRead);

				DataBuffer buffer = new DataBuffer(arr);

				lock(LockObject)
					Incoming.Enqueue(buffer);
			}
		}

		public void Disconnect() {
			if(Connection != null) {
				Connection.Shutdown(SocketShutdown.Both);
				Connection.Close();
				Connection = null;

				OnDisconnected?.Invoke(this, new ProviderEventArgs());
			}
		}

		public void Dispose() {
			if (Connection != null)
				Disconnect();
		}

		public IEnumerable<DataBuffer> GetIncomingMessages() {
			lock(LockObject) {
				var t = new Queue<DataBuffer>(Incoming);
				Incoming = new Queue<DataBuffer>();
				return t;
			}
		}

		public void SendMessage(DataBuffer data) {
			var bytes = data.ToBytes();
			Connection.BeginSend(bytes, 0, bytes.Length, 0, OnSend, Connection);
		}

		private void OnSend(IAsyncResult ar) {
			var conn = ar.AsyncState as Socket;
			int sent = conn.EndSend(ar);
		}

		public void UpdateClient() {

		}
	}
}
