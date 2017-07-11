using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertConfirmContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private CertConfirmContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static CertConfirmContent GetInstance(object obj)
		{
			if (obj is CertConfirmContent)
			{
				return (CertConfirmContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertConfirmContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual CertStatus[] ToCertStatusArray()
		{
			CertStatus[] array = new CertStatus[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertStatus.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
