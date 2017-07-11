using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertStatus : Asn1Encodable
	{
		private readonly Asn1OctetString certHash;

		private readonly DerInteger certReqId;

		private readonly PkiStatusInfo statusInfo;

		public virtual Asn1OctetString CertHash
		{
			get
			{
				return this.certHash;
			}
		}

		public virtual DerInteger CertReqID
		{
			get
			{
				return this.certReqId;
			}
		}

		public virtual PkiStatusInfo StatusInfo
		{
			get
			{
				return this.statusInfo;
			}
		}

		private CertStatus(Asn1Sequence seq)
		{
			this.certHash = Asn1OctetString.GetInstance(seq[0]);
			this.certReqId = DerInteger.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				this.statusInfo = PkiStatusInfo.GetInstance(seq[2]);
			}
		}

		public CertStatus(byte[] certHash, BigInteger certReqId)
		{
			this.certHash = new DerOctetString(certHash);
			this.certReqId = new DerInteger(certReqId);
		}

		public CertStatus(byte[] certHash, BigInteger certReqId, PkiStatusInfo statusInfo)
		{
			this.certHash = new DerOctetString(certHash);
			this.certReqId = new DerInteger(certReqId);
			this.statusInfo = statusInfo;
		}

		public static CertStatus GetInstance(object obj)
		{
			if (obj is CertStatus)
			{
				return (CertStatus)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertStatus((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certHash,
				this.certReqId
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.statusInfo
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
