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

		public void InitializeRouter(IRouterModel Model) {
			throw new NotImplementedException();
		}

		public void InitializeShard(IShardModel Model) {
			throw new NotImplementedException();
		}

		public void InitializeUser(IUserModel Model) {
			throw new NotImplementedException();
		}

		public void SendToUser(IUserModel User, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, IUserModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack, IEnumerable<IUserModel> Exclude) {
			throw new NotImplementedException();
		}

		public void SendToUsers(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToRouter(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShard(IShardModel Shard, Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, IShardModel Exclude) {
			throw new NotImplementedException();
		}

		public void SendToShards(Packet Pack, IEnumerable<IShardModel> Exclude) {
			throw new NotImplementedException();
		}
	}
}
