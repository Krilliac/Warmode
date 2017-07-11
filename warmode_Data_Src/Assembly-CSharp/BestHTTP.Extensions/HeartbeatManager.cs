using System;
using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	public sealed class HeartbeatManager
	{
		private List<IHeartbeat> Heartbeats = new List<IHeartbeat>();

		private IHeartbeat[] UpdateArray;

		private DateTime LastUpdate = DateTime.MinValue;

		public void Subscribe(IHeartbeat heartbeat)
		{
			List<IHeartbeat> heartbeats = this.Heartbeats;
			lock (heartbeats)
			{
				if (!this.Heartbeats.Contains(heartbeat))
				{
					this.Heartbeats.Add(heartbeat);
				}
			}
		}

		public void Unsubscribe(IHeartbeat heartbeat)
		{
			List<IHeartbeat> heartbeats = this.Heartbeats;
			lock (heartbeats)
			{
				this.Heartbeats.Remove(heartbeat);
			}
		}

		public void Update()
		{
			if (this.LastUpdate == DateTime.MinValue)
			{
				this.LastUpdate = DateTime.UtcNow;
			}
			else
			{
				TimeSpan dif = DateTime.UtcNow - this.LastUpdate;
				this.LastUpdate = DateTime.UtcNow;
				int num = 0;
				List<IHeartbeat> heartbeats = this.Heartbeats;
				lock (heartbeats)
				{
					if (this.UpdateArray == null || this.UpdateArray.Length < this.Heartbeats.Count)
					{
						Array.Resize<IHeartbeat>(ref this.UpdateArray, this.Heartbeats.Count);
					}
					this.Heartbeats.CopyTo(0, this.UpdateArray, 0, this.Heartbeats.Count);
					num = this.Heartbeats.Count;
				}
				for (int i = 0; i < num; i++)
				{
					try
					{
						this.UpdateArray[i].OnHeartbeatUpdate(dif);
					}
					catch
					{
					}
				}
			}
		}
	}
}
