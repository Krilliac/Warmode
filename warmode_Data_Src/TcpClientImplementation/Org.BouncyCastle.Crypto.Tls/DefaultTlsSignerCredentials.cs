using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsSignerCredentials : AbstractTlsSignerCredentials
	{
		protected readonly TlsContext mContext;

		protected readonly Certificate mCertificate;

		protected readonly AsymmetricKeyParameter mPrivateKey;

		protected readonly SignatureAndHashAlgorithm mSignatureAndHashAlgorithm;

		protected readonly TlsSigner mSigner;

		public override Certificate Certificate
		{
			get
			{
				return this.mCertificate;
			}
		}

		public override SignatureAndHashAlgorithm SignatureAndHashAlgorithm
		{
			get
			{
				return this.mSignatureAndHashAlgorithm;
			}
		}

		public DefaultTlsSignerCredentials(TlsContext context, Certificate certificate, AsymmetricKeyParameter privateKey) : this(context, certificate, privateKey, null)
		{
		}

		public DefaultTlsSignerCredentials(TlsContext context, Certificate certificate, AsymmetricKeyParameter privateKey, SignatureAndHashAlgorithm signatureAndHashAlgorithm)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (certificate.IsEmpty)
			{
				throw new ArgumentException("cannot be empty", "clientCertificate");
			}
			if (privateKey == null)
			{
				throw new ArgumentNullException("privateKey");
			}
			if (!privateKey.IsPrivate)
			{
				throw new ArgumentException("must be private", "privateKey");
			}
			if (TlsUtilities.IsTlsV12(context) && signatureAndHashAlgorithm == null)
			{
				throw new ArgumentException("cannot be null for (D)TLS 1.2+", "signatureAndHashAlgorithm");
			}
			if (privateKey is RsaKeyParameters)
			{
				this.mSigner = new TlsRsaSigner();
			}
			else if (privateKey is DsaPrivateKeyParameters)
			{
				this.mSigner = new TlsDssSigner();
			}
			else
			{
				if (!(privateKey is ECPrivateKeyParameters))
				{
					throw new ArgumentException("type not supported: " + privateKey.GetType().FullName, "privateKey");
				}
				this.mSigner = new TlsECDsaSigner();
			}
			this.mSigner.Init(context);
			this.mContext = context;
			this.mCertificate = certificate;
			this.mPrivateKey = privateKey;
			this.mSignatureAndHashAlgorithm = signatureAndHashAlgorithm;
		}

		public override byte[] GenerateCertificateSignature(byte[] hash)
		{
			byte[] result;
			try
			{
				if (TlsUtilities.IsTlsV12(this.mContext))
				{
					result = this.mSigner.GenerateRawSignature(this.mSignatureAndHashAlgorithm, this.mPrivateKey, hash);
				}
				else
				{
					result = this.mSigner.GenerateRawSignature(this.mPrivateKey, hash);
				}
			}
			catch (CryptoException alertCause)
			{
				throw new TlsFatalAlert(80, alertCause);
			}
			return result;
		}
	}
}
