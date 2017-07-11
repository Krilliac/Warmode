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
	public class OcspReqGenerator
	{
		private class RequestObject
		{
			internal CertificateID certId;

			internal X509Extensions extensions;

			public RequestObject(CertificateID certId, X509Extensions extensions)
			{
				this.certId = certId;
				this.extensions = extensions;
			}

			public Request ToRequest()
			{
				return new Request(this.certId.ToAsn1Object(), this.extensions);
			}
		}

		private IList list = Platform.CreateArrayList();

		private GeneralName requestorName;

		private X509Extensions requestExtensions;

		public IEnumerable SignatureAlgNames
		{
			get
			{
				return OcspUtilities.AlgNames;
			}
		}

		public void AddRequest(CertificateID certId)
		{
			this.list.Add(new OcspReqGenerator.RequestObject(certId, null));
		}

		public void AddRequest(CertificateID certId, X509Extensions singleRequestExtensions)
		{
			this.list.Add(new OcspReqGenerator.RequestObject(certId, singleRequestExtensions));
		}

		public void SetRequestorName(X509Name requestorName)
		{
			try
			{
				this.requestorName = new GeneralName(4, requestorName);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("cannot encode principal", innerException);
			}
		}

		public void SetRequestorName(GeneralName requestorName)
		{
			this.requestorName = requestorName;
		}

		public void SetRequestExtensions(X509Extensions requestExtensions)
		{
			this.requestExtensions = requestExtensions;
		}

		private OcspReq GenerateRequest(DerObjectIdentifier signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, SecureRandom random)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (OcspReqGenerator.RequestObject requestObject in this.list)
			{
				try
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						requestObject.ToRequest()
					});
				}
				catch (Exception e)
				{
					throw new OcspException("exception creating Request", e);
				}
			}
			TbsRequest tbsRequest = new TbsRequest(this.requestorName, new DerSequence(asn1EncodableVector), this.requestExtensions);
			ISigner signer = null;
			Signature optionalSignature = null;
			if (signingAlgorithm != null)
			{
				if (this.requestorName == null)
				{
					throw new OcspException("requestorName must be specified if request is signed.");
				}
				try
				{
					signer = SignerUtilities.GetSigner(signingAlgorithm.Id);
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
				DerBitString signatureValue = null;
				try
				{
					byte[] encoded = tbsRequest.GetEncoded();
					signer.BlockUpdate(encoded, 0, encoded.Length);
					signatureValue = new DerBitString(signer.GenerateSignature());
				}
				catch (Exception ex2)
				{
					throw new OcspException("exception processing TBSRequest: " + ex2, ex2);
				}
				AlgorithmIdentifier signatureAlgorithm = new AlgorithmIdentifier(signingAlgorithm, DerNull.Instance);
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
					optionalSignature = new Signature(signatureAlgorithm, signatureValue, new DerSequence(asn1EncodableVector2));
				}
				else
				{
					optionalSignature = new Signature(signatureAlgorithm, signatureValue);
				}
			}
			return new OcspReq(new OcspRequest(tbsRequest, optionalSignature));
		}

		public OcspReq Generate()
		{
			return this.GenerateRequest(null, null, null, null);
		}

		public OcspReq Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain)
		{
			return this.Generate(signingAlgorithm, privateKey, chain, null);
		}

		public OcspReq Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, SecureRandom random)
		{
			if (signingAlgorithm == null)
			{
				throw new ArgumentException("no signing algorithm specified");
			}
			OcspReq result;
			try
			{
				DerObjectIdentifier algorithmOid = OcspUtilities.GetAlgorithmOid(signingAlgorithm);
				result = this.GenerateRequest(algorithmOid, privateKey, chain, random);
			}
			catch (ArgumentException)
			{
				throw new ArgumentException("unknown signing algorithm specified: " + signingAlgorithm);
			}
			return result;
		}
	}
}
