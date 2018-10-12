using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services {
	public class NetServiceCollection : IDisposable {
		private Dictionary<Type, INetService> serviceMap;
		private List<INetService> serviceList;

		public NetServiceCollection() {
			serviceMap = new Dictionary<Type, INetService>();
			serviceList = new List<INetService>();
		}

		public void InitializeServices(NetContext context) {
			for (int i = 0; i < serviceList.Count; i++) {
				serviceList[i].InitializeService(context);
			}
		}

		public T Register<T>() where T : INetService {
			var inst = Activator.CreateInstance<T>();
			return Register(inst);
		}

		public T Register<T>(T inst) where T : INetService {
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

		public void UpdateServices() {
			for(int i = 0; i < serviceList.Count; i++) {
				serviceList[i].UpdateService();
			}
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
