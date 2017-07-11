using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsEncryptionCredentials : AbstractTlsEncryptionCredentials
	{
		protected readonly TlsContext mContext;

		protected readonly Certificate mCertificate;

		protected readonly AsymmetricKeyParameter mPrivateKey;

		public override Certificate Certificate
		{
			get
			{
				return this.mCertificate;
			}
		}

		public DefaultTlsEncryptionCredentials(TlsContext context, Certificate certificate, AsymmetricKeyParameter privateKey)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (certificate.IsEmpty)
			{
				throw new ArgumentException("cannot be empty", "certificate");
			}
			if (privateKey == null)
			{
				throw new ArgumentNullException("'privateKey' cannot be null");
			}
			if (!privateKey.IsPrivate)
			{
				throw new ArgumentException("must be private", "privateKey");
			}
			if (!(privateKey is RsaKeyParameters))
			{
				throw new ArgumentException("type not supported: " + privateKey.GetType().FullName, "privateKey");
			}
			this.mContext = context;
			this.mCertificate = certificate;
			this.mPrivateKey = privateKey;
		}

		public override byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret)
		{
			return TlsRsaUtilities.SafeDecryptPreMasterSecret(this.mContext, (RsaKeyParameters)this.mPrivateKey, encryptedPreMasterSecret);
		}
	}
}
