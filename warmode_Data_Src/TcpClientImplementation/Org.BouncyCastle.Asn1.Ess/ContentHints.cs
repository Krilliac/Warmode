using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	public class ContentHints : Asn1Encodable
	{
		private readonly DerUtf8String contentDescription;

		private readonly DerObjectIdentifier contentType;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public DerUtf8String ContentDescription
		{
			get
			{
				return this.contentDescription;
			}
		}

		public static ContentHints GetInstance(object o)
		{
			if (o == null || o is ContentHints)
			{
				return (ContentHints)o;
			}
			if (o is Asn1Sequence)
			{
				return new ContentHints((Asn1Sequence)o);
			}
			throw new ArgumentException("unknown object in 'ContentHints' factory : " + o.GetType().Name + ".");
		}

		private ContentHints(Asn1Sequence seq)
		{
			IAsn1Convertible asn1Convertible = seq[0];
			if (asn1Convertible.ToAsn1Object() is DerUtf8String)
			{
				this.contentDescription = DerUtf8String.GetInstance(asn1Convertible);
				this.contentType = DerObjectIdentifier.GetInstance(seq[1]);
				return;
			}
			this.contentType = DerObjectIdentifier.GetInstance(seq[0]);
		}

		public ContentHints(DerObjectIdentifier contentType)
		{
			this.contentType = contentType;
			this.contentDescription = null;
		}

		public ContentHints(DerObjectIdentifier contentType, DerUtf8String contentDescription)
		{
			this.contentType = contentType;
			this.contentDescription = contentDescription;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.contentDescription != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.contentDescription
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.contentType
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
