using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.ModelTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.Regions
{
	public partial class RegionService<TRegion, TFocus> : DistributedService
		where TRegion : IRegionModel
		where TFocus : IDistributedModel {

		public override void InitializeRouter() {
			base.InitializeRouter();

			FocusModels.ModelAddedEvent += Router_OnFocusAdded;
			FocusModels.ModelUpdatedEvent += Router_OnFocusUpdated;
			FocusModels.ModelRemovedEvent += Router_OnFocusRemoved;
		}

		protected override void UpdateRouter() {
			base.UpdateRouter();
		}

		private void Router_OnFocusAdded(object sender, ModelTrackerCollection<TFocus>.ModelAddedEventArgs e) {
			throw new NotImplementedException();
		}

		private void Router_OnFocusUpdated(object sender, ModelTrackerCollection<TFocus>.ModelUpdatedEventArgs e) {
			throw new NotImplementedException();
		}

		private void Router_OnFocusRemoved(object sender, ModelTrackerCollection<TFocus>.ModelRemovedEventArgs e) {
			throw new NotImplementedException();
		}
	}
}
