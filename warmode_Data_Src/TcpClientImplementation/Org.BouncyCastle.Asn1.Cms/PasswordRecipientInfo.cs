using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class PasswordRecipientInfo : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly AlgorithmIdentifier keyDerivationAlgorithm;

		private readonly AlgorithmIdentifier keyEncryptionAlgorithm;

		private readonly Asn1OctetString encryptedKey;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public AlgorithmIdentifier KeyDerivationAlgorithm
		{
			get
			{
				return this.keyDerivationAlgorithm;
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

		public PasswordRecipientInfo(AlgorithmIdentifier keyEncryptionAlgorithm, Asn1OctetString encryptedKey)
		{
			this.version = new DerInteger(0);
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.encryptedKey = encryptedKey;
		}

		public PasswordRecipientInfo(AlgorithmIdentifier keyDerivationAlgorithm, AlgorithmIdentifier keyEncryptionAlgorithm, Asn1OctetString encryptedKey)
		{
			this.version = new DerInteger(0);
			this.keyDerivationAlgorithm = keyDerivationAlgorithm;
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.encryptedKey = encryptedKey;
		}

		public PasswordRecipientInfo(Asn1Sequence seq)
		{
			this.version = (DerInteger)seq[0];
			if (seq[1] is Asn1TaggedObject)
			{
				this.keyDerivationAlgorithm = AlgorithmIdentifier.GetInstance((Asn1TaggedObject)seq[1], false);
				this.keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[2]);
				this.encryptedKey = (Asn1OctetString)seq[3];
				return;
			}
			this.keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			this.encryptedKey = (Asn1OctetString)seq[2];
		}

		public static PasswordRecipientInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return PasswordRecipientInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static PasswordRecipientInfo GetInstance(object obj)
		{
			if (obj == null || obj is PasswordRecipientInfo)
			{
				return (PasswordRecipientInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PasswordRecipientInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid PasswordRecipientInfo: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version
			});
			if (this.keyDerivationAlgorithm != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.keyDerivationAlgorithm)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.keyEncryptionAlgorithm,
				this.encryptedKey
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
