using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class ResponderID : Asn1Encodable, IAsn1Choice
	{
		private readonly Asn1Encodable id;

		public virtual X509Name Name
		{
			get
			{
				if (this.id is Asn1OctetString)
				{
					return null;
				}
				return X509Name.GetInstance(this.id);
			}
		}

		public static ResponderID GetInstance(object obj)
		{
			if (obj == null || obj is ResponderID)
			{
				return (ResponderID)obj;
			}
			if (obj is DerOctetString)
			{
				return new ResponderID((DerOctetString)obj);
			}
			if (!(obj is Asn1TaggedObject))
			{
				return new ResponderID(X509Name.GetInstance(obj));
			}
			Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)obj;
			if (asn1TaggedObject.TagNo == 1)
			{
				return new ResponderID(X509Name.GetInstance(asn1TaggedObject, true));
			}
			return new ResponderID(Asn1OctetString.GetInstance(asn1TaggedObject, true));
		}

		public ResponderID(Asn1OctetString id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.id = id;
		}

		public ResponderID(X509Name id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.id = id;
		}

		public static ResponderID GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return ResponderID.GetInstance(obj.GetObject());
		}

		public virtual byte[] GetKeyHash()
		{
			if (this.id is Asn1OctetString)
			{
				return ((Asn1OctetString)this.id).GetOctets();
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.id is Asn1OctetString)
			{
				return new DerTaggedObject(true, 2, this.id);
			}
			return new DerTaggedObject(true, 1, this.id);
		}
	}
}
