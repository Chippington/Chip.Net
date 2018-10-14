using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services.SharedData {
	public class P_SetData : Packet {

		public byte[] Data { get; set; }
		public int Key { get; set; }

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
		}
	}
}
