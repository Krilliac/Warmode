using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class PskTlsServer : AbstractTlsServer
	{
		protected TlsPskIdentityManager mPskIdentityManager;

		public PskTlsServer(TlsPskIdentityManager pskIdentityManager) : this(new DefaultTlsCipherFactory(), pskIdentityManager)
		{
		}

		public PskTlsServer(TlsCipherFactory cipherFactory, TlsPskIdentityManager pskIdentityManager) : base(cipherFactory)
		{
			this.mPskIdentityManager = pskIdentityManager;
		}

		protected virtual TlsEncryptionCredentials GetRsaEncryptionCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual DHParameters GetDHParameters()
		{
			return DHStandardGroups.rfc5114_1024_160;
		}

		protected override int[] GetCipherSuites()
		{
			return new int[]
			{
				49207,
				49205,
				178,
				144
			};
		}

		public override TlsCredentials GetCredentials()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite);
			int num = keyExchangeAlgorithm;
			switch (num)
			{
			case 13:
			case 14:
				break;
			case 15:
				return this.GetRsaEncryptionCredentials();
			default:
				if (num != 24)
				{
					throw new TlsFatalAlert(80);
				}
				break;
			}
			return null;
		}

		public override TlsKeyExchange GetKeyExchange()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite);
			int num = keyExchangeAlgorithm;
			switch (num)
			{
			case 13:
			case 14:
			case 15:
				break;
			default:
				if (num != 24)
				{
					throw new TlsFatalAlert(80);
				}
				break;
			}
			return this.CreatePskKeyExchange(keyExchangeAlgorithm);
		}

		protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
		{
			return new TlsPskKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, null, this.mPskIdentityManager, this.GetDHParameters(), this.mNamedCurves, this.mClientECPointFormats, this.mServerECPointFormats);
		}
	}
}
