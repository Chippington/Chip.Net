using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data {
	internal class Callback {
		private Type type;
		private Action<Packet> callback;
		public Callback(Type type, Action<Packet> callback) {
			this.callback = callback;
			this.type = type;
		}

		public virtual void Invoke(Packet packet) {
			callback.Invoke(packet);
		}
	}

	internal class Callback<T> : Callback where T : Packet {
		private Action<T> callback;
		public Callback(Action<T> callback) : base(typeof(T), i => callback((T)i)) {
			this.callback = callback;
		}

		public override void Invoke(Packet packet) {
			base.Invoke(packet);
		}
	}

	public class PacketRouter {
		public bool UseParentTypes { get; set; } = false;

		private Dictionary<Type, List<Callback>> clientRouteMap;
		private Dictionary<Type, List<Callback>> serverRouteMap;

		public PacketRouter() {
			clientRouteMap = new Dictionary<Type, List<Callback>>();
			serverRouteMap = new Dictionary<Type, List<Callback>>();
		}

		public void Route<T>(Action<T> callback) where T : Packet {
			GetList(clientRouteMap, typeof(T)).Add(new Callback<T>(callback));
			GetList(serverRouteMap, typeof(T)).Add(new Callback<T>(callback));
		}

		public void RouteClient<T>(Action<T> callback) where T : Packet {
			GetList(clientRouteMap, typeof(T)).Add(new Callback<T>(callback));
		}

		public void RouteServer<T>(Action<T> callback) where T : Packet {
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
					cl[i].Invoke(packet);
			}
		}

		public void InvokeServer(Packet packet) {
			var type = packet.GetType();
			var sv = GetList(serverRouteMap, packet.GetType(), false);

			while ((type = type.BaseType) != null && typeof(Packet).IsAssignableFrom(type) && sv == null) {
				sv = GetList(serverRouteMap, type, false);
				type = type.BaseType;
			}

			if (sv != null) {
				for (int i = 0; i < sv.Count; i++)
					sv[i].Invoke(packet);
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