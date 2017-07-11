using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class ProofOfPossession : Asn1Encodable, IAsn1Choice
	{
		public const int TYPE_RA_VERIFIED = 0;

		public const int TYPE_SIGNING_KEY = 1;

		public const int TYPE_KEY_ENCIPHERMENT = 2;

		public const int TYPE_KEY_AGREEMENT = 3;

		private readonly int tagNo;

		private readonly Asn1Encodable obj;

		public virtual int Type
		{
			get
			{
				return this.tagNo;
			}
		}

		public virtual Asn1Encodable Object
		{
			get
			{
				return this.obj;
			}
		}

		private ProofOfPossession(Asn1TaggedObject tagged)
		{
			this.tagNo = tagged.TagNo;
			switch (this.tagNo)
			{
			case 0:
				this.obj = DerNull.Instance;
				return;
			case 1:
				this.obj = PopoSigningKey.GetInstance(tagged, false);
				return;
			case 2:
			case 3:
				this.obj = PopoPrivKey.GetInstance(tagged, false);
				return;
			default:
				throw new ArgumentException("unknown tag: " + this.tagNo, "tagged");
			}
		}

		public static ProofOfPossession GetInstance(object obj)
		{
			if (obj is ProofOfPossession)
			{
				return (ProofOfPossession)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new ProofOfPossession((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public ProofOfPossession()
		{
			this.tagNo = 0;
			this.obj = DerNull.Instance;
		}

		public ProofOfPossession(PopoSigningKey Poposk)
		{
			this.tagNo = 1;
			this.obj = Poposk;
		}

		public ProofOfPossession(int type, PopoPrivKey privkey)
		{
			this.tagNo = type;
			this.obj = privkey;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerTaggedObject(false, this.tagNo, this.obj);
		}
	}
}
