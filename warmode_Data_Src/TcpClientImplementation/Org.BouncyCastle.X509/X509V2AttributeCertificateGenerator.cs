using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509
{
	public class X509V2AttributeCertificateGenerator
	{
		private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

		private V2AttributeCertificateInfoGenerator acInfoGen;

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

		public X509V2AttributeCertificateGenerator()
		{
			this.acInfoGen = new V2AttributeCertificateInfoGenerator();
		}

		public void Reset()
		{
			this.acInfoGen = new V2AttributeCertificateInfoGenerator();
			this.extGenerator.Reset();
		}

		public void SetHolder(AttributeCertificateHolder holder)
		{
			this.acInfoGen.SetHolder(holder.holder);
		}

		public void SetIssuer(AttributeCertificateIssuer issuer)
		{
			this.acInfoGen.SetIssuer(AttCertIssuer.GetInstance(issuer.form));
		}

		public void SetSerialNumber(BigInteger serialNumber)
		{
			this.acInfoGen.SetSerialNumber(new DerInteger(serialNumber));
		}

		public void SetNotBefore(DateTime date)
		{
			this.acInfoGen.SetStartDate(new DerGeneralizedTime(date));
		}

		public void SetNotAfter(DateTime date)
		{
			this.acInfoGen.SetEndDate(new DerGeneralizedTime(date));
		}

		public void SetSignatureAlgorithm(string signatureAlgorithm)
		{
			this.signatureAlgorithm = signatureAlgorithm;
			try
			{
				this.sigOID = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception)
			{
				throw new ArgumentException("Unknown signature type requested");
			}
			this.sigAlgId = X509Utilities.GetSigAlgID(this.sigOID, signatureAlgorithm);
			this.acInfoGen.SetSignature(this.sigAlgId);
		}

		public void AddAttribute(X509Attribute attribute)
		{
			this.acInfoGen.AddAttribute(AttributeX509.GetInstance(attribute.ToAsn1Object()));
		}

		public void SetIssuerUniqueId(bool[] iui)
		{
			throw Platform.CreateNotImplementedException("SetIssuerUniqueId()");
		}

		public void AddExtension(string oid, bool critical, Asn1Encodable extensionValue)
		{
			this.extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
		}

		public void AddExtension(string oid, bool critical, byte[] extensionValue)
		{
			this.extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
		}

		public IX509AttributeCertificate Generate(AsymmetricKeyParameter publicKey)
		{
			return this.Generate(publicKey, null);
		}

		public IX509AttributeCertificate Generate(AsymmetricKeyParameter publicKey, SecureRandom random)
		{
			if (!this.extGenerator.IsEmpty)
			{
				this.acInfoGen.SetExtensions(this.extGenerator.Generate());
			}
			AttributeCertificateInfo attributeCertificateInfo = this.acInfoGen.GenerateAttributeCertificateInfo();
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				attributeCertificateInfo,
				this.sigAlgId
			});
			IX509AttributeCertificate result;
			try
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerBitString(X509Utilities.GetSignatureForObject(this.sigOID, this.signatureAlgorithm, publicKey, random, attributeCertificateInfo))
				});
				result = new X509V2AttributeCertificate(AttributeCertificate.GetInstance(new DerSequence(asn1EncodableVector)));
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("constructed invalid certificate", e);
			}
			return result;
		}
	}
}
