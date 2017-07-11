using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class BasicOcspResponse : Asn1Encodable
	{
		private readonly ResponseData tbsResponseData;

		private readonly AlgorithmIdentifier signatureAlgorithm;

		private readonly DerBitString signature;

		private readonly Asn1Sequence certs;

		public ResponseData TbsResponseData
		{
			get
			{
				return this.tbsResponseData;
			}
		}

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get
			{
				return this.signatureAlgorithm;
			}
		}

		public DerBitString Signature
		{
			get
			{
				return this.signature;
			}
		}

		public Asn1Sequence Certs
		{
			get
			{
				return this.certs;
			}
		}

		public static BasicOcspResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return BasicOcspResponse.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static BasicOcspResponse GetInstance(object obj)
		{
			if (obj == null || obj is BasicOcspResponse)
			{
				return (BasicOcspResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new BasicOcspResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public BasicOcspResponse(ResponseData tbsResponseData, AlgorithmIdentifier signatureAlgorithm, DerBitString signature, Asn1Sequence certs)
		{
			this.tbsResponseData = tbsResponseData;
			this.signatureAlgorithm = signatureAlgorithm;
			this.signature = signature;
			this.certs = certs;
		}

		private BasicOcspResponse(Asn1Sequence seq)
		{
			this.tbsResponseData = ResponseData.GetInstance(seq[0]);
			this.signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			this.signature = (DerBitString)seq[2];
			if (seq.Count > 3)
			{
				this.certs = Asn1Sequence.GetInstance((Asn1TaggedObject)seq[3], true);
			}
		}

		[Obsolete("Use TbsResponseData property instead")]
		public ResponseData GetTbsResponseData()
		{
			return this.tbsResponseData;
		}

		[Obsolete("Use SignatureAlgorithm property instead")]
		public AlgorithmIdentifier GetSignatureAlgorithm()
		{
			return this.signatureAlgorithm;
		}

		[Obsolete("Use Signature property instead")]
		public DerBitString GetSignature()
		{
			return this.signature;
		}

		[Obsolete("Use Certs property instead")]
		public Asn1Sequence GetCerts()
		{
			return this.certs;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.tbsResponseData,
				this.signatureAlgorithm,
				this.signature
			});
			if (this.certs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.certs)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
