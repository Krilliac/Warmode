using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509
{
	public class X509V1CertificateGenerator
	{
		private V1TbsCertificateGenerator tbsGen;

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

		public X509V1CertificateGenerator()
		{
			this.tbsGen = new V1TbsCertificateGenerator();
		}

		public void Reset()
		{
			this.tbsGen = new V1TbsCertificateGenerator();
		}

		public void SetSerialNumber(BigInteger serialNumber)
		{
			if (serialNumber.SignValue <= 0)
			{
				throw new ArgumentException("serial number must be a positive integer", "serialNumber");
			}
			this.tbsGen.SetSerialNumber(new DerInteger(serialNumber));
		}

		public void SetIssuerDN(X509Name issuer)
		{
			this.tbsGen.SetIssuer(issuer);
		}

		public void SetNotBefore(DateTime date)
		{
			this.tbsGen.SetStartDate(new Time(date));
		}

		public void SetNotAfter(DateTime date)
		{
			this.tbsGen.SetEndDate(new Time(date));
		}

		public void SetSubjectDN(X509Name subject)
		{
			this.tbsGen.SetSubject(subject);
		}

		public void SetPublicKey(AsymmetricKeyParameter publicKey)
		{
			try
			{
				this.tbsGen.SetSubjectPublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
			}
			catch (Exception ex)
			{
				throw new ArgumentException("unable to process key - " + ex.ToString());
			}
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
				throw new ArgumentException("Unknown signature type requested", "signatureAlgorithm");
			}
			this.sigAlgId = X509Utilities.GetSigAlgID(this.sigOID, signatureAlgorithm);
			this.tbsGen.SetSignature(this.sigAlgId);
		}

		public X509Certificate Generate(AsymmetricKeyParameter privateKey)
		{
			return this.Generate(privateKey, null);
		}

		public X509Certificate Generate(AsymmetricKeyParameter privateKey, SecureRandom random)
		{
			TbsCertificateStructure tbsCertificateStructure = this.tbsGen.GenerateTbsCertificate();
			byte[] signatureForObject;
			try
			{
				signatureForObject = X509Utilities.GetSignatureForObject(this.sigOID, this.signatureAlgorithm, privateKey, random, tbsCertificateStructure);
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("exception encoding TBS cert", e);
			}
			X509Certificate result;
			try
			{
				result = this.GenerateJcaObject(tbsCertificateStructure, signatureForObject);
			}
			catch (CertificateParsingException e2)
			{
				throw new CertificateEncodingException("exception producing certificate object", e2);
			}
			return result;
		}

		private X509Certificate GenerateJcaObject(TbsCertificateStructure tbsCert, byte[] signature)
		{
			return new X509Certificate(new X509CertificateStructure(tbsCert, this.sigAlgId, new DerBitString(signature)));
		}
	}
}
