using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class OptionalValidity : Asn1Encodable
	{
		private readonly Time notBefore;

		private readonly Time notAfter;

		public virtual Time NotBefore
		{
			get
			{
				return this.notBefore;
			}
		}

		public virtual Time NotAfter
		{
			get
			{
				return this.notAfter;
			}
		}

		private OptionalValidity(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				if (asn1TaggedObject.TagNo == 0)
				{
					this.notBefore = Time.GetInstance(asn1TaggedObject, true);
				}
				else
				{
					this.notAfter = Time.GetInstance(asn1TaggedObject, true);
				}
			}
		}

		public static OptionalValidity GetInstance(object obj)
		{
			if (obj == null || obj is OptionalValidity)
			{
				return (OptionalValidity)obj;
			}
			return new OptionalValidity(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.notBefore != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.notBefore)
				});
			}
			if (this.notAfter != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.notAfter)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
