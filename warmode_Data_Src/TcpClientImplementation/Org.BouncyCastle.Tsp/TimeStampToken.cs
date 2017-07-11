using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampToken
	{
		private class CertID
		{
			private EssCertID certID;

			private EssCertIDv2 certIDv2;

			public IssuerSerial IssuerSerial
			{
				get
				{
					if (this.certID == null)
					{
						return this.certIDv2.IssuerSerial;
					}
					return this.certID.IssuerSerial;
				}
			}

			internal CertID(EssCertID certID)
			{
				this.certID = certID;
				this.certIDv2 = null;
			}

			internal CertID(EssCertIDv2 certID)
			{
				this.certIDv2 = certID;
				this.certID = null;
			}

			public string GetHashAlgorithmName()
			{
				if (this.certID != null)
				{
					return "SHA-1";
				}
				if (NistObjectIdentifiers.IdSha256.Equals(this.certIDv2.HashAlgorithm.ObjectID))
				{
					return "SHA-256";
				}
				return this.certIDv2.HashAlgorithm.ObjectID.Id;
			}

			public AlgorithmIdentifier GetHashAlgorithm()
			{
				if (this.certID == null)
				{
					return this.certIDv2.HashAlgorithm;
				}
				return new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1);
			}

			public byte[] GetCertHash()
			{
				if (this.certID == null)
				{
					return this.certIDv2.GetCertHash();
				}
				return this.certID.GetCertHash();
			}
		}

		private readonly CmsSignedData tsToken;

		private readonly SignerInformation tsaSignerInfo;

		private readonly TimeStampTokenInfo tstInfo;

		private readonly TimeStampToken.CertID certID;

		public TimeStampTokenInfo TimeStampInfo
		{
			get
			{
				return this.tstInfo;
			}
		}

		public SignerID SignerID
		{
			get
			{
				return this.tsaSignerInfo.SignerID;
			}
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable SignedAttributes
		{
			get
			{
				return this.tsaSignerInfo.SignedAttributes;
			}
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable UnsignedAttributes
		{
			get
			{
				return this.tsaSignerInfo.UnsignedAttributes;
			}
		}

		public TimeStampToken(Org.BouncyCastle.Asn1.Cms.ContentInfo contentInfo) : this(new CmsSignedData(contentInfo))
		{
		}

		public TimeStampToken(CmsSignedData signedData)
		{
			this.tsToken = signedData;
			if (!this.tsToken.SignedContentType.Equals(PkcsObjectIdentifiers.IdCTTstInfo))
			{
				throw new TspValidationException("ContentInfo object not for a time stamp.");
			}
			ICollection signers = this.tsToken.GetSignerInfos().GetSigners();
			if (signers.Count != 1)
			{
				throw new ArgumentException("Time-stamp token signed by " + signers.Count + " signers, but it must contain just the TSA signature.");
			}
			IEnumerator enumerator = signers.GetEnumerator();
			enumerator.MoveNext();
			this.tsaSignerInfo = (SignerInformation)enumerator.Current;
			try
			{
				CmsProcessable signedContent = this.tsToken.SignedContent;
				MemoryStream memoryStream = new MemoryStream();
				signedContent.Write(memoryStream);
				this.tstInfo = new TimeStampTokenInfo(TstInfo.GetInstance(Asn1Object.FromByteArray(memoryStream.ToArray())));
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = this.tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificate];
				if (attribute != null)
				{
					SigningCertificate instance = SigningCertificate.GetInstance(attribute.AttrValues[0]);
					this.certID = new TimeStampToken.CertID(EssCertID.GetInstance(instance.GetCerts()[0]));
				}
				else
				{
					attribute = this.tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
					if (attribute == null)
					{
						throw new TspValidationException("no signing certificate attribute found, time stamp invalid.");
					}
					SigningCertificateV2 instance2 = SigningCertificateV2.GetInstance(attribute.AttrValues[0]);
					this.certID = new TimeStampToken.CertID(EssCertIDv2.GetInstance(instance2.GetCerts()[0]));
				}
			}
			catch (CmsException ex)
			{
				throw new TspException(ex.Message, ex.InnerException);
			}
		}

		public IX509Store GetCertificates(string type)
		{
			return this.tsToken.GetCertificates(type);
		}

		public IX509Store GetCrls(string type)
		{
			return this.tsToken.GetCrls(type);
		}

		public IX509Store GetAttributeCertificates(string type)
		{
			return this.tsToken.GetAttributeCertificates(type);
		}

		public void Validate(X509Certificate cert)
		{
			try
			{
				byte[] b = DigestUtilities.CalculateDigest(this.certID.GetHashAlgorithmName(), cert.GetEncoded());
				if (!Arrays.ConstantTimeAreEqual(this.certID.GetCertHash(), b))
				{
					throw new TspValidationException("certificate hash does not match certID hash.");
				}
				if (this.certID.IssuerSerial != null)
				{
					if (!this.certID.IssuerSerial.Serial.Value.Equals(cert.SerialNumber))
					{
						throw new TspValidationException("certificate serial number does not match certID for signature.");
					}
					GeneralName[] names = this.certID.IssuerSerial.Issuer.GetNames();
					X509Name issuerX509Principal = PrincipalUtilities.GetIssuerX509Principal(cert);
					bool flag = false;
					for (int num = 0; num != names.Length; num++)
					{
						if (names[num].TagNo == 4 && X509Name.GetInstance(names[num].Name).Equivalent(issuerX509Principal))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new TspValidationException("certificate name does not match certID for signature. ");
					}
				}
				TspUtil.ValidateCertificate(cert);
				cert.CheckValidity(this.tstInfo.GenTime);
				if (!this.tsaSignerInfo.Verify(cert))
				{
					throw new TspValidationException("signature not created by certificate.");
				}
			}
			catch (CmsException ex)
			{
				if (ex.InnerException != null)
				{
					throw new TspException(ex.Message, ex.InnerException);
				}
				throw new TspException("CMS exception: " + ex, ex);
			}
			catch (CertificateEncodingException ex2)
			{
				throw new TspException("problem processing certificate: " + ex2, ex2);
			}
			catch (SecurityUtilityException ex3)
			{
				throw new TspException("cannot find algorithm: " + ex3.Message, ex3);
			}
		}

		public CmsSignedData ToCmsSignedData()
		{
			return this.tsToken;
		}

		public byte[] GetEncoded()
		{
			return this.tsToken.GetEncoded();
		}
	}
}
