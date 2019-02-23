using Chip.Net.Data;
using Chip.Net.Default.Basic;
using Chip.Net.Providers.SocketProvider;
using Chip.Net.Providers.TCP;
using Chip.Net.Services.NetTime;
using Chip.Net.Services.Ping;
using Chip.Net.Services.RFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
				ctx.Services.Register<TestRFCService>();
				return ctx;
			}
		}

		public class TestNonSerializable {
			public string data1 { get; set; }
		}

		public class TestSerializable : Serializable
		{
			public string data1 { get; set; }
			public string data2 { get; set; }
			public string data { get; set; }

			public TestSerializable Inner { get; set; }
			public TestNonSerializable NonSerializable { get; set; }
			public IEnumerable<string> TestEnum { get; set; }

			public IEnumerable<IEnumerable<byte>> Intricate { get; set; }
			public TestEnum Enum { get; set; }
		}

		public class TestRFCService : RFCService {
			public Action<TestSerializable> ClientMethod { get; set; }
			public Action<TestSerializable> ServerMethod { get; set; }
			public Action<TestNonSerializable, string> ClientMethodTwo { get; set; }
			public Action<TestNonSerializable, string> ServerMethodTwo { get; set; }

			public override void InitializeService(NetContext context) {
				base.InitializeService(context);

				ClientMethod = ClientAction<TestSerializable>(clientMethod);
				ServerMethod = ServerAction<TestSerializable>(serverMethod);
				ClientMethodTwo = ClientAction<TestNonSerializable, string>(clientMethodTwo);
				ServerMethodTwo = ServerAction<TestNonSerializable, string>(serverMethodTwo);
			}

			private void clientMethodTwo(TestNonSerializable obj, string data) {
				var isClient = IsClient;
				var isServer = IsServer;

				ServerMethodTwo.Invoke(obj, data);
				Console.WriteLine("Sent to server");
			}

			private void serverMethodTwo(TestNonSerializable obj, string data) {
				var isClient = IsClient;
				var isServer = IsServer;

				ClientMethodTwo.Invoke(obj, data);
				Console.WriteLine("Sent to client");
			}

			private void serverMethod(TestSerializable obj) {
				var isClient = IsClient;
				var isServer = IsServer;

				ClientMethod.Invoke(obj);
				Console.WriteLine("Sent to client");
			}

			private void clientMethod(TestSerializable obj) {
				var isClient = IsClient;
				var isServer = IsServer;

				ServerMethod.Invoke(obj);
				Console.WriteLine("Sent to server");
			}
		}

		public enum TestEnum {
			One, Two, Three
		}

		public class TestStatic<T> {
			public static int i { get; set; }
		}

        static void Main(string[] args)
        {
			//var tttt = typeof(TestEnum);

			//TestStatic<int>.i = 5;
			//TestStatic<string>.i = 10;

			//Stopwatch sw = new Stopwatch();
			//sw.Start();

			//int iterations = 0;
			//while (sw.ElapsedMilliseconds < 1000) {
			//	iterations++;
			//	TestSerializable ss = new TestSerializable();
			//	ss.data = "Hello world!";
			//	ss.data1 = "Data 1";
			//	ss.NonSerializable = new TestNonSerializable() {
			//		data1 = "This should be serialized too"
			//	};
			//	ss.Enum = TestEnum.Three;
			//	ss.Intricate = new byte[2][] {
			//		new byte[2] { 0, 1 },
			//		new byte[2] { 2, 3 }
			//	};

			//	//s.data2 = "Data 2";
			//	ss.Inner = new TestSerializable() {
			//		data = "Hello inner world!",
			//		Inner = new TestSerializable() {
			//			data2 = "Super secret inner world!",
			//		},
			//		TestEnum = new List<string>() {
			//			"str1",
			//			"str2",
			//			"str3",
			//			"str4",
			//		}
			//	};

			//	DataBuffer b = new DataBuffer();
			//	ss.WriteTo(b);

			//	b.Seek(0);
			//	TestSerializable sss = new TestSerializable();
			//	sss.ReadFrom(b);
			//}


			//TestPacketTwo t = new TestPacketTwo();
			//t.TestInt = 1;
			//t.TestIntTwo = 2;
			//t.TestFloat = 3f;
			//t.TestFloatTwo = 4f;
			//t.TestString = "Hello";
			//t.TestStringTwo = "World";

			//DataBuffer bb = new DataBuffer();
			//t.WriteTo(bb);

			//bb.Seek(0);
			//TestPacketTwo result = new TestPacketTwo();
			//result.ReadFrom(bb);

			INetServer sv = new BasicServer();
			sv.InitializeServer(Context);
			sv.StartServer(new TCPServerProvider());

			INetClient cl = new BasicClient();
			cl.InitializeClient(Context);
			cl.OnConnected += (arg) => {
				TestSerializable s = new TestSerializable();
				s.data = "Hello world!";
				s.data1 = "Data 1";
				//s.data2 = "Data 2";
				s.Inner = new TestSerializable() {
					data = "Hello inner world!",
					Inner = new TestSerializable() {
						data2 = "Super secret inner world!",
					},
					TestEnum = new List<string>() {
						"str1",
						"str2",
						"str3",
						"str4",
					}
				};

				//TestNonSerializable ns = new TestNonSerializable();
				//ns.data1 = "Hello world!";

				cl.Context.Services.Get<TestRFCService>().ServerMethod(s);
			};

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
