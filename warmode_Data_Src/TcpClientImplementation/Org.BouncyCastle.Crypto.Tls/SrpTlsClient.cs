using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SrpTlsClient : AbstractTlsClient
	{
		protected TlsSrpGroupVerifier mGroupVerifier;

		protected byte[] mIdentity;

		protected byte[] mPassword;

		protected override List<string> HostNames
		{
			get;
			set;
		}

		protected virtual bool RequireSrpServerExtension
		{
			get
			{
				return false;
			}
		}

		public SrpTlsClient(byte[] identity, byte[] password) : this(new DefaultTlsCipherFactory(), new DefaultTlsSrpGroupVerifier(), identity, password)
		{
		}

		public SrpTlsClient(TlsCipherFactory cipherFactory, byte[] identity, byte[] password) : this(cipherFactory, new DefaultTlsSrpGroupVerifier(), identity, password)
		{
		}

		public SrpTlsClient(TlsCipherFactory cipherFactory, TlsSrpGroupVerifier groupVerifier, byte[] identity, byte[] password) : base(cipherFactory)
		{
			this.mGroupVerifier = groupVerifier;
			this.mIdentity = Arrays.Clone(identity);
			this.mPassword = Arrays.Clone(password);
		}

		public override int[] GetCipherSuites()
		{
			return new int[]
			{
				49182
			};
		}

		public override IDictionary GetClientExtensions()
		{
			IDictionary dictionary = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
			TlsSrpUtilities.AddSrpExtension(dictionary, this.mIdentity);
			return dictionary;
		}

		public override void ProcessServerExtensions(IDictionary serverExtensions)
		{
			if (!TlsUtilities.HasExpectedEmptyExtensionData(serverExtensions, 12, 47) && this.RequireSrpServerExtension)
			{
				throw new TlsFatalAlert(47);
			}
			base.ProcessServerExtensions(serverExtensions);
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

		public override TlsAuthentication GetAuthentication()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsKeyExchange CreateSrpKeyExchange(int keyExchange)
		{
			return new TlsSrpKeyExchange(keyExchange, this.mSupportedSignatureAlgorithms, this.mGroupVerifier, this.mIdentity, this.mPassword);
		}
	}
}
