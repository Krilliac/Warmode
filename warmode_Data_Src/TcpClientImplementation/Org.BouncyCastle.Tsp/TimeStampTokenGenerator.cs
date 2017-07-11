using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
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
	public class TimeStampTokenGenerator
	{
		private int accuracySeconds = -1;

		private int accuracyMillis = -1;

		private int accuracyMicros = -1;

		private bool ordering;

		private GeneralName tsa;

		private string tsaPolicyOID;

		private AsymmetricKeyParameter key;

		private X509Certificate cert;

		private string digestOID;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr;

		private IX509Store x509Certs;

		private IX509Store x509Crls;

		public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID) : this(key, cert, digestOID, tsaPolicyOID, null, null)
		{
		}

		public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.key = key;
			this.cert = cert;
			this.digestOID = digestOID;
			this.tsaPolicyOID = tsaPolicyOID;
			this.unsignedAttr = unsignedAttr;
			TspUtil.ValidateCertificate(cert);
			IDictionary dictionary;
			if (signedAttr != null)
			{
				dictionary = signedAttr.ToDictionary();
			}
			else
			{
				dictionary = Platform.CreateHashtable();
			}
			try
			{
				byte[] hash = DigestUtilities.CalculateDigest("SHA-1", cert.GetEncoded());
				EssCertID essCertID = new EssCertID(hash);
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = new Org.BouncyCastle.Asn1.Cms.Attribute(PkcsObjectIdentifiers.IdAASigningCertificate, new DerSet(new SigningCertificate(essCertID)));
				dictionary[attribute.AttrType] = attribute;
			}
			catch (CertificateEncodingException e)
			{
				throw new TspException("Exception processing certificate.", e);
			}
			catch (SecurityUtilityException e2)
			{
				throw new TspException("Can't find a SHA-1 implementation.", e2);
			}
			this.signedAttr = new Org.BouncyCastle.Asn1.Cms.AttributeTable(dictionary);
		}

		public void SetCertificates(IX509Store certificates)
		{
			this.x509Certs = certificates;
		}

		public void SetCrls(IX509Store crls)
		{
			this.x509Crls = crls;
		}

		public void SetAccuracySeconds(int accuracySeconds)
		{
			this.accuracySeconds = accuracySeconds;
		}

		public void SetAccuracyMillis(int accuracyMillis)
		{
			this.accuracyMillis = accuracyMillis;
		}

		public void SetAccuracyMicros(int accuracyMicros)
		{
			this.accuracyMicros = accuracyMicros;
		}

		public void SetOrdering(bool ordering)
		{
			this.ordering = ordering;
		}

		public void SetTsa(GeneralName tsa)
		{
			this.tsa = tsa;
		}

		public TimeStampToken Generate(TimeStampRequest request, BigInteger serialNumber, DateTime genTime)
		{
			DerObjectIdentifier objectID = new DerObjectIdentifier(request.MessageImprintAlgOid);
			AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(objectID, DerNull.Instance);
			MessageImprint messageImprint = new MessageImprint(hashAlgorithm, request.GetMessageImprintDigest());
			Accuracy accuracy = null;
			if (this.accuracySeconds > 0 || this.accuracyMillis > 0 || this.accuracyMicros > 0)
			{
				DerInteger seconds = null;
				if (this.accuracySeconds > 0)
				{
					seconds = new DerInteger(this.accuracySeconds);
				}
				DerInteger millis = null;
				if (this.accuracyMillis > 0)
				{
					millis = new DerInteger(this.accuracyMillis);
				}
				DerInteger micros = null;
				if (this.accuracyMicros > 0)
				{
					micros = new DerInteger(this.accuracyMicros);
				}
				accuracy = new Accuracy(seconds, millis, micros);
			}
			DerBoolean derBoolean = null;
			if (this.ordering)
			{
				derBoolean = DerBoolean.GetInstance(this.ordering);
			}
			DerInteger nonce = null;
			if (request.Nonce != null)
			{
				nonce = new DerInteger(request.Nonce);
			}
			DerObjectIdentifier tsaPolicyId = new DerObjectIdentifier(this.tsaPolicyOID);
			if (request.ReqPolicy != null)
			{
				tsaPolicyId = new DerObjectIdentifier(request.ReqPolicy);
			}
			TstInfo tstInfo = new TstInfo(tsaPolicyId, messageImprint, new DerInteger(serialNumber), new DerGeneralizedTime(genTime), accuracy, derBoolean, nonce, this.tsa, request.Extensions);
			TimeStampToken result;
			try
			{
				CmsSignedDataGenerator cmsSignedDataGenerator = new CmsSignedDataGenerator();
				byte[] derEncoded = tstInfo.GetDerEncoded();
				if (request.CertReq)
				{
					cmsSignedDataGenerator.AddCertificates(this.x509Certs);
				}
				cmsSignedDataGenerator.AddCrls(this.x509Crls);
				cmsSignedDataGenerator.AddSigner(this.key, this.cert, this.digestOID, this.signedAttr, this.unsignedAttr);
				CmsSignedData signedData = cmsSignedDataGenerator.Generate(PkcsObjectIdentifiers.IdCTTstInfo.Id, new CmsProcessableByteArray(derEncoded), true);
				result = new TimeStampToken(signedData);
			}
			catch (CmsException e)
			{
				throw new TspException("Error generating time-stamp token", e);
			}
			catch (IOException e2)
			{
				throw new TspException("Exception encoding info", e2);
			}
			catch (X509StoreException e3)
			{
				throw new TspException("Exception handling CertStore", e3);
			}
			return result;
		}
	}
}
