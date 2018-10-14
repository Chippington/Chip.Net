using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Services {
	public class NetServiceCollection : IDisposable {
		private Dictionary<Type, INetService> serviceMap;
		private List<INetService> serviceList;

		private Dictionary<int, INetService> idToService;
		private Dictionary<INetService, int> serviceToId;

		public bool Locked { get; private set; }

		public NetServiceCollection() {
			serviceMap = new Dictionary<Type, INetService>();
			serviceList = new List<INetService>();
			Locked = false;
		}

		public void InitializeServices(NetContext context) {
			for (int i = 0; i < serviceList.Count; i++) {
				serviceList[i].InitializeService(context);
			}
		}

		public void LockServices() {
			if (Locked)
				throw new Exception("Services can only be locked once");

			Locked = true;
			idToService = new Dictionary<int, INetService>();
			serviceToId = new Dictionary<INetService, int>();

			var ordered = serviceList.OrderBy(i => i.GetType().Name).ToList();
			for (int i = 0; i < ordered.Count; i++) {
				idToService.Add(i, ordered[i]);
				serviceToId.Add(ordered[i], i);
			}
		}

		public int GetServiceId(INetService svc) {
			return serviceToId[svc];
		}

		public INetService GetServiceFromId(int id) {
			return idToService[id];
		}

		public void StartServices() {
			for (int i = 0; i < serviceList.Count; i++) {
				serviceList[i].StartService();
			}
		}
		public void UpdateServices() {
			for (int i = 0; i < serviceList.Count; i++) {
				serviceList[i].UpdateService();
			}
		}

		public void StopServices() {
			for (int i = 0; i < serviceList.Count; i++) {
				serviceList[i].StopService();
			}
		}

		public T Register<T>() where T : INetService {
			if (Locked)
				throw new Exception("Services have been locked");

			var inst = Activator.CreateInstance<T>();
			return Register(inst);
		}

		public T Register<T>(T inst) where T : INetService {
			if (Locked)
				throw new Exception("Services have been locked");

			serviceMap.Add(typeof(T), inst);
			serviceList.Add(inst);
			return inst;
		}

		public T Get<T>() {
			if (serviceMap.ContainsKey(typeof(T)) == false)
				return default(T);

			return (T)serviceMap[typeof(T)];
		}

		public IEnumerable<INetService> Get() {
			return serviceList.AsReadOnly();
		}

		public void Dispose() {
			foreach (var svc in serviceList)
				svc.Dispose();

			serviceList.Clear();
			serviceList = null;
			serviceMap.Clear();
			serviceMap = null;
		}
	}
}
