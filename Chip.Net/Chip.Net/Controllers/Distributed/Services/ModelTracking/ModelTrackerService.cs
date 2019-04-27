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
		private Func<ValidationContext, IUserModel, bool> ShardValidate;
		private Func<ValidationContext, IUserModel, bool> UserValidate;

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

			ShardAddModel.Receive += addModel;
			ShardRemoveModel.Receive += removeModel;
			ShardUpdateModel.Receive += updateModel;
			ShardUpdateSet.Receive += updateSet;

			UserAddModel = CreateUserChannel<AddModel>();
			UserRemoveModel = CreateUserChannel<RemoveModel>();
			UserUpdateModel = CreateUserChannel<UpdateModel>();
			UserUpdateSet = CreateUserChannel<UpdateSet>();

			UserAddModel.Receive += addModel;
			UserRemoveModel.Receive += removeModel;
			UserUpdateModel.Receive += updateModel;
			UserUpdateSet.Receive += updateSet;

			Models = new ModelTrackerCollection<TModel>();
			Models.ModelAddedEvent += OnModelAdded;
			Models.ModelRemovedEvent += OnModelRemoved;
			Models.ModelUpdatedEvent += OnModelUpdated;
		}

		public struct ValidationContext {
			public TModel Model { get; set; }
			public NetUser User { get; set; }
			public ModelTrackerPacket Source { get; set; }
		}

		public void SetShardValidation(Func<ValidationContext, IUserModel, bool> func) {

		}

		public void SetUserValidation(Func<ValidationContext, IUserModel, bool> func) {

		}

		private void addModel(object sender, AddModel e) {
			throw new NotImplementedException();
		}

		private void removeModel(object sender, RemoveModel e) {
			throw new NotImplementedException();
		}

		private void updateModel(object sender, UpdateModel e) {
			throw new NotImplementedException();
		}

		private void updateSet(object sender, UpdateSet e) {
			throw new NotImplementedException();
		}

		private void OnModelAdded(object sender, ModelTrackerCollection<TModel>.ModelAddedEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnModelRemoved(object sender, ModelTrackerCollection<TModel>.ModelRemovedEventArgs e) {
			throw new NotImplementedException();
		}

		private void OnModelUpdated(object sender, ModelTrackerCollection<TModel>.ModelUpdatedEventArgs e) {
			throw new NotImplementedException();
		}
	}
}
