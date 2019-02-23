using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Services.RFC
{
	public class RFCService : NetService {
		private NetUser currentUser;
		protected NetUser CurrentUser {
			get {
				return currentUser;
			}
		}

		protected IReadOnlyList<NetUser> AllUsers {
			get {
				return GetUsers();
			}
		}

		private byte svActionId;
		private byte clActionId;

		private Dictionary<byte, Action<object[], bool>> svActionMap;
		private Dictionary<byte, Action<object[], bool>> clActionMap;

		private Dictionary<byte, Type[]> svTypeMap;
		private Dictionary<byte, Type[]> clTypeMap;

		private List<NetUser> userList;

		public RFCService() {
			userList = new List<NetUser>();

			svActionMap = new Dictionary<byte, Action<object[], bool>>();
			clActionMap = new Dictionary<byte, Action<object[], bool>>();

			svTypeMap = new Dictionary<byte, Type[]>();
			clTypeMap = new Dictionary<byte, Type[]>();
		}

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);

			context.Packets.Register<RFCExecute>();
			Router.Route<RFCExecute>(onExecute);

			if(IsServer) {
				var sv = context.Services.Get<INetServer>();
				sv.OnUserConnected += (arg) => {
					userList.Add(arg.User);
				};

				sv.OnUserDisconnected += (arg) => {
					userList.Remove(arg.User);
				};
			}
		}

		protected Action<T> SharedAction<T>(Action<T> real) {
			var a = RemoteAction((p) => real((T)p[0]), new Type[] { typeof(T) });
			return (realVal) => a.Invoke(new object[] { realVal });
		}

		protected Action<T1, T2> SharedAction<T1, T2>(Action<T1, T2> real) {
			var a = RemoteAction((p) => real((T1)p[0], (T2)p[1]), new Type[] { typeof(T1), typeof(T2) });
			return (val1, val2) => a.Invoke(new object[] { val1, val2 });
		}

		protected Action<T1, T2, T3> SharedAction<T1, T2, T3>(Action<T1, T2, T3> real) {
			var a = RemoteAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2]), new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			return (val1, val2, val3) => a.Invoke(new object[] { val1, val2, val3 });
		}

		protected Action<T1, T2, T3, T4> SharedAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real) {
			var a = RemoteAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3]), new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			return (val1, val2, val3, val4) => a.Invoke(new object[] { val1, val2, val3, val4 });
		}

		protected Action<T> ServerAction<T>(Action<T> real) {
			var a = ServerAction((p) => real((T)p[0]), new Type[] { typeof(T) });
			return (realVal) => a.Invoke(new object[] { realVal });
		}

		protected Action<T1, T2> ServerAction<T1, T2>(Action<T1, T2> real) {
			var a = ServerAction((p) => real((T1)p[0], (T2)p[1]), new Type[] { typeof(T1), typeof(T2) });
			return (val1, val2) => a.Invoke(new object[] { val1, val2 });
		}

		protected Action<T1, T2, T3> ServerAction<T1, T2, T3>(Action<T1, T2, T3> real) {
			var a = ServerAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2]), new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			return (val1, val2, val3) => a.Invoke(new object[] { val1, val2, val3 });
		}

		protected Action<T1, T2, T3, T4> ServerAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real) {
			var a = ServerAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3]), new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			return (val1, val2, val3, val4) => a.Invoke(new object[] { val1, val2, val3, val4 });
		}

		protected Action<T> ClientAction<T>(Action<T> real) {
			var a = ClientAction((p) => real((T)p[0]), new Type[] { typeof(T) });
			return (realVal) => a.Invoke(new object[] { realVal });
		}

		protected Action<T1, T2> ClientAction<T1, T2>(Action<T1, T2> real) {
			var a = ClientAction((p) => real((T1)p[0], (T2)p[1]), new Type[] { typeof(T1), typeof(T2) });
			return (val1, val2) => a.Invoke(new object[] { val1, val2 });
		}

		protected Action<T1, T2, T3> ClientAction<T1, T2, T3>(Action<T1, T2, T3> real) {
			var a = ClientAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2]), new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			return (val1, val2, val3) => a.Invoke(new object[] { val1, val2, val3 });
		}

		protected Action<T1, T2, T3, T4> ClientAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real) {
			var a = ClientAction((p) => real((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3]), new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			return (val1, val2, val3, val4) => a.Invoke(new object[] { val1, val2, val3, val4 });
		}

		private Action<object[]> RemoteAction(Action<object[]> real, Type[] types) {
			byte id = svActionId++;

			Action<object[], bool> action = (param, isReceiving) => {
				byte _id = id;

				if(isReceiving) {
					real.Invoke(param);
				} else {
					RFCExecute msg = new RFCExecute();
					msg.FunctionId = _id;
					var buff = new DataBuffer();
					WriteModelsToBuffer(buff, param);
					msg.FunctionParameters = buff.ToBytes();

					if (IsServer) {
						SendPacketToClient(CurrentUser, msg);
					}

					if (IsClient) {
						SendPacketToServer(msg);
					}
				}
			};

			svActionMap[id] = (obj, rec) => action(obj, rec);
			clActionMap[id] = (obj, rec) => action(obj, rec);
			svTypeMap[id] = types;
			clTypeMap[id] = types;

			return (param) => action(param, false);
		}

		private Action<object[]> ServerAction(Action<object[]> real, Type[] types) {
			byte id = svActionId++;

			Action<object[], bool> action = (param, isReceiving) => {
				byte _id = id;
				if (IsServer) {
					real.Invoke(param);
				}

				if (IsClient) {
					RFCExecute msg = new RFCExecute();
					msg.FunctionId = _id;
					var buff = new DataBuffer();
					WriteModelsToBuffer(buff, param);
					msg.FunctionParameters = buff.ToBytes();
					SendPacketToServer(msg);
				}
			};

			svActionMap[id] = (obj, rec) => action(obj, rec);
			svTypeMap[id] = types;

			return (param) => action(param, false);
		}

		private Action<object[]> ClientAction(Action<object[]> real, Type[] types) {
			byte id = clActionId++;

			Action<object[], bool> action = (param, isReceiving) => {
				byte _id = id;
				if (IsClient) {
					real.Invoke(param);
				}

				if (IsServer) {
					RFCExecute msg = new RFCExecute();
					msg.FunctionId = _id;
					var buff = new DataBuffer();
					WriteModelsToBuffer(buff, param);
					msg.FunctionParameters = buff.ToBytes();

					var user = GetCurrentUser();
					msg.Recipient = user;
					SendPacketToClient(user, msg);
				}
			};

			clActionMap[id] = (obj, rec) => action(obj, rec);
			clTypeMap[id] = types;

			return (param) => action(param, false);
		}

		private void WriteModelsToBuffer(DataBuffer buffer, object[] param) {
			for(int i = 0; i < param.Length; i++) {
				var pType = param[i].GetType();
				if (DynamicSerializer.CanReadWrite(pType)) {
					DynamicSerializer.Write(buffer, pType, param[i]);
				}
			}
		}

		private void onExecute(RFCExecute obj) {
			object[] param = null;
			Type[] modelTypes = null;

			var buff = new DataBuffer(obj.FunctionParameters);
			buff.Seek(0);

			Action<object[], bool> action = null;
			if (IsServer) {
				SetCurrentUser(obj.Sender);
				action = svActionMap[obj.FunctionId];
				modelTypes = svTypeMap[obj.FunctionId];
			}

			if(IsClient) {
				action = clActionMap[obj.FunctionId];
				modelTypes = clTypeMap[obj.FunctionId];
			}

			param = new object[modelTypes.Length];
			for (int i = 0; i < param.Length; i++) {
				var modelType = modelTypes[i];

				if (DynamicSerializer.CanReadWrite(modelType)) {
					var model = DynamicSerializer.Read(buff, modelType);
					param[i] = model;
				}
			}

			action.Invoke(param, true);
		}

		public void Broadcast(Action userAction) {
			this.Broadcast(GetUsers(), null, _ => userAction());
		}

		public void Broadcast(Action<NetUser> userAction) {
			this.Broadcast(GetUsers(), null, u => userAction(u));
		}

		public void Broadcast(NetUser exclude, Action userAction) {
			this.Broadcast(GetUsers(), new NetUser[] { exclude }, _ => userAction());
		}

		public void Broadcast(NetUser exclude, Action<NetUser> userAction) {
			this.Broadcast(GetUsers(), new NetUser[] { exclude }, u => userAction(u));
		}

		public void Broadcast(IEnumerable<NetUser> recipients, Action userAction) {
			this.Broadcast(recipients, null, _ => userAction());
		}

		public void Broadcast(IEnumerable<NetUser> recipients, Action<NetUser> userAction) {
			this.Broadcast(recipients, null, u => userAction(u));
		}

		public void Broadcast(IEnumerable<NetUser> recipients, IEnumerable<NetUser> exclude, Action userAction) {
			this.Broadcast(recipients, exclude, _ => userAction());
		}

		public void Broadcast(IEnumerable<NetUser> recipients, IEnumerable<NetUser> exclude, Action<NetUser> userAction) {
			var originalUser = currentUser;
			if (recipients == null) recipients = GetUsers();
			foreach (var user in recipients) {
				if (exclude != null && exclude.Contains(user))
					continue;

				SetCurrentUser(user);
				userAction.Invoke(user);
			}

			SetCurrentUser(originalUser);
		}

		public NetUser GetCurrentUser() {
			return currentUser;
		}

		public IReadOnlyList<NetUser> GetUsers() {
			return userList.AsReadOnly();
		}

		public void SetCurrentUser(NetUser user) {
			currentUser = user;
		}

		public override void StartService() {
			base.StartService();
		}

		public override void UpdateService() {
			base.UpdateService();
		}

		public override void StopService() {
			base.StopService();
		}
	}
}
