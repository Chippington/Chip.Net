using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chip.Net.Services.NetTime {
	public class NetTimeService : NetService {
		private Stopwatch serverTimer;
		private Stopwatch clientTimer;
		private Stopwatch pingTimer;

		private double netTime;
		private double offset;

		public double NetTime {
			get {
				return netTime;
			}
		}

		public override void InitializeService(NetContext context) {
			base.InitializeService(context);
			clientTimer = new Stopwatch();
			serverTimer = new Stopwatch();
			pingTimer = new Stopwatch();

			if(IsClient)
				Client.OnConnected += OnConnected;

			context.Packets.Register<P_GetNetTime>();
			context.Packets.Register<P_SetNetTime>();

			Router.RouteClient<P_SetNetTime>(OnSetNetTime);
			Router.RouteServer<P_GetNetTime>(OnGetNetTime);
		}

		private void OnSetNetTime(P_SetNetTime obj) {
			var svNetTime = obj.NetTime;
			var diff = pingTimer.Elapsed.TotalSeconds;
			svNetTime += diff / 2d;

			offset = svNetTime - clientTimer.Elapsed.TotalSeconds;

			ScheduleEvent(250, () => {
				pingTimer.Reset();
				pingTimer.Start();
				P_GetNetTime msg = new P_GetNetTime();
				SendPacketToServer(msg);
			});
		}

		private void OnGetNetTime(P_GetNetTime obj) {
			P_SetNetTime reply = new P_SetNetTime();
			reply.NetTime = GetNetTime();
			SendPacketToClient(obj.Sender, reply);
		}

		public override void StartService() {
			base.StartService();
			if (IsServer) {
				serverTimer.Reset();
				serverTimer.Start();
			}

			if(IsClient) {
				clientTimer.Reset();
				clientTimer.Start();
			}
		}

		private void OnConnected(object sender, NetEventArgs args) {
			P_GetNetTime msg = new P_GetNetTime();
			SendPacketToServer(msg);

			pingTimer.Reset();
			pingTimer.Start();
		}

		public override void UpdateService() {
			base.UpdateService();

			if(IsServer)
				netTime = serverTimer.Elapsed.TotalSeconds;

			if (IsClient)
				netTime = clientTimer.Elapsed.TotalSeconds + offset;
		}

		public override void StopService() {
			base.StopService();
		}

		public double GetNetTime() {
			return netTime;
		}
	}
}
