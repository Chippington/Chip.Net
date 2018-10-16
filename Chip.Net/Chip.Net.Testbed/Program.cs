using Chip.Net.Default.Basic;
using Chip.Net.Providers.TCP;
using Chip.Net.Services.NetTime;
using Chip.Net.Services.Ping;
using System;

namespace Chip.Net.Testbed
{
    class Program
    {
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
