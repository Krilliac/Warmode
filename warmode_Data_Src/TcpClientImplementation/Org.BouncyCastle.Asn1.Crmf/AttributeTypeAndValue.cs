using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class AttributeTypeAndValue : Asn1Encodable
	{
		private readonly DerObjectIdentifier type;

		private readonly Asn1Encodable value;

		public virtual DerObjectIdentifier Type
		{
			get
			{
				return this.type;
			}
		}

		public virtual Asn1Encodable Value
		{
			get
			{
				return this.value;
			}
		}

		private AttributeTypeAndValue(Asn1Sequence seq)
		{
			this.type = (DerObjectIdentifier)seq[0];
			this.value = seq[1];
		}

		public static AttributeTypeAndValue GetInstance(object obj)
		{
			if (obj is AttributeTypeAndValue)
			{
				return (AttributeTypeAndValue)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AttributeTypeAndValue((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public AttributeTypeAndValue(string oid, Asn1Encodable value) : this(new DerObjectIdentifier(oid), value)
		{
		}

		public AttributeTypeAndValue(DerObjectIdentifier type, Asn1Encodable value)
		{
			this.type = type;
			this.value = value;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.type,
				this.value
			});
		}
	}
}
