using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class Packet : ISerializable {
		public void ReadFrom(DataBuffer buffer) {
			throw new NotImplementedException();
		}

		public void WriteTo(DataBuffer buffer) {
			throw new NotImplementedException();
		}
	}
}
