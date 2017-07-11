using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class RevokedInfo : Asn1Encodable
	{
		private readonly DerGeneralizedTime revocationTime;

		private readonly CrlReason revocationReason;

		public DerGeneralizedTime RevocationTime
		{
			get
			{
				return this.revocationTime;
			}
		}

		public CrlReason RevocationReason
		{
			get
			{
				return this.revocationReason;
			}
		}

		public static RevokedInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return RevokedInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static RevokedInfo GetInstance(object obj)
		{
			if (obj == null || obj is RevokedInfo)
			{
				return (RevokedInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RevokedInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public RevokedInfo(DerGeneralizedTime revocationTime) : this(revocationTime, null)
		{
		}

		public RevokedInfo(DerGeneralizedTime revocationTime, CrlReason revocationReason)
		{
			if (revocationTime == null)
			{
				throw new ArgumentNullException("revocationTime");
			}
			this.revocationTime = revocationTime;
			this.revocationReason = revocationReason;
		}

		private RevokedInfo(Asn1Sequence seq)
		{
			this.revocationTime = (DerGeneralizedTime)seq[0];
			if (seq.Count > 1)
			{
				this.revocationReason = new CrlReason(DerEnumerated.GetInstance((Asn1TaggedObject)seq[1], true));
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.revocationTime
			});
			if (this.revocationReason != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.revocationReason)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
