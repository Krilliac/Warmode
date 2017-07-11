using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertTemplate : Asn1Encodable
	{
		private readonly Asn1Sequence seq;

		private readonly DerInteger version;

		private readonly DerInteger serialNumber;

		private readonly AlgorithmIdentifier signingAlg;

		private readonly X509Name issuer;

		private readonly OptionalValidity validity;

		private readonly X509Name subject;

		private readonly SubjectPublicKeyInfo publicKey;

		private readonly DerBitString issuerUID;

		private readonly DerBitString subjectUID;

		private readonly X509Extensions extensions;

		public virtual int Version
		{
			get
			{
				return this.version.Value.IntValue;
			}
		}

		public virtual DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public virtual AlgorithmIdentifier SigningAlg
		{
			get
			{
				return this.signingAlg;
			}
		}

		public virtual X509Name Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public virtual OptionalValidity Validity
		{
			get
			{
				return this.validity;
			}
		}

		public virtual X509Name Subject
		{
			get
			{
				return this.subject;
			}
		}

		public virtual SubjectPublicKeyInfo PublicKey
		{
			get
			{
				return this.publicKey;
			}
		}

		public virtual DerBitString IssuerUID
		{
			get
			{
				return this.issuerUID;
			}
		}

		public virtual DerBitString SubjectUID
		{
			get
			{
				return this.subjectUID;
			}
		}

		public virtual X509Extensions Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		private CertTemplate(Asn1Sequence seq)
		{
			this.seq = seq;
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.version = DerInteger.GetInstance(asn1TaggedObject, false);
					break;
				case 1:
					this.serialNumber = DerInteger.GetInstance(asn1TaggedObject, false);
					break;
				case 2:
					this.signingAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 3:
					this.issuer = X509Name.GetInstance(asn1TaggedObject, true);
					break;
				case 4:
					this.validity = OptionalValidity.GetInstance(Asn1Sequence.GetInstance(asn1TaggedObject, false));
					break;
				case 5:
					this.subject = X509Name.GetInstance(asn1TaggedObject, true);
					break;
				case 6:
					this.publicKey = SubjectPublicKeyInfo.GetInstance(asn1TaggedObject, false);
					break;
				case 7:
					this.issuerUID = DerBitString.GetInstance(asn1TaggedObject, false);
					break;
				case 8:
					this.subjectUID = DerBitString.GetInstance(asn1TaggedObject, false);
					break;
				case 9:
					this.extensions = X509Extensions.GetInstance(asn1TaggedObject, false);
					break;
				default:
					throw new ArgumentException("unknown tag: " + asn1TaggedObject.TagNo, "seq");
				}
			}
		}

		public static CertTemplate GetInstance(object obj)
		{
			if (obj is CertTemplate)
			{
				return (CertTemplate)obj;
			}
			if (obj != null)
			{
				return new CertTemplate(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
