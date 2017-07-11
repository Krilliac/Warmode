using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class CrlID : Asn1Encodable
	{
		private readonly DerIA5String crlUrl;

		private readonly DerInteger crlNum;

		private readonly DerGeneralizedTime crlTime;

		public DerIA5String CrlUrl
		{
			get
			{
				return this.crlUrl;
			}
		}

		public DerInteger CrlNum
		{
			get
			{
				return this.crlNum;
			}
		}

		public DerGeneralizedTime CrlTime
		{
			get
			{
				return this.crlTime;
			}
		}

		public CrlID(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.crlUrl = DerIA5String.GetInstance(asn1TaggedObject, true);
					break;
				case 1:
					this.crlNum = DerInteger.GetInstance(asn1TaggedObject, true);
					break;
				case 2:
					this.crlTime = DerGeneralizedTime.GetInstance(asn1TaggedObject, true);
					break;
				default:
					throw new ArgumentException("unknown tag number: " + asn1TaggedObject.TagNo);
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.crlUrl != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.crlUrl)
				});
			}
			if (this.crlNum != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.crlNum)
				});
			}
			if (this.crlTime != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.crlTime)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
