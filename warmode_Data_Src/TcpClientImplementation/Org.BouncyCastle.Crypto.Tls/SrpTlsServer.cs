using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SrpTlsServer : AbstractTlsServer
	{
		protected TlsSrpIdentityManager mSrpIdentityManager;

		protected byte[] mSrpIdentity;

		protected TlsSrpLoginParameters mLoginParameters;

		public SrpTlsServer(TlsSrpIdentityManager srpIdentityManager) : this(new DefaultTlsCipherFactory(), srpIdentityManager)
		{
		}

		public SrpTlsServer(TlsCipherFactory cipherFactory, TlsSrpIdentityManager srpIdentityManager) : base(cipherFactory)
		{
			this.mSrpIdentityManager = srpIdentityManager;
		}

		protected virtual TlsSignerCredentials GetDsaSignerCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsSignerCredentials GetRsaSignerCredentials()
		{
			throw new TlsFatalAlert(80);
		}

		protected override int[] GetCipherSuites()
		{
			return new int[]
			{
				49186,
				49183,
				49185,
				49182,
				49184,
				49181
			};
		}

		public override void ProcessClientExtensions(IDictionary clientExtensions)
		{
			base.ProcessClientExtensions(clientExtensions);
			this.mSrpIdentity = TlsSrpUtilities.GetSrpExtension(clientExtensions);
		}

		public override int GetSelectedCipherSuite()
		{
			int selectedCipherSuite = base.GetSelectedCipherSuite();
			if (TlsSrpUtilities.IsSrpCipherSuite(selectedCipherSuite))
			{
				if (this.mSrpIdentity != null)
				{
					this.mLoginParameters = this.mSrpIdentityManager.GetLoginParameters(this.mSrpIdentity);
				}
				if (this.mLoginParameters == null)
				{
					throw new TlsFatalAlert(115);
				}
			}
			return selectedCipherSuite;
		}

		public override TlsCredentials GetCredentials()
		{
			switch (TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite))
			{
			case 21:
				return null;
			case 22:
				return this.GetDsaSignerCredentials();
			case 23:
				return this.GetRsaSignerCredentials();
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public override TlsKeyExchange GetKeyExchange()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(this.mSelectedCipherSuite);
			switch (keyExchangeAlgorithm)
			{
			case 21:
			case 22:
			case 23:
				return this.CreateSrpKeyExchange(keyExchangeAlgorithm);
			default:
				throw new TlsFatalAlert(80);
			}
		}

		protected virtual TlsKeyExchange CreateSrpKeyExchange(int keyExchange)
		{
			return new TlsSrpKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.mSrpIdentity, this.mLoginParameters);
		}
	}
}
