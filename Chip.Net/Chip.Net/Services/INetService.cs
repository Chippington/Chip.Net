using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Services {
	public interface INetService : IDisposable {
		void UpdateService();
	}
}