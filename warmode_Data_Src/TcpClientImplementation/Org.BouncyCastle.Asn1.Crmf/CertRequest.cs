using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertRequest : Asn1Encodable
	{
		private readonly DerInteger certReqId;

		private readonly CertTemplate certTemplate;

		private readonly Controls controls;

		public virtual DerInteger CertReqID
		{
			get
			{
				return this.certReqId;
			}
		}

		public virtual CertTemplate CertTemplate
		{
			get
			{
				return this.certTemplate;
			}
		}

		public virtual Controls Controls
		{
			get
			{
				return this.controls;
			}
		}

		private CertRequest(Asn1Sequence seq)
		{
			this.certReqId = DerInteger.GetInstance(seq[0]);
			this.certTemplate = CertTemplate.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				this.controls = Controls.GetInstance(seq[2]);
			}
		}

		public static CertRequest GetInstance(object obj)
		{
			if (obj is CertRequest)
			{
				return (CertRequest)obj;
			}
			if (obj != null)
			{
				return new CertRequest(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public CertRequest(int certReqId, CertTemplate certTemplate, Controls controls) : this(new DerInteger(certReqId), certTemplate, controls)
		{
		}

		public CertRequest(DerInteger certReqId, CertTemplate certTemplate, Controls controls)
		{
			this.certReqId = certReqId;
			this.certTemplate = certTemplate;
			this.controls = controls;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certReqId,
				this.certTemplate
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.controls
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
