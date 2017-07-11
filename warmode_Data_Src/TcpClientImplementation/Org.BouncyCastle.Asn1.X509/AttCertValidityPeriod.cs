using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AttCertValidityPeriod : Asn1Encodable
	{
		private readonly DerGeneralizedTime notBeforeTime;

		private readonly DerGeneralizedTime notAfterTime;

		public DerGeneralizedTime NotBeforeTime
		{
			get
			{
				return this.notBeforeTime;
			}
		}

		public DerGeneralizedTime NotAfterTime
		{
			get
			{
				return this.notAfterTime;
			}
		}

		public static AttCertValidityPeriod GetInstance(object obj)
		{
			if (obj is AttCertValidityPeriod || obj == null)
			{
				return (AttCertValidityPeriod)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AttCertValidityPeriod((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static AttCertValidityPeriod GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return AttCertValidityPeriod.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		private AttCertValidityPeriod(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.notBeforeTime = DerGeneralizedTime.GetInstance(seq[0]);
			this.notAfterTime = DerGeneralizedTime.GetInstance(seq[1]);
		}

		public AttCertValidityPeriod(DerGeneralizedTime notBeforeTime, DerGeneralizedTime notAfterTime)
		{
			this.notBeforeTime = notBeforeTime;
			this.notAfterTime = notAfterTime;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.notBeforeTime,
				this.notAfterTime
			});
		}
	}
}
