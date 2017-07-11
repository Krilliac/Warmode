using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal class CmsAuthEnvelopedData
	{
		private class AuthEnvelopedSecureReadable : CmsSecureReadable
		{
			private readonly CmsAuthEnvelopedData parent;

			public AlgorithmIdentifier Algorithm
			{
				get
				{
					return this.parent.authEncAlg;
				}
			}

			public object CryptoObject
			{
				get
				{
					return null;
				}
			}

			internal AuthEnvelopedSecureReadable(CmsAuthEnvelopedData parent)
			{
				this.parent = parent;
			}

			public CmsReadable GetReadable(KeyParameter key)
			{
				throw new CmsException("AuthEnveloped data decryption not yet implemented");
			}
		}

		internal RecipientInformationStore recipientInfoStore;

		internal ContentInfo contentInfo;

		private OriginatorInfo originator;

		private AlgorithmIdentifier authEncAlg;

		private Asn1Set authAttrs;

		private byte[] mac;

		private Asn1Set unauthAttrs;

		public CmsAuthEnvelopedData(byte[] authEnvData) : this(CmsUtilities.ReadContentInfo(authEnvData))
		{
		}

		public CmsAuthEnvelopedData(Stream authEnvData) : this(CmsUtilities.ReadContentInfo(authEnvData))
		{
		}

		public CmsAuthEnvelopedData(ContentInfo contentInfo)
		{
			this.contentInfo = contentInfo;
			AuthEnvelopedData instance = AuthEnvelopedData.GetInstance(contentInfo.Content);
			this.originator = instance.OriginatorInfo;
			Asn1Set recipientInfos = instance.RecipientInfos;
			EncryptedContentInfo authEncryptedContentInfo = instance.AuthEncryptedContentInfo;
			this.authEncAlg = authEncryptedContentInfo.ContentEncryptionAlgorithm;
			CmsSecureReadable secureReadable = new CmsAuthEnvelopedData.AuthEnvelopedSecureReadable(this);
			this.recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(recipientInfos, secureReadable);
			this.authAttrs = instance.AuthAttrs;
			this.mac = instance.Mac.GetOctets();
			this.unauthAttrs = instance.UnauthAttrs;
		}
	}
}
