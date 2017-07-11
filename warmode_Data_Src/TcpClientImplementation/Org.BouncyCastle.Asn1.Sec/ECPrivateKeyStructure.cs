using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Sec
{
	public class ECPrivateKeyStructure : Asn1Encodable
	{
		private readonly Asn1Sequence seq;

		public static ECPrivateKeyStructure GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is ECPrivateKeyStructure)
			{
				return (ECPrivateKeyStructure)obj;
			}
			return new ECPrivateKeyStructure(Asn1Sequence.GetInstance(obj));
		}

		public ECPrivateKeyStructure(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			this.seq = seq;
		}

		public ECPrivateKeyStructure(BigInteger key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.seq = new DerSequence(new Asn1Encodable[]
			{
				new DerInteger(1),
				new DerOctetString(key.ToByteArrayUnsigned())
			});
		}

		public ECPrivateKeyStructure(BigInteger key, Asn1Encodable parameters) : this(key, null, parameters)
		{
		}

		public ECPrivateKeyStructure(BigInteger key, DerBitString publicKey, Asn1Encodable parameters)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				new DerInteger(1),
				new DerOctetString(key.ToByteArrayUnsigned())
			});
			if (parameters != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, parameters)
				});
			}
			if (publicKey != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, publicKey)
				});
			}
			this.seq = new DerSequence(asn1EncodableVector);
		}

		public virtual BigInteger GetKey()
		{
			Asn1OctetString asn1OctetString = (Asn1OctetString)this.seq[1];
			return new BigInteger(1, asn1OctetString.GetOctets());
		}

		public virtual DerBitString GetPublicKey()
		{
			return (DerBitString)this.GetObjectInTag(1);
		}

		public virtual Asn1Object GetParameters()
		{
			return this.GetObjectInTag(0);
		}

		private Asn1Object GetObjectInTag(int tagNo)
		{
			foreach (Asn1Encodable asn1Encodable in this.seq)
			{
				Asn1Object asn1Object = asn1Encodable.ToAsn1Object();
				if (asn1Object is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Object;
					if (asn1TaggedObject.TagNo == tagNo)
					{
						return asn1TaggedObject.GetObject();
					}
				}
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
