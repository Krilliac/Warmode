using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KeyAgreeRecipientInfo : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorIdentifierOrKey originator;

		private Asn1OctetString ukm;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		private Asn1Sequence recipientEncryptedKeys;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public OriginatorIdentifierOrKey Originator
		{
			get
			{
				return this.originator;
			}
		}

		public Asn1OctetString UserKeyingMaterial
		{
			get
			{
				return this.ukm;
			}
		}

		public AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				return this.keyEncryptionAlgorithm;
			}
		}

		public Asn1Sequence RecipientEncryptedKeys
		{
			get
			{
				return this.recipientEncryptedKeys;
			}
		}

		public KeyAgreeRecipientInfo(OriginatorIdentifierOrKey originator, Asn1OctetString ukm, AlgorithmIdentifier keyEncryptionAlgorithm, Asn1Sequence recipientEncryptedKeys)
		{
			this.version = new DerInteger(3);
			this.originator = originator;
			this.ukm = ukm;
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.recipientEncryptedKeys = recipientEncryptedKeys;
		}

		public KeyAgreeRecipientInfo(Asn1Sequence seq)
		{
			int index = 0;
			this.version = (DerInteger)seq[index++];
			this.originator = OriginatorIdentifierOrKey.GetInstance((Asn1TaggedObject)seq[index++], true);
			if (seq[index] is Asn1TaggedObject)
			{
				this.ukm = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[index++], true);
			}
			this.keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[index++]);
			this.recipientEncryptedKeys = (Asn1Sequence)seq[index++];
		}

		public static KeyAgreeRecipientInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return KeyAgreeRecipientInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static KeyAgreeRecipientInfo GetInstance(object obj)
		{
			if (obj == null || obj is KeyAgreeRecipientInfo)
			{
				return (KeyAgreeRecipientInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KeyAgreeRecipientInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Illegal object in KeyAgreeRecipientInfo: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				new DerTaggedObject(true, 0, this.originator)
			});
			if (this.ukm != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.ukm)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.keyEncryptionAlgorithm,
				this.recipientEncryptedKeys
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
