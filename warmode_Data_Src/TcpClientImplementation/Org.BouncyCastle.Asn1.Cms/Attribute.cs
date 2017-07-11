using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class Attribute : Asn1Encodable
	{
		private DerObjectIdentifier attrType;

		private Asn1Set attrValues;

		public DerObjectIdentifier AttrType
		{
			get
			{
				return this.attrType;
			}
		}

		public Asn1Set AttrValues
		{
			get
			{
				return this.attrValues;
			}
		}

		public static Attribute GetInstance(object obj)
		{
			if (obj == null || obj is Attribute)
			{
				return (Attribute)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Attribute((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public Attribute(Asn1Sequence seq)
		{
			this.attrType = (DerObjectIdentifier)seq[0];
			this.attrValues = (Asn1Set)seq[1];
		}

		public Attribute(DerObjectIdentifier attrType, Asn1Set attrValues)
		{
			this.attrType = attrType;
			this.attrValues = attrValues;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.attrType,
				this.attrValues
			});
		}
	}
}
