using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KekIdentifier : Asn1Encodable
	{
		private Asn1OctetString keyIdentifier;

		private DerGeneralizedTime date;

		private OtherKeyAttribute other;

		public Asn1OctetString KeyIdentifier
		{
			get
			{
				return this.keyIdentifier;
			}
		}

		public DerGeneralizedTime Date
		{
			get
			{
				return this.date;
			}
		}

		public OtherKeyAttribute Other
		{
			get
			{
				return this.other;
			}
		}

		public KekIdentifier(byte[] keyIdentifier, DerGeneralizedTime date, OtherKeyAttribute other)
		{
			this.keyIdentifier = new DerOctetString(keyIdentifier);
			this.date = date;
			this.other = other;
		}

		public KekIdentifier(Asn1Sequence seq)
		{
			this.keyIdentifier = (Asn1OctetString)seq[0];
			switch (seq.Count)
			{
			case 1:
				return;
			case 2:
				if (seq[1] is DerGeneralizedTime)
				{
					this.date = (DerGeneralizedTime)seq[1];
					return;
				}
				this.other = OtherKeyAttribute.GetInstance(seq[2]);
				return;
			case 3:
				this.date = (DerGeneralizedTime)seq[1];
				this.other = OtherKeyAttribute.GetInstance(seq[2]);
				return;
			default:
				throw new ArgumentException("Invalid KekIdentifier");
			}
		}

		public static KekIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return KekIdentifier.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static KekIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is KekIdentifier)
			{
				return (KekIdentifier)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KekIdentifier((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid KekIdentifier: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.keyIdentifier
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.date,
				this.other
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
