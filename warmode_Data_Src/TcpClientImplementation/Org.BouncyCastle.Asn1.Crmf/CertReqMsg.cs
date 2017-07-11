using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertReqMsg : Asn1Encodable
	{
		private readonly CertRequest certReq;

		private readonly ProofOfPossession popo;

		private readonly Asn1Sequence regInfo;

		public virtual CertRequest CertReq
		{
			get
			{
				return this.certReq;
			}
		}

		public virtual ProofOfPossession Popo
		{
			get
			{
				return this.popo;
			}
		}

		private CertReqMsg(Asn1Sequence seq)
		{
			this.certReq = CertRequest.GetInstance(seq[0]);
			for (int i = 1; i < seq.Count; i++)
			{
				object obj = seq[i];
				if (obj is Asn1TaggedObject || obj is ProofOfPossession)
				{
					this.popo = ProofOfPossession.GetInstance(obj);
				}
				else
				{
					this.regInfo = Asn1Sequence.GetInstance(obj);
				}
			}
		}

		public static CertReqMsg GetInstance(object obj)
		{
			if (obj is CertReqMsg)
			{
				return (CertReqMsg)obj;
			}
			if (obj != null)
			{
				return new CertReqMsg(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public CertReqMsg(CertRequest certReq, ProofOfPossession popo, AttributeTypeAndValue[] regInfo)
		{
			if (certReq == null)
			{
				throw new ArgumentNullException("certReq");
			}
			this.certReq = certReq;
			this.popo = popo;
			if (regInfo != null)
			{
				this.regInfo = new DerSequence(regInfo);
			}
		}

		public virtual AttributeTypeAndValue[] GetRegInfo()
		{
			if (this.regInfo == null)
			{
				return null;
			}
			AttributeTypeAndValue[] array = new AttributeTypeAndValue[this.regInfo.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = AttributeTypeAndValue.GetInstance(this.regInfo[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certReq
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.popo
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.regInfo
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
