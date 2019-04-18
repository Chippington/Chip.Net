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

		private Dictionary<int, TModel> modelMap;
		private Queue<int> availableIds;
		private int nextId;

		private int GetNextId() {
			if (availableIds.Count > 0)
				return availableIds.Dequeue();

			return nextId++;
		}

		private void RecycleId(int id) {
			if (modelMap.ContainsKey(id))
				throw new Exception("Cannot recycle ID, model not disposed.");

			availableIds.Enqueue(id);
		}

		public ModelTrackerCollection() {
			modelMap = new Dictionary<int, TModel>();
			availableIds = new Queue<int>();
			nextId = 1;
		}

		public void Add(TModel item) {
			item.Id = GetNextId();
			Add(item, item.Id);
		}

		private void Add(TModel item, int id) {
			item.Id = id;
			modelMap[item.Id] = item;

			ModelAddedEvent?.Invoke(this, new ModelAddedEventArgs() {
				Model = item,
			});

			while (nextId < id)
				if (modelMap.ContainsKey(nextId) == false)
					RecycleId(nextId++);
		}

		public void Update(int itemId, TModel item) {
			if (itemId != item.Id)
				if (modelMap.ContainsKey(item.Id))
					if (modelMap[item.Id].Equals(item))
						Remove(item.Id);

			if(modelMap.ContainsKey(itemId)) {
				var old = modelMap[itemId];
				modelMap[itemId] = item;

				ModelUpdatedEvent?.Invoke(this, new ModelUpdatedEventArgs() {
					OldModel = old,
					UpdatedModel = item,
				});
			} else {
				Add(item, itemId);
			}
		}

		public void Clear() {
			if(modelMap != null)
				modelMap.Clear();

			nextId = 1;
			availableIds.Clear();
		}

		public bool Contains(TModel item) {
			return modelMap.ContainsKey(item.Id) && modelMap[item.Id].Equals(item);
		}

		public bool Contains(int itemId) {
			return modelMap.ContainsKey(itemId);
		}

		public void CopyTo(TModel[] array, int arrayIndex) {
			modelMap.Values.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TModel> GetEnumerator() {
			return modelMap.Values.GetEnumerator();
		}

		public bool Remove(TModel item) {
			if(Contains(item)) {
				return Remove(item.Id);
			}

			return false;
		}

		public bool Remove(int itemId) {
			return modelMap.Remove(itemId);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return modelMap.Values.GetEnumerator();
		}

		public void Dispose() {
			if(modelMap != null)
				modelMap.Clear();

			if(availableIds != null)
				availableIds.Clear();
		}
	}
}
