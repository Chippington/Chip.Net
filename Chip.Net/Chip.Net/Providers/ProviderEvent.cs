﻿using Chip.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Providers
{
	public class ProviderEventArgs {
		public User UserData { get; set; }
	}

	public delegate void ProviderEvent(ProviderEventArgs args);
}
