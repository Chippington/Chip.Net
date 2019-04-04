using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Services.RFC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Controllers.Distributed.Services.RFC
{
    public class DistributedRFCService : RFCService, IDistributedService {
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

		protected Action<T1> RouterAction<T1>(Action<T1> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2> RouterAction<T1, T2>(Action<T1, T2> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3> RouterAction<T1, T2, T3>(Action<T1, T2, T3> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3, T4> RouterAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real, bool useContext = true) {
			return null;
		}

		protected Action<T1> ShardAction<T1>(Action<T1> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2> ShardAction<T1, T2>(Action<T1, T2> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3> ShardAction<T1, T2, T3>(Action<T1, T2, T3> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3, T4> ShardAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real, bool useContext = true) {
			return null;
		}

		protected Action<T1> UserAction<T1>(Action<T1> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2> UserAction<T1, T2>(Action<T1, T2> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3> UserAction<T1, T2, T3>(Action<T1, T2, T3> real, bool useContext = true) {
			return null;
		}

		protected Action<T1, T2, T3, T4> UserAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> real, bool useContext = true) {
			return null;
		}
	}
}
