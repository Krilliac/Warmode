using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class GenRepContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private GenRepContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static GenRepContent GetInstance(object obj)
		{
			if (obj is GenRepContent)
			{
				return (GenRepContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new GenRepContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public GenRepContent(params InfoTypeAndValue[] itv)
		{
			this.content = new DerSequence(itv);
		}

		public virtual InfoTypeAndValue[] ToInfoTypeAndValueArray()
		{
			InfoTypeAndValue[] array = new InfoTypeAndValue[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = InfoTypeAndValue.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
