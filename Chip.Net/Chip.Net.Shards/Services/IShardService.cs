using Chip.Net.Services;
using Chip.Net.Shards.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Shards.Services {
	public interface IShardService<TShardModel> : INetService where TShardModel : IShardModel {
		bool IsRouter { get; set; }
		bool IsShard { get; set; }
		bool IsUser { get; set;  }

		TShardModel Shard { get; set; }
	}
}
