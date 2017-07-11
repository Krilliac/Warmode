using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class DefaultTlsServer : AbstractTlsServer
	{
		public DefaultTlsServer()
		{
		}

		public DefaultTlsServer(TlsCipherFactory cipherFactory) : base(cipherFactory)
		{
		}

		protected virtual TlsSignerCredentials GetDsaSignerCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsSignerCredentials GetECDsaSignerCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsEncryptionCredentials GetRsaEncryptionCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsSignerCredentials GetRsaSignerCredentials()
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
				49200,
				49199,
				49192,
				49191,
				49172,
				49171,
				159,
				158,
				107,
				103,
				57,
				51,
				157,
				156,
				61,
				60,
				53,
				47
			};
		}

		public override TlsCredentials GetCredentials()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite);
			int num = keyExchangeAlgorithm;
			switch (num)
			{
			case 1:
				return this.GetRsaEncryptionCredentials();
			case 2:
			case 4:
			case 6:
				goto IL_69;
			case 3:
			case 7:
				return this.GetDsaSignerCredentials();
			case 5:
				break;
			default:
				switch (num)
				{
				case 16:
				case 17:
					return this.GetECDsaSignerCredentials();
				case 18:
					goto IL_69;
				case 19:
					break;
				default:
					goto IL_69;
				}
				break;
			}
			return this.GetRsaSignerCredentials();
			IL_69:
			throw new TlsFatalAlert(80);
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
			return new TlsDHKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.GetDHParameters());
		}

		protected virtual TlsKeyExchange CreateDheKeyExchange(int keyExchange)
		{
			return new TlsDheKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.GetDHParameters());
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
