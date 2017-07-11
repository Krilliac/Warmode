using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Ocsp
{
	public class BasicOcspResp : X509ExtensionBase
	{
		private readonly BasicOcspResponse resp;

		private readonly ResponseData data;

		public int Version
		{
			get
			{
				return this.data.Version.Value.IntValue + 1;
			}
		}

		public RespID ResponderId
		{
			get
			{
				return new RespID(this.data.ResponderID);
			}
		}

		public DateTime ProducedAt
		{
			get
			{
				return this.data.ProducedAt.ToDateTime();
			}
		}

		public SingleResp[] Responses
		{
			get
			{
				Asn1Sequence responses = this.data.Responses;
				SingleResp[] array = new SingleResp[responses.Count];
				for (int num = 0; num != array.Length; num++)
				{
					array[num] = new SingleResp(SingleResponse.GetInstance(responses[num]));
				}
				return array;
			}
		}

		public X509Extensions ResponseExtensions
		{
			get
			{
				return this.data.ResponseExtensions;
			}
		}

		public string SignatureAlgName
		{
			get
			{
				return OcspUtilities.GetAlgorithmName(this.resp.SignatureAlgorithm.ObjectID);
			}
		}

		public string SignatureAlgOid
		{
			get
			{
				return this.resp.SignatureAlgorithm.ObjectID.Id;
			}
		}

		public BasicOcspResp(BasicOcspResponse resp)
		{
			this.resp = resp;
			this.data = resp.TbsResponseData;
		}

		public byte[] GetTbsResponseData()
		{
			byte[] derEncoded;
			try
			{
				derEncoded = this.data.GetDerEncoded();
			}
			catch (IOException e)
			{
				throw new OcspException("problem encoding tbsResponseData", e);
			}
			return derEncoded;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.ResponseExtensions;
		}

		[Obsolete("RespData class is no longer required as all functionality is available on this class")]
		public RespData GetResponseData()
		{
			return new RespData(this.data);
		}

		public byte[] GetSignature()
		{
			return this.resp.Signature.GetBytes();
		}

		private IList GetCertList()
		{
			IList list = Platform.CreateArrayList();
			Asn1Sequence certs = this.resp.Certs;
			if (certs != null)
			{
				foreach (Asn1Encodable asn1Encodable in certs)
				{
					try
					{
						list.Add(new X509CertificateParser().ReadCertificate(asn1Encodable.GetEncoded()));
					}
					catch (IOException e)
					{
						throw new OcspException("can't re-encode certificate!", e);
					}
					catch (CertificateException e2)
					{
						throw new OcspException("can't re-encode certificate!", e2);
					}
				}
			}
			return list;
		}

		public X509Certificate[] GetCerts()
		{
			IList certList = this.GetCertList();
			X509Certificate[] array = new X509Certificate[certList.Count];
			for (int i = 0; i < certList.Count; i++)
			{
				array[i] = (X509Certificate)certList[i];
			}
			return array;
		}

		public IX509Store GetCertificates(string type)
		{
			IX509Store result;
			try
			{
				result = X509StoreFactory.Create("Certificate/" + type, new X509CollectionStoreParameters(this.GetCertList()));
			}
			catch (Exception e)
			{
				throw new OcspException("can't setup the CertStore", e);
			}
			return result;
		}

		public bool Verify(AsymmetricKeyParameter publicKey)
		{
			bool result;
			try
			{
				ISigner signer = SignerUtilities.GetSigner(this.SignatureAlgName);
				signer.Init(false, publicKey);
				byte[] derEncoded = this.data.GetDerEncoded();
				signer.BlockUpdate(derEncoded, 0, derEncoded.Length);
				result = signer.VerifySignature(this.GetSignature());
			}
			catch (Exception ex)
			{
				throw new OcspException("exception processing sig: " + ex, ex);
			}
			return result;
		}

		public byte[] GetEncoded()
		{
			return this.resp.GetEncoded();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BasicOcspResp basicOcspResp = obj as BasicOcspResp;
			return basicOcspResp != null && this.resp.Equals(basicOcspResp.resp);
		}

		public override int GetHashCode()
		{
			return this.resp.GetHashCode();
		}
	}
}
