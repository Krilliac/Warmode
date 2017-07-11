using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsAgreementCredentials : AbstractTlsAgreementCredentials
	{
		protected readonly Certificate mCertificate;

		protected readonly AsymmetricKeyParameter mPrivateKey;

		protected readonly IBasicAgreement mBasicAgreement;

		protected readonly bool mTruncateAgreement;

		public override Certificate Certificate
		{
			get
			{
				return this.mCertificate;
			}
		}

		public DefaultTlsAgreementCredentials(Certificate certificate, AsymmetricKeyParameter privateKey)
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
				throw new ArgumentNullException("privateKey");
			}
			if (!privateKey.IsPrivate)
			{
				throw new ArgumentException("must be private", "privateKey");
			}
			if (privateKey is DHPrivateKeyParameters)
			{
				this.mBasicAgreement = new DHBasicAgreement();
				this.mTruncateAgreement = true;
			}
			else
			{
				if (!(privateKey is ECPrivateKeyParameters))
				{
					throw new ArgumentException("type not supported: " + privateKey.GetType().FullName, "privateKey");
				}
				this.mBasicAgreement = new ECDHBasicAgreement();
				this.mTruncateAgreement = false;
			}
			this.mCertificate = certificate;
			this.mPrivateKey = privateKey;
		}

		public override byte[] GenerateAgreement(AsymmetricKeyParameter peerPublicKey)
		{
			this.mBasicAgreement.Init(this.mPrivateKey);
			BigInteger n = this.mBasicAgreement.CalculateAgreement(peerPublicKey);
			if (this.mTruncateAgreement)
			{
				return BigIntegers.AsUnsignedByteArray(n);
			}
			return BigIntegers.AsUnsignedByteArray(this.mBasicAgreement.GetFieldSize(), n);
		}
	}
}
