using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services.Ping
{
	public class P_Ping : Packet { }
	public class P_SetPing : Packet {
		public double Ping { get; set; }

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			buffer.Write((double)Ping);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Ping = buffer.ReadDouble();
		}
	}
}
