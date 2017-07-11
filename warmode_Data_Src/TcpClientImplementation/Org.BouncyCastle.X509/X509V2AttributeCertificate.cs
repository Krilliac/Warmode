using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509
{
	public class X509V2AttributeCertificate : X509ExtensionBase, IX509AttributeCertificate, IX509Extension
	{
		private readonly AttributeCertificate cert;

		private readonly DateTime notBefore;

		private readonly DateTime notAfter;

		public virtual int Version
		{
			get
			{
				return this.cert.ACInfo.Version.Value.IntValue + 1;
			}
		}

		public virtual BigInteger SerialNumber
		{
			get
			{
				return this.cert.ACInfo.SerialNumber.Value;
			}
		}

		public virtual AttributeCertificateHolder Holder
		{
			get
			{
				return new AttributeCertificateHolder((Asn1Sequence)this.cert.ACInfo.Holder.ToAsn1Object());
			}
		}

		public virtual AttributeCertificateIssuer Issuer
		{
			get
			{
				return new AttributeCertificateIssuer(this.cert.ACInfo.Issuer);
			}
		}

		public virtual DateTime NotBefore
		{
			get
			{
				return this.notBefore;
			}
		}

		public virtual DateTime NotAfter
		{
			get
			{
				return this.notAfter;
			}
		}

		public virtual bool IsValidNow
		{
			get
			{
				return this.IsValid(DateTime.UtcNow);
			}
		}

		private static AttributeCertificate GetObject(Stream input)
		{
			AttributeCertificate instance;
			try
			{
				instance = AttributeCertificate.GetInstance(Asn1Object.FromStream(input));
			}
			catch (IOException ex)
			{
				throw ex;
			}
			catch (Exception innerException)
			{
				throw new IOException("exception decoding certificate structure", innerException);
			}
			return instance;
		}

		public X509V2AttributeCertificate(Stream encIn) : this(X509V2AttributeCertificate.GetObject(encIn))
		{
		}

		public X509V2AttributeCertificate(byte[] encoded) : this(new MemoryStream(encoded, false))
		{
		}

		internal X509V2AttributeCertificate(AttributeCertificate cert)
		{
			this.cert = cert;
			try
			{
				this.notAfter = cert.ACInfo.AttrCertValidityPeriod.NotAfterTime.ToDateTime();
				this.notBefore = cert.ACInfo.AttrCertValidityPeriod.NotBeforeTime.ToDateTime();
			}
			catch (Exception innerException)
			{
				throw new IOException("invalid data structure in certificate!", innerException);
			}
		}

		public virtual bool[] GetIssuerUniqueID()
		{
			DerBitString issuerUniqueID = this.cert.ACInfo.IssuerUniqueID;
			if (issuerUniqueID != null)
			{
				byte[] bytes = issuerUniqueID.GetBytes();
				bool[] array = new bool[bytes.Length * 8 - issuerUniqueID.PadBits];
				for (int num = 0; num != array.Length; num++)
				{
					array[num] = (((int)bytes[num / 8] & 128 >> num % 8) != 0);
				}
				return array;
			}
			return null;
		}

		public virtual bool IsValid(DateTime date)
		{
			return date.CompareTo(this.NotBefore) >= 0 && date.CompareTo(this.NotAfter) <= 0;
		}

		public virtual void CheckValidity()
		{
			this.CheckValidity(DateTime.UtcNow);
		}

		public virtual void CheckValidity(DateTime date)
		{
			if (date.CompareTo(this.NotAfter) > 0)
			{
				throw new CertificateExpiredException("certificate expired on " + this.NotAfter);
			}
			if (date.CompareTo(this.NotBefore) < 0)
			{
				throw new CertificateNotYetValidException("certificate not valid until " + this.NotBefore);
			}
		}

		public virtual byte[] GetSignature()
		{
			return this.cert.SignatureValue.GetBytes();
		}

		public virtual void Verify(AsymmetricKeyParameter publicKey)
		{
			if (!this.cert.SignatureAlgorithm.Equals(this.cert.ACInfo.Signature))
			{
				throw new CertificateException("Signature algorithm in certificate info not same as outer certificate");
			}
			ISigner signer = SignerUtilities.GetSigner(this.cert.SignatureAlgorithm.ObjectID.Id);
			signer.Init(false, publicKey);
			try
			{
				byte[] encoded = this.cert.ACInfo.GetEncoded();
				signer.BlockUpdate(encoded, 0, encoded.Length);
			}
			catch (IOException exception)
			{
				throw new SignatureException("Exception encoding certificate info object", exception);
			}
			if (!signer.VerifySignature(this.GetSignature()))
			{
				throw new InvalidKeyException("Public key presented not for certificate signature");
			}
		}

		public virtual byte[] GetEncoded()
		{
			return this.cert.GetEncoded();
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.cert.ACInfo.Extensions;
		}

		public virtual X509Attribute[] GetAttributes()
		{
			Asn1Sequence attributes = this.cert.ACInfo.Attributes;
			X509Attribute[] array = new X509Attribute[attributes.Count];
			for (int num = 0; num != attributes.Count; num++)
			{
				array[num] = new X509Attribute(attributes[num]);
			}
			return array;
		}

		public virtual X509Attribute[] GetAttributes(string oid)
		{
			Asn1Sequence attributes = this.cert.ACInfo.Attributes;
			IList list = Platform.CreateArrayList();
			for (int num = 0; num != attributes.Count; num++)
			{
				X509Attribute x509Attribute = new X509Attribute(attributes[num]);
				if (x509Attribute.Oid.Equals(oid))
				{
					list.Add(x509Attribute);
				}
			}
			if (list.Count < 1)
			{
				return null;
			}
			X509Attribute[] array = new X509Attribute[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = (X509Attribute)list[i];
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			X509V2AttributeCertificate x509V2AttributeCertificate = obj as X509V2AttributeCertificate;
			return x509V2AttributeCertificate != null && this.cert.Equals(x509V2AttributeCertificate.cert);
		}

		public override int GetHashCode()
		{
			return this.cert.GetHashCode();
		}
	}
}
