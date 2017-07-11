using Org.BouncyCastle.Asn1.Cms;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class EncryptedKey : Asn1Encodable, IAsn1Choice
	{
		private readonly EnvelopedData envelopedData;

		private readonly EncryptedValue encryptedValue;

		public virtual bool IsEncryptedValue
		{
			get
			{
				return this.encryptedValue != null;
			}
		}

		public virtual Asn1Encodable Value
		{
			get
			{
				if (this.encryptedValue != null)
				{
					return this.encryptedValue;
				}
				return this.envelopedData;
			}
		}

		public static EncryptedKey GetInstance(object o)
		{
			if (o is EncryptedKey)
			{
				return (EncryptedKey)o;
			}
			if (o is Asn1TaggedObject)
			{
				return new EncryptedKey(EnvelopedData.GetInstance((Asn1TaggedObject)o, false));
			}
			if (o is EncryptedValue)
			{
				return new EncryptedKey((EncryptedValue)o);
			}
			return new EncryptedKey(EncryptedValue.GetInstance(o));
		}

		public EncryptedKey(EnvelopedData envelopedData)
		{
			this.envelopedData = envelopedData;
		}

		public EncryptedKey(EncryptedValue encryptedValue)
		{
			this.encryptedValue = encryptedValue;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.encryptedValue != null)
			{
				return this.encryptedValue.ToAsn1Object();
			}
			return new DerTaggedObject(false, 0, this.envelopedData);
		}
	}
}
