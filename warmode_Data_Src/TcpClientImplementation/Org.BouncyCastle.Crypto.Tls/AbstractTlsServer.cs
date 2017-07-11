using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AbstractTlsServer : AbstractTlsPeer, TlsServer, TlsPeer
	{
		protected TlsCipherFactory mCipherFactory;

		protected TlsServerContext mContext;

		protected ProtocolVersion mClientVersion;

		protected int[] mOfferedCipherSuites;

		protected byte[] mOfferedCompressionMethods;

		protected IDictionary mClientExtensions;

		protected bool mEncryptThenMacOffered;

		protected short mMaxFragmentLengthOffered;

		protected bool mTruncatedHMacOffered;

		protected IList mSupportedSignatureAlgorithms;

		protected bool mEccCipherSuitesOffered;

		protected int[] mNamedCurves;

		protected byte[] mClientECPointFormats;

		protected byte[] mServerECPointFormats;

		protected ProtocolVersion mServerVersion;

		protected int mSelectedCipherSuite;

		protected byte mSelectedCompressionMethod;

		protected IDictionary mServerExtensions;

		protected virtual bool AllowEncryptThenMac
		{
			get
			{
				return true;
			}
		}

		protected virtual bool AllowTruncatedHMac
		{
			get
			{
				return false;
			}
		}

		protected virtual ProtocolVersion MaximumVersion
		{
			get
			{
				return ProtocolVersion.TLSv11;
			}
		}

		protected virtual ProtocolVersion MinimumVersion
		{
			get
			{
				return ProtocolVersion.TLSv10;
			}
		}

		public AbstractTlsServer() : this(new DefaultTlsCipherFactory())
		{
		}

		public AbstractTlsServer(TlsCipherFactory cipherFactory)
		{
			this.mCipherFactory = cipherFactory;
		}

		protected virtual IDictionary CheckServerExtensions()
		{
			return this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(this.mServerExtensions);
		}

		protected abstract int[] GetCipherSuites();

		protected byte[] GetCompressionMethods()
		{
			return new byte[1];
		}

		protected virtual bool SupportsClientEccCapabilities(int[] namedCurves, byte[] ecPointFormats)
		{
			if (namedCurves == null)
			{
				return TlsEccUtilities.HasAnySupportedNamedCurves();
			}
			for (int i = 0; i < namedCurves.Length; i++)
			{
				int namedCurve = namedCurves[i];
				if (NamedCurve.IsValid(namedCurve) && (!NamedCurve.RefersToASpecificNamedCurve(namedCurve) || TlsEccUtilities.IsSupportedNamedCurve(namedCurve)))
				{
					return true;
				}
			}
			return false;
		}

		public virtual void Init(TlsServerContext context)
		{
			this.mContext = context;
		}

		public virtual void NotifyClientVersion(ProtocolVersion clientVersion)
		{
			this.mClientVersion = clientVersion;
		}

		public virtual void NotifyFallback(bool isFallback)
		{
			if (isFallback && this.MaximumVersion.IsLaterVersionOf(this.mClientVersion))
			{
				throw new TlsFatalAlert(86);
			}
		}

		public virtual void NotifyOfferedCipherSuites(int[] offeredCipherSuites)
		{
			this.mOfferedCipherSuites = offeredCipherSuites;
			this.mEccCipherSuitesOffered = TlsEccUtilities.ContainsEccCipherSuites(this.mOfferedCipherSuites);
		}

		public virtual void NotifyOfferedCompressionMethods(byte[] offeredCompressionMethods)
		{
			this.mOfferedCompressionMethods = offeredCompressionMethods;
		}

		public virtual void ProcessClientExtensions(IDictionary clientExtensions)
		{
			this.mClientExtensions = clientExtensions;
			if (clientExtensions != null)
			{
				this.mEncryptThenMacOffered = TlsExtensionsUtilities.HasEncryptThenMacExtension(clientExtensions);
				this.mMaxFragmentLengthOffered = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions);
				if (this.mMaxFragmentLengthOffered >= 0 && !MaxFragmentLength.IsValid((byte)this.mMaxFragmentLengthOffered))
				{
					throw new TlsFatalAlert(47);
				}
				this.mTruncatedHMacOffered = TlsExtensionsUtilities.HasTruncatedHMacExtension(clientExtensions);
				this.mSupportedSignatureAlgorithms = TlsUtilities.GetSignatureAlgorithmsExtension(clientExtensions);
				if (this.mSupportedSignatureAlgorithms != null && !TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(this.mClientVersion))
				{
					throw new TlsFatalAlert(47);
				}
				this.mNamedCurves = TlsEccUtilities.GetSupportedEllipticCurvesExtension(clientExtensions);
				this.mClientECPointFormats = TlsEccUtilities.GetSupportedPointFormatsExtension(clientExtensions);
			}
			if (!this.mEccCipherSuitesOffered && (this.mNamedCurves != null || this.mClientECPointFormats != null))
			{
				throw new TlsFatalAlert(47);
			}
		}

		public virtual ProtocolVersion GetServerVersion()
		{
			if (this.MinimumVersion.IsEqualOrEarlierVersionOf(this.mClientVersion))
			{
				ProtocolVersion maximumVersion = this.MaximumVersion;
				if (this.mClientVersion.IsEqualOrEarlierVersionOf(maximumVersion))
				{
					return this.mServerVersion = this.mClientVersion;
				}
				if (this.mClientVersion.IsLaterVersionOf(maximumVersion))
				{
					return this.mServerVersion = maximumVersion;
				}
			}
			throw new TlsFatalAlert(70);
		}

		public virtual int GetSelectedCipherSuite()
		{
			bool flag = this.SupportsClientEccCapabilities(this.mNamedCurves, this.mClientECPointFormats);
			int[] cipherSuites = this.GetCipherSuites();
			for (int i = 0; i < cipherSuites.Length; i++)
			{
				int num = cipherSuites[i];
				if (Arrays.Contains(this.mOfferedCipherSuites, num) && (flag || !TlsEccUtilities.IsEccCipherSuite(num)) && TlsUtilities.IsValidCipherSuiteForVersion(num, this.mServerVersion))
				{
					return this.mSelectedCipherSuite = num;
				}
			}
			throw new TlsFatalAlert(40);
		}

		public virtual byte GetSelectedCompressionMethod()
		{
			byte[] compressionMethods = this.GetCompressionMethods();
			for (int i = 0; i < compressionMethods.Length; i++)
			{
				if (Arrays.Contains(this.mOfferedCompressionMethods, compressionMethods[i]))
				{
					return this.mSelectedCompressionMethod = compressionMethods[i];
				}
			}
			throw new TlsFatalAlert(40);
		}

		public virtual IDictionary GetServerExtensions()
		{
			if (this.mEncryptThenMacOffered && this.AllowEncryptThenMac && TlsUtilities.IsBlockCipherSuite(this.mSelectedCipherSuite))
			{
				TlsExtensionsUtilities.AddEncryptThenMacExtension(this.CheckServerExtensions());
			}
			if (this.mMaxFragmentLengthOffered >= 0 && TlsUtilities.IsValidUint8((int)this.mMaxFragmentLengthOffered) && MaxFragmentLength.IsValid((byte)this.mMaxFragmentLengthOffered))
			{
				TlsExtensionsUtilities.AddMaxFragmentLengthExtension(this.CheckServerExtensions(), (byte)this.mMaxFragmentLengthOffered);
			}
			if (this.mTruncatedHMacOffered && this.AllowTruncatedHMac)
			{
				TlsExtensionsUtilities.AddTruncatedHMacExtension(this.CheckServerExtensions());
			}
			if (this.mClientECPointFormats != null && TlsEccUtilities.IsEccCipherSuite(this.mSelectedCipherSuite))
			{
				this.mServerECPointFormats = new byte[]
				{
					0,
					1,
					2
				};
				TlsEccUtilities.AddSupportedPointFormatsExtension(this.CheckServerExtensions(), this.mServerECPointFormats);
			}
			return this.mServerExtensions;
		}

		public virtual IList GetServerSupplementalData()
		{
			return null;
		}

		public abstract TlsCredentials GetCredentials();

		public virtual CertificateStatus GetCertificateStatus()
		{
			return null;
		}

		public abstract TlsKeyExchange GetKeyExchange();

		public virtual CertificateRequest GetCertificateRequest()
		{
			return null;
		}

		public virtual void ProcessClientSupplementalData(IList clientSupplementalData)
		{
			if (clientSupplementalData != null)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public virtual void NotifyClientCertificate(Certificate clientCertificate)
		{
			throw new TlsFatalAlert(80);
		}

		public override TlsCompression GetCompression()
		{
			byte b = this.mSelectedCompressionMethod;
			if (b == 0)
			{
				return new TlsNullCompression();
			}
			throw new TlsFatalAlert(80);
		}

		public override TlsCipher GetCipher()
		{
			int encryptionAlgorithm = TlsUtilities.GetEncryptionAlgorithm(this.mSelectedCipherSuite);
			int macAlgorithm = TlsUtilities.GetMacAlgorithm(this.mSelectedCipherSuite);
			return this.mCipherFactory.CreateCipher(this.mContext, encryptionAlgorithm, macAlgorithm);
		}

		public virtual NewSessionTicket GetNewSessionTicket()
		{
			return new NewSessionTicket(0L, TlsUtilities.EmptyBytes);
		}
	}
}
