using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KekRecipientInfo : Asn1Encodable
	{
		private DerInteger version;

		private KekIdentifier kekID;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		private Asn1OctetString encryptedKey;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public KekIdentifier KekID
		{
			get
			{
				return this.kekID;
			}
		}

		public AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				return this.keyEncryptionAlgorithm;
			}
		}

		public Asn1OctetString EncryptedKey
		{
			get
			{
				return this.encryptedKey;
			}
		}

		public KekRecipientInfo(KekIdentifier kekID, AlgorithmIdentifier keyEncryptionAlgorithm, Asn1OctetString encryptedKey)
		{
			this.version = new DerInteger(4);
			this.kekID = kekID;
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.encryptedKey = encryptedKey;
		}

		public KekRecipientInfo(Asn1Sequence seq)
		{
			this.version = (DerInteger)seq[0];
			this.kekID = KekIdentifier.GetInstance(seq[1]);
			this.keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[2]);
			this.encryptedKey = (Asn1OctetString)seq[3];
		}

		public static KekRecipientInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return KekRecipientInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static KekRecipientInfo GetInstance(object obj)
		{
			if (obj == null || obj is KekRecipientInfo)
			{
				return (KekRecipientInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KekRecipientInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid KekRecipientInfo: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.version,
				this.kekID,
				this.keyEncryptionAlgorithm,
				this.encryptedKey
			});
		}
	}
}
