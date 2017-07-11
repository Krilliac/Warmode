using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class PasswordRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private AlgorithmIdentifier keyDerivationAlgorithm;

		private KeyParameter keyEncryptionKey;

		private string keyEncryptionKeyOID;

		internal AlgorithmIdentifier KeyDerivationAlgorithm
		{
			set
			{
				this.keyDerivationAlgorithm = value;
			}
		}

		internal KeyParameter KeyEncryptionKey
		{
			set
			{
				this.keyEncryptionKey = value;
			}
		}

		internal string KeyEncryptionKeyOID
		{
			set
			{
				this.keyEncryptionKeyOID = value;
			}
		}

		internal PasswordRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			string rfc3211WrapperName = PasswordRecipientInfoGenerator.Helper.GetRfc3211WrapperName(this.keyEncryptionKeyOID);
			IWrapper wrapper = PasswordRecipientInfoGenerator.Helper.CreateWrapper(rfc3211WrapperName);
			int num = rfc3211WrapperName.StartsWith("DESEDE") ? 8 : 16;
			byte[] array = new byte[num];
			random.NextBytes(array);
			ICipherParameters parameters = new ParametersWithIV(this.keyEncryptionKey, array);
			wrapper.Init(true, new ParametersWithRandom(parameters, random));
			Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
			DerSequence parameters2 = new DerSequence(new Asn1Encodable[]
			{
				new DerObjectIdentifier(this.keyEncryptionKeyOID),
				new DerOctetString(array)
			});
			AlgorithmIdentifier keyEncryptionAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgPwriKek, parameters2);
			return new RecipientInfo(new PasswordRecipientInfo(this.keyDerivationAlgorithm, keyEncryptionAlgorithm, encryptedKey));
		}
	}
}
