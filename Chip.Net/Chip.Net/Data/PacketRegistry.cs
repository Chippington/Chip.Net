using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Data
{
    public class PacketRegistry
    {
		private List<Type> tempTypeList;
		private Dictionary<Type, int> typeToId;
		private Dictionary<int, Type> idToType;

		public bool Locked { get; private set; }

		public PacketRegistry() {
			Locked = false;
			tempTypeList = new List<Type>();
		}

		public void Register<T>() where T : Packet {
			if (Locked)
				throw new Exception("Packet registry has been locked");

			if(tempTypeList.Contains(typeof(T)) == false)
				tempTypeList.Add(typeof(T));
		}

		public void LockPackets() {
			if (Locked)
				throw new Exception("Packet registry can only be locked once");

			Locked = true;

			typeToId = new Dictionary<Type, int>();
			idToType = new Dictionary<int, Type>();

			tempTypeList = tempTypeList.OrderBy(i => i.GetType().Name).ToList();
			for(int i = 0; i < tempTypeList.Count; i++) {
				typeToId.Add(tempTypeList[i], i);
				idToType.Add(i, tempTypeList[i]);
			}

			tempTypeList = null;
		}

		public int GetID<T>() where T : Packet {
			return typeToId[typeof(T)];
		}

		public int GetID(Type type) {
			return typeToId[type];
		}

		public Type GetType(int id) {
			return idToType[id];
		}

		public Packet CreateFromId(int id) {
			return (Packet)Activator.CreateInstance(GetType(id));
		}
    }
}
