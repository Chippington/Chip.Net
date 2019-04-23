using Chip.Net.Data;
using Chip.Net.Controllers.Basic;
using Chip.Net.Providers.Sockets;
using Chip.Net.Providers.TCP;
using Chip.Net.Services.NetTime;
using Chip.Net.Services.Ping;
using Chip.Net.Services.RFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chip.Net.Controllers;
using Chip.Net.Services;
using Chip.Net.Services.UserList;

namespace Chip.Net.Testbed {
	public class TestPacket : Packet {
		public int Data { get; set; }

		public TestPacket() {
			Data = 0;
		}

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			buffer.Write((int)Data);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Data = buffer.ReadInt32();
		}
	}

	class Program {
		public class TestRouter {
			private List<Node> Nodes;
			private HashSet<string> Types;

			public class Node {
				public string Key { get; private set; }
				public int Order { get; set; }

				public Node(string key) {
					this.Key = key;
				}

				public virtual void Handle(object obj) {

				}
			}

			public class Node<T> : Node {
				public EventHandler<T> Receive { get; set; }
				public EventHandler<T> Send { get; set; }

				public Node(string key) : base(typeof(T).ToString() + key) {

				}

				public override void Handle(object obj) {
					Receive.Invoke(this, (T)obj);
				}
			}

			public TestRouter() {
				Nodes = new List<Node>();
				Types = new HashSet<string>();
			}

			public Node<T> CreateNode<T>(string key = null) {
				if (key == null)
					key = "";

				var n = new Node<T>(key);
				if (Types.Contains(n.Key))
					throw new Exception();

				Types.Add(n.Key);
				Nodes.Add(n);
				Nodes = Nodes.OrderBy(i => i.Key).ToList();
				for (int i = 0; i < Nodes.Count; i++)
					Nodes[i].Order = i;

				return n;
			}

			public int ToIndex(Node node) {
				return node.Order;
			}

			public Node FromIndex(int index) {
				return Nodes[index];
			}
		}

		public class TestService : NetService {

		}

		static void Main(string[] args) {
			NetContext ctx = new NetContext();
			ctx.IPAddress = "localhost";
			ctx.Port = 11111;
			ctx.Services.Register<UserListService>();
			BasicServer server = new BasicServer();
			server.InitializeServer(ctx.Clone(), new TCPServerProvider());
			var ch1 = server.Router.Route<TestPacket>();
			server.StartServer();

			BasicClient client = new BasicClient();
			client.InitializeClient(ctx.Clone(), new TCPClientProvider());
			var ch2 = client.Router.Route<TestPacket>();

			ch1.Receive += (e) => {
				e.Data.Data++;
				ch1.Send(new OutgoingMessage(e.Data));
				Console.WriteLine(e.Data.Data);
			};

			ch2.Receive += (e) => {
				e.Data.Data++;
				ch2.Send(new OutgoingMessage(e.Data));
				Console.WriteLine(e.Data.Data);
			};

			client.OnConnected += (s, e) => {
				//ch2.Send(new OutgoingMessage(new TestPacket() {
				//	Data = 0,
				//}));
			};

			client.StartClient();

			while (true) {
				server.UpdateServer();
				client.UpdateClient();

				System.Threading.Thread.Sleep(10);
			}
		}
	}
}