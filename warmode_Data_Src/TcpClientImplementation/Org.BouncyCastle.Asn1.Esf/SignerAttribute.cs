using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class SignerAttribute : Asn1Encodable
	{
		private Asn1Sequence claimedAttributes;

		private AttributeCertificate certifiedAttributes;

		public virtual Asn1Sequence ClaimedAttributes
		{
			get
			{
				return this.claimedAttributes;
			}
		}

		public virtual AttributeCertificate CertifiedAttributes
		{
			get
			{
				return this.certifiedAttributes;
			}
		}

		public static SignerAttribute GetInstance(object obj)
		{
			if (obj == null || obj is SignerAttribute)
			{
				return (SignerAttribute)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SignerAttribute(obj);
			}
			throw new ArgumentException("Unknown object in 'SignerAttribute' factory: " + obj.GetType().Name, "obj");
		}

		private SignerAttribute(object obj)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)obj;
			DerTaggedObject derTaggedObject = (DerTaggedObject)asn1Sequence[0];
			if (derTaggedObject.TagNo == 0)
			{
				this.claimedAttributes = Asn1Sequence.GetInstance(derTaggedObject, true);
				return;
			}
			if (derTaggedObject.TagNo == 1)
			{
				this.certifiedAttributes = AttributeCertificate.GetInstance(derTaggedObject);
				return;
			}
			throw new ArgumentException("illegal tag.", "obj");
		}

		public SignerAttribute(Asn1Sequence claimedAttributes)
		{
			this.claimedAttributes = claimedAttributes;
		}

		public SignerAttribute(AttributeCertificate certifiedAttributes)
		{
			this.certifiedAttributes = certifiedAttributes;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.claimedAttributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.claimedAttributes)
				});
			}
			else
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(1, this.certifiedAttributes)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
