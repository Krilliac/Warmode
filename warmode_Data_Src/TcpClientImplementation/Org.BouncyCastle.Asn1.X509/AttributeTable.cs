using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AttributeTable
	{
		private readonly IDictionary attributes;

		public AttributeTable(IDictionary attrs)
		{
			this.attributes = Platform.CreateHashtable(attrs);
		}

		[Obsolete]
		public AttributeTable(Hashtable attrs)
		{
			this.attributes = Platform.CreateHashtable(attrs);
		}

		public AttributeTable(Asn1EncodableVector v)
		{
			this.attributes = Platform.CreateHashtable(v.Count);
			for (int num = 0; num != v.Count; num++)
			{
				AttributeX509 instance = AttributeX509.GetInstance(v[num]);
				this.attributes.Add(instance.AttrType, instance);
			}
		}

		public AttributeTable(Asn1Set s)
		{
			this.attributes = Platform.CreateHashtable(s.Count);
			for (int num = 0; num != s.Count; num++)
			{
				AttributeX509 instance = AttributeX509.GetInstance(s[num]);
				this.attributes.Add(instance.AttrType, instance);
			}
		}

		public AttributeX509 Get(DerObjectIdentifier oid)
		{
			return (AttributeX509)this.attributes[oid];
		}

		[Obsolete("Use 'ToDictionary' instead")]
		public Hashtable ToHashtable()
		{
			return new Hashtable(this.attributes);
		}

		public IDictionary ToDictionary()
		{
			return Platform.CreateHashtable(this.attributes);
		}
	}
}
