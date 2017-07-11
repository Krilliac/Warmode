using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class RecipientKeyIdentifier : Asn1Encodable
	{
		private Asn1OctetString subjectKeyIdentifier;

		private DerGeneralizedTime date;

		private OtherKeyAttribute other;

		public Asn1OctetString SubjectKeyIdentifier
		{
			get
			{
				return this.subjectKeyIdentifier;
			}
		}

		public DerGeneralizedTime Date
		{
			get
			{
				return this.date;
			}
		}

		public OtherKeyAttribute OtherKeyAttribute
		{
			get
			{
				return this.other;
			}
		}

		public RecipientKeyIdentifier(Asn1OctetString subjectKeyIdentifier, DerGeneralizedTime date, OtherKeyAttribute other)
		{
			this.subjectKeyIdentifier = subjectKeyIdentifier;
			this.date = date;
			this.other = other;
		}

		public RecipientKeyIdentifier(byte[] subjectKeyIdentifier) : this(subjectKeyIdentifier, null, null)
		{
		}

		public RecipientKeyIdentifier(byte[] subjectKeyIdentifier, DerGeneralizedTime date, OtherKeyAttribute other)
		{
			this.subjectKeyIdentifier = new DerOctetString(subjectKeyIdentifier);
			this.date = date;
			this.other = other;
		}

		public RecipientKeyIdentifier(Asn1Sequence seq)
		{
			this.subjectKeyIdentifier = Asn1OctetString.GetInstance(seq[0]);
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
				throw new ArgumentException("Invalid RecipientKeyIdentifier");
			}
		}

		public static RecipientKeyIdentifier GetInstance(Asn1TaggedObject ato, bool explicitly)
		{
			return RecipientKeyIdentifier.GetInstance(Asn1Sequence.GetInstance(ato, explicitly));
		}

		public static RecipientKeyIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is RecipientKeyIdentifier)
			{
				return (RecipientKeyIdentifier)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RecipientKeyIdentifier((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid RecipientKeyIdentifier: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.subjectKeyIdentifier
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
