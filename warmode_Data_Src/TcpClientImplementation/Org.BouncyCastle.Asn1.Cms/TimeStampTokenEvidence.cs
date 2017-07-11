using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class TimeStampTokenEvidence : Asn1Encodable
	{
		private TimeStampAndCrl[] timeStampAndCrls;

		public TimeStampTokenEvidence(TimeStampAndCrl[] timeStampAndCrls)
		{
			this.timeStampAndCrls = timeStampAndCrls;
		}

		public TimeStampTokenEvidence(TimeStampAndCrl timeStampAndCrl)
		{
			this.timeStampAndCrls = new TimeStampAndCrl[]
			{
				timeStampAndCrl
			};
		}

		private TimeStampTokenEvidence(Asn1Sequence seq)
		{
			this.timeStampAndCrls = new TimeStampAndCrl[seq.Count];
			int num = 0;
			foreach (Asn1Encodable asn1Encodable in seq)
			{
				this.timeStampAndCrls[num++] = TimeStampAndCrl.GetInstance(asn1Encodable.ToAsn1Object());
			}
		}

		public static TimeStampTokenEvidence GetInstance(Asn1TaggedObject tagged, bool isExplicit)
		{
			return TimeStampTokenEvidence.GetInstance(Asn1Sequence.GetInstance(tagged, isExplicit));
		}

		public static TimeStampTokenEvidence GetInstance(object obj)
		{
			if (obj is TimeStampTokenEvidence)
			{
				return (TimeStampTokenEvidence)obj;
			}
			if (obj != null)
			{
				return new TimeStampTokenEvidence(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public virtual TimeStampAndCrl[] ToTimeStampAndCrlArray()
		{
			return (TimeStampAndCrl[])this.timeStampAndCrls.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(this.timeStampAndCrls);
		}
	}
}
