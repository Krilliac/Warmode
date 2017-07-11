using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CrlValidatedID : Asn1Encodable
	{
		private readonly OtherHash crlHash;

		private readonly CrlIdentifier crlIdentifier;

		public OtherHash CrlHash
		{
			get
			{
				return this.crlHash;
			}
		}

		public CrlIdentifier CrlIdentifier
		{
			get
			{
				return this.crlIdentifier;
			}
		}

		public static CrlValidatedID GetInstance(object obj)
		{
			if (obj == null || obj is CrlValidatedID)
			{
				return (CrlValidatedID)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CrlValidatedID((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CrlValidatedID' factory: " + obj.GetType().Name, "obj");
		}

		private CrlValidatedID(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.crlHash = OtherHash.GetInstance(seq[0].ToAsn1Object());
			if (seq.Count > 1)
			{
				this.crlIdentifier = CrlIdentifier.GetInstance(seq[1].ToAsn1Object());
			}
		}

		public CrlValidatedID(OtherHash crlHash) : this(crlHash, null)
		{
		}

		public CrlValidatedID(OtherHash crlHash, CrlIdentifier crlIdentifier)
		{
			if (crlHash == null)
			{
				throw new ArgumentNullException("crlHash");
			}
			this.crlHash = crlHash;
			this.crlIdentifier = crlIdentifier;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.crlHash.ToAsn1Object()
			});
			if (this.crlIdentifier != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.crlIdentifier.ToAsn1Object()
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
