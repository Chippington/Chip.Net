using Chip.Net.Controllers;
using Chip.Net.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Data {
	public class MessageChannel {
		protected PacketRouter Parent;

		public Type SourceType { get; private set; }
		public string Key { get; private set; }
		public short Order { get; set; }

		public MessageChannel(PacketRouter parent, string key, Type type) {
			this.SourceType = type;
			this.Parent = parent;
			this.Key = key;
		}

		public virtual void Handle(IncomingMessage message) { }
	}

	public class MessageChannel<T> : MessageChannel where T : Packet {
		public delegate void MessageEvent(IncomingMessage<T> incoming);
		public MessageEvent Receive { get; set; }

		public MessageChannel(PacketRouter parent, string key)
			: base(parent, key, typeof(T)) { }

		public override void Handle(IncomingMessage message) {
			Receive?.Invoke(message.AsGeneric<T>());
		}

		public void Send(T data) {
			this.Send(new OutgoingMessage<T>(data));
		}

		public void Send(T data, NetUser recipient) {
			this.Send(new OutgoingMessage<T>(data, recipient));
		}

		public void Send(T data, IEnumerable<NetUser> recipients) {
			this.Send(new OutgoingMessage<T>(data, recipients));
		}

		public void Send(OutgoingMessage<T> message) {
			Parent.Send(this, message);
		}
	}

	public class RouterPacket {
		public byte[] Bytes { get; set; }
	}

	public class PacketRouter {
		private List<MessageChannel> Nodes;
		private HashSet<string> Types;

		private Action<NetUser, DataBuffer> SendAction;
		private Func<object, NetUser> KeyToUserFunc;

		private PacketRouter Parent;
		private string Prefix;

		public PacketRouter(PacketRouter Parent, string prefix) {
			this.Parent = Parent;
			this.Prefix = prefix;
		}

		public PacketRouter(INetServerProvider provider, INetServerController controller) {
			Nodes = new List<MessageChannel>();
			Types = new HashSet<string>();

			var prov = provider;
			var cont = controller;

			provider.DataReceived += OnDataReceived;
			SendAction = (u, d) => {
				if (u == null) {
					foreach (var user in cont.GetUsers())
						prov.SendMessage(user.UserKey, d);

					return;
				}

				prov.SendMessage(u.UserKey, d);
			};

			KeyToUserFunc = (k) => {
				return controller.GetUsers().First(i => i.UserKey == k);
			};
		}

		public PacketRouter(INetClientProvider provider, INetClientController controller) {
			Nodes = new List<MessageChannel>();
			Types = new HashSet<string>();

			provider.DataReceived += OnDataReceived;
			SendAction = (u, d) => provider.SendMessage(d);
			KeyToUserFunc = (k) => null;
		}

		private void OnDataReceived(object s, ProviderDataEventArgs e) {
			var buffer = e.Data;
			var index = buffer.ReadInt16();

			var channel = Resolve(index);
			var packet = (Packet)Activator.CreateInstance(channel.SourceType);

			packet.ReadFrom(buffer);
			NetUser sender = null;
			if (e.UserKey != null)
				sender = KeyToUserFunc(e.UserKey);

			channel.Handle(new IncomingMessage() {
				Data = packet,
				Sender = sender,
			});
		}

		public virtual MessageChannel<T> Route<T>(string key = null, bool usePrefix = true) where T : Packet {
			if (key == null)
				key = "";

            if (usePrefix)
                key = typeof(T).ToString() + "_" + key;

            if (Parent != null) {
				return Parent.Route<T>(Prefix + key);
			}

			var n = new MessageChannel<T>(this, key);
			if (Types.Contains(n.Key))
				throw new Exception();

			Types.Add(n.Key);
			Nodes.Add(n);
			Nodes = Nodes.OrderBy(i => i.Key).ToList();
			for (short i = 0; i < Nodes.Count; i++)
				Nodes[i].Order = i;

			return n;
		}

		public virtual short ToIndex(MessageChannel channel) {
			if (Parent != null)
				return Parent.ToIndex(channel);

			return channel.Order;
		}

		public virtual MessageChannel Resolve(short index) {
			if (Parent != null)
				return Parent.Resolve(index);

			return Nodes[index];
		}

		public void Send(MessageChannel source, OutgoingMessage message) {
			if(Parent != null) {
				Parent.Send(source, message);
				return;
			}

			var index = ToIndex(source);
			DataBuffer buffer = new DataBuffer();
			buffer.Write((short)index);
			message.Data.WriteTo(buffer);

			if(message.Recipients != null && message.Recipients.Count() > 0) {
				foreach (var u in message.Recipients)
					SendAction.Invoke(u, buffer);
			} else {
				SendAction(null, buffer);
			}
		}
	}
}