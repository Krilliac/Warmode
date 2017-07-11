using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.X509
{
	public class X509Attribute : Asn1Encodable
	{
		private readonly AttributeX509 attr;

		public string Oid
		{
			get
			{
				return this.attr.AttrType.Id;
			}
		}

		internal X509Attribute(Asn1Encodable at)
		{
			this.attr = AttributeX509.GetInstance(at);
		}

		public X509Attribute(string oid, Asn1Encodable value)
		{
			this.attr = new AttributeX509(new DerObjectIdentifier(oid), new DerSet(value));
		}

		public X509Attribute(string oid, Asn1EncodableVector value)
		{
			this.attr = new AttributeX509(new DerObjectIdentifier(oid), new DerSet(value));
		}

		public Asn1Encodable[] GetValues()
		{
			Asn1Set attrValues = this.attr.AttrValues;
			Asn1Encodable[] array = new Asn1Encodable[attrValues.Count];
			for (int num = 0; num != attrValues.Count; num++)
			{
				array[num] = attrValues[num];
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.attr.ToAsn1Object();
		}
	}
}
