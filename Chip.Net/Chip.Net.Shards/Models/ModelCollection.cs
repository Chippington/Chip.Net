using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Shards.Models {
	public class ModelCollection<T> : IEnumerable<T>, ICollection<T> where T : IModelBase {
		private Dictionary<int, T> Map { get; set; }
		private Queue<int> IdQueue { get; set; }
		private int NextId { get; set; }

		public int Count => Map.Count;

		public bool IsReadOnly => false;

		public ModelCollection() {
			IdQueue = new Queue<int>();
			NextId = 1;
		}

		public T this[int id] {
			get {
				if (Map.ContainsKey(id) == false)
					return default(T);

				return Map[id];
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return Map.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Map.Values.GetEnumerator();
		}

		public void Add(T item) {
			int id = -1;
			if (IdQueue.Count > 0)
				id = IdQueue.Dequeue();
			else
				id = NextId++;

			Map[id] = item;
			item.Id = id;
		}

		public void Clear() {
			IdQueue.Clear();
			Map.Clear();
			NextId = 1;
		}

		public bool Contains(T item) {
			return Map.ContainsKey(item.Id) && Map[item.Id].Equals(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			Map.Values.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			if (Contains(item))
				return Map.Remove(item.Id);

			return false;
		}
	}
}
