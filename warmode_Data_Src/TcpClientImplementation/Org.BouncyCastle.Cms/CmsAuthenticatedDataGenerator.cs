using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsAuthenticatedDataGenerator : CmsAuthenticatedGenerator
	{
		public CmsAuthenticatedDataGenerator()
		{
		}

		public CmsAuthenticatedDataGenerator(SecureRandom rand) : base(rand)
		{
		}

		private CmsAuthenticatedData Generate(CmsProcessable content, string macOid, CipherKeyGenerator keyGen)
		{
			KeyParameter keyParameter;
			AlgorithmIdentifier algorithmIdentifier;
			Asn1OctetString content2;
			Asn1OctetString mac2;
			try
			{
				byte[] array = keyGen.GenerateKey();
				keyParameter = ParameterUtilities.CreateKeyParameter(macOid, array);
				Asn1Encodable asn1Params = this.GenerateAsn1Parameters(macOid, array);
				ICipherParameters cipherParameters;
				algorithmIdentifier = this.GetAlgorithmIdentifier(macOid, keyParameter, asn1Params, out cipherParameters);
				IMac mac = MacUtilities.GetMac(macOid);
				mac.Init(keyParameter);
				MemoryStream memoryStream = new MemoryStream();
				Stream stream = new TeeOutputStream(memoryStream, new MacOutputStream(mac));
				content.Write(stream);
				stream.Close();
				memoryStream.Close();
				content2 = new BerOctetString(memoryStream.ToArray());
				byte[] str = MacUtilities.DoFinal(mac);
				mac2 = new DerOctetString(str);
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
			ContentInfo encapsulatedContent = new ContentInfo(CmsObjectIdentifiers.Data, content2);
			ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.AuthenticatedData, new AuthenticatedData(null, new DerSet(asn1EncodableVector), algorithmIdentifier, null, encapsulatedContent, null, mac2, null));
			return new CmsAuthenticatedData(contentInfo);
		}

		public CmsAuthenticatedData Generate(CmsProcessable content, string encryptionOid)
		{
			CmsAuthenticatedData result;
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
	}
}
