using System;

namespace Org.BouncyCastle.Asn1.Cms
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
			if (obj == null || obj is ContentInfo)
			{
				return (ContentInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ContentInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name);
		}

		public static ContentInfo GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return ContentInfo.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		private ContentInfo(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.contentType = (DerObjectIdentifier)seq[0];
			if (seq.Count > 1)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[1];
				if (!asn1TaggedObject.IsExplicit() || asn1TaggedObject.TagNo != 0)
				{
					throw new ArgumentException("Bad tag for 'content'", "seq");
				}
				this.content = asn1TaggedObject.GetObject();
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