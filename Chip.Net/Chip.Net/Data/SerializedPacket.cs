using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chip.Net.Data {
	public class SerializedPacket : Packet {
		public class PacketSerializer {
			private Dictionary<Type, Action<object, DataBuffer>> WriteFunctions = new Dictionary<Type, Action<object, DataBuffer>>() {
				{ typeof(byte), (o, b) => b.Write((byte)o) },
				{ typeof(int), (o, b) => b.Write((int)o) },
				{ typeof(uint), (o, b) => b.Write((uint)o) },
				{ typeof(short), (o, b) => b.Write((short)o) },
				{ typeof(ushort), (o, b) => b.Write((ushort)o) },
				{ typeof(long), (o, b) => b.Write((long)o) },
				{ typeof(ulong), (o, b) => b.Write((ulong)o) },
				{ typeof(float), (o, b) => b.Write((float)o) },
				{ typeof(double), (o, b) => b.Write((double)o) },
				{ typeof(string), (o, b) => b.Write((string)o) },
			};

			private Dictionary<Type, Func<DataBuffer, object>> ReadFunctions = new Dictionary<Type, Func<DataBuffer, object>>() {
				{typeof(byte), (b) => b.ReadByte() },
				{typeof(int), (b) => b.ReadInt32() },
				{typeof(short), (b) => b.ReadInt16() },
				{typeof(uint), (b) => b.ReadUInt32() },
				{typeof(ushort), (b) => b.ReadUInt16() },
				{typeof(long), (b) => b.ReadInt64() },
				{typeof(float), (b) => b.ReadFloat() },
				{typeof(double), (b) => b.ReadDouble() },
				{typeof(string), (b) => b.ReadString() },
			};

			private List<PropertyInfo> properties = new List<PropertyInfo>();

			public PacketSerializer(Type type) {
				var propertiesTemp = type.GetProperties()
					.Where(i => WriteFunctions.ContainsKey(i.PropertyType))
					.OrderBy(i => i.PropertyType.FullName);

				this.properties = propertiesTemp.ToList();

				var writes = properties.Select(i => WriteFunctions[i.PropertyType]).ToList();
				var reads = properties.Select(i => ReadFunctions[i.PropertyType]).ToList();

				this.Writes = writes;
				this.Reads = reads;
			}

			public Type PacketType { get; set; }
			public List<Action<object, DataBuffer>> Writes { get; set; } = new List<Action<object, DataBuffer>>();
			public List<Func<DataBuffer, object>> Reads { get; set; } = new List<Func<DataBuffer, object>>();

			public void Write(DataBuffer buffer, object instance) {
				for(int i = 0; i < properties.Count; i++) {
					WriteFunctions[properties[i].PropertyType].Invoke(
						properties[i].GetValue(instance), buffer);
				}
			}

			public void Read(DataBuffer buffer, object instance) {
				for (int i = 0; i < properties.Count; i++) {
					var result = ReadFunctions[properties[i].PropertyType].Invoke(buffer);
					properties[i].SetValue(instance, result);
				}
			}
		}

		public SerializedPacket() {
			lock (lockObject) {
				if (packetMap.ContainsKey(GetType()) == false) {
					packetMap.Add(GetType(), new PacketSerializer(GetType()));
				}
			}
		}

		private object lockObject = new object();
		private static Dictionary<Type, PacketSerializer> packetMap = new Dictionary<Type, PacketSerializer>();


		public override void WriteTo(DataBuffer buffer) {
			var p = packetMap[GetType()];
			p.Write(buffer, this);
		}

		public override void ReadFrom(DataBuffer buffer) {
			var p = packetMap[GetType()];
			p.Read(buffer, this);
		}
	}
}