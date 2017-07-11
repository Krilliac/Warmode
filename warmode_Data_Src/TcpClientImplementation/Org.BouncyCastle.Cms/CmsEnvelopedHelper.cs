using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal class CmsEnvelopedHelper
	{
		internal class CmsAuthenticatedSecureReadable : CmsSecureReadable
		{
			private AlgorithmIdentifier algorithm;

			private IMac mac;

			private CmsReadable readable;

			public AlgorithmIdentifier Algorithm
			{
				get
				{
					return this.algorithm;
				}
			}

			public object CryptoObject
			{
				get
				{
					return this.mac;
				}
			}

			internal CmsAuthenticatedSecureReadable(AlgorithmIdentifier algorithm, CmsReadable readable)
			{
				this.algorithm = algorithm;
				this.readable = readable;
			}

			public CmsReadable GetReadable(KeyParameter sKey)
			{
				string id = this.algorithm.ObjectID.Id;
				try
				{
					this.mac = MacUtilities.GetMac(id);
					this.mac.Init(sKey);
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
					throw new CmsException("error decoding algorithm parameters.", e3);
				}
				CmsReadable result;
				try
				{
					result = new CmsProcessableInputStream(new TeeInputStream(this.readable.GetInputStream(), new MacOutputStream(this.mac)));
				}
				catch (IOException e4)
				{
					throw new CmsException("error reading content.", e4);
				}
				return result;
			}
		}

		internal class CmsEnvelopedSecureReadable : CmsSecureReadable
		{
			private AlgorithmIdentifier algorithm;

			private IBufferedCipher cipher;

			private CmsReadable readable;

			public AlgorithmIdentifier Algorithm
			{
				get
				{
					return this.algorithm;
				}
			}

			public object CryptoObject
			{
				get
				{
					return this.cipher;
				}
			}

			internal CmsEnvelopedSecureReadable(AlgorithmIdentifier algorithm, CmsReadable readable)
			{
				this.algorithm = algorithm;
				this.readable = readable;
			}

			public CmsReadable GetReadable(KeyParameter sKey)
			{
				try
				{
					this.cipher = CipherUtilities.GetCipher(this.algorithm.ObjectID);
					Asn1Encodable parameters = this.algorithm.Parameters;
					Asn1Object asn1Object = (parameters == null) ? null : parameters.ToAsn1Object();
					ICipherParameters cipherParameters = sKey;
					if (asn1Object != null && !(asn1Object is Asn1Null))
					{
						cipherParameters = ParameterUtilities.GetCipherParameters(this.algorithm.ObjectID, cipherParameters, asn1Object);
					}
					else
					{
						string id = this.algorithm.ObjectID.Id;
						if (id.Equals(CmsEnvelopedGenerator.DesEde3Cbc) || id.Equals("1.3.6.1.4.1.188.7.1.1.2") || id.Equals("1.2.840.113533.7.66.10"))
						{
							cipherParameters = new ParametersWithIV(cipherParameters, new byte[8]);
						}
					}
					this.cipher.Init(false, cipherParameters);
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
					throw new CmsException("error decoding algorithm parameters.", e3);
				}
				CmsReadable result;
				try
				{
					result = new CmsProcessableInputStream(new CipherStream(this.readable.GetInputStream(), this.cipher, null));
				}
				catch (IOException e4)
				{
					throw new CmsException("error reading content.", e4);
				}
				return result;
			}
		}

		internal static readonly CmsEnvelopedHelper Instance;

		private static readonly IDictionary KeySizes;

		private static readonly IDictionary BaseCipherNames;

		static CmsEnvelopedHelper()
		{
			CmsEnvelopedHelper.Instance = new CmsEnvelopedHelper();
			CmsEnvelopedHelper.KeySizes = Platform.CreateHashtable();
			CmsEnvelopedHelper.BaseCipherNames = Platform.CreateHashtable();
			CmsEnvelopedHelper.KeySizes.Add(CmsEnvelopedGenerator.DesEde3Cbc, 192);
			CmsEnvelopedHelper.KeySizes.Add(CmsEnvelopedGenerator.Aes128Cbc, 128);
			CmsEnvelopedHelper.KeySizes.Add(CmsEnvelopedGenerator.Aes192Cbc, 192);
			CmsEnvelopedHelper.KeySizes.Add(CmsEnvelopedGenerator.Aes256Cbc, 256);
			CmsEnvelopedHelper.BaseCipherNames.Add(CmsEnvelopedGenerator.DesEde3Cbc, "DESEDE");
			CmsEnvelopedHelper.BaseCipherNames.Add(CmsEnvelopedGenerator.Aes128Cbc, "AES");
			CmsEnvelopedHelper.BaseCipherNames.Add(CmsEnvelopedGenerator.Aes192Cbc, "AES");
			CmsEnvelopedHelper.BaseCipherNames.Add(CmsEnvelopedGenerator.Aes256Cbc, "AES");
		}

		private string GetAsymmetricEncryptionAlgName(string encryptionAlgOid)
		{
			if (PkcsObjectIdentifiers.RsaEncryption.Id.Equals(encryptionAlgOid))
			{
				return "RSA/ECB/PKCS1Padding";
			}
			return encryptionAlgOid;
		}

		internal IBufferedCipher CreateAsymmetricCipher(string encryptionOid)
		{
			string asymmetricEncryptionAlgName = this.GetAsymmetricEncryptionAlgName(encryptionOid);
			if (!asymmetricEncryptionAlgName.Equals(encryptionOid))
			{
				try
				{
					return CipherUtilities.GetCipher(asymmetricEncryptionAlgName);
				}
				catch (SecurityUtilityException)
				{
				}
			}
			return CipherUtilities.GetCipher(encryptionOid);
		}

		internal IWrapper CreateWrapper(string encryptionOid)
		{
			IWrapper wrapper;
			try
			{
				wrapper = WrapperUtilities.GetWrapper(encryptionOid);
			}
			catch (SecurityUtilityException)
			{
				wrapper = WrapperUtilities.GetWrapper(this.GetAsymmetricEncryptionAlgName(encryptionOid));
			}
			return wrapper;
		}

		internal string GetRfc3211WrapperName(string oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			string text = (string)CmsEnvelopedHelper.BaseCipherNames[oid];
			if (text == null)
			{
				throw new ArgumentException("no name for " + oid, "oid");
			}
			return text + "RFC3211Wrap";
		}

		internal int GetKeySize(string oid)
		{
			if (!CmsEnvelopedHelper.KeySizes.Contains(oid))
			{
				throw new ArgumentException("no keysize for " + oid, "oid");
			}
			return (int)CmsEnvelopedHelper.KeySizes[oid];
		}

		internal static RecipientInformationStore BuildRecipientInformationStore(Asn1Set recipientInfos, CmsSecureReadable secureReadable)
		{
			IList list = Platform.CreateArrayList();
			for (int num = 0; num != recipientInfos.Count; num++)
			{
				RecipientInfo instance = RecipientInfo.GetInstance(recipientInfos[num]);
				CmsEnvelopedHelper.ReadRecipientInfo(list, instance, secureReadable);
			}
			return new RecipientInformationStore(list);
		}

		private static void ReadRecipientInfo(IList infos, RecipientInfo info, CmsSecureReadable secureReadable)
		{
			Asn1Encodable info2 = info.Info;
			if (info2 is KeyTransRecipientInfo)
			{
				infos.Add(new KeyTransRecipientInformation((KeyTransRecipientInfo)info2, secureReadable));
				return;
			}
			if (info2 is KekRecipientInfo)
			{
				infos.Add(new KekRecipientInformation((KekRecipientInfo)info2, secureReadable));
				return;
			}
			if (info2 is KeyAgreeRecipientInfo)
			{
				KeyAgreeRecipientInformation.ReadRecipientInfo(infos, (KeyAgreeRecipientInfo)info2, secureReadable);
				return;
			}
			if (info2 is PasswordRecipientInfo)
			{
				infos.Add(new PasswordRecipientInformation((PasswordRecipientInfo)info2, secureReadable));
			}
		}
	}
}
