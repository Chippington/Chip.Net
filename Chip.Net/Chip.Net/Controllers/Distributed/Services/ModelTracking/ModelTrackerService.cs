using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.RFC;
using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.ModelTracking
{
    public partial class ModelTrackerService<TModel> : DistributedService where TModel : IDistributedModel
    {
		public ModelTrackerCollection<TModel> Models { get; set; }

		private ShardChannel<AddModel> ShardAddModel;
		private ShardChannel<RemoveModel> ShardRemoveModel;
		private ShardChannel<UpdateModel> ShardUpdateModel;
		private ShardChannel<UpdateSet> ShardUpdateSet;

		private UserChannel<AddModel> UserAddModel;
		private UserChannel<RemoveModel> UserRemoveModel;
		private UserChannel<UpdateModel> UserUpdateModel;
		private UserChannel<UpdateSet> UserUpdateSet;

		protected override void InitializeDistributedService() {
			base.InitializeDistributedService();

			ShardAddModel = CreateShardChannel<AddModel>();
			ShardRemoveModel = CreateShardChannel<RemoveModel>();
			ShardUpdateModel = CreateShardChannel<UpdateModel>();
			ShardUpdateSet = CreateShardChannel<UpdateSet>();

			UserAddModel = CreateUserChannel<AddModel>();
			UserRemoveModel = CreateUserChannel<RemoveModel>();
			UserUpdateModel = CreateUserChannel<UpdateModel>();
			UserUpdateSet = CreateUserChannel<UpdateSet>();
		}

		protected override void InitializeContext(NetContext context) {
			base.InitializeContext(context);

			context.Packets.Register<AddModel>();
			context.Packets.Register<RemoveModel>();
			context.Packets.Register<UpdateModel>();
			context.Packets.Register<UpdateSet>();
		}
	}
}
