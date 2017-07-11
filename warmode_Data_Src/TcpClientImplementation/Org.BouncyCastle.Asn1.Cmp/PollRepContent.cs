using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PollRepContent : Asn1Encodable
	{
		private readonly DerInteger certReqId;

		private readonly DerInteger checkAfter;

		private readonly PkiFreeText reason;

		public virtual DerInteger CertReqID
		{
			get
			{
				return this.certReqId;
			}
		}

		public virtual DerInteger CheckAfter
		{
			get
			{
				return this.checkAfter;
			}
		}

		public virtual PkiFreeText Reason
		{
			get
			{
				return this.reason;
			}
		}

		private PollRepContent(Asn1Sequence seq)
		{
			this.certReqId = DerInteger.GetInstance(seq[0]);
			this.checkAfter = DerInteger.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				this.reason = PkiFreeText.GetInstance(seq[2]);
			}
		}

		public static PollRepContent GetInstance(object obj)
		{
			if (obj is PollRepContent)
			{
				return (PollRepContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PollRepContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certReqId,
				this.checkAfter
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.reason
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
