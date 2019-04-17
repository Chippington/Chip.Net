using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public class Message {
		public Packet Data { get; set; }
	}

	public class IncomingMessage : Message {
		public NetUser Sender { get; set; }
	}

	public class IncomingMessage<T> : IncomingMessage where T : Packet {
		new public T Data { get; set; }
	}

	public class OutgoingMessage : Message {
		public IEnumerable<NetUser> Recipients { get; set; }
	}
}
