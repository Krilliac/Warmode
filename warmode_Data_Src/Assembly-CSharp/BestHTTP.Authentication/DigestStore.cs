using System;
using System.Collections.Generic;

namespace BestHTTP.Authentication
{
	internal static class DigestStore
	{
		private static Dictionary<string, Digest> Digests = new Dictionary<string, Digest>();

		private static object Locker = new object();

		public static Digest Get(Uri uri)
		{
			object locker = DigestStore.Locker;
			Digest result;
			lock (locker)
			{
				Digest digest = null;
				if (DigestStore.Digests.TryGetValue(uri.Host, out digest) && !digest.IsUriProtected(uri))
				{
					result = null;
				}
				else
				{
					result = digest;
				}
			}
			return result;
		}

		public static Digest GetOrCreate(Uri uri)
		{
			object locker = DigestStore.Locker;
			Digest result;
			lock (locker)
			{
				Digest digest = null;
				if (!DigestStore.Digests.TryGetValue(uri.Host, out digest))
				{
					DigestStore.Digests.Add(uri.Host, digest = new Digest(uri));
				}
				result = digest;
			}
			return result;
		}

		public static void Remove(Uri uri)
		{
			object locker = DigestStore.Locker;
			lock (locker)
			{
				DigestStore.Digests.Remove(uri.Host);
			}
		}
	}
}
