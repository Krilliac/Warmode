using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class Controls : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private Controls(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static Controls GetInstance(object obj)
		{
			if (obj is Controls)
			{
				return (Controls)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Controls((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public Controls(params AttributeTypeAndValue[] atvs)
		{
			this.content = new DerSequence(atvs);
		}

		public virtual AttributeTypeAndValue[] ToAttributeTypeAndValueArray()
		{
			AttributeTypeAndValue[] array = new AttributeTypeAndValue[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = AttributeTypeAndValue.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
