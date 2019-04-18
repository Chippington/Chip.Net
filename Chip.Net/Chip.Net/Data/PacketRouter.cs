using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Data {
	internal class Callback {
		private Type type;
		private Action<IncomingMessage> callback;
		public Callback(Type type, Action<IncomingMessage> callback) {
			this.callback = callback;
			this.type = type;
		}

		public virtual void Invoke(Packet packet, NetUser sender) {
			IncomingMessage msg = new IncomingMessage();
			msg.Data = packet;
			msg.Sender = sender;
			callback.Invoke(msg);
		}
	}

	internal class Callback<T> : Callback where T : Packet {
		private Action<IncomingMessage<T>> callback;
		public Callback(Action<IncomingMessage<T>> callback) : base(typeof(T), i => callback(new IncomingMessage<T>() { Data = (T)i.Data, Sender = i.Sender })) {
			this.callback = callback;
		}

		public override void Invoke(Packet packet, NetUser sender) {
			base.Invoke(packet, sender);
		}
	}

	public class PacketRouter {
		public PacketRouter Parent { get; private set; }
		public PacketRouter Root {
			get {
				var cur = this;
				while (cur.Parent != null) cur = cur.Parent;
				return cur;
			}
		}

		private PacketRouter[] routers;
		private string orderKey;
		private int routerId;

		private Dictionary<Type, List<Callback>> clientRouteMap;
		private Dictionary<Type, List<Callback>> serverRouteMap;

		public PacketRouter(PacketRouter parent, string orderKey) {
			this.Parent = parent;
			clientRouteMap = new Dictionary<Type, List<Callback>>();
			serverRouteMap = new Dictionary<Type, List<Callback>>();

			if (Root.routers == null)
				Root.routers = new PacketRouter[0];

			this.orderKey = orderKey;
			var rs = Root.routers.ToList();
			rs.Add(this);
			Root.routers = rs.OrderBy(i => i.orderKey).ToArray();
			Root.AssignIds();
		}

		private void AssignIds() {
			for (int i = 0; i < routers.Length; i++)
				routers[i].routerId = i;
		}

		public void WriteHeader(DataBuffer buffer, PacketRouter source) {
			buffer.Write((byte)source.routerId);
		}

		public PacketRouter ReadHeader(DataBuffer buffer) {
			return routers[buffer.ReadByte()];
		}

		public void Route<T>(Action<IncomingMessage<T>> callback) where T : Packet {
			GetList(clientRouteMap, typeof(T)).Add(new Callback<T>(callback));
			GetList(serverRouteMap, typeof(T)).Add(new Callback<T>(callback));
		}

		public void RouteClient<T>(Action<IncomingMessage<T>> callback) where T : Packet {
			GetList(clientRouteMap, typeof(T)).Add(new Callback<T>(callback));
		}

		public void RouteServer<T>(Action<IncomingMessage<T>> callback) where T : Packet {
			GetList(serverRouteMap, typeof(T)).Add(new Callback<T>(callback));
		}

		public void InvokeClient(Packet packet) {
			var type = packet.GetType();
			var cl = GetList(clientRouteMap, packet.GetType(), false);

			while ((type = type.BaseType) != null && typeof(Packet).IsAssignableFrom(type) && cl == null) {
				cl = GetList(clientRouteMap, type, false);
			}

			if (cl != null) {
				for (int i = 0; i < cl.Count; i++)
					cl[i].Invoke(packet, null);
			}
		}

		public void InvokeServer(Packet packet, NetUser sender) {
			var type = packet.GetType();
			var sv = GetList(serverRouteMap, packet.GetType(), false);

			while ((type = type.BaseType) != null && typeof(Packet).IsAssignableFrom(type) && sv == null) {
				sv = GetList(serverRouteMap, type, false);
				type = type.BaseType;
			}

			if (sv != null) {
				for (int i = 0; i < sv.Count; i++)
					sv[i].Invoke(packet, sender);
			}
		}

		private List<Callback> GetList(Dictionary<Type, List<Callback>> map, Type type, bool create = true) {
			if (map.ContainsKey(type) == false) {
				if (create == false)
					return null;

				map.Add(type, new List<Callback>());
			}

			return map[type];
		}
	}
}