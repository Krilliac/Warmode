using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class OtherRevRefs : Asn1Encodable
	{
		private readonly DerObjectIdentifier otherRevRefType;

		private readonly Asn1Object otherRevRefs;

		public DerObjectIdentifier OtherRevRefType
		{
			get
			{
				return this.otherRevRefType;
			}
		}

		public Asn1Object OtherRevRefsObject
		{
			get
			{
				return this.otherRevRefs;
			}
		}

		public static OtherRevRefs GetInstance(object obj)
		{
			if (obj == null || obj is OtherRevRefs)
			{
				return (OtherRevRefs)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OtherRevRefs((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'OtherRevRefs' factory: " + obj.GetType().Name, "obj");
		}

		private OtherRevRefs(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.otherRevRefType = (DerObjectIdentifier)seq[0].ToAsn1Object();
			this.otherRevRefs = seq[1].ToAsn1Object();
		}

		public OtherRevRefs(DerObjectIdentifier otherRevRefType, Asn1Encodable otherRevRefs)
		{
			if (otherRevRefType == null)
			{
				throw new ArgumentNullException("otherRevRefType");
			}
			if (otherRevRefs == null)
			{
				throw new ArgumentNullException("otherRevRefs");
			}
			this.otherRevRefType = otherRevRefType;
			this.otherRevRefs = otherRevRefs.ToAsn1Object();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.otherRevRefType,
				this.otherRevRefs
			});
		}
	}
}