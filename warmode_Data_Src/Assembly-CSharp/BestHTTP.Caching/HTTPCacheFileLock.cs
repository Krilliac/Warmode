using System;
using System.Collections.Generic;

namespace BestHTTP.Caching
{
	internal sealed class HTTPCacheFileLock
	{
		private static Dictionary<Uri, object> FileLocks = new Dictionary<Uri, object>();

		private static object SyncRoot = new object();

		internal static object Acquire(Uri uri)
		{
			object syncRoot = HTTPCacheFileLock.SyncRoot;
			object result;
			lock (syncRoot)
			{
				object obj;
				if (!HTTPCacheFileLock.FileLocks.TryGetValue(uri, out obj))
				{
					HTTPCacheFileLock.FileLocks.Add(uri, obj = new object());
				}
				result = obj;
			}
			return result;
		}

		internal static void Remove(Uri uri)
		{
			object syncRoot = HTTPCacheFileLock.SyncRoot;
			lock (syncRoot)
			{
				if (HTTPCacheFileLock.FileLocks.ContainsKey(uri))
				{
					HTTPCacheFileLock.FileLocks.Remove(uri);
				}
			}
		}

		internal static void Clear()
		{
			object syncRoot = HTTPCacheFileLock.SyncRoot;
			lock (syncRoot)
			{
				HTTPCacheFileLock.FileLocks.Clear();
			}
		}
	}
}
