﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chip.Net.Data
{
	public class DynamicSerializer
	{
		private delegate void WriteFunc(object instance, PropertyInfo prop, DataBuffer buffer);
		private delegate void ReadFunc(object instance, PropertyInfo prop, DataBuffer buffer);

		private static Dictionary<Type, WriteFunc> WriteFunctions = new Dictionary<Type, WriteFunc>() {
			{ typeof(byte), (inst, prop, buffer) => buffer.Write((byte)prop.GetValue(inst)) },
			{ typeof(int), (inst, prop, buffer) => buffer.Write((int)prop.GetValue(inst)) },
			{ typeof(uint), (inst, prop, buffer) => buffer.Write((uint)prop.GetValue(inst)) },
			{ typeof(short), (inst, prop, buffer) => buffer.Write((short)prop.GetValue(inst)) },
			{ typeof(ushort), (inst, prop, buffer) => buffer.Write((ushort)prop.GetValue(inst)) },
			{ typeof(long), (inst, prop, buffer) => buffer.Write((long)prop.GetValue(inst)) },
			{ typeof(ulong), (inst, prop, buffer) => buffer.Write((ulong)prop.GetValue(inst)) },
			{ typeof(float), (inst, prop, buffer) => buffer.Write((float)prop.GetValue(inst)) },
			{ typeof(double), (inst, prop, buffer) => buffer.Write((double)prop.GetValue(inst)) },
			{ typeof(string), (inst, prop, buffer) => buffer.Write((string)prop.GetValue(inst)) },
			{ typeof(ISerializable), (inst, prop, buffer) => {
				ISerializable s = (ISerializable)prop.GetValue(inst);
				s.WriteTo(buffer);
			} },

		};

		private static Dictionary<Type, ReadFunc> ReadFunctions = new Dictionary<Type, ReadFunc>() {
			{typeof(byte), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadByte()) },
			{typeof(int), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadInt32()) },
			{typeof(short), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadInt16()) },
			{typeof(uint), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadUInt32()) },
			{typeof(ushort), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadUInt16()) },
			{typeof(long), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadInt64()) },
			{typeof(ulong), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadUInt64()) },
			{typeof(float), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadFloat()) },
			{typeof(double), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadDouble()) },
			{typeof(string), (inst, prop, buffer) => prop.SetValue(inst, buffer.ReadString()) },
			{typeof(ISerializable), (inst, prop, buffer) => {
				var propType = prop.PropertyType;
				var propInst = (ISerializable)Activator.CreateInstance(propType);
				propInst.ReadFrom(buffer);
				prop.SetValue(inst, propInst);
			} },
		};

		private List<PropertyInfo> properties = new List<PropertyInfo>();

		public DynamicSerializer(Type type) {
			Func<Type, Type> SelectType = (tt) => {
				if (typeof(ISerializable).IsAssignableFrom(tt))
					return typeof(ISerializable);

				if (WriteFunctions.ContainsKey(tt))
					return tt;

				return null;
			};

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

		public void Write(DataBuffer buffer, object instance) {
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
				if(Writes[i].Item1.GetValue(instance) != null)
					Writes[i].Item2.Invoke(instance, Writes[i].Item1, buffer);
			}
		}

		public void Read(DataBuffer buffer, object instance) {
			byte flags = buffer.ReadByte();
			byte mask = 1;

			for (int i = Reads.Count - 1; i >= 0; i--) {
				if((flags & mask) == mask) {
					Reads[i].Item2.Invoke(instance, Reads[i].Item1, buffer);
				}

				mask = (byte)(mask << 1);
			}
		}
	}
}
