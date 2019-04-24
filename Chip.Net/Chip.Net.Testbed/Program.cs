﻿using Chip.Net.Data;
using Chip.Net.Controllers.Basic;
using Chip.Net.Providers.Sockets;
using Chip.Net.Providers.TCP;
using Chip.Net.Services.NetTime;
using Chip.Net.Services.Ping;
using Chip.Net.Services.RFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chip.Net.Controllers;
using Chip.Net.Services;
using Chip.Net.Services.UserList;
using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed;
using Chip.Net.Providers.Direct;

namespace Chip.Net.Testbed {
	public class TestPacket : Packet {
		public int Data { get; set; }

		public TestPacket() {
			Data = 0;
		}

		public override void WriteTo(DataBuffer buffer) {
			base.WriteTo(buffer);
			buffer.Write((int)Data);
		}

		public override void ReadFrom(DataBuffer buffer) {
			base.ReadFrom(buffer);
			Data = buffer.ReadInt32();
		}
	}

	public class UserModel : IUserModel {
		public string Name { get; set; }
		public string UUID { get; set; }
		public int Id { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			buffer.Write((string)Name);
			buffer.Write((string)UUID);
			buffer.Write((int)Id);
		}

		public void WriteTo(DataBuffer buffer) {
			Name = buffer.ReadString();
			UUID = buffer.ReadString();
			Id = buffer.ReadInt32();
		}
	}

	public class ShardModel : IShardModel {
		public int Id { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			buffer.Write((int)Id);
		}

		public void WriteTo(DataBuffer buffer) {
			Id = buffer.ReadInt32();
		}
	}

	public class RouterModel : IRouterModel {
		public string Name { get; set; }
		public int Id { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			buffer.Write((string)Name);
			buffer.Write((int)Id);
		}

		public void WriteTo(DataBuffer buffer) {
			Name = buffer.ReadString();
			Id = buffer.ReadInt32();
		}
	}

	public class Router : RouterServer<RouterModel, ShardModel, UserModel> { }
	public class Shard : ShardClient<RouterModel, ShardModel, UserModel> { }
	public class User : UserClient<RouterModel, ShardModel, UserModel> { }

	class Program {
		

		static void Main(string[] args) {
			NetContext ctx = new NetContext();
			ctx.Port = 11111;
			ctx.MaxConnections = 10;
			ctx.IPAddress = "localhost";
			ctx.Services.Register<UserListService>();

			Router router = new Router();
			router.InitializeServer(ctx, new DirectServerProvider(), 11111, new DirectServerProvider(), 11112);
			router.StartShardServer();
			router.StartUserServer();

			Func<Shard> makeShard = () => {
				var sh = new Shard();
				var c = ctx.Clone();
				c.Port = 11111;

				sh.InitializeClient(c, new DirectClientProvider());
				sh.StartClient();
				return sh;
			};

			Func<User> makeUser = () => {
				var us = new User();
				var c = ctx.Clone();
				c.Port = 11112;

				us.InitializeClient(c, new DirectClientProvider());
				us.StartClient();
				return us;
			};

			List<Shard> shards = new List<Shard>();
			List<User> users = new List<User>();

			for (int i = 0; i < 5; i++)
				shards.Add(makeShard());

			for (int i = 0; i < 5; i++)
				users.Add(makeUser());

			while(true) {
				router.UpdateServer();
				foreach (var s in shards) s.UpdateClient();
				foreach (var u in users) u.UpdateClient();

				System.Threading.Thread.Sleep(10);
			}
		}
	}
}