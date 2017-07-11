using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertResponse : Asn1Encodable
	{
		private readonly DerInteger certReqId;

		private readonly PkiStatusInfo status;

		private readonly CertifiedKeyPair certifiedKeyPair;

		private readonly Asn1OctetString rspInfo;

		public virtual DerInteger CertReqID
		{
			get
			{
				return this.certReqId;
			}
		}

		public virtual PkiStatusInfo Status
		{
			get
			{
				return this.status;
			}
		}

		public virtual CertifiedKeyPair CertifiedKeyPair
		{
			get
			{
				return this.certifiedKeyPair;
			}
		}

		private CertResponse(Asn1Sequence seq)
		{
			this.certReqId = DerInteger.GetInstance(seq[0]);
			this.status = PkiStatusInfo.GetInstance(seq[1]);
			if (seq.Count >= 3)
			{
				if (seq.Count == 3)
				{
					Asn1Encodable asn1Encodable = seq[2];
					if (asn1Encodable is Asn1OctetString)
					{
						this.rspInfo = Asn1OctetString.GetInstance(asn1Encodable);
						return;
					}
					this.certifiedKeyPair = CertifiedKeyPair.GetInstance(asn1Encodable);
					return;
				}
				else
				{
					this.certifiedKeyPair = CertifiedKeyPair.GetInstance(seq[2]);
					this.rspInfo = Asn1OctetString.GetInstance(seq[3]);
				}
			}
		}

		public static CertResponse GetInstance(object obj)
		{
			if (obj is CertResponse)
			{
				return (CertResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public CertResponse(DerInteger certReqId, PkiStatusInfo status) : this(certReqId, status, null, null)
		{
		}

		public CertResponse(DerInteger certReqId, PkiStatusInfo status, CertifiedKeyPair certifiedKeyPair, Asn1OctetString rspInfo)
		{
			if (certReqId == null)
			{
				throw new ArgumentNullException("certReqId");
			}
			if (status == null)
			{
				throw new ArgumentNullException("status");
			}
			this.certReqId = certReqId;
			this.status = status;
			this.certifiedKeyPair = certifiedKeyPair;
			this.rspInfo = rspInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certReqId,
				this.status
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.certifiedKeyPair
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.rspInfo
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
