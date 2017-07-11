using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AbstractTlsKeyExchange : TlsKeyExchange
	{
		protected readonly int mKeyExchange;

		protected IList mSupportedSignatureAlgorithms;

		protected TlsContext mContext;

		public virtual bool RequiresServerKeyExchange
		{
			get
			{
				return false;
			}
		}

		protected AbstractTlsKeyExchange(int keyExchange, IList supportedSignatureAlgorithms)
		{
			this.mKeyExchange = keyExchange;
			this.mSupportedSignatureAlgorithms = supportedSignatureAlgorithms;
		}

		public virtual void Init(TlsContext context)
		{
			this.mContext = context;
			ProtocolVersion clientVersion = context.ClientVersion;
			if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(clientVersion))
			{
				if (this.mSupportedSignatureAlgorithms == null)
				{
					switch (this.mKeyExchange)
					{
					case 1:
					case 5:
					case 9:
					case 15:
					case 18:
					case 19:
					case 23:
						this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultRsaSignatureAlgorithms();
						return;
					case 3:
					case 7:
					case 22:
						this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultDssSignatureAlgorithms();
						return;
					case 13:
					case 14:
					case 21:
					case 24:
						return;
					case 16:
					case 17:
						this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultECDsaSignatureAlgorithms();
						return;
					}
					throw new InvalidOperationException("unsupported key exchange algorithm");
				}
			}
			else if (this.mSupportedSignatureAlgorithms != null)
			{
				throw new InvalidOperationException("supported_signature_algorithms not allowed for " + clientVersion);
			}
		}

		public abstract void SkipServerCredentials();

		public virtual void ProcessServerCertificate(Certificate serverCertificate)
		{
			IList arg_06_0 = this.mSupportedSignatureAlgorithms;
		}

		public virtual void ProcessServerCredentials(TlsCredentials serverCredentials)
		{
			this.ProcessServerCertificate(serverCredentials.Certificate);
		}

		public virtual byte[] GenerateServerKeyExchange()
		{
			if (this.RequiresServerKeyExchange)
			{
				throw new TlsFatalAlert(80);
			}
			return null;
		}

		public virtual void SkipServerKeyExchange()
		{
			if (this.RequiresServerKeyExchange)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public virtual void ProcessServerKeyExchange(Stream input)
		{
			if (!this.RequiresServerKeyExchange)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public abstract void ValidateCertificateRequest(CertificateRequest certificateRequest);

		public virtual void SkipClientCredentials()
		{
		}

		public abstract void ProcessClientCredentials(TlsCredentials clientCredentials);

		public virtual void ProcessClientCertificate(Certificate clientCertificate)
		{
		}

		public abstract void GenerateClientKeyExchange(Stream output);

		public virtual void ProcessClientKeyExchange(Stream input)
		{
			throw new TlsFatalAlert(80);
		}

		public abstract byte[] GeneratePremasterSecret();
	}
}
