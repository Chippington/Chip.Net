using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Chip.Net {
	public interface ISerializable {
		void WriteTo(DataBuffer buffer);
		void ReadFrom(DataBuffer buffer);
	}

	public class Serializable : ISerializable {
		private static Dictionary<Type, DynamicSerializer> SerializerMap { get; set; } = new Dictionary<Type, DynamicSerializer>();
		private DynamicSerializer GetSerializer() {
			if (SerializerMap.ContainsKey(GetType()))
				return SerializerMap[GetType()];

			var serializer = DynamicSerializer.Get(GetType());
			SerializerMap.Add(GetType(), serializer);
			return serializer;
		}

		public void ReadFrom(DataBuffer buffer) {
			GetSerializer().ReadFrom(buffer, this);
		}

		public void WriteTo(DataBuffer buffer) {
			GetSerializer().WriteTo(buffer, this);
		}
	}
}
