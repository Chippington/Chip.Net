using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.Data
{
	public interface IMessage {
		Packet Data { get; set; }
	}

	public interface IMessage<T> where T : Packet {
		T Data { get; set; }
	}

	public struct IncomingMessage : IMessage {
		public Packet Data { get; set; }
		public NetUser Sender { get; set; }
	}

	public struct IncomingMessage<T> : IMessage<T> where T : Packet {
		public T Data { get; set; }
	}

	public struct OutgoingMessage : IMessage {
		public Packet Data { get; set; }
		public IEnumerable<NetUser> Recipients { get; set; }
	}

	public struct OutgoingMessage<T> : IMessage<T> where T : Packet {
		public T Data { get; set; }
	}
}
