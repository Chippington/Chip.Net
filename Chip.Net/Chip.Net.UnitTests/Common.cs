using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests {
	public static class Common {
		private static int port = 11111;
		public static int Port { get { return port++; } }
	}
}
