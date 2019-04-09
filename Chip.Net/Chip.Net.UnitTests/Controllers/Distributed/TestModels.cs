using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
    public class TestUserModel : IUserModel {
		public uint Id { get; set; }
		public string Name { get; set; }
		public string UUID { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			Id = buffer.ReadUInt32();
			Name = buffer.ReadString();
			UUID = buffer.ReadString();
		}

		public void WriteTo(DataBuffer buffer) {
			buffer.Write((uint)Id);
			buffer.Write((string)Name);
			buffer.Write((string)UUID);
		}
	}

	public class TestShardModel : IShardModel {
		public uint Id { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			Id = buffer.ReadUInt32();
		}

		public void WriteTo(DataBuffer buffer) {
			buffer.Write((uint)Id);
		}
	}

	public class TestRouterModel : IRouterModel {
		public string Name { get; set; }
		public uint Id { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			Name = buffer.ReadString();
			Id = buffer.ReadUInt32();
		}

		public void WriteTo(DataBuffer buffer) {
			buffer.Write((string)Name);
			buffer.Write((uint)Id);
		}
	}
}
