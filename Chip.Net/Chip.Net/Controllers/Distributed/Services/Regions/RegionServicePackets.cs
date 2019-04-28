using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.Regions
{
    public partial class RegionService<TRegion, TFocus> where TRegion : IRegionModel where TFocus : IDistributedModel {
		public class FocusPacket : Packet {
			public TFocus Focus { get; set; }

			public override void WriteTo(DataBuffer buffer) {
				base.WriteTo(buffer);
				Focus.WriteTo(buffer);
			}

			public override void ReadFrom(DataBuffer buffer) {
				base.ReadFrom(buffer);
				this.Focus = Activator.CreateInstance<TFocus>();
				this.Focus.ReadFrom(buffer);
			}
		}
	}
}
