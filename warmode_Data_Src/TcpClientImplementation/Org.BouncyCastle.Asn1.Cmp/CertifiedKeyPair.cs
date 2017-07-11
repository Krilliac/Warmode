using Org.BouncyCastle.Asn1.Crmf;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertifiedKeyPair : Asn1Encodable
	{
		private readonly CertOrEncCert certOrEncCert;

		private readonly EncryptedValue privateKey;

		private readonly PkiPublicationInfo publicationInfo;

		public virtual CertOrEncCert CertOrEncCert
		{
			get
			{
				return this.certOrEncCert;
			}
		}

		public virtual EncryptedValue PrivateKey
		{
			get
			{
				return this.privateKey;
			}
		}

		public virtual PkiPublicationInfo PublicationInfo
		{
			get
			{
				return this.publicationInfo;
			}
		}

		private CertifiedKeyPair(Asn1Sequence seq)
		{
			this.certOrEncCert = CertOrEncCert.GetInstance(seq[0]);
			if (seq.Count >= 2)
			{
				if (seq.Count == 2)
				{
					Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[1]);
					if (instance.TagNo == 0)
					{
						this.privateKey = EncryptedValue.GetInstance(instance.GetObject());
						return;
					}
					this.publicationInfo = PkiPublicationInfo.GetInstance(instance.GetObject());
					return;
				}
				else
				{
					this.privateKey = EncryptedValue.GetInstance(Asn1TaggedObject.GetInstance(seq[1]));
					this.publicationInfo = PkiPublicationInfo.GetInstance(Asn1TaggedObject.GetInstance(seq[2]));
				}
			}
		}

		public static CertifiedKeyPair GetInstance(object obj)
		{
			if (obj is CertifiedKeyPair)
			{
				return (CertifiedKeyPair)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertifiedKeyPair((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public CertifiedKeyPair(CertOrEncCert certOrEncCert) : this(certOrEncCert, null, null)
		{
		}

		public CertifiedKeyPair(CertOrEncCert certOrEncCert, EncryptedValue privateKey, PkiPublicationInfo publicationInfo)
		{
			if (certOrEncCert == null)
			{
				throw new ArgumentNullException("certOrEncCert");
			}
			this.certOrEncCert = certOrEncCert;
			this.privateKey = privateKey;
			this.publicationInfo = publicationInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certOrEncCert
			});
			if (this.privateKey != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.privateKey)
				});
			}
			if (this.publicationInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.publicationInfo)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
