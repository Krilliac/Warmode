using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class GenMsgContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private GenMsgContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static GenMsgContent GetInstance(object obj)
		{
			if (obj is GenMsgContent)
			{
				return (GenMsgContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new GenMsgContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public GenMsgContent(params InfoTypeAndValue[] itv)
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
