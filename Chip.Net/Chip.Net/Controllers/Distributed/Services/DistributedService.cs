using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Data;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services
{
	public class DistributedService : NetService, IDistributedService {
		public bool IsShard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsRouter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void InitializeRouter(RouterModel Model) {
			throw new NotImplementedException();
		}

		public void InitializeShard(ShardModel Model) {
			throw new NotImplementedException();
		}

		public void InitializeUser(UserModel Model) {
			throw new NotImplementedException();
		}

		public void SendToUser(UserModel User, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, UserModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, IEnumerable<UserModel> Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToRouter(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShard(ShardModel Shard, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, ShardModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, IEnumerable<ShardModel> Exclude) {
			throw new NotImplementedException();
		}
	}
}
