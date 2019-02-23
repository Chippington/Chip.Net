using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class Packet : ISerializable {
		[IgnoreProperty]
		public NetUser Sender { get; set; }

		[IgnoreProperty]
		public NetUser Recipient { get; set; }

		[IgnoreProperty]
		public NetUser Exclude { get; set; }

		public virtual void ReadFrom(DataBuffer buffer) { }

		public virtual void WriteTo(DataBuffer buffer) { }
	}
}
