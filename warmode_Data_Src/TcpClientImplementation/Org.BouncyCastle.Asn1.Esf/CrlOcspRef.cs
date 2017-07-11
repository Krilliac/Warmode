using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CrlOcspRef : Asn1Encodable
	{
		private readonly CrlListID crlids;

		private readonly OcspListID ocspids;

		private readonly OtherRevRefs otherRev;

		public CrlListID CrlIDs
		{
			get
			{
				return this.crlids;
			}
		}

		public OcspListID OcspIDs
		{
			get
			{
				return this.ocspids;
			}
		}

		public OtherRevRefs OtherRev
		{
			get
			{
				return this.otherRev;
			}
		}

		public static CrlOcspRef GetInstance(object obj)
		{
			if (obj == null || obj is CrlOcspRef)
			{
				return (CrlOcspRef)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CrlOcspRef((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CrlOcspRef' factory: " + obj.GetType().Name, "obj");
		}

		private CrlOcspRef(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				Asn1Object @object = asn1TaggedObject.GetObject();
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.crlids = CrlListID.GetInstance(@object);
					break;
				case 1:
					this.ocspids = OcspListID.GetInstance(@object);
					break;
				case 2:
					this.otherRev = OtherRevRefs.GetInstance(@object);
					break;
				default:
					throw new ArgumentException("Illegal tag in CrlOcspRef", "seq");
				}
			}
		}

		public CrlOcspRef(CrlListID crlids, OcspListID ocspids, OtherRevRefs otherRev)
		{
			this.crlids = crlids;
			this.ocspids = ocspids;
			this.otherRev = otherRev;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.crlids != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.crlids.ToAsn1Object())
				});
			}
			if (this.ocspids != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.ocspids.ToAsn1Object())
				});
			}
			if (this.otherRev != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.otherRev.ToAsn1Object())
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
