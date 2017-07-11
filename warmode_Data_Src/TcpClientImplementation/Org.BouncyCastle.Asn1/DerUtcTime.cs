using Org.BouncyCastle.Utilities;
using System;
using System.Globalization;

namespace Org.BouncyCastle.Asn1
{
	public class DerUtcTime : Asn1Object
	{
		private readonly string time;

		public string TimeString
		{
			get
			{
				if (this.time.IndexOf('-') < 0 && this.time.IndexOf('+') < 0)
				{
					if (this.time.Length == 11)
					{
						return this.time.Substring(0, 10) + "00GMT+00:00";
					}
					return this.time.Substring(0, 12) + "GMT+00:00";
				}
				else
				{
					int num = this.time.IndexOf('-');
					if (num < 0)
					{
						num = this.time.IndexOf('+');
					}
					string text = this.time;
					if (num == this.time.Length - 3)
					{
						text += "00";
					}
					if (num == 10)
					{
						return string.Concat(new string[]
						{
							text.Substring(0, 10),
							"00GMT",
							text.Substring(10, 3),
							":",
							text.Substring(13, 2)
						});
					}
					return string.Concat(new string[]
					{
						text.Substring(0, 12),
						"GMT",
						text.Substring(12, 3),
						":",
						text.Substring(15, 2)
					});
				}
			}
		}

		[Obsolete("Use 'AdjustedTimeString' property instead")]
		public string AdjustedTime
		{
			get
			{
				return this.AdjustedTimeString;
			}
		}

		public string AdjustedTimeString
		{
			get
			{
				string timeString = this.TimeString;
				string str = (timeString[0] < '5') ? "20" : "19";
				return str + timeString;
			}
		}

		public static DerUtcTime GetInstance(object obj)
		{
			if (obj == null || obj is DerUtcTime)
			{
				return (DerUtcTime)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerUtcTime GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerUtcTime)
			{
				return DerUtcTime.GetInstance(@object);
			}
			return new DerUtcTime(((Asn1OctetString)@object).GetOctets());
		}

		public DerUtcTime(string time)
		{
			if (time == null)
			{
				throw new ArgumentNullException("time");
			}
			this.time = time;
			try
			{
				this.ToDateTime();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("invalid date string: " + ex.Message);
			}
		}

		public DerUtcTime(DateTime time)
		{
			this.time = time.ToString("yyMMddHHmmss") + "Z";
		}

		internal DerUtcTime(byte[] bytes)
		{
			this.time = Strings.FromAsciiByteArray(bytes);
		}

		public DateTime ToDateTime()
		{
			return this.ParseDateString(this.TimeString, "yyMMddHHmmss'GMT'zzz");
		}

		public DateTime ToAdjustedDateTime()
		{
			return this.ParseDateString(this.AdjustedTimeString, "yyyyMMddHHmmss'GMT'zzz");
		}

		private DateTime ParseDateString(string dateStr, string formatStr)
		{
			return DateTime.ParseExact(dateStr, formatStr, DateTimeFormatInfo.InvariantInfo).ToUniversalTime();
		}

		private byte[] GetOctets()
		{
			return Strings.ToAsciiByteArray(this.time);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(23, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerUtcTime derUtcTime = asn1Object as DerUtcTime;
			return derUtcTime != null && this.time.Equals(derUtcTime.time);
		}

		protected override int Asn1GetHashCode()
		{
			return this.time.GetHashCode();
		}

		public override string ToString()
		{
			return this.time;
		}
	}
}
