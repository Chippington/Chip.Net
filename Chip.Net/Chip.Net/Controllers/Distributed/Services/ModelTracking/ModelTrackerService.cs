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

		private MessageChannel<AddModel> ShardAddModel;
		private MessageChannel<RemoveModel> ShardRemoveModel;
		private MessageChannel<UpdateModel> ShardUpdateModel;
		private MessageChannel<UpdateSet> ShardUpdateSet;

		private MessageChannel<AddModel> UserAddModel;
		private MessageChannel<RemoveModel> UserRemoveModel;
		private MessageChannel<UpdateModel> UserUpdateModel;
		private MessageChannel<UpdateSet> UserUpdateSet;

		public override void GlobalInitialize(NetContext context) {
			base.GlobalInitialize(context);

			ShardAddModel = RouteRouterShard<AddModel>();
			ShardRemoveModel = RouteRouterShard<RemoveModel>();
			ShardUpdateModel = RouteRouterShard<UpdateModel>();
			ShardUpdateSet = RouteRouterShard<UpdateSet>();

			UserAddModel = RouteRouterUser<AddModel>();
			UserRemoveModel = RouteRouterUser<RemoveModel>();
			UserUpdateModel = RouteRouterUser<UpdateModel>();
			UserUpdateSet = RouteRouterUser<UpdateSet>();
		}

		public override void InitializeContext(NetContext context) {
			base.InitializeContext(context);

			context.Packets.Register<AddModel>();
			context.Packets.Register<RemoveModel>();
			context.Packets.Register<UpdateModel>();
			context.Packets.Register<UpdateSet>();
		}

		protected virtual void Server_OnAddModel(IncomingMessage<AddModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Server_OnRemoveModel(IncomingMessage<RemoveModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Server_UpdateModel(IncomingMessage<UpdateModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Server_UpdateSet(IncomingMessage<UpdateSet> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Client_OnAddModel(IncomingMessage<AddModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Client_OnRemoveModel(IncomingMessage<RemoveModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Client_UpdateModel(IncomingMessage<UpdateModel> obj) {
			throw new NotImplementedException();
		}

		protected virtual void Client_UpdateSet(IncomingMessage<UpdateSet> obj) {
			throw new NotImplementedException();
		}
	}
}
