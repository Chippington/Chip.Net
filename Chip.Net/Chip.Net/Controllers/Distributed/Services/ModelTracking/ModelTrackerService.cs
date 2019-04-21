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

		public override void GlobalInitialize(NetContext context) {
			base.GlobalInitialize(context);

			Router.RouteServer<AddModel>(Server_OnAddModel);
			Router.RouteServer<RemoveModel>(Server_OnRemoveModel);
			Router.RouteServer<UpdateModel>(Server_UpdateModel);
			Router.RouteServer<UpdateSet>(Server_UpdateSet);

			Router.RouteClient<AddModel>(Client_OnAddModel);
			Router.RouteClient<RemoveModel>(Client_OnRemoveModel);
			Router.RouteClient<UpdateModel>(Client_UpdateModel);
			Router.RouteClient<UpdateSet>(Client_UpdateSet);
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
