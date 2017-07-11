using System;

namespace Org.BouncyCastle.Asn1.Cms.Ecc
{
	public class MQVuserKeyingMaterial : Asn1Encodable
	{
		private OriginatorPublicKey ephemeralPublicKey;

		private Asn1OctetString addedukm;

		public OriginatorPublicKey EphemeralPublicKey
		{
			get
			{
				return this.ephemeralPublicKey;
			}
		}

		public Asn1OctetString AddedUkm
		{
			get
			{
				return this.addedukm;
			}
		}

		public MQVuserKeyingMaterial(OriginatorPublicKey ephemeralPublicKey, Asn1OctetString addedukm)
		{
			this.ephemeralPublicKey = ephemeralPublicKey;
			this.addedukm = addedukm;
		}

		private MQVuserKeyingMaterial(Asn1Sequence seq)
		{
			this.ephemeralPublicKey = OriginatorPublicKey.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.addedukm = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[1], true);
			}
		}

		public static MQVuserKeyingMaterial GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return MQVuserKeyingMaterial.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static MQVuserKeyingMaterial GetInstance(object obj)
		{
			if (obj == null || obj is MQVuserKeyingMaterial)
			{
				return (MQVuserKeyingMaterial)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new MQVuserKeyingMaterial((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid MQVuserKeyingMaterial: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.ephemeralPublicKey
			});
			if (this.addedukm != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.addedukm)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
