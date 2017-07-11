using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsEnvelopedDataGenerator : CmsEnvelopedGenerator
	{
		public CmsEnvelopedDataGenerator()
		{
		}

		public CmsEnvelopedDataGenerator(SecureRandom rand) : base(rand)
		{
		}

		private CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid, CipherKeyGenerator keyGen)
		{
			AlgorithmIdentifier contentEncryptionAlgorithm = null;
			KeyParameter keyParameter;
			Asn1OctetString encryptedContent;
			try
			{
				byte[] array = keyGen.GenerateKey();
				keyParameter = ParameterUtilities.CreateKeyParameter(encryptionOid, array);
				Asn1Encodable asn1Params = this.GenerateAsn1Parameters(encryptionOid, array);
				ICipherParameters parameters;
				contentEncryptionAlgorithm = this.GetAlgorithmIdentifier(encryptionOid, keyParameter, asn1Params, out parameters);
				IBufferedCipher cipher = CipherUtilities.GetCipher(encryptionOid);
				cipher.Init(true, new ParametersWithRandom(parameters, this.rand));
				MemoryStream memoryStream = new MemoryStream();
				CipherStream cipherStream = new CipherStream(memoryStream, null, cipher);
				content.Write(cipherStream);
				cipherStream.Close();
				encryptedContent = new BerOctetString(memoryStream.ToArray());
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e2)
			{
				throw new CmsException("key invalid in message.", e2);
			}
			catch (IOException e3)
			{
				throw new CmsException("exception decoding algorithm parameters.", e3);
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (RecipientInfoGenerator recipientInfoGenerator in this.recipientInfoGenerators)
			{
				try
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						recipientInfoGenerator.Generate(keyParameter, this.rand)
					});
				}
				catch (InvalidKeyException e4)
				{
					throw new CmsException("key inappropriate for algorithm.", e4);
				}
				catch (GeneralSecurityException e5)
				{
					throw new CmsException("error making encrypted content.", e5);
				}
			}
			EncryptedContentInfo encryptedContentInfo = new EncryptedContentInfo(CmsObjectIdentifiers.Data, contentEncryptionAlgorithm, encryptedContent);
			Asn1Set unprotectedAttrs = null;
			if (this.unprotectedAttributeGenerator != null)
			{
				Org.BouncyCastle.Asn1.Cms.AttributeTable attributes = this.unprotectedAttributeGenerator.GetAttributes(Platform.CreateHashtable());
				unprotectedAttrs = new BerSet(attributes.ToAsn1EncodableVector());
			}
			ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.EnvelopedData, new EnvelopedData(null, new DerSet(asn1EncodableVector), encryptedContentInfo, unprotectedAttrs));
			return new CmsEnvelopedData(contentInfo);
		}

		public CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid)
		{
			CmsEnvelopedData result;
			try
			{
				CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
				keyGenerator.Init(new KeyGenerationParameters(this.rand, keyGenerator.DefaultStrength));
				result = this.Generate(content, encryptionOid, keyGenerator);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("can't find key generation algorithm.", e);
			}
			return result;
		}

		public CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid, int keySize)
		{
			CmsEnvelopedData result;
			try
			{
				CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
				keyGenerator.Init(new KeyGenerationParameters(this.rand, keySize));
				result = this.Generate(content, encryptionOid, keyGenerator);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("can't find key generation algorithm.", e);
			}
			return result;
		}
	}
}
