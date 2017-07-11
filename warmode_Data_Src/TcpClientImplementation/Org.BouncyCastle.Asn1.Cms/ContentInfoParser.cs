using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class ContentInfoParser
	{
		private DerObjectIdentifier contentType;

		private Asn1TaggedObjectParser content;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public ContentInfoParser(Asn1SequenceParser seq)
		{
			this.contentType = (DerObjectIdentifier)seq.ReadObject();
			this.content = (Asn1TaggedObjectParser)seq.ReadObject();
		}

		public IAsn1Convertible GetContent(int tag)
		{
			if (this.content == null)
			{
				return null;
			}
			return this.content.GetObjectParser(tag, true);
		}
	}
}
