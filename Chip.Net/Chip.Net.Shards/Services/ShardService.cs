using System;
using System.Collections.Generic;
using System.Text;
using Chip.Net.Data;
using Chip.Net.Shards.Models;

namespace Chip.Net.Shards.Services
{
	public class ShardService<TShardModel> : IShardService<TShardModel> where TShardModel : IShardModel {
		public TShardModel Shard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public PacketRouter Router => throw new NotImplementedException();

		public bool IsServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsRouter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsShard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Dispose() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingClientPackets() {
			throw new NotImplementedException();
		}

		public IEnumerable<Packet> GetOutgoingServerPackets() {
			throw new NotImplementedException();
		}

		public void InitializeService(NetContext context) {
			throw new NotImplementedException();
		}

		public void StartService() {
			throw new NotImplementedException();
		}

		public void StopService() {
			throw new NotImplementedException();
		}

		public void UpdateService() {
			throw new NotImplementedException();
		}
	}
}
