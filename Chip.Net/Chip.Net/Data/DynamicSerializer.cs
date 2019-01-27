using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chip.Net.Data
{
	public class DynamicSerializer
	{
		private delegate void WriteFunc(object data, DataBuffer buffer);
		private delegate object ReadFunc(Type type, DataBuffer buffer);

		private static Dictionary<Type, WriteFunc> WriteFunctions = new Dictionary<Type, WriteFunc>() {
			{ typeof(byte), (data, buffer) => buffer.Write((byte)data) },
			{ typeof(int), (data, buffer) => buffer.Write((int)data) },
			{ typeof(uint), (data, buffer) => buffer.Write((uint)data) },
			{ typeof(short), (data, buffer) => buffer.Write((short)data) },
			{ typeof(ushort), (data, buffer) => buffer.Write((ushort)data) },
			{ typeof(long), (data, buffer) => buffer.Write((long)data) },
			{ typeof(ulong), (data, buffer) => buffer.Write((ulong)data) },
			{ typeof(float), (data, buffer) => buffer.Write((float)data) },
			{ typeof(double), (data, buffer) => buffer.Write((double)data) },
			{ typeof(string), (data, buffer) => buffer.Write((string)data) },
			{ typeof(ISerializable), (data, buffer) => {
				((ISerializable)data).WriteTo(buffer);
			} },
			{ typeof(IEnumerable), (data, buffer) => {
				var dType = data.GetType();
				var genericType = data.GetType().GetElementType();
				if(genericType == null)
					genericType = data.GetType().GenericTypeArguments[0];

				var en = (data as IEnumerable).GetEnumerator();
				Queue<object> toWrite = new Queue<object>();
				while(en.MoveNext()) {
					var el = en.Current;
					toWrite.Enqueue(el);
				}

				buffer.Write((ushort)toWrite.Count);
				while(toWrite.Count > 0) {
					Write(buffer, toWrite.Dequeue());
				}
			} }
		};

		private static Dictionary<Type, ReadFunc> ReadFunctions = new Dictionary<Type, ReadFunc>() {
			{typeof(byte), (type, buffer) => buffer.ReadByte() },
			{typeof(int), (type, buffer) => buffer.ReadInt32() },
			{typeof(short), (type, buffer) => buffer.ReadInt16() },
			{typeof(uint), (type, buffer) => buffer.ReadUInt32() },
			{typeof(ushort), (type, buffer) => buffer.ReadUInt16() },
			{typeof(long), (type, buffer) => buffer.ReadInt64() },
			{typeof(ulong), (type, buffer) => buffer.ReadUInt64() },
			{typeof(float), (type, buffer) => buffer.ReadFloat() },
			{typeof(double), (type, buffer) => buffer.ReadDouble() },
			{typeof(string), (type, buffer) => buffer.ReadString() },
			{typeof(ISerializable), (type, buffer) => {
				var dataInst = (ISerializable)Activator.CreateInstance(type);
				dataInst.ReadFrom(buffer);
				return dataInst;
			} },
			{typeof(IEnumerable), (type, buffer) => {
				var genericType = type.GetElementType();
				if(genericType == null)
					genericType = type.GenericTypeArguments[0];

				var count = buffer.ReadUInt16();
				IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
				for(int i = 0; i < count; i++) {
					var inst = Read(genericType, buffer);
					list.Add(inst);
				}

				return list;
			} },
		};

		public static void Write(DataBuffer buffer, object data, Type type = null) {
			if (type == null)
				type = SelectType(data.GetType());

			WriteFunctions[type].Invoke(data, buffer);
		}

		public static object Read(Type type, DataBuffer buffer) {
			return ReadFunctions[SelectType(type)].Invoke(type, buffer);
		}

		public static bool HasType(Type type, bool selectProper = false) {
			if (selectProper) type = SelectType(type);
			return WriteFunctions.ContainsKey(type);
		}

		private List<PropertyInfo> properties = new List<PropertyInfo>();

		public DynamicSerializer(Type type) {
			var propertiesTemp = type.GetProperties()
				.Where(i => SelectType(i.PropertyType) != null)
				.OrderBy(i => i.PropertyType.FullName);

			this.properties = propertiesTemp.ToList();


			var writes = properties.Select(i => new	Tuple<PropertyInfo, WriteFunc>(i, WriteFunctions[SelectType(i.PropertyType)])).ToList();
			var reads = properties.Select(i => new Tuple<PropertyInfo, ReadFunc>(i, ReadFunctions[SelectType(i.PropertyType)])).ToList();

			this.Writes = writes;
			this.Reads = reads;
		}

		public Type PacketType { get; set; }
		private List<Tuple<PropertyInfo, WriteFunc>> Writes { get; set; } = new List<Tuple<PropertyInfo, WriteFunc>>();
		private List<Tuple<PropertyInfo, ReadFunc>> Reads { get; set; } = new List<Tuple<PropertyInfo, ReadFunc>>();

		public static Type SelectType(Type tt) {
			if (WriteFunctions.ContainsKey(tt))
				return tt;

			if (typeof(ISerializable).IsAssignableFrom(tt))
				return typeof(ISerializable);

			if (typeof(IEnumerable).IsAssignableFrom(tt))
				return typeof(IEnumerable);

			if(tt.GetConstructors().Any(i => i.GetParameters().Count() == 0)) {
				DynamicSerializer s = new DynamicSerializer(tt);
				WriteFunctions[tt] = (data, buffer) => {
					s.WriteTo(buffer, data);
				};

				ReadFunctions[tt] = (type, buffer) => {
					var inst = Activator.CreateInstance(tt);
					s.ReadFrom(buffer, inst);
					return inst;
				};

				return tt;
			}

			return null;
		}

		public void WriteTo(DataBuffer buffer, object instance) {
			byte flags = 0;
			for (int i = 0; i < Writes.Count; i++) {
				flags = (byte)(flags << 1);

				var prop = Writes[i].Item1;
				if (prop.GetValue(instance) != null) {
					flags = (byte)(flags | 1);
				}
			}

			buffer.Write((byte)flags);
			for (int i = Writes.Count - 1; i >= 0; i--) {
				var val = Writes[i].Item1.GetValue(instance);
				if (val != null)
					Write(buffer, val);
			}
		}

		public void ReadFrom(DataBuffer buffer, object instance) {
			byte flags = buffer.ReadByte();
			byte mask = 1;

			for (int i = Reads.Count - 1; i >= 0; i--) {
				if((flags & mask) == mask) {
					var val = Read(Reads[i].Item1.PropertyType, buffer);
					Reads[i].Item1.SetValue(instance, val);
				}

				mask = (byte)(mask << 1);
			}
		}
	}
}
