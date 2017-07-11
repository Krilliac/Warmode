using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using System;

namespace Org.BouncyCastle.Tsp
{
	public class GenTimeAccuracy
	{
		private Accuracy accuracy;

		public int Seconds
		{
			get
			{
				return this.GetTimeComponent(this.accuracy.Seconds);
			}
		}

		public int Millis
		{
			get
			{
				return this.GetTimeComponent(this.accuracy.Millis);
			}
		}

		public int Micros
		{
			get
			{
				return this.GetTimeComponent(this.accuracy.Micros);
			}
		}

		public GenTimeAccuracy(Accuracy accuracy)
		{
			this.accuracy = accuracy;
		}

		private int GetTimeComponent(DerInteger time)
		{
			if (time != null)
			{
				return time.Value.IntValue;
			}
			return 0;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Seconds,
				".",
				this.Millis.ToString("000"),
				this.Micros.ToString("000")
			});
		}
	}
}
