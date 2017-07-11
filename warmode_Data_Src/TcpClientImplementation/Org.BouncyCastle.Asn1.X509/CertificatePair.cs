using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CertificatePair : Asn1Encodable
	{
		private X509CertificateStructure forward;

		private X509CertificateStructure reverse;

		public X509CertificateStructure Forward
		{
			get
			{
				return this.forward;
			}
		}

		public X509CertificateStructure Reverse
		{
			get
			{
				return this.reverse;
			}
		}

		public static CertificatePair GetInstance(object obj)
		{
			if (obj == null || obj is CertificatePair)
			{
				return (CertificatePair)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertificatePair((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private CertificatePair(Asn1Sequence seq)
		{
			if (seq.Count != 1 && seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			foreach (object current in seq)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(current);
				if (instance.TagNo == 0)
				{
					this.forward = X509CertificateStructure.GetInstance(instance, true);
				}
				else
				{
					if (instance.TagNo != 1)
					{
						throw new ArgumentException("Bad tag number: " + instance.TagNo);
					}
					this.reverse = X509CertificateStructure.GetInstance(instance, true);
				}
			}
		}

		public CertificatePair(X509CertificateStructure forward, X509CertificateStructure reverse)
		{
			this.forward = forward;
			this.reverse = reverse;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.forward != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.forward)
				});
			}
			if (this.reverse != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(1, this.reverse)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
