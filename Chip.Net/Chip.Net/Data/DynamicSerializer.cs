using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chip.Net.Data
{
    public static class DynamicSerializer {
		public static Dictionary<Type, DataWriter> Writers { get; private set; } = new Dictionary<Type, DataWriter>() {
			{ typeof(Byte), new DataWriter(typeof(byte), (b, v) => b.Write((Byte)v)) },
			{ typeof(Int16), new DataWriter(typeof(byte), (b, v) => b.Write((Int16)v)) },
			{ typeof(Int32), new DataWriter(typeof(byte), (b, v) => b.Write((Int32)v)) },
			{ typeof(Int64), new DataWriter(typeof(byte), (b, v) => b.Write((Int64)v)) },
			{ typeof(UInt16), new DataWriter(typeof(byte), (b, v) => b.Write((UInt16)v)) },
			{ typeof(UInt32), new DataWriter(typeof(byte), (b, v) => b.Write((UInt32)v)) },
			{ typeof(UInt64), new DataWriter(typeof(byte), (b, v) => b.Write((UInt64)v)) },
			{ typeof(float), new DataWriter(typeof(byte), (b, v) => b.Write((float)v)) },
			{ typeof(double), new DataWriter(typeof(byte), (b, v) => b.Write((double)v)) },
			{ typeof(string), new DataWriter(typeof(byte), (b, v) => b.Write((string)v)) },
			{ typeof(byte[]), new DataWriter(typeof(byte), (b, v) => b.Write((byte[])v)) },
			{ typeof(DataBuffer), new DataWriter(typeof(byte), (b, v) => b.Write((DataBuffer)v)) },
		};

		public static Dictionary<Type, DataReader> Readers { get; private set; } = new Dictionary<Type, DataReader>() {
			{ typeof(Byte), new DataReader(typeof(byte), (b, v) => b.ReadByte()) },
			{ typeof(Int16), new DataReader(typeof(byte), (b, v) => b.ReadInt16()) },
			{ typeof(Int32), new DataReader(typeof(byte), (b, v) => b.ReadInt32()) },
			{ typeof(Int64), new DataReader(typeof(byte), (b, v) => b.ReadInt64()) },
			{ typeof(UInt16), new DataReader(typeof(byte), (b, v) => b.ReadUInt16()) },
			{ typeof(UInt32), new DataReader(typeof(byte), (b, v) => b.ReadUInt32()) },
			{ typeof(UInt64), new DataReader(typeof(byte), (b, v) => b.ReadUInt64()) },
			{ typeof(float), new DataReader(typeof(byte), (b, v) => b.ReadFloat()) },
			{ typeof(double), new DataReader(typeof(byte), (b, v) => b.ReadDouble()) },
			{ typeof(string), new DataReader(typeof(byte), (b, v) => b.ReadString()) },
			{ typeof(byte[]), new DataReader(typeof(byte), (b, v) => b.ReadByteArray()) },
			{ typeof(DataBuffer), new DataReader(typeof(byte), (b, v) => b.ReadBuffer()) },
		};

		public static void Write(DataBuffer buffer, Type type, object instance) {
			if (CanReadWrite(type) == false)
				throw new Exception("Invalid model: Can not read/write");

			if(Writers.ContainsKey(type) == false) {
				Writers.Add(type, new DataWriter(type));
			}

			var writer = Writers[type];
			writer.Write(buffer, instance);
		}

		public static void Write<T>(DataBuffer buffer, T instance) {
			Write(buffer, typeof(T), instance);
		}

		public static object Read(DataBuffer buffer, Type type, object existing = null) {
			if (CanReadWrite(type) == false)
				throw new Exception("Invalid model: Can not read/write");

			if(Readers.ContainsKey(type) == false) {
				Readers.Add(type, new DataReader(type));
			}

			var reader = Readers[type];
			var model = reader.Read(buffer, existing);

			return model;
		}

		public static T Read<T>(DataBuffer buffer) {
			return (T)Read(buffer, typeof(T));
		}

		public static bool CanReadWrite(Type type) {
			return (Writers.ContainsKey(type) && Readers.ContainsKey(type)) || 
				type.GetConstructors().Any(i => i.GetParameters().Count() == 0 ||
				typeof(ICollection).IsAssignableFrom(type) ||
				typeof(ISerializable).IsAssignableFrom(type));
		}
    }

	public class DataWriter {
		private Action<DataBuffer, object> WriteAction;
		private PropertyInfo[] Properties;

		private Type ListType;

		public DataWriter(Type type) {
			if(typeof(ICollection).IsAssignableFrom(type)) {
				WriteAction = WriteList;

				if (type.GenericTypeArguments.Any())
					ListType = type.GenericTypeArguments.First();
				else
					ListType = type.GetElementType();

				return;
			}

			Properties = DataHelpers.GetSerializableProperties(type);
			WriteAction = WritePropertyValues;
		}

		private void WriteList(DataBuffer buffer, object inst) {
			IList list = inst as IList;

			short count = (short)list.Count;
			buffer.Write((short)count);

			for (int i = 0; i < count; i++) {
				if (list[i].GetType() != ListType)
					throw new Exception("Type mismatch");

				DynamicSerializer.Write(buffer, ListType, list[i]);
			}
		}

		private void WritePropertyValues(DataBuffer buff, object inst) {
			object[] values = new object[Properties.Length];
			Type[] types = new Type[values.Length];
			bool[] isNull = new bool[values.Length];
			for (int i = 0; i < values.Length; i++) {
				values[i] = Properties[i].GetValue(inst);
				types[i] = Properties[i].PropertyType;
				isNull[i] = values[i] == null;
			}

			var fold = DataHelpers.Fold(isNull);
			for (int i = 0; i < fold.Length; i++)
				buff.Write((byte)fold[i]);

			for (int i = 0; i < values.Length; i++)
				if (values[i] != null)
					DynamicSerializer.Write(buff, types[i], values[i]);
		}

		public DataWriter(Type type, Action<DataBuffer, object> writer) {
			this.WriteAction = writer;
		}

		public void Write(DataBuffer buffer, object instance) {
			WriteAction.Invoke(buffer, instance);
		}
	}

	public class DataReader {
		private Func<DataBuffer, object, object> ReadFunction;
		private PropertyInfo[] Properties;
		private Type ModelType;
		private Type ListType;

		public DataReader(Type type) {
			if (typeof(ICollection).IsAssignableFrom(type)) {

				if (type.GenericTypeArguments.Any()) {
					ReadFunction = ReadList;
					ListType = type.GenericTypeArguments.First();
				} else {
					ReadFunction = ReadArray;
					ListType = type.GetElementType();
				}

				return;
			}

			Properties = DataHelpers.GetSerializableProperties(type);
			ReadFunction = ReadPropertyValues;
			ModelType = type;
		}

		private object ReadList(DataBuffer buffer, object inst) {
			IList ret = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ListType));

			var count = buffer.ReadInt16();
			for(int i = 0; i < count; i++) {
				ret.Add(DynamicSerializer.Read(buffer, ListType));
			}

			return ret;
		}

		private object ReadArray(DataBuffer buffer, object inst) {
			var count = buffer.ReadInt16();
			Array arr = (Array)Activator.CreateInstance(ListType.MakeArrayType(), new object[] { (int)count });

			for(int i = 0; i < count; i++) {
				arr.SetValue(DynamicSerializer.Read(buffer, ListType), i);
			}

			return arr;
		}

		private object ReadPropertyValues(DataBuffer buff, object inst) {
			if (inst == null)
				inst = Activator.CreateInstance(ModelType);

			Type[] types = new Type[Properties.Length];
			for (int i = 0; i < Properties.Length; i++) {
				types[i] = Properties[i].PropertyType;
			}

			var foldBytes = (Properties.Length + 7) / 8;
			byte[] fold = new byte[foldBytes];
			for (int i = 0; i < fold.Length; i++)
				fold[i] = buff.ReadByte();

			var isNull = DataHelpers.Unfold(fold);
			for (int i = 0; i < Properties.Length; i++)
				if (isNull[i] == false)
					Properties[i].SetValue(inst, DynamicSerializer.Read(buff, types[i], Properties[i].GetValue(inst)));

			return inst;
		}

		public DataReader(Type type, Func<DataBuffer, object, object> reader) {
			this.ReadFunction = reader;
		}

		public object Read(DataBuffer buffer, object existing = null) {
			return ReadFunction(buffer, existing);
		}
	}

	public class DataHelpers {
		public static PropertyInfo[] GetSerializableProperties(Type type) {
			return type.GetProperties()
				.Where(i => DynamicSerializer.CanReadWrite(type))
				.ToArray();
		}

		public static byte[] Fold(bool[] flags) {
			int byteCount = (flags.Length + 7) / 8;
			byte[] ret = new byte[byteCount];

			byte mask = 1;
			byte index = 0;
			for (int i = 0; i < flags.Length; i++) {
				if (flags[i])
					ret[index] = (byte)(ret[index] | mask);

				mask = (byte)(mask * 2);
				if (mask == 0) {
					mask = 1;
					index++;
				}
			}

			return ret;
		}

		public static bool[] Unfold(byte[] bytes) {
			bool[] ret = new bool[bytes.Length * 8];

			byte mask = 1;
			byte index = 0;
			for (int i = 0; i < ret.Length; i++) {
				if ((bytes[index] & mask) == mask) {
					ret[i] = true;
				}

				mask = (byte)(mask * 2);
				if (mask == 0) {
					mask = 1;
					index++;
				}
			}

			return ret;
		}
	}
}
