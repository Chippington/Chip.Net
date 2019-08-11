using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip.Net.Data
{
	public class Message {
		public Packet Data { get; set; }
        public string Route { get; set; }

        public override string ToString() {
            return string.Format("Message[Type: {0}, Route: \"{1}\"]", this.Data.GetType().Name, this.Route.Split('.').Last());
        }
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
        private T _data;
		new public T Data {
            get {
                return _data;
            }

            set {
                ((IncomingMessage)this).Data = value;
                this._data = value;
            }
        }
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

	public class OutgoingMessage<T> : OutgoingMessage where T : Packet {
		new public T Data { get; set; }

		public OutgoingMessage(T data) : base(data) {
			this.Data = data;
		}

		public OutgoingMessage(T data, NetUser recipient) : base(data, recipient) {
			this.Data = data;
		}

		public OutgoingMessage(T data, IEnumerable<NetUser> recipients) : base(data, recipients) {
			this.Data = data;
		}
	}
}
