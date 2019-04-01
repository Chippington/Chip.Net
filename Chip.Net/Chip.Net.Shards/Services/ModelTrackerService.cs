using Chip.Net.Shards.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Shards.Services
{
    public class ModelTrackerService<TShardModel, TModel> : ShardService<TShardModel> where TModel : IModelBase where TShardModel : IShardModel
    {
    }
}
