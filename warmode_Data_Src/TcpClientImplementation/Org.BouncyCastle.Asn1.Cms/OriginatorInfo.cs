using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class OriginatorInfo : Asn1Encodable
	{
		private Asn1Set certs;

		private Asn1Set crls;

		public Asn1Set Certificates
		{
			get
			{
				return this.certs;
			}
		}

		public Asn1Set Crls
		{
			get
			{
				return this.crls;
			}
		}

		public OriginatorInfo(Asn1Set certs, Asn1Set crls)
		{
			this.certs = certs;
			this.crls = crls;
		}

		public OriginatorInfo(Asn1Sequence seq)
		{
			switch (seq.Count)
			{
			case 0:
				return;
			case 1:
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[0];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.certs = Asn1Set.GetInstance(asn1TaggedObject, false);
					return;
				case 1:
					this.crls = Asn1Set.GetInstance(asn1TaggedObject, false);
					return;
				default:
					throw new ArgumentException("Bad tag in OriginatorInfo: " + asn1TaggedObject.TagNo);
				}
				break;
			}
			case 2:
				this.certs = Asn1Set.GetInstance((Asn1TaggedObject)seq[0], false);
				this.crls = Asn1Set.GetInstance((Asn1TaggedObject)seq[1], false);
				return;
			default:
				throw new ArgumentException("OriginatorInfo too big");
			}
		}

		public static OriginatorInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return OriginatorInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OriginatorInfo GetInstance(object obj)
		{
			if (obj == null || obj is OriginatorInfo)
			{
				return (OriginatorInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OriginatorInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid OriginatorInfo: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.certs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.certs)
				});
			}
			if (this.crls != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.crls)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
