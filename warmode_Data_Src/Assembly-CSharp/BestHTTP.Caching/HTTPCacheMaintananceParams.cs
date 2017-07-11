using System;

namespace BestHTTP.Caching
{
	public sealed class HTTPCacheMaintananceParams
	{
		public TimeSpan DeleteOlder
		{
			get;
			private set;
		}

		public ulong MaxCacheSize
		{
			get;
			private set;
		}

		public HTTPCacheMaintananceParams(TimeSpan deleteOlder, ulong maxCacheSize)
		{
			this.DeleteOlder = deleteOlder;
			this.MaxCacheSize = maxCacheSize;
		}
	}
}
