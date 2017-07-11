using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class KekRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private KeyParameter keyEncryptionKey;

		private string keyEncryptionKeyOID;

		private KekIdentifier kekIdentifier;

		private AlgorithmIdentifier keyEncryptionAlgorithm;

		internal KekIdentifier KekIdentifier
		{
			set
			{
				this.kekIdentifier = value;
			}
		}

		internal KeyParameter KeyEncryptionKey
		{
			set
			{
				this.keyEncryptionKey = value;
				this.keyEncryptionAlgorithm = KekRecipientInfoGenerator.DetermineKeyEncAlg(this.keyEncryptionKeyOID, this.keyEncryptionKey);
			}
		}

		internal string KeyEncryptionKeyOID
		{
			set
			{
				this.keyEncryptionKeyOID = value;
			}
		}

		internal KekRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			IWrapper wrapper = KekRecipientInfoGenerator.Helper.CreateWrapper(this.keyEncryptionAlgorithm.ObjectID.Id);
			wrapper.Init(true, new ParametersWithRandom(this.keyEncryptionKey, random));
			Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
			return new RecipientInfo(new KekRecipientInfo(this.kekIdentifier, this.keyEncryptionAlgorithm, encryptedKey));
		}

		private static AlgorithmIdentifier DetermineKeyEncAlg(string algorithm, KeyParameter key)
		{
			if (algorithm.StartsWith("DES"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCms3DesWrap, DerNull.Instance);
			}
			if (algorithm.StartsWith("RC2"))
			{
				return new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgCmsRC2Wrap, new DerInteger(58));
			}
			if (algorithm.StartsWith("AES"))
			{
				int num = key.GetKey().Length * 8;
				DerObjectIdentifier objectID;
				if (num == 128)
				{
					objectID = NistObjectIdentifiers.IdAes128Wrap;
				}
				else if (num == 192)
				{
					objectID = NistObjectIdentifiers.IdAes192Wrap;
				}
				else
				{
					if (num != 256)
					{
						throw new ArgumentException("illegal keysize in AES");
					}
					objectID = NistObjectIdentifiers.IdAes256Wrap;
				}
				return new AlgorithmIdentifier(objectID);
			}
			if (algorithm.StartsWith("SEED"))
			{
				return new AlgorithmIdentifier(KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
			}
			if (algorithm.StartsWith("CAMELLIA"))
			{
				int num2 = key.GetKey().Length * 8;
				DerObjectIdentifier objectID2;
				if (num2 == 128)
				{
					objectID2 = NttObjectIdentifiers.IdCamellia128Wrap;
				}
				else if (num2 == 192)
				{
					objectID2 = NttObjectIdentifiers.IdCamellia192Wrap;
				}
				else
				{
					if (num2 != 256)
					{
						throw new ArgumentException("illegal keysize in Camellia");
					}
					objectID2 = NttObjectIdentifiers.IdCamellia256Wrap;
				}
				return new AlgorithmIdentifier(objectID2);
			}
			throw new ArgumentException("unknown algorithm");
		}
	}
}
