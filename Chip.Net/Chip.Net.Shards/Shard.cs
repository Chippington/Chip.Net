using Chip.Net.Providers;
using Chip.Net.Shards.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Shards
{
    public class Shard<TUserModel, TShardModel>
		where TUserModel : IUserModel
		where TShardModel : IShardModel {

		public delegate void ShardEvent(TShardModel Shard);
		public delegate void ShardUserEvent(TUserModel User);
		public ShardEvent ShardConnectedToRouterEvent { get; set; }
		public ShardEvent ShardDisconnectedFromRouterEvent { get; set; }



		public Shard(NetContext context, INetClient client) {

		}

		public void StartShard(INetClientProvider provider) {

		}
    }
}
