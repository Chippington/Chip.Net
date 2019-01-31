using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services.RFC
{
	public class RFCService : NetService {
		private NetUser currentUser;
		public NetUser CurrentUser {
			get {
				return currentUser;
			}
		}

		public IReadOnlyList<NetUser> AllUsers {
			get {
				return GetUsers();
			}
		}

		private byte svActionId;
		private byte clActionId;

		private Dictionary<byte, Action<object[]>> svActionMap;
		private Dictionary<byte, Action<object[]>> clActionMap;

		private Dictionary<byte, Type[]> svTypeMap;
		private Dictionary<byte, Type[]> clTypeMap;

		private Dictionary<Type, DynamicSerializer> serializerMap;
		private List<NetUser> userList;

		public RFCService() {
			userList = new List<NetUser>();
			serializerMap = new Dictionary<Type, DynamicSerializer>();

			svActionMap = new Dictionary<byte, Action<object[]>>();
			clActionMap = new Dictionary<byte, Action<object[]>>();

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

		private Action<object[]> ServerAction(Action<object[]> real, Type[] types) {
			byte id = svActionId++;

			Action<object[]> action = (param) => {
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

			svActionMap[id] = (obj) => action(obj);
			svTypeMap[id] = types;

			return action;
		}

		private Action<object[]> ClientAction(Action<object[]> real, Type[] types) {
			byte id = clActionId++;

			Action<object[]> action = (param) => {
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

			clActionMap[id] = (obj) => action(obj);
			clTypeMap[id] = types;

			return action;
		}

		private void WriteModelsToBuffer(DataBuffer buffer, object[] param) {
			for(int i = 0; i < param.Length; i++) {
				var pType = param[i].GetType();
				if (DynamicSerializer.HasType(pType)) {
					DynamicSerializer.Write(buffer, param[i], pType);
					continue;
				}

				var s = GetSerializer(pType);
				s.WriteTo(buffer, param[i]);
			}
		}

		private void onExecute(RFCExecute obj) {
			object[] param = null;
			Type[] modelTypes = null;

			var buff = new DataBuffer(obj.FunctionParameters);
			buff.Seek(0);

			Action<object[]> action = null;
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
				var serializer = GetSerializer(modelType);

				if (DynamicSerializer.HasType(modelType)) {
					var model = DynamicSerializer.Read(modelType, buff);
					param[i] = model;
				} else {
					var model = Activator.CreateInstance(modelType);
					serializer.ReadFrom(buff, model);
					param[i] = model;
				}
			}

			action.Invoke(param);
		}

		public void Broadcast(Action<NetUser> userAction) {
			this.Broadcast(GetUsers(), userAction);
		}

		public void Broadcast(IEnumerable<NetUser> recipients, Action<NetUser> userAction) {
			var originalUser = currentUser;
			if (recipients == null) recipients = GetUsers();
			foreach (var user in recipients) {
				SetCurrentUser(user);
				userAction.Invoke(user);
			}

			SetCurrentUser(originalUser);
		}

		public void Broadcast(Action userAction) {
			this.Broadcast(GetUsers(), userAction);
		}

		public void Broadcast(IEnumerable<NetUser> recipients, Action userAction) {
			var originalUser = currentUser;
			if (recipients == null) recipients = GetUsers();
			foreach (var user in recipients) {
				SetCurrentUser(user);
				userAction.Invoke();
			}

			SetCurrentUser(originalUser);
		}

		public NetUser GetCurrentUser() {
			return currentUser;
		}

		public IReadOnlyList<NetUser> GetUsers() {
			return userList.AsReadOnly();
		}

		protected void SetCurrentUser(NetUser user) {
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

		private DynamicSerializer GetSerializer(Type type) {
			if (serializerMap.ContainsKey(type) == false) {
				serializerMap[type] = DynamicSerializer.Get(type);
			}

			var serializer = serializerMap[type];
			return serializer;
		}
	}
}
