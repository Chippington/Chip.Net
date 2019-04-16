using Chip.Net.Controllers.Distributed.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.ModelTracking
{
	public class ModelTrackerCollection<TModel> : ICollection<TModel>, IDisposable where TModel : IDistributedModel {
		public int Count => throw new NotImplementedException();
		public bool Disposed { get; private set; }

		public struct ModelAddedEventArgs { public TModel Model { get; set; } }
		public struct ModelRemovedEventArgs { public TModel Model { get; set; } }
		public struct ModelUpdatedEventArgs {
			public TModel UpdatedModel { get; set; }
			public TModel OldModel { get; set; }
		}

		public EventHandler<ModelAddedEventArgs> ModelAddedEvent { get; set; }
		public EventHandler<ModelUpdatedEventArgs> ModelUpdatedEvent { get; set; }
		public EventHandler<ModelRemovedEventArgs> ModelRemovedEvent { get; set; }

		public TModel this[int id] {
			get {
				return default(TModel);
			}
		}

		public TModel this[uint id] {
			get {
				return default(TModel);
			}
		}

		public bool IsReadOnly => throw new NotImplementedException();

		public void Add(TModel item) {
			throw new NotImplementedException();
		}

		public void Update(int itemId, TModel item) {
			throw new NotImplementedException();
		}

		public void Clear() {
			throw new NotImplementedException();
		}

		public bool Contains(TModel item) {
			throw new NotImplementedException();
		}

		public bool Contains(int itemId) {
			throw new NotImplementedException();
		}

		public void CopyTo(TModel[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public IEnumerator<TModel> GetEnumerator() {
			throw new NotImplementedException();
		}

		public bool Remove(TModel item) {
			throw new NotImplementedException();
		}

		public bool Remove(int itemId) {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}
	}
}
