using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509
{
	public class X509V2CrlGenerator
	{
		private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

		private V2TbsCertListGenerator tbsGen;

		private DerObjectIdentifier sigOID;

		private AlgorithmIdentifier sigAlgId;

		private string signatureAlgorithm;

		public IEnumerable SignatureAlgNames
		{
			get
			{
				return X509Utilities.GetAlgNames();
			}
		}

		public X509V2CrlGenerator()
		{
			this.tbsGen = new V2TbsCertListGenerator();
		}

		public void Reset()
		{
			this.tbsGen = new V2TbsCertListGenerator();
			this.extGenerator.Reset();
		}

		public void SetIssuerDN(X509Name issuer)
		{
			this.tbsGen.SetIssuer(issuer);
		}

		public void SetThisUpdate(DateTime date)
		{
			this.tbsGen.SetThisUpdate(new Time(date));
		}

		public void SetNextUpdate(DateTime date)
		{
			this.tbsGen.SetNextUpdate(new Time(date));
		}

		public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, int reason)
		{
			this.tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason);
		}

		public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, int reason, DateTime invalidityDate)
		{
			this.tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason, new DerGeneralizedTime(invalidityDate));
		}

		public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, X509Extensions extensions)
		{
			this.tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), extensions);
		}

		public void AddCrl(X509Crl other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			ISet revokedCertificates = other.GetRevokedCertificates();
			if (revokedCertificates != null)
			{
				foreach (X509CrlEntry x509CrlEntry in revokedCertificates)
				{
					try
					{
						this.tbsGen.AddCrlEntry(Asn1Sequence.GetInstance(Asn1Object.FromByteArray(x509CrlEntry.GetEncoded())));
					}
					catch (IOException e)
					{
						throw new CrlException("exception processing encoding of CRL", e);
					}
				}
			}
		}

		public void SetSignatureAlgorithm(string signatureAlgorithm)
		{
			this.signatureAlgorithm = signatureAlgorithm;
			try
			{
				this.sigOID = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Unknown signature type requested", innerException);
			}
			this.sigAlgId = X509Utilities.GetSigAlgID(this.sigOID, signatureAlgorithm);
			this.tbsGen.SetSignature(this.sigAlgId);
		}

		public void AddExtension(string oid, bool critical, Asn1Encodable extensionValue)
		{
			this.extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
		}

		public void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extensionValue)
		{
			this.extGenerator.AddExtension(oid, critical, extensionValue);
		}

		public void AddExtension(string oid, bool critical, byte[] extensionValue)
		{
			this.extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, new DerOctetString(extensionValue));
		}

		public void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extensionValue)
		{
			this.extGenerator.AddExtension(oid, critical, new DerOctetString(extensionValue));
		}

		public X509Crl Generate(AsymmetricKeyParameter privateKey)
		{
			return this.Generate(privateKey, null);
		}

		public X509Crl Generate(AsymmetricKeyParameter privateKey, SecureRandom random)
		{
			TbsCertificateList tbsCertificateList = this.GenerateCertList();
			byte[] signatureForObject;
			try
			{
				signatureForObject = X509Utilities.GetSignatureForObject(this.sigOID, this.signatureAlgorithm, privateKey, random, tbsCertificateList);
			}
			catch (IOException e)
			{
				throw new CrlException("cannot generate CRL encoding", e);
			}
			return this.GenerateJcaObject(tbsCertificateList, signatureForObject);
		}

		private TbsCertificateList GenerateCertList()
		{
			if (!this.extGenerator.IsEmpty)
			{
				this.tbsGen.SetExtensions(this.extGenerator.Generate());
			}
			return this.tbsGen.GenerateTbsCertList();
		}

		private X509Crl GenerateJcaObject(TbsCertificateList tbsCrl, byte[] signature)
		{
			return new X509Crl(CertificateList.GetInstance(new DerSequence(new Asn1Encodable[]
			{
				tbsCrl,
				this.sigAlgId,
				new DerBitString(signature)
			})));
		}
	}
}
