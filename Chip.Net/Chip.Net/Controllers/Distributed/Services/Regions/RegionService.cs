using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.ModelTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.Regions
{
	public interface IRegionResolver<TRegion, TFocus> 
		where TRegion : IRegionModel 
		where TFocus : IDistributedModel {

		TRegion Resolve(TFocus focus);
	}

	public partial class RegionService<TRegion, TFocus> : DistributedService 
		where TRegion : IRegionModel 
		where TFocus : IDistributedModel {

		public IRegionResolver<TRegion, TFocus> Resolver { get; private set; }

		public ShardChannel<FocusPacket> ShardAddFocus { get; private set; }
		public ShardChannel<FocusPacket> ShardRemoveFocus { get; private set; }
		public ShardChannel<FocusPacket> ShardUpdateFocus { get; private set; }

		public UserChannel<FocusPacket> UserAddFocus { get; private set; }
		public UserChannel<FocusPacket> UserRemoveFocus { get; private set; }
		public UserChannel<FocusPacket> UserUpdateFocus { get; private set; }

		public ModelTrackerCollection<TFocus> FocusModels;

		public RegionService(IRegionResolver<TRegion, TFocus> resolver) {
			this.FocusModels = new ModelTrackerCollection<TFocus>();
			this.Resolver = resolver;
		}

		protected override void InitializeDistributedService() {
			base.InitializeDistributedService();

			this.ShardAddFocus = CreateShardChannel<FocusPacket>("add");
			this.ShardRemoveFocus = CreateShardChannel<FocusPacket>("remove");
			this.ShardUpdateFocus = CreateShardChannel<FocusPacket>("update");

			this.UserAddFocus = CreateUserChannel<FocusPacket>("add");
			this.UserRemoveFocus = CreateUserChannel<FocusPacket>("remove");
			this.UserUpdateFocus = CreateUserChannel<FocusPacket>("update");
		}
	}
}
