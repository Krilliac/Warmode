using System;
using System.Collections;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AbstractTlsClient : AbstractTlsPeer, TlsClient, TlsPeer
	{
		protected TlsCipherFactory mCipherFactory;

		protected TlsClientContext mContext;

		protected IList mSupportedSignatureAlgorithms;

		protected int[] mNamedCurves;

		protected byte[] mClientECPointFormats;

		protected byte[] mServerECPointFormats;

		protected int mSelectedCipherSuite;

		protected short mSelectedCompressionMethod;

		protected abstract List<string> HostNames
		{
			get;
			set;
		}

		public virtual ProtocolVersion ClientHelloRecordLayerVersion
		{
			get
			{
				return this.ClientVersion;
			}
		}

		public virtual ProtocolVersion ClientVersion
		{
			get
			{
				return ProtocolVersion.TLSv12;
			}
		}

		public virtual bool IsFallback
		{
			get
			{
				return false;
			}
		}

		public virtual ProtocolVersion MinimumVersion
		{
			get
			{
				return ProtocolVersion.TLSv10;
			}
		}

		public AbstractTlsClient() : this(new DefaultTlsCipherFactory())
		{
		}

		public AbstractTlsClient(TlsCipherFactory cipherFactory)
		{
			this.mCipherFactory = cipherFactory;
		}

		protected virtual bool AllowUnexpectedServerExtension(int extensionType, byte[] extensionData)
		{
			if (extensionType == 10)
			{
				TlsEccUtilities.ReadSupportedEllipticCurvesExtension(extensionData);
				return true;
			}
			return false;
		}

		protected virtual void CheckForUnexpectedServerExtension(IDictionary serverExtensions, int extensionType)
		{
			byte[] extensionData = TlsUtilities.GetExtensionData(serverExtensions, extensionType);
			if (extensionData != null && !this.AllowUnexpectedServerExtension(extensionType, extensionData))
			{
				throw new TlsFatalAlert(47);
			}
		}

		public virtual void Init(TlsClientContext context)
		{
			this.mContext = context;
		}

		public virtual TlsSession GetSessionToResume()
		{
			return null;
		}

		public virtual IDictionary GetClientExtensions()
		{
			IDictionary dictionary = null;
			ProtocolVersion clientVersion = this.mContext.ClientVersion;
			if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(clientVersion))
			{
				this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultSupportedSignatureAlgorithms();
				dictionary = TlsExtensionsUtilities.EnsureExtensionsInitialised(dictionary);
				TlsUtilities.AddSignatureAlgorithmsExtension(dictionary, this.mSupportedSignatureAlgorithms);
			}
			if (TlsEccUtilities.ContainsEccCipherSuites(this.GetCipherSuites()))
			{
				this.mNamedCurves = new int[]
				{
					23,
					24
				};
				this.mClientECPointFormats = new byte[]
				{
					0,
					1,
					2
				};
				dictionary = TlsExtensionsUtilities.EnsureExtensionsInitialised(dictionary);
				TlsEccUtilities.AddSupportedEllipticCurvesExtension(dictionary, this.mNamedCurves);
				TlsEccUtilities.AddSupportedPointFormatsExtension(dictionary, this.mClientECPointFormats);
			}
			if (this.HostNames != null && this.HostNames.Count > 0)
			{
				List<ServerName> list = new List<ServerName>(this.HostNames.Count);
				for (int i = 0; i < this.HostNames.Count; i++)
				{
					list.Add(new ServerName(0, this.HostNames[i]));
				}
				TlsExtensionsUtilities.AddServerNameExtension(dictionary, new ServerNameList(list));
			}
			return dictionary;
		}

		public virtual void NotifyServerVersion(ProtocolVersion serverVersion)
		{
			if (!this.MinimumVersion.IsEqualOrEarlierVersionOf(serverVersion))
			{
				throw new TlsFatalAlert(70);
			}
		}

		public abstract int[] GetCipherSuites();

		public virtual byte[] GetCompressionMethods()
		{
			return new byte[1];
		}

		public virtual void NotifySessionID(byte[] sessionID)
		{
		}

		public virtual void NotifySelectedCipherSuite(int selectedCipherSuite)
		{
			this.mSelectedCipherSuite = selectedCipherSuite;
		}

		public virtual void NotifySelectedCompressionMethod(byte selectedCompressionMethod)
		{
			this.mSelectedCompressionMethod = (short)selectedCompressionMethod;
		}

		public virtual void ProcessServerExtensions(IDictionary serverExtensions)
		{
			if (serverExtensions != null)
			{
				this.CheckForUnexpectedServerExtension(serverExtensions, 13);
				this.CheckForUnexpectedServerExtension(serverExtensions, 10);
				if (TlsEccUtilities.IsEccCipherSuite(this.mSelectedCipherSuite))
				{
					this.mServerECPointFormats = TlsEccUtilities.GetSupportedPointFormatsExtension(serverExtensions);
					return;
				}
				this.CheckForUnexpectedServerExtension(serverExtensions, 11);
			}
		}

		public virtual void ProcessServerSupplementalData(IList serverSupplementalData)
		{
			if (serverSupplementalData != null)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public abstract TlsKeyExchange GetKeyExchange();

		public abstract TlsAuthentication GetAuthentication();

		public virtual IList GetClientSupplementalData()
		{
			return null;
		}

		public override TlsCompression GetCompression()
		{
			switch (this.mSelectedCompressionMethod)
			{
			case 0:
				return new TlsNullCompression();
			case 1:
				return new TlsDeflateCompression();
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public override TlsCipher GetCipher()
		{
			int encryptionAlgorithm = TlsUtilities.GetEncryptionAlgorithm(this.mSelectedCipherSuite);
			int macAlgorithm = TlsUtilities.GetMacAlgorithm(this.mSelectedCipherSuite);
			return this.mCipherFactory.CreateCipher(this.mContext, encryptionAlgorithm, macAlgorithm);
		}

		public virtual void NotifyNewSessionTicket(NewSessionTicket newSessionTicket)
		{
		}
	}
}
