using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SocketIO
{
	public sealed class SocketOptions
	{
		private float randomizationFactor;

		private string BuiltQueryParams;

		public bool Reconnection
		{
			get;
			set;
		}

		public int ReconnectionAttempts
		{
			get;
			set;
		}

		public TimeSpan ReconnectionDelay
		{
			get;
			set;
		}

		public TimeSpan ReconnectionDelayMax
		{
			get;
			set;
		}

		public float RandomizationFactor
		{
			get
			{
				return this.randomizationFactor;
			}
			set
			{
				this.randomizationFactor = Math.Min(1f, Math.Max(0f, value));
			}
		}

		public TimeSpan Timeout
		{
			get;
			set;
		}

		public bool AutoConnect
		{
			get;
			set;
		}

		public Dictionary<string, string> AdditionalQueryParams
		{
			get;
			set;
		}

		public bool QueryParamsOnlyForHandshake
		{
			get;
			set;
		}

		public SocketOptions()
		{
			this.Reconnection = true;
			this.ReconnectionAttempts = 2147483647;
			this.ReconnectionDelay = TimeSpan.FromMilliseconds(1000.0);
			this.ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000.0);
			this.RandomizationFactor = 0.5f;
			this.Timeout = TimeSpan.FromMilliseconds(20000.0);
			this.AutoConnect = true;
			this.QueryParamsOnlyForHandshake = true;
		}

		internal string BuildQueryParams()
		{
			if (this.AdditionalQueryParams == null || this.AdditionalQueryParams.Count == 0)
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(this.BuiltQueryParams))
			{
				return this.BuiltQueryParams;
			}
			StringBuilder stringBuilder = new StringBuilder(this.AdditionalQueryParams.Count * 4);
			foreach (KeyValuePair<string, string> current in this.AdditionalQueryParams)
			{
				stringBuilder.Append("&");
				stringBuilder.Append(current.Key);
				if (!string.IsNullOrEmpty(current.Value))
				{
					stringBuilder.Append("=");
					stringBuilder.Append(current.Value);
				}
			}
			return this.BuiltQueryParams = stringBuilder.ToString();
		}
	}
}
