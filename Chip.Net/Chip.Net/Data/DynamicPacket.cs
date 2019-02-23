using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data {
	public class Packet<TModel> : Packet {
		public TModel Model { get; set; }

		public Packet() {
			Model = Activator.CreateInstance<TModel>();
		}

		public Packet(TModel model) {
			this.Model = model;
		}

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			DynamicSerializer.Write(buffer, typeof(TModel), Model);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Model = DynamicSerializer.Read<TModel>(buffer);
		}
	}
}
