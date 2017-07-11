using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class InfoTypeAndValue : Asn1Encodable
	{
		private readonly DerObjectIdentifier infoType;

		private readonly Asn1Encodable infoValue;

		public virtual DerObjectIdentifier InfoType
		{
			get
			{
				return this.infoType;
			}
		}

		public virtual Asn1Encodable InfoValue
		{
			get
			{
				return this.infoValue;
			}
		}

		private InfoTypeAndValue(Asn1Sequence seq)
		{
			this.infoType = DerObjectIdentifier.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.infoValue = seq[1];
			}
		}

		public static InfoTypeAndValue GetInstance(object obj)
		{
			if (obj is InfoTypeAndValue)
			{
				return (InfoTypeAndValue)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new InfoTypeAndValue((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public InfoTypeAndValue(DerObjectIdentifier infoType)
		{
			this.infoType = infoType;
			this.infoValue = null;
		}

		public InfoTypeAndValue(DerObjectIdentifier infoType, Asn1Encodable optionalValue)
		{
			this.infoType = infoType;
			this.infoValue = optionalValue;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.infoType
			});
			if (this.infoValue != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.infoValue
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
