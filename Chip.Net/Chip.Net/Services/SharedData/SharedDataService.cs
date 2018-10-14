using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chip.Net.Data;

namespace Chip.Net.Services.SharedData
{
    public class SharedDataService : NetService
    {
		private static readonly string Key_Service = "shareddatasvc";
		private static readonly string Key_UserMap = "shareddatadata";
		private static readonly string Key_UserDateMap = "shareddatadates";
		private static readonly string Key_UserId = "shareddataid";

		private int nextId = 0;
		private DataBuffer buffer;

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);
			buffer = new DataBuffer();
		}

		public override void StartService() {
			base.StartService();
			if (IsServer) {
				var sv = Context.Services.Get<INetServer>();
				sv.OnUserConnected += OnUserConnected;
				sv.OnUserDisconnected += OnUserDisconnected;
			}
		}

		private void OnUserConnected(NetEventArgs args) {
			var user = args.User;
			user.SetLocal<SharedDataService>(Key_Service, this);
			user.SetLocal<Dictionary<int, byte[]>>(Key_UserMap, new Dictionary<int, byte[]>());
			user.SetLocal<Dictionary<int, DateTime>>(Key_UserDateMap, new Dictionary<int, DateTime>());
			user.SetLocal<int>(Key_UserId, nextId++);
		}

		private void OnUserDisconnected(NetEventArgs args) {
			var user = args.User;
			user.SetLocal<SharedDataService>(Key_Service, null);
			user.SetLocal<Dictionary<int, byte[]>>(Key_UserMap, null);
			user.SetLocal<Dictionary<int, DateTime>>(Key_UserDateMap, null);
			user.SetLocal<int>(Key_UserId, 0);
		}

		private int GetUserId(NetUser user) {
			return user.GetLocal<int>(Key_UserId);
		}

		private Dictionary<int, byte[]> GetUserMap(NetUser user) {
			return user.GetLocal<Dictionary<int, byte[]>>(Key_UserMap);
		}

		private Dictionary<int, DateTime> GetUserDateMap(NetUser user) {
			return user.GetLocal<Dictionary<int, DateTime>>(Key_UserDateMap);
		}

		public void SetShared(NetUser user, int key, ISerializable data) {

		}

		public void SetShared<T>(NetUser user, int key, T data) where T : ISerializable {

		}

		public T GetShared<T>(NetUser user, int key) where T : ISerializable {

		}
	}

	public static class SharedDataExtensions {
		public static void SetShared(this NetUser user, int key, ISerializable data) {

		}
	}
}
