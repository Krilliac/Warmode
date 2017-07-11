using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AttributeCertificateInfo : Asn1Encodable
	{
		internal readonly DerInteger version;

		internal readonly Holder holder;

		internal readonly AttCertIssuer issuer;

		internal readonly AlgorithmIdentifier signature;

		internal readonly DerInteger serialNumber;

		internal readonly AttCertValidityPeriod attrCertValidityPeriod;

		internal readonly Asn1Sequence attributes;

		internal readonly DerBitString issuerUniqueID;

		internal readonly X509Extensions extensions;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public Holder Holder
		{
			get
			{
				return this.holder;
			}
		}

		public AttCertIssuer Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public AlgorithmIdentifier Signature
		{
			get
			{
				return this.signature;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public AttCertValidityPeriod AttrCertValidityPeriod
		{
			get
			{
				return this.attrCertValidityPeriod;
			}
		}

		public Asn1Sequence Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public DerBitString IssuerUniqueID
		{
			get
			{
				return this.issuerUniqueID;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		public static AttributeCertificateInfo GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return AttributeCertificateInfo.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static AttributeCertificateInfo GetInstance(object obj)
		{
			if (obj is AttributeCertificateInfo)
			{
				return (AttributeCertificateInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AttributeCertificateInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private AttributeCertificateInfo(Asn1Sequence seq)
		{
			if (seq.Count < 7 || seq.Count > 9)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.version = DerInteger.GetInstance(seq[0]);
			this.holder = Holder.GetInstance(seq[1]);
			this.issuer = AttCertIssuer.GetInstance(seq[2]);
			this.signature = AlgorithmIdentifier.GetInstance(seq[3]);
			this.serialNumber = DerInteger.GetInstance(seq[4]);
			this.attrCertValidityPeriod = AttCertValidityPeriod.GetInstance(seq[5]);
			this.attributes = Asn1Sequence.GetInstance(seq[6]);
			for (int i = 7; i < seq.Count; i++)
			{
				Asn1Encodable asn1Encodable = seq[i];
				if (asn1Encodable is DerBitString)
				{
					this.issuerUniqueID = DerBitString.GetInstance(seq[i]);
				}
				else if (asn1Encodable is Asn1Sequence || asn1Encodable is X509Extensions)
				{
					this.extensions = X509Extensions.GetInstance(seq[i]);
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.holder,
				this.issuer,
				this.signature,
				this.serialNumber,
				this.attrCertValidityPeriod,
				this.attributes
			});
			if (this.issuerUniqueID != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.issuerUniqueID
				});
			}
			if (this.extensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.extensions
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
