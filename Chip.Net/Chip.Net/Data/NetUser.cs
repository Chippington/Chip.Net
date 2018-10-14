using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class NetUser
	{
		public object UserKey { get; private set; }

		private Dictionary<string, object> localDataMap;

		public NetUser(object userKey)
		{
			this.UserKey = userKey;
			localDataMap = new Dictionary<string, object>();
		}

		public override int GetHashCode()
		{
			return UserKey.GetHashCode();
		}

		public void SetLocal(string key, object data)
		{
			localDataMap[key] = data;
		}

		public object GetLocal(string key)
		{
			if (localDataMap.ContainsKey(key))
				return localDataMap[key];

			return null;
		}

		public void SetLocal<T>(string key, T data)
		{
			SetLocal(key, data);
		}

		public T GetLocal<T>(string key)
		{
			return (T)GetLocal(key);
		}
	}
}
