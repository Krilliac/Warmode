using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public interface CmsAttributeTableGenerator
	{
		AttributeTable GetAttributes(IDictionary parameters);
	}
}
