using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CrlAnnContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private CrlAnnContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static CrlAnnContent GetInstance(object obj)
		{
			if (obj is CrlAnnContent)
			{
				return (CrlAnnContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CrlAnnContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual CertificateList[] ToCertificateListArray()
		{
			CertificateList[] array = new CertificateList[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertificateList.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
