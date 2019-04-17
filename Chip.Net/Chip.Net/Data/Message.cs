using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class IncomingMessage {
		public Packet Data { get; set; }
		public NetUser Sender { get; set; }
	}

	public class IncomingMessage<T> : IncomingMessage where T : Packet {
		public T Data { get; set; }
		public NetUser Sender { get; set; }
	}

	public class OutgoingMessage {
		public Packet Data { get; set; }
		public IEnumerable<NetUser> Recipients { get; set; }
	}

	public class OutgoingMessage<T> : OutgoingMessage where T : Packet {
		public T Data { get; set; }
	}
}
