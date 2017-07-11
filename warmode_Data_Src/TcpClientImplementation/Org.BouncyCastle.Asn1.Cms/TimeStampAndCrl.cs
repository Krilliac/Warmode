using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class TimeStampAndCrl : Asn1Encodable
	{
		private ContentInfo timeStamp;

		private CertificateList crl;

		public virtual ContentInfo TimeStampToken
		{
			get
			{
				return this.timeStamp;
			}
		}

		public virtual CertificateList Crl
		{
			get
			{
				return this.crl;
			}
		}

		public TimeStampAndCrl(ContentInfo timeStamp)
		{
			this.timeStamp = timeStamp;
		}

		private TimeStampAndCrl(Asn1Sequence seq)
		{
			this.timeStamp = ContentInfo.GetInstance(seq[0]);
			if (seq.Count == 2)
			{
				this.crl = CertificateList.GetInstance(seq[1]);
			}
		}

		public static TimeStampAndCrl GetInstance(object obj)
		{
			if (obj is TimeStampAndCrl)
			{
				return (TimeStampAndCrl)obj;
			}
			if (obj != null)
			{
				return new TimeStampAndCrl(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.timeStamp
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.crl
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
