using System;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class PskTlsClient : AbstractTlsClient
	{
		protected TlsPskIdentity mPskIdentity;

		protected override List<string> HostNames
		{
			get;
			set;
		}

		public PskTlsClient(TlsPskIdentity pskIdentity) : this(new DefaultTlsCipherFactory(), pskIdentity)
		{
		}

		public PskTlsClient(TlsCipherFactory cipherFactory, TlsPskIdentity pskIdentity) : base(cipherFactory)
		{
			this.mPskIdentity = pskIdentity;
		}

		public override int[] GetCipherSuites()
		{
			return new int[]
			{
				49207,
				49205,
				178,
				144
			};
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

		public override TlsAuthentication GetAuthentication()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
		{
			return new TlsPskKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.mPskIdentity, null, null, this.mNamedCurves, this.mClientECPointFormats, this.mServerECPointFormats);
		}
	}
}
