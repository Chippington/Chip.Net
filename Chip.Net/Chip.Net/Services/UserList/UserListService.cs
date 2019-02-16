using Chip.Net.Data;
using Chip.Net.Services.RFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Services.UserList {
	public class UserListService : RFCService {
		public NetUser ThisUser { get; set; }
		public IReadOnlyList<NetUser> UserList { get; private set; }

		private List<NetUser> userList;
		private Action<NetUser> ClSetUser;
		private Action<NetUser> ClAddUser;
		private Action<NetUser> ClRemoveUser;
		private Action<List<NetUser>> ClSetUserList;

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);
			userList = new List<NetUser>();
			UserList = userList.AsReadOnly();

			if(IsServer) {
				var sv = context.Services.Get<INetServer>();
				sv.OnUserConnected += onUserConnected;
				sv.OnUserDisconnected += onUserDisconnected;
			}

			ClSetUser = ClientAction<NetUser>(_clSetUser);
			ClAddUser = ClientAction<NetUser>(_clAddUser);
			ClRemoveUser = ClientAction<NetUser>(_clRemoveUser);
			ClSetUserList = ClientAction<List<NetUser>>(_clSetUserList);
		}

		private void _clSetUser(NetUser obj) {
			this.ThisUser = obj;
		}

		private void _clAddUser(NetUser obj) {
			this.userList.Add(obj);
		}

		private void _clRemoveUser(NetUser obj) {
			this.userList = userList
				.Where(i => i.UserId != obj.UserId)
				.ToList();
		}

		private void _clSetUserList(List<NetUser> obj) {
			this.userList = obj;
		}

		private void onUserDisconnected(NetEventArgs args) {
			userList.Remove(args.User);
		}

		private void onUserConnected(NetEventArgs args) {
			userList.Add(args.User);

			SetCurrentUser(args.User);
			ClSetUserList(userList);

			Broadcast(CurrentUser, () => {
				ClAddUser(args.User);
			});
		}
	}
}
