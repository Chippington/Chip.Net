using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data {
	public class DynamicPacket<TModel> : Packet {
		private static DynamicSerializer s;
		protected static DynamicSerializer Serializer {
			get {
				if (s != null) return s;
				return s = DynamicSerializer.Get(typeof(TModel));
			}
		}

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			Serializer.WriteTo(buffer, this);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Serializer.ReadFrom(buffer, this);
		}
	}
}
