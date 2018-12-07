using Chip.Net.Data;
using Chip.Net.Default.Basic;
using Chip.Net.Providers.TCP;
using Chip.Net.Services.NetTime;
using Chip.Net.Services.Ping;
using System;

namespace Chip.Net.Testbed
{
    class Program
    {
		class TestPacket : SerializedPacket {
			public int TestInt { get; set; }
			public float TestFloat { get; set; }
			public string TestString { get; set; }
		}

		class TestPacketTwo : TestPacket {
			public int TestIntTwo { get; set; }
			public float TestFloatTwo { get; set; }
			public string TestStringTwo { get; set; }
		}

		static NetContext Context {
			get {
				var ctx = new NetContext();
				ctx.IPAddress = "localhost";
				ctx.Port = 11111;

				ctx.Services.Register<PingService>();
				ctx.Services.Register<NetTimeService>();
				return ctx;
			}
		}

        static void Main(string[] args)
        {
			TestPacketTwo t = new TestPacketTwo();
			t.TestInt = 1;
			t.TestIntTwo = 2;
			t.TestFloat = 3f;
			t.TestFloatTwo = 4f;
			t.TestString = "Hello";
			t.TestStringTwo = "World";

			DataBuffer b = new DataBuffer();
			t.WriteTo(b);

			b.Seek(0);
			TestPacketTwo result = new TestPacketTwo();
			result.ReadFrom(b);

			INetServer sv = new BasicServer();
			sv.InitializeServer(Context);
			sv.StartServer(new TCPServerProvider());

			INetClient cl = new BasicClient();
			cl.InitializeClient(Context);
			cl.StartClient(new TCPClientProvider());

			while(true) {
				System.Threading.Thread.Sleep(10);
				sv.UpdateServer();
				cl.UpdateClient();

				var users = sv.GetUsers();
				var svc = sv.Context.Services.Get<PingService>();
				var svc2 = cl.Context.Services.Get<NetTimeService>();
				var svc3 = sv.Context.Services.Get<NetTimeService>();
				foreach (var user in users) {
					var ping = svc.GetPing(user);
					//Console.WriteLine("Ping: " + ping.ToString());
					//Console.WriteLine("NetTime: " + svc2.GetNetTime());
					Console.WriteLine(svc2.NetTime - svc3.NetTime);
				}
			}
        }
    }
}
