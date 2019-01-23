using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services.RFC
{
	public class RFCExecute : Packet
	{
		public byte FunctionId { get; set; }
		public byte[] FunctionParameters { get; set; }

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);

			buffer.Write((byte)FunctionId);
			buffer.Write((byte[])FunctionParameters);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);

			FunctionId = buffer.ReadByte();
			FunctionParameters = buffer.ReadByteArray();
		}
	}
}
