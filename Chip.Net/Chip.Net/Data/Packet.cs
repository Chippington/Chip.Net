using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class Packet : ISerializable {
		public NetUser Sender { get; set; }
		public NetUser Recipient { get; set; }
		public NetUser Exclude { get; set; }

		public virtual void ReadFrom(DataBuffer buffer) { }

		public virtual void WriteTo(DataBuffer buffer) { }
	}
}
