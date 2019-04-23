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

		public IncomingMessage<T> AsGeneric<T>() where T : Packet {
			return new IncomingMessage<T>() {
				Data = (T)this.Data,
				Sender = this.Sender,
			};
		}
	}

	public class IncomingMessage<T> : IncomingMessage where T : Packet {
		new public T Data { get; set; }
	}

	public class OutgoingMessage : Message {
		public OutgoingMessage(Packet data) {
			Recipients = null;
			Data = data;
		}

		public OutgoingMessage(Packet data, NetUser recipient) {
			Recipients = new NetUser[] { recipient };
			Data = data;
		}

		public OutgoingMessage(Packet data, IEnumerable<NetUser> recipients) {
			Recipients = recipients;
			Data = data;
		}

		public IEnumerable<NetUser> Recipients { get; set; }
	}
}
