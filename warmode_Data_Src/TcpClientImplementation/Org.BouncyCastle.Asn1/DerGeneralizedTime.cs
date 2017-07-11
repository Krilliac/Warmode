using Org.BouncyCastle.Utilities;
using System;
using System.Globalization;
using System.Text;

namespace Org.BouncyCastle.Asn1
{
	public class DerGeneralizedTime : Asn1Object
	{
		private readonly string time;

		public string TimeString
		{
			get
			{
				return this.time;
			}
		}

		private bool HasFractionalSeconds
		{
			get
			{
				return this.time.IndexOf('.') == 14;
			}
		}

		public static DerGeneralizedTime GetInstance(object obj)
		{
			if (obj == null || obj is DerGeneralizedTime)
			{
				return (DerGeneralizedTime)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerGeneralizedTime GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerGeneralizedTime)
			{
				return DerGeneralizedTime.GetInstance(@object);
			}
			return new DerGeneralizedTime(((Asn1OctetString)@object).GetOctets());
		}

		public DerGeneralizedTime(string time)
		{
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

		public DerGeneralizedTime(DateTime time)
		{
			this.time = time.ToString("yyyyMMddHHmmss\\Z");
		}

		internal DerGeneralizedTime(byte[] bytes)
		{
			this.time = Strings.FromAsciiByteArray(bytes);
		}

		public string GetTime()
		{
			if (this.time[this.time.Length - 1] == 'Z')
			{
				return this.time.Substring(0, this.time.Length - 1) + "GMT+00:00";
			}
			int num = this.time.Length - 5;
			char c = this.time[num];
			if (c == '-' || c == '+')
			{
				return string.Concat(new string[]
				{
					this.time.Substring(0, num),
					"GMT",
					this.time.Substring(num, 3),
					":",
					this.time.Substring(num + 3)
				});
			}
			num = this.time.Length - 3;
			c = this.time[num];
			if (c == '-' || c == '+')
			{
				return this.time.Substring(0, num) + "GMT" + this.time.Substring(num) + ":00";
			}
			return this.time + this.CalculateGmtOffset();
		}

		private string CalculateGmtOffset()
		{
			char c = '+';
			DateTime dateTime = this.ToDateTime();
			TimeSpan timeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
			if (timeSpan.CompareTo(TimeSpan.Zero) < 0)
			{
				c = '-';
				timeSpan = timeSpan.Duration();
			}
			int hours = timeSpan.Hours;
			int minutes = timeSpan.Minutes;
			return string.Concat(new object[]
			{
				"GMT",
				c,
				DerGeneralizedTime.Convert(hours),
				":",
				DerGeneralizedTime.Convert(minutes)
			});
		}

		private static string Convert(int time)
		{
			if (time < 10)
			{
				return "0" + time;
			}
			return time.ToString();
		}

		public DateTime ToDateTime()
		{
			string text = this.time;
			bool makeUniversal = false;
			string format;
			if (text.EndsWith("Z"))
			{
				if (this.HasFractionalSeconds)
				{
					int count = text.Length - text.IndexOf('.') - 2;
					format = "yyyyMMddHHmmss." + this.FString(count) + "\\Z";
				}
				else
				{
					format = "yyyyMMddHHmmss\\Z";
				}
			}
			else if (this.time.IndexOf('-') > 0 || this.time.IndexOf('+') > 0)
			{
				text = this.GetTime();
				makeUniversal = true;
				if (this.HasFractionalSeconds)
				{
					int count2 = text.IndexOf("GMT") - 1 - text.IndexOf('.');
					format = "yyyyMMddHHmmss." + this.FString(count2) + "'GMT'zzz";
				}
				else
				{
					format = "yyyyMMddHHmmss'GMT'zzz";
				}
			}
			else if (this.HasFractionalSeconds)
			{
				int count3 = text.Length - 1 - text.IndexOf('.');
				format = "yyyyMMddHHmmss." + this.FString(count3);
			}
			else
			{
				format = "yyyyMMddHHmmss";
			}
			return this.ParseDateString(text, format, makeUniversal);
		}

		private string FString(int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append('f');
			}
			return stringBuilder.ToString();
		}

		private DateTime ParseDateString(string s, string format, bool makeUniversal)
		{
			DateTimeStyles dateTimeStyles = DateTimeStyles.None;
			if (format.EndsWith("Z"))
			{
				try
				{
					dateTimeStyles = (DateTimeStyles)Enum.Parse(typeof(DateTimeStyles), "AssumeUniversal");
				}
				catch (Exception)
				{
				}
				dateTimeStyles |= DateTimeStyles.AdjustToUniversal;
			}
			DateTime result = DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, dateTimeStyles);
			if (!makeUniversal)
			{
				return result;
			}
			return result.ToUniversalTime();
		}

		private byte[] GetOctets()
		{
			return Strings.ToAsciiByteArray(this.time);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(24, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerGeneralizedTime derGeneralizedTime = asn1Object as DerGeneralizedTime;
			return derGeneralizedTime != null && this.time.Equals(derGeneralizedTime.time);
		}

		protected override int Asn1GetHashCode()
		{
			return this.time.GetHashCode();
		}
	}
}
