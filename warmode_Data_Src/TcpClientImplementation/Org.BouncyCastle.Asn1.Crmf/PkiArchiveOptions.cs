using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class PkiArchiveOptions : Asn1Encodable, IAsn1Choice
	{
		public const int encryptedPrivKey = 0;

		public const int keyGenParameters = 1;

		public const int archiveRemGenPrivKey = 2;

		private readonly Asn1Encodable value;

		public virtual int Type
		{
			get
			{
				if (this.value is EncryptedKey)
				{
					return 0;
				}
				if (this.value is Asn1OctetString)
				{
					return 1;
				}
				return 2;
			}
		}

		public virtual Asn1Encodable Value
		{
			get
			{
				return this.value;
			}
		}

		public static PkiArchiveOptions GetInstance(object obj)
		{
			if (obj is PkiArchiveOptions)
			{
				return (PkiArchiveOptions)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new PkiArchiveOptions((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		private PkiArchiveOptions(Asn1TaggedObject tagged)
		{
			switch (tagged.TagNo)
			{
			case 0:
				this.value = EncryptedKey.GetInstance(tagged.GetObject());
				return;
			case 1:
				this.value = Asn1OctetString.GetInstance(tagged, false);
				return;
			case 2:
				this.value = DerBoolean.GetInstance(tagged, false);
				return;
			default:
				throw new ArgumentException("unknown tag number: " + tagged.TagNo, "tagged");
			}
		}

		public PkiArchiveOptions(EncryptedKey encKey)
		{
			this.value = encKey;
		}

		public PkiArchiveOptions(Asn1OctetString keyGenParameters)
		{
			this.value = keyGenParameters;
		}

		public PkiArchiveOptions(bool archiveRemGenPrivKey)
		{
			this.value = DerBoolean.GetInstance(archiveRemGenPrivKey);
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.value is EncryptedKey)
			{
				return new DerTaggedObject(true, 0, this.value);
			}
			if (this.value is Asn1OctetString)
			{
				return new DerTaggedObject(false, 1, this.value);
			}
			return new DerTaggedObject(false, 2, this.value);
		}
	}
}
