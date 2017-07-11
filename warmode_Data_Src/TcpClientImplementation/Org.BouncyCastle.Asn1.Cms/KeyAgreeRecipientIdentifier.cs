using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KeyAgreeRecipientIdentifier : Asn1Encodable, IAsn1Choice
	{
		private readonly IssuerAndSerialNumber issuerSerial;

		private readonly RecipientKeyIdentifier rKeyID;

		public IssuerAndSerialNumber IssuerAndSerialNumber
		{
			get
			{
				return this.issuerSerial;
			}
		}

		public RecipientKeyIdentifier RKeyID
		{
			get
			{
				return this.rKeyID;
			}
		}

		public static KeyAgreeRecipientIdentifier GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return KeyAgreeRecipientIdentifier.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static KeyAgreeRecipientIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is KeyAgreeRecipientIdentifier)
			{
				return (KeyAgreeRecipientIdentifier)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KeyAgreeRecipientIdentifier(IssuerAndSerialNumber.GetInstance(obj));
			}
			if (obj is Asn1TaggedObject && ((Asn1TaggedObject)obj).TagNo == 0)
			{
				return new KeyAgreeRecipientIdentifier(RecipientKeyIdentifier.GetInstance((Asn1TaggedObject)obj, false));
			}
			throw new ArgumentException("Invalid KeyAgreeRecipientIdentifier: " + obj.GetType().FullName, "obj");
		}

		public KeyAgreeRecipientIdentifier(IssuerAndSerialNumber issuerSerial)
		{
			this.issuerSerial = issuerSerial;
		}

		public KeyAgreeRecipientIdentifier(RecipientKeyIdentifier rKeyID)
		{
			this.rKeyID = rKeyID;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.issuerSerial != null)
			{
				return this.issuerSerial.ToAsn1Object();
			}
			return new DerTaggedObject(false, 0, this.rKeyID);
		}
	}
}
