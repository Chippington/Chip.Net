using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.RFC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.ModelTracking
{
    public class ModelTrackerService<TModel> : DistributedRFCService where TModel : IDistributedModel
    {
		public ModelTrackerCollection<TModel> Models { get; set; }

		public TModel Create() { return default(TModel); }
		public TModel Create(int modelId) { return default(TModel); }
		public void Add(TModel Model) { }
		public void Add(TModel Model, int modelId) { }
		public bool Remove(TModel Model) { return false; }
		public bool Remove(int ModelId) { return false; }
		public bool Contains(TModel Model) { return false; }
		public bool Contains(int ModelId) { return false; }
    }
}
