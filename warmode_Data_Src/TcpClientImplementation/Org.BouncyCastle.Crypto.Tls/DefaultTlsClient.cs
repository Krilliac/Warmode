using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class DefaultTlsClient : AbstractTlsClient
	{
		public DefaultTlsClient()
		{
		}

		public DefaultTlsClient(TlsCipherFactory cipherFactory) : base(cipherFactory)
		{
		}

		public override int[] GetCipherSuites()
		{
			return new int[]
			{
				49195,
				49187,
				49161,
				49199,
				49191,
				49171,
				162,
				64,
				50,
				158,
				103,
				51,
				156,
				60,
				47
			};
		}

		public override TlsKeyExchange GetKeyExchange()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite);
			int num = keyExchangeAlgorithm;
			switch (num)
			{
			case 1:
				return this.CreateRsaKeyExchange();
			case 2:
			case 4:
			case 6:
			case 8:
				break;
			case 3:
			case 5:
				return this.CreateDheKeyExchange(keyExchangeAlgorithm);
			case 7:
			case 9:
				return this.CreateDHKeyExchange(keyExchangeAlgorithm);
			default:
				switch (num)
				{
				case 16:
				case 18:
					return this.CreateECDHKeyExchange(keyExchangeAlgorithm);
				case 17:
				case 19:
					return this.CreateECDheKeyExchange(keyExchangeAlgorithm);
				}
				break;
			}
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsKeyExchange CreateDHKeyExchange(int keyExchange)
		{
			return new TlsDHKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, null);
		}

		protected virtual TlsKeyExchange CreateDheKeyExchange(int keyExchange)
		{
			return new TlsDheKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, null);
		}

		protected virtual TlsKeyExchange CreateECDHKeyExchange(int keyExchange)
		{
			return new TlsECDHKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.mNamedCurves, this.mClientECPointFormats, this.mServerECPointFormats);
		}

		protected virtual TlsKeyExchange CreateECDheKeyExchange(int keyExchange)
		{
			return new TlsECDheKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.mNamedCurves, this.mClientECPointFormats, this.mServerECPointFormats);
		}

		protected virtual TlsKeyExchange CreateRsaKeyExchange()
		{
			return new TlsRsaKeyExchange(this.mSupportedSignatureAlgorithms);
		}
	}
}
