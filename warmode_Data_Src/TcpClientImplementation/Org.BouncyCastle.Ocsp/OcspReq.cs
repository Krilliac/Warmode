using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Ocsp
{
	public class OcspReq : X509ExtensionBase
	{
		private OcspRequest req;

		public int Version
		{
			get
			{
				return this.req.TbsRequest.Version.Value.IntValue + 1;
			}
		}

		public GeneralName RequestorName
		{
			get
			{
				return GeneralName.GetInstance(this.req.TbsRequest.RequestorName);
			}
		}

		public X509Extensions RequestExtensions
		{
			get
			{
				return X509Extensions.GetInstance(this.req.TbsRequest.RequestExtensions);
			}
		}

		public string SignatureAlgOid
		{
			get
			{
				if (!this.IsSigned)
				{
					return null;
				}
				return this.req.OptionalSignature.SignatureAlgorithm.ObjectID.Id;
			}
		}

		public bool IsSigned
		{
			get
			{
				return this.req.OptionalSignature != null;
			}
		}

		public OcspReq(OcspRequest req)
		{
			this.req = req;
		}

		public OcspReq(byte[] req) : this(new Asn1InputStream(req))
		{
		}

		public OcspReq(Stream inStr) : this(new Asn1InputStream(inStr))
		{
		}

		private OcspReq(Asn1InputStream aIn)
		{
			try
			{
				this.req = OcspRequest.GetInstance(aIn.ReadObject());
			}
			catch (ArgumentException ex)
			{
				throw new IOException("malformed request: " + ex.Message);
			}
			catch (InvalidCastException ex2)
			{
				throw new IOException("malformed request: " + ex2.Message);
			}
		}

		public byte[] GetTbsRequest()
		{
			byte[] encoded;
			try
			{
				encoded = this.req.TbsRequest.GetEncoded();
			}
			catch (IOException e)
			{
				throw new OcspException("problem encoding tbsRequest", e);
			}
			return encoded;
		}

		public Req[] GetRequestList()
		{
			Asn1Sequence requestList = this.req.TbsRequest.RequestList;
			Req[] array = new Req[requestList.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = new Req(Request.GetInstance(requestList[num]));
			}
			return array;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.RequestExtensions;
		}

		public byte[] GetSignature()
		{
			if (!this.IsSigned)
			{
				return null;
			}
			return this.req.OptionalSignature.SignatureValue.GetBytes();
		}

		private IList GetCertList()
		{
			IList list = Platform.CreateArrayList();
			Asn1Sequence certs = this.req.OptionalSignature.Certs;
			if (certs != null)
			{
				foreach (Asn1Encodable asn1Encodable in certs)
				{
					try
					{
						list.Add(new X509CertificateParser().ReadCertificate(asn1Encodable.GetEncoded()));
					}
					catch (Exception e)
					{
						throw new OcspException("can't re-encode certificate!", e);
					}
				}
			}
			return list;
		}

		public X509Certificate[] GetCerts()
		{
			if (!this.IsSigned)
			{
				return null;
			}
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
			if (!this.IsSigned)
			{
				return null;
			}
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
			if (!this.IsSigned)
			{
				throw new OcspException("attempt to Verify signature on unsigned object");
			}
			bool result;
			try
			{
				ISigner signer = SignerUtilities.GetSigner(this.SignatureAlgOid);
				signer.Init(false, publicKey);
				byte[] encoded = this.req.TbsRequest.GetEncoded();
				signer.BlockUpdate(encoded, 0, encoded.Length);
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
			return this.req.GetEncoded();
		}
	}
}
