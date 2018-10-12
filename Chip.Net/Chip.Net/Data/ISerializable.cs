using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public interface ISerializable {
		void WriteTo(DataBuffer buffer);
		void ReadFrom(DataBuffer buffer);
	}
}
