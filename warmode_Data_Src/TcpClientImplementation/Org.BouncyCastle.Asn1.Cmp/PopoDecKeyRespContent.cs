using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PopoDecKeyRespContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private PopoDecKeyRespContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static PopoDecKeyRespContent GetInstance(object obj)
		{
			if (obj is PopoDecKeyRespContent)
			{
				return (PopoDecKeyRespContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PopoDecKeyRespContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual DerInteger[] ToDerIntegerArray()
		{
			DerInteger[] array = new DerInteger[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = DerInteger.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
