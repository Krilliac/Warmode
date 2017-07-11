using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class OobCertHash : Asn1Encodable
	{
		private readonly AlgorithmIdentifier hashAlg;

		private readonly CertId certId;

		private readonly DerBitString hashVal;

		public virtual AlgorithmIdentifier HashAlg
		{
			get
			{
				return this.hashAlg;
			}
		}

		public virtual CertId CertID
		{
			get
			{
				return this.certId;
			}
		}

		private OobCertHash(Asn1Sequence seq)
		{
			int num = seq.Count - 1;
			this.hashVal = DerBitString.GetInstance(seq[num--]);
			for (int i = num; i >= 0; i--)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[i];
				if (asn1TaggedObject.TagNo == 0)
				{
					this.hashAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
				}
				else
				{
					this.certId = CertId.GetInstance(asn1TaggedObject, true);
				}
			}
		}

		public static OobCertHash GetInstance(object obj)
		{
			if (obj is OobCertHash)
			{
				return (OobCertHash)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OobCertHash((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			this.AddOptional(asn1EncodableVector, 0, this.hashAlg);
			this.AddOptional(asn1EncodableVector, 1, this.certId);
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.hashVal
			});
			return new DerSequence(asn1EncodableVector);
		}

		private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, tagNo, obj)
				});
			}
		}
	}
}
