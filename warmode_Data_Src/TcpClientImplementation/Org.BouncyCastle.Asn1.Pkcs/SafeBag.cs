using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class SafeBag : Asn1Encodable
	{
		private readonly DerObjectIdentifier bagID;

		private readonly Asn1Object bagValue;

		private readonly Asn1Set bagAttributes;

		public DerObjectIdentifier BagID
		{
			get
			{
				return this.bagID;
			}
		}

		public Asn1Object BagValue
		{
			get
			{
				return this.bagValue;
			}
		}

		public Asn1Set BagAttributes
		{
			get
			{
				return this.bagAttributes;
			}
		}

		public SafeBag(DerObjectIdentifier oid, Asn1Object obj)
		{
			this.bagID = oid;
			this.bagValue = obj;
			this.bagAttributes = null;
		}

		public SafeBag(DerObjectIdentifier oid, Asn1Object obj, Asn1Set bagAttributes)
		{
			this.bagID = oid;
			this.bagValue = obj;
			this.bagAttributes = bagAttributes;
		}

		public SafeBag(Asn1Sequence seq)
		{
			this.bagID = (DerObjectIdentifier)seq[0];
			this.bagValue = ((DerTaggedObject)seq[1]).GetObject();
			if (seq.Count == 3)
			{
				this.bagAttributes = (Asn1Set)seq[2];
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.bagID,
				new DerTaggedObject(0, this.bagValue)
			});
			if (this.bagAttributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.bagAttributes
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
