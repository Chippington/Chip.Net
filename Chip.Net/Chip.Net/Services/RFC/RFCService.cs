using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services.RFC
{
	public class RFCService : NetService {
		private NetUser currentUser;
		private byte svActionId;
		private byte clActionId;

		private Dictionary<byte, Action<object>> svActionMap;
		private Dictionary<byte, Action<object>> clActionMap;

		private Dictionary<byte, Type> svTypeMap;
		private Dictionary<byte, Type> clTypeMap;

		private Dictionary<Type, DynamicSerializer> serializerMap;

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);

			context.Packets.Register<RFCExecute>();
			Router.Route<RFCExecute>(onExecute);

			serializerMap = new Dictionary<Type, DynamicSerializer>();

			svActionMap = new Dictionary<byte, Action<object>>();
			clActionMap = new Dictionary<byte, Action<object>>();

			svTypeMap = new Dictionary<byte, Type>();
			clTypeMap = new Dictionary<byte, Type>();
		}

		protected Action<T> ServerAction<T>(Action<T> real) {
			byte id = svActionId++;

			var serializer = GetSerializer(typeof(T));
			Action<T> action = (model) => {
				byte _id = id;
				var _ss = serializer;

				if (IsServer) {
					real.Invoke(model);
				}

				if (IsClient) {
					//send to server
					RFCExecute msg = new RFCExecute();
					msg.FunctionId = _id;
					var buff = new DataBuffer();
					_ss.WriteTo(buff, model);
					msg.FunctionParameters = buff.ToBytes();
					SendPacketToServer(msg);
				}
			};

			svActionMap[id] = (obj) => action((T)obj);
			svTypeMap[id] = typeof(T);

			return action;
		}

		protected Action<T> ClientAction<T>(Action<T> real) {
			byte id = clActionId++;

			var serializer = GetSerializer(typeof(T));
			Action<T> action = (model) => {
				byte _id = id;
				var _ss = serializer;

				if (IsServer) {
					//send to current user
					RFCExecute msg = new RFCExecute();
					msg.FunctionId = _id;
					var buff = new DataBuffer();
					_ss.WriteTo(buff, model);
					msg.FunctionParameters = buff.ToBytes();

					var user = GetCurrentUser();
					msg.Recipient = user;
					SendPacketToClient(user, msg);
				}

				if (IsClient) {
					real.Invoke(model);
				}
			};

			clActionMap[id] = (obj) => action((T)obj);
			clTypeMap[id] = typeof(T);

			return action;
		}

		private void onExecute(RFCExecute obj) {
			object model = null;
			Action<object> action = null;
			if (IsServer) {
				SetCurrentUser(obj.Sender);
				action = svActionMap[obj.FunctionId];

				var modelType = svTypeMap[obj.FunctionId];
				var serializer = GetSerializer(modelType);
				model = Activator.CreateInstance(modelType);

				var buff = new DataBuffer(obj.FunctionParameters);
				buff.Seek(0);
				serializer.ReadFrom(buff, model);
			}

			if(IsClient) {
				action = clActionMap[obj.FunctionId];
				var modelType = clTypeMap[obj.FunctionId];
				var serializer = GetSerializer(modelType);
				model = Activator.CreateInstance(modelType);

				var buff = new DataBuffer(obj.FunctionParameters);
				buff.Seek(0);
				serializer.ReadFrom(buff, model);
			}

			action.Invoke(model);
		}

		public NetUser GetCurrentUser() {
			return currentUser;
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
				serializerMap[type] = new DynamicSerializer(type);
			}

			var serializer = serializerMap[type];
			return serializer;
		}
	}
}
