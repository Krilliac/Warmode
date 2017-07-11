using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509
{
	public class X509V3CertificateGenerator
	{
		private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

		private V3TbsCertificateGenerator tbsGen;

		private DerObjectIdentifier sigOid;

		private AlgorithmIdentifier sigAlgId;

		private string signatureAlgorithm;

		public IEnumerable SignatureAlgNames
		{
			get
			{
				return X509Utilities.GetAlgNames();
			}
		}

		public X509V3CertificateGenerator()
		{
			this.tbsGen = new V3TbsCertificateGenerator();
		}

		public void Reset()
		{
			this.tbsGen = new V3TbsCertificateGenerator();
			this.extGenerator.Reset();
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
			this.tbsGen.SetSubjectPublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
		}

		public void SetSignatureAlgorithm(string signatureAlgorithm)
		{
			this.signatureAlgorithm = signatureAlgorithm;
			try
			{
				this.sigOid = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception)
			{
				throw new ArgumentException("Unknown signature type requested: " + signatureAlgorithm);
			}
			this.sigAlgId = X509Utilities.GetSigAlgID(this.sigOid, signatureAlgorithm);
			this.tbsGen.SetSignature(this.sigAlgId);
		}

		public void SetSubjectUniqueID(bool[] uniqueID)
		{
			this.tbsGen.SetSubjectUniqueID(this.booleanToBitString(uniqueID));
		}

		public void SetIssuerUniqueID(bool[] uniqueID)
		{
			this.tbsGen.SetIssuerUniqueID(this.booleanToBitString(uniqueID));
		}

		private DerBitString booleanToBitString(bool[] id)
		{
			byte[] array = new byte[(id.Length + 7) / 8];
			for (int num = 0; num != id.Length; num++)
			{
				if (id[num])
				{
					byte[] expr_1F_cp_0 = array;
					int expr_1F_cp_1 = num / 8;
					expr_1F_cp_0[expr_1F_cp_1] |= (byte)(1 << 7 - num % 8);
				}
			}
			int num2 = id.Length % 8;
			if (num2 == 0)
			{
				return new DerBitString(array);
			}
			return new DerBitString(array, 8 - num2);
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

		public void CopyAndAddExtension(string oid, bool critical, X509Certificate cert)
		{
			this.CopyAndAddExtension(new DerObjectIdentifier(oid), critical, cert);
		}

		public void CopyAndAddExtension(DerObjectIdentifier oid, bool critical, X509Certificate cert)
		{
			Asn1OctetString extensionValue = cert.GetExtensionValue(oid);
			if (extensionValue == null)
			{
				throw new CertificateParsingException("extension " + oid + " not present");
			}
			try
			{
				Asn1Encodable extensionValue2 = X509ExtensionUtilities.FromExtensionValue(extensionValue);
				this.AddExtension(oid, critical, extensionValue2);
			}
			catch (Exception ex)
			{
				throw new CertificateParsingException(ex.Message, ex);
			}
		}

		public X509Certificate Generate(AsymmetricKeyParameter privateKey)
		{
			return this.Generate(privateKey, null);
		}

		public X509Certificate Generate(AsymmetricKeyParameter privateKey, SecureRandom random)
		{
			TbsCertificateStructure tbsCertificateStructure = this.GenerateTbsCert();
			byte[] signatureForObject;
			try
			{
				signatureForObject = X509Utilities.GetSignatureForObject(this.sigOid, this.signatureAlgorithm, privateKey, random, tbsCertificateStructure);
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

		private TbsCertificateStructure GenerateTbsCert()
		{
			if (!this.extGenerator.IsEmpty)
			{
				this.tbsGen.SetExtensions(this.extGenerator.Generate());
			}
			return this.tbsGen.GenerateTbsCertificate();
		}

		private X509Certificate GenerateJcaObject(TbsCertificateStructure tbsCert, byte[] signature)
		{
			return new X509Certificate(new X509CertificateStructure(tbsCert, this.sigAlgId, new DerBitString(signature)));
		}
	}
}
