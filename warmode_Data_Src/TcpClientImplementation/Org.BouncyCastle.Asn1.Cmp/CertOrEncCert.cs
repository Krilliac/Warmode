using Org.BouncyCastle.Asn1.Crmf;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertOrEncCert : Asn1Encodable, IAsn1Choice
	{
		private readonly CmpCertificate certificate;

		private readonly EncryptedValue encryptedCert;

		public virtual CmpCertificate Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		public virtual EncryptedValue EncryptedCert
		{
			get
			{
				return this.encryptedCert;
			}
		}

		private CertOrEncCert(Asn1TaggedObject tagged)
		{
			if (tagged.TagNo == 0)
			{
				this.certificate = CmpCertificate.GetInstance(tagged.GetObject());
				return;
			}
			if (tagged.TagNo == 1)
			{
				this.encryptedCert = EncryptedValue.GetInstance(tagged.GetObject());
				return;
			}
			throw new ArgumentException("unknown tag: " + tagged.TagNo, "tagged");
		}

		public static CertOrEncCert GetInstance(object obj)
		{
			if (obj is CertOrEncCert)
			{
				return (CertOrEncCert)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new CertOrEncCert((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public CertOrEncCert(CmpCertificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			this.certificate = certificate;
		}

		public CertOrEncCert(EncryptedValue encryptedCert)
		{
			if (encryptedCert == null)
			{
				throw new ArgumentNullException("encryptedCert");
			}
			this.encryptedCert = encryptedCert;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.certificate != null)
			{
				return new DerTaggedObject(true, 0, this.certificate);
			}
			return new DerTaggedObject(true, 1, this.encryptedCert);
		}
	}
}
