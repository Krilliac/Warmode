using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class Attributes : Asn1Encodable
	{
		private readonly Asn1Set attributes;

		private Attributes(Asn1Set attributes)
		{
			this.attributes = attributes;
		}

		public Attributes(Asn1EncodableVector v)
		{
			this.attributes = new BerSet(v);
		}

		public static Attributes GetInstance(object obj)
		{
			if (obj is Attributes)
			{
				return (Attributes)obj;
			}
			if (obj != null)
			{
				return new Attributes(Asn1Set.GetInstance(obj));
			}
			return null;
		}

		public virtual Attribute[] GetAttributes()
		{
			Attribute[] array = new Attribute[this.attributes.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = Attribute.GetInstance(this.attributes[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.attributes;
		}
	}
}
