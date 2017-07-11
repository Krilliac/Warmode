using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class RecipientIdentifier : Asn1Encodable, IAsn1Choice
	{
		private Asn1Encodable id;

		public bool IsTagged
		{
			get
			{
				return this.id is Asn1TaggedObject;
			}
		}

		public Asn1Encodable ID
		{
			get
			{
				if (this.id is Asn1TaggedObject)
				{
					return Asn1OctetString.GetInstance((Asn1TaggedObject)this.id, false);
				}
				return IssuerAndSerialNumber.GetInstance(this.id);
			}
		}

		public RecipientIdentifier(IssuerAndSerialNumber id)
		{
			this.id = id;
		}

		public RecipientIdentifier(Asn1OctetString id)
		{
			this.id = new DerTaggedObject(false, 0, id);
		}

		public RecipientIdentifier(Asn1Object id)
		{
			this.id = id;
		}

		public static RecipientIdentifier GetInstance(object o)
		{
			if (o == null || o is RecipientIdentifier)
			{
				return (RecipientIdentifier)o;
			}
			if (o is IssuerAndSerialNumber)
			{
				return new RecipientIdentifier((IssuerAndSerialNumber)o);
			}
			if (o is Asn1OctetString)
			{
				return new RecipientIdentifier((Asn1OctetString)o);
			}
			if (o is Asn1Object)
			{
				return new RecipientIdentifier((Asn1Object)o);
			}
			throw new ArgumentException("Illegal object in RecipientIdentifier: " + o.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.id.ToAsn1Object();
		}
	}
}
