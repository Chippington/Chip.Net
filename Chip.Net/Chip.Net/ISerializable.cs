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
		public void ReadFrom(DataBuffer buffer) {
			DynamicSerializer.Write(buffer, GetType(), this);
		}

		public void WriteTo(DataBuffer buffer) {
			DynamicSerializer.Read(buffer, GetType(), this);
		}
	}
}
