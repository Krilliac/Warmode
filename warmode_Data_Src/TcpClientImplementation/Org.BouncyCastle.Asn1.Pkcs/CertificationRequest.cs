using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class CertificationRequest : Asn1Encodable
	{
		protected CertificationRequestInfo reqInfo;

		protected AlgorithmIdentifier sigAlgId;

		protected DerBitString sigBits;

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get
			{
				return this.sigAlgId;
			}
		}

		public DerBitString Signature
		{
			get
			{
				return this.sigBits;
			}
		}

		public static CertificationRequest GetInstance(object obj)
		{
			if (obj is CertificationRequest)
			{
				return (CertificationRequest)obj;
			}
			if (obj != null)
			{
				return new CertificationRequest((Asn1Sequence)obj);
			}
			return null;
		}

		protected CertificationRequest()
		{
		}

		public CertificationRequest(CertificationRequestInfo requestInfo, AlgorithmIdentifier algorithm, DerBitString signature)
		{
			this.reqInfo = requestInfo;
			this.sigAlgId = algorithm;
			this.sigBits = signature;
		}

		public CertificationRequest(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			this.reqInfo = CertificationRequestInfo.GetInstance(seq[0]);
			this.sigAlgId = AlgorithmIdentifier.GetInstance(seq[1]);
			this.sigBits = DerBitString.GetInstance(seq[2]);
		}

		public CertificationRequestInfo GetCertificationRequestInfo()
		{
			return this.reqInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.reqInfo,
				this.sigAlgId,
				this.sigBits
			});
		}
	}
}
