using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services {
	public interface IDistributedService : INetService {
		bool IsShard { get; set; }
		bool IsUser { get; set; }
		bool IsRouter { get; set; }

		void InitializeShard(IShardModel Model);
		void InitializeRouter(IRouterModel Model);
		void InitializeUser(IUserModel Model);
	}
}
