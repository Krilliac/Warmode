using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class OriginatorIdentifierOrKey : Asn1Encodable, IAsn1Choice
	{
		private Asn1Encodable id;

		public Asn1Encodable ID
		{
			get
			{
				return this.id;
			}
		}

		public IssuerAndSerialNumber IssuerAndSerialNumber
		{
			get
			{
				if (this.id is IssuerAndSerialNumber)
				{
					return (IssuerAndSerialNumber)this.id;
				}
				return null;
			}
		}

		public SubjectKeyIdentifier SubjectKeyIdentifier
		{
			get
			{
				if (this.id is Asn1TaggedObject && ((Asn1TaggedObject)this.id).TagNo == 0)
				{
					return SubjectKeyIdentifier.GetInstance((Asn1TaggedObject)this.id, false);
				}
				return null;
			}
		}

		[Obsolete("Use 'OriginatorPublicKey' property")]
		public OriginatorPublicKey OriginatorKey
		{
			get
			{
				return this.OriginatorPublicKey;
			}
		}

		public OriginatorPublicKey OriginatorPublicKey
		{
			get
			{
				if (this.id is Asn1TaggedObject && ((Asn1TaggedObject)this.id).TagNo == 1)
				{
					return OriginatorPublicKey.GetInstance((Asn1TaggedObject)this.id, false);
				}
				return null;
			}
		}

		public OriginatorIdentifierOrKey(IssuerAndSerialNumber id)
		{
			this.id = id;
		}

		[Obsolete("Use version taking a 'SubjectKeyIdentifier'")]
		public OriginatorIdentifierOrKey(Asn1OctetString id) : this(new SubjectKeyIdentifier(id))
		{
		}

		public OriginatorIdentifierOrKey(SubjectKeyIdentifier id)
		{
			this.id = new DerTaggedObject(false, 0, id);
		}

		public OriginatorIdentifierOrKey(OriginatorPublicKey id)
		{
			this.id = new DerTaggedObject(false, 1, id);
		}

		[Obsolete("Use more specific version")]
		public OriginatorIdentifierOrKey(Asn1Object id)
		{
			this.id = id;
		}

		private OriginatorIdentifierOrKey(Asn1TaggedObject id)
		{
			this.id = id;
		}

		public static OriginatorIdentifierOrKey GetInstance(Asn1TaggedObject o, bool explicitly)
		{
			if (!explicitly)
			{
				throw new ArgumentException("Can't implicitly tag OriginatorIdentifierOrKey");
			}
			return OriginatorIdentifierOrKey.GetInstance(o.GetObject());
		}

		public static OriginatorIdentifierOrKey GetInstance(object o)
		{
			if (o == null || o is OriginatorIdentifierOrKey)
			{
				return (OriginatorIdentifierOrKey)o;
			}
			if (o is IssuerAndSerialNumber)
			{
				return new OriginatorIdentifierOrKey((IssuerAndSerialNumber)o);
			}
			if (o is SubjectKeyIdentifier)
			{
				return new OriginatorIdentifierOrKey((SubjectKeyIdentifier)o);
			}
			if (o is OriginatorPublicKey)
			{
				return new OriginatorIdentifierOrKey((OriginatorPublicKey)o);
			}
			if (o is Asn1TaggedObject)
			{
				return new OriginatorIdentifierOrKey((Asn1TaggedObject)o);
			}
			throw new ArgumentException("Invalid OriginatorIdentifierOrKey: " + o.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.id.ToAsn1Object();
		}
	}
}
