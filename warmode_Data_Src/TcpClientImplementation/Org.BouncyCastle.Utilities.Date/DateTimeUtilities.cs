using System;

namespace Org.BouncyCastle.Utilities.Date
{
	public class DateTimeUtilities
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

		private DateTimeUtilities()
		{
		}

		public static long DateTimeToUnixMs(DateTime dateTime)
		{
			if (dateTime.CompareTo(DateTimeUtilities.UnixEpoch) < 0)
			{
				throw new ArgumentException("DateTime value may not be before the epoch", "dateTime");
			}
			return (dateTime.Ticks - DateTimeUtilities.UnixEpoch.Ticks) / 10000L;
		}

		public static DateTime UnixMsToDateTime(long unixMs)
		{
			return new DateTime(unixMs * 10000L + DateTimeUtilities.UnixEpoch.Ticks);
		}

		public static long CurrentUnixMs()
		{
			return DateTimeUtilities.DateTimeToUnixMs(DateTime.UtcNow);
		}
	}
}
