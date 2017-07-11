using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Ocsp
{
	public class BasicOcspRespGenerator
	{
		private class ResponseObject
		{
			internal CertificateID certId;

			internal CertStatus certStatus;

			internal DerGeneralizedTime thisUpdate;

			internal DerGeneralizedTime nextUpdate;

			internal X509Extensions extensions;

			public ResponseObject(CertificateID certId, CertificateStatus certStatus, DateTime thisUpdate, X509Extensions extensions) : this(certId, certStatus, new DerGeneralizedTime(thisUpdate), null, extensions)
			{
			}

			public ResponseObject(CertificateID certId, CertificateStatus certStatus, DateTime thisUpdate, DateTime nextUpdate, X509Extensions extensions) : this(certId, certStatus, new DerGeneralizedTime(thisUpdate), new DerGeneralizedTime(nextUpdate), extensions)
			{
			}

			private ResponseObject(CertificateID certId, CertificateStatus certStatus, DerGeneralizedTime thisUpdate, DerGeneralizedTime nextUpdate, X509Extensions extensions)
			{
				this.certId = certId;
				if (certStatus == null)
				{
					this.certStatus = new CertStatus();
				}
				else if (certStatus is UnknownStatus)
				{
					this.certStatus = new CertStatus(2, DerNull.Instance);
				}
				else
				{
					RevokedStatus revokedStatus = (RevokedStatus)certStatus;
					CrlReason revocationReason = revokedStatus.HasRevocationReason ? new CrlReason(revokedStatus.RevocationReason) : null;
					this.certStatus = new CertStatus(new RevokedInfo(new DerGeneralizedTime(revokedStatus.RevocationTime), revocationReason));
				}
				this.thisUpdate = thisUpdate;
				this.nextUpdate = nextUpdate;
				this.extensions = extensions;
			}

			public SingleResponse ToResponse()
			{
				return new SingleResponse(this.certId.ToAsn1Object(), this.certStatus, this.thisUpdate, this.nextUpdate, this.extensions);
			}
		}

		private readonly IList list = Platform.CreateArrayList();

		private X509Extensions responseExtensions;

		private RespID responderID;

		public IEnumerable SignatureAlgNames
		{
			get
			{
				return OcspUtilities.AlgNames;
			}
		}

		public BasicOcspRespGenerator(RespID responderID)
		{
			this.responderID = responderID;
		}

		public BasicOcspRespGenerator(AsymmetricKeyParameter publicKey)
		{
			this.responderID = new RespID(publicKey);
		}

		public void AddResponse(CertificateID certID, CertificateStatus certStatus)
		{
			this.list.Add(new BasicOcspRespGenerator.ResponseObject(certID, certStatus, DateTime.UtcNow, null));
		}

		public void AddResponse(CertificateID certID, CertificateStatus certStatus, X509Extensions singleExtensions)
		{
			this.list.Add(new BasicOcspRespGenerator.ResponseObject(certID, certStatus, DateTime.UtcNow, singleExtensions));
		}

		public void AddResponse(CertificateID certID, CertificateStatus certStatus, DateTime nextUpdate, X509Extensions singleExtensions)
		{
			this.list.Add(new BasicOcspRespGenerator.ResponseObject(certID, certStatus, DateTime.UtcNow, nextUpdate, singleExtensions));
		}

		public void AddResponse(CertificateID certID, CertificateStatus certStatus, DateTime thisUpdate, DateTime nextUpdate, X509Extensions singleExtensions)
		{
			this.list.Add(new BasicOcspRespGenerator.ResponseObject(certID, certStatus, thisUpdate, nextUpdate, singleExtensions));
		}

		public void SetResponseExtensions(X509Extensions responseExtensions)
		{
			this.responseExtensions = responseExtensions;
		}

		private BasicOcspResp GenerateResponse(string signatureName, AsymmetricKeyParameter privateKey, X509Certificate[] chain, DateTime producedAt, SecureRandom random)
		{
			DerObjectIdentifier algorithmOid;
			try
			{
				algorithmOid = OcspUtilities.GetAlgorithmOid(signatureName);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("unknown signing algorithm specified", innerException);
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (BasicOcspRespGenerator.ResponseObject responseObject in this.list)
			{
				try
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						responseObject.ToResponse()
					});
				}
				catch (Exception e)
				{
					throw new OcspException("exception creating Request", e);
				}
			}
			ResponseData responseData = new ResponseData(this.responderID.ToAsn1Object(), new DerGeneralizedTime(producedAt), new DerSequence(asn1EncodableVector), this.responseExtensions);
			ISigner signer = null;
			try
			{
				signer = SignerUtilities.GetSigner(signatureName);
				if (random != null)
				{
					signer.Init(true, new ParametersWithRandom(privateKey, random));
				}
				else
				{
					signer.Init(true, privateKey);
				}
			}
			catch (Exception ex)
			{
				throw new OcspException("exception creating signature: " + ex, ex);
			}
			DerBitString signature = null;
			try
			{
				byte[] derEncoded = responseData.GetDerEncoded();
				signer.BlockUpdate(derEncoded, 0, derEncoded.Length);
				signature = new DerBitString(signer.GenerateSignature());
			}
			catch (Exception ex2)
			{
				throw new OcspException("exception processing TBSRequest: " + ex2, ex2);
			}
			AlgorithmIdentifier sigAlgID = OcspUtilities.GetSigAlgID(algorithmOid);
			DerSequence certs = null;
			if (chain != null && chain.Length > 0)
			{
				Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
				try
				{
					for (int num = 0; num != chain.Length; num++)
					{
						asn1EncodableVector2.Add(new Asn1Encodable[]
						{
							X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(chain[num].GetEncoded()))
						});
					}
				}
				catch (IOException e2)
				{
					throw new OcspException("error processing certs", e2);
				}
				catch (CertificateEncodingException e3)
				{
					throw new OcspException("error encoding certs", e3);
				}
				certs = new DerSequence(asn1EncodableVector2);
			}
			return new BasicOcspResp(new BasicOcspResponse(responseData, sigAlgID, signature, certs));
		}

		public BasicOcspResp Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, DateTime thisUpdate)
		{
			return this.Generate(signingAlgorithm, privateKey, chain, thisUpdate, null);
		}

		public BasicOcspResp Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, DateTime producedAt, SecureRandom random)
		{
			if (signingAlgorithm == null)
			{
				throw new ArgumentException("no signing algorithm specified");
			}
			return this.GenerateResponse(signingAlgorithm, privateKey, chain, producedAt, random);
		}
	}
}
