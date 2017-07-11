using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class DefaultAuthenticatedAttributeTableGenerator : CmsAttributeTableGenerator
	{
		private readonly IDictionary table;

		public DefaultAuthenticatedAttributeTableGenerator()
		{
			this.table = Platform.CreateHashtable();
		}

		public DefaultAuthenticatedAttributeTableGenerator(AttributeTable attributeTable)
		{
			if (attributeTable != null)
			{
				this.table = attributeTable.ToDictionary();
				return;
			}
			this.table = Platform.CreateHashtable();
		}

		protected virtual IDictionary CreateStandardAttributeTable(IDictionary parameters)
		{
			IDictionary dictionary = Platform.CreateHashtable(this.table);
			if (!dictionary.Contains(CmsAttributes.ContentType))
			{
				DerObjectIdentifier obj = (DerObjectIdentifier)parameters[CmsAttributeTableParameter.ContentType];
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.ContentType, new DerSet(obj));
				dictionary[attribute.AttrType] = attribute;
			}
			if (!dictionary.Contains(CmsAttributes.MessageDigest))
			{
				byte[] str = (byte[])parameters[CmsAttributeTableParameter.Digest];
				Org.BouncyCastle.Asn1.Cms.Attribute attribute2 = new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.MessageDigest, new DerSet(new DerOctetString(str)));
				dictionary[attribute2.AttrType] = attribute2;
			}
			return dictionary;
		}

		public virtual AttributeTable GetAttributes(IDictionary parameters)
		{
			IDictionary attrs = this.CreateStandardAttributeTable(parameters);
			return new AttributeTable(attrs);
		}
	}
}
