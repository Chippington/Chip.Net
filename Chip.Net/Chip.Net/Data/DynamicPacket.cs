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

		public TModel Model { get; set; }

		public DynamicPacket() {
			Model = Activator.CreateInstance<TModel>();
		}

		public DynamicPacket(TModel model) {
			this.Model = model;
		}

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			Serializer.WriteTo(buffer, Model);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Serializer.ReadFrom(buffer, Model);
		}
	}
}
