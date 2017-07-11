using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class ContentInfo : Asn1Encodable
	{
		private readonly DerObjectIdentifier contentType;

		private readonly Asn1Encodable content;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public Asn1Encodable Content
		{
			get
			{
				return this.content;
			}
		}

		public static ContentInfo GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			ContentInfo contentInfo = obj as ContentInfo;
			if (contentInfo != null)
			{
				return contentInfo;
			}
			return new ContentInfo(Asn1Sequence.GetInstance(obj));
		}

		private ContentInfo(Asn1Sequence seq)
		{
			this.contentType = (DerObjectIdentifier)seq[0];
			if (seq.Count > 1)
			{
				this.content = ((Asn1TaggedObject)seq[1]).GetObject();
			}
		}

		public ContentInfo(DerObjectIdentifier contentType, Asn1Encodable content)
		{
			this.contentType = contentType;
			this.content = content;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.contentType
			});
			if (this.content != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new BerTaggedObject(0, this.content)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
