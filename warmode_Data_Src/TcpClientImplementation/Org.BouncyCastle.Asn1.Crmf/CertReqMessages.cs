using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertReqMessages : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private CertReqMessages(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static CertReqMessages GetInstance(object obj)
		{
			if (obj is CertReqMessages)
			{
				return (CertReqMessages)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertReqMessages((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public CertReqMessages(params CertReqMsg[] msgs)
		{
			this.content = new DerSequence(msgs);
		}

		public virtual CertReqMsg[] ToCertReqMsgArray()
		{
			CertReqMsg[] array = new CertReqMsg[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertReqMsg.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
