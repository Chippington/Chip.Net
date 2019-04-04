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

		public TModel this[int id] {
			get {
				return default(TModel);
			}
		}

		public bool IsReadOnly => throw new NotImplementedException();

		public void Add(TModel item) {
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
