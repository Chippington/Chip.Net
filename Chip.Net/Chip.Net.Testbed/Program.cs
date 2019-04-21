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

namespace Chip.Net.Testbed {
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

		static void Main(string[] args) {
			TestRouter r1 = new TestRouter();
			TestRouter r2 = new TestRouter();

			var r1_intnode1 = r1.CreateNode<int>();
			var r1_intnode2 = r1.CreateNode<float>();
			var r1_intnode3 = r1.CreateNode<double>();
			var r1_intnode4 = r1.CreateNode<string>();
			var r1_intnode5 = r1.CreateNode<long>();

			var r2_intnode3 = r2.CreateNode<double>();
			var r2_intnode2 = r2.CreateNode<float>();
			var r2_intnode4 = r2.CreateNode<string>();
			var r2_intnode1 = r2.CreateNode<int>();
			var r2_intnode5 = r2.CreateNode<long>();

			r1_intnode1.Send += (s, e) => {
				r2.FromIndex(r1.ToIndex(r1_intnode1)).Handle(e);
			};

			r1_intnode2.Send += (s, e) => {
				r2.FromIndex(r1.ToIndex(r1_intnode2)).Handle(e);
			};

			r1_intnode3.Send += (s, e) => {
				r2.FromIndex(r1.ToIndex(r1_intnode3)).Handle(e);
			};

			r2_intnode3.Receive += (s, e) => {
				Console.WriteLine("Recv");
			};

			r1_intnode3.Send(new object(), 5d);
		}
	}
}