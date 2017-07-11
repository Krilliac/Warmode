using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public sealed class SessionParameters
	{
		public sealed class Builder
		{
			private int mCipherSuite = -1;

			private short mCompressionAlgorithm = -1;

			private byte[] mMasterSecret;

			private Certificate mPeerCertificate;

			private byte[] mPskIdentity;

			private byte[] mSrpIdentity;

			private byte[] mEncodedServerExtensions;

			public SessionParameters Build()
			{
				this.Validate(this.mCipherSuite >= 0, "cipherSuite");
				this.Validate(this.mCompressionAlgorithm >= 0, "compressionAlgorithm");
				this.Validate(this.mMasterSecret != null, "masterSecret");
				return new SessionParameters(this.mCipherSuite, (byte)this.mCompressionAlgorithm, this.mMasterSecret, this.mPeerCertificate, this.mPskIdentity, this.mSrpIdentity, this.mEncodedServerExtensions);
			}

			public SessionParameters.Builder SetCipherSuite(int cipherSuite)
			{
				this.mCipherSuite = cipherSuite;
				return this;
			}

			public SessionParameters.Builder SetCompressionAlgorithm(byte compressionAlgorithm)
			{
				this.mCompressionAlgorithm = (short)compressionAlgorithm;
				return this;
			}

			public SessionParameters.Builder SetMasterSecret(byte[] masterSecret)
			{
				this.mMasterSecret = masterSecret;
				return this;
			}

			public SessionParameters.Builder SetPeerCertificate(Certificate peerCertificate)
			{
				this.mPeerCertificate = peerCertificate;
				return this;
			}

			public SessionParameters.Builder SetPskIdentity(byte[] pskIdentity)
			{
				this.mPskIdentity = pskIdentity;
				return this;
			}

			public SessionParameters.Builder SetSrpIdentity(byte[] srpIdentity)
			{
				this.mSrpIdentity = srpIdentity;
				return this;
			}

			public SessionParameters.Builder SetServerExtensions(IDictionary serverExtensions)
			{
				if (serverExtensions == null)
				{
					this.mEncodedServerExtensions = null;
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					TlsProtocol.WriteExtensions(memoryStream, serverExtensions);
					this.mEncodedServerExtensions = memoryStream.ToArray();
				}
				return this;
			}

			private void Validate(bool condition, string parameter)
			{
				if (!condition)
				{
					throw new InvalidOperationException("Required session parameter '" + parameter + "' not configured");
				}
			}
		}

		private int mCipherSuite;

		private byte mCompressionAlgorithm;

		private byte[] mMasterSecret;

		private Certificate mPeerCertificate;

		private byte[] mPskIdentity;

		private byte[] mSrpIdentity;

		private byte[] mEncodedServerExtensions;

		public int CipherSuite
		{
			get
			{
				return this.mCipherSuite;
			}
		}

		public byte CompressionAlgorithm
		{
			get
			{
				return this.mCompressionAlgorithm;
			}
		}

		public byte[] MasterSecret
		{
			get
			{
				return this.mMasterSecret;
			}
		}

		public Certificate PeerCertificate
		{
			get
			{
				return this.mPeerCertificate;
			}
		}

		public byte[] PskIdentity
		{
			get
			{
				return this.mPskIdentity;
			}
		}

		public byte[] SrpIdentity
		{
			get
			{
				return this.mSrpIdentity;
			}
		}

		private SessionParameters(int cipherSuite, byte compressionAlgorithm, byte[] masterSecret, Certificate peerCertificate, byte[] pskIdentity, byte[] srpIdentity, byte[] encodedServerExtensions)
		{
			this.mCipherSuite = cipherSuite;
			this.mCompressionAlgorithm = compressionAlgorithm;
			this.mMasterSecret = Arrays.Clone(masterSecret);
			this.mPeerCertificate = peerCertificate;
			this.mPskIdentity = Arrays.Clone(pskIdentity);
			this.mSrpIdentity = Arrays.Clone(srpIdentity);
			this.mEncodedServerExtensions = encodedServerExtensions;
		}

		public void Clear()
		{
			if (this.mMasterSecret != null)
			{
				Arrays.Fill(this.mMasterSecret, 0);
			}
		}

		public SessionParameters Copy()
		{
			return new SessionParameters(this.mCipherSuite, this.mCompressionAlgorithm, this.mMasterSecret, this.mPeerCertificate, this.mPskIdentity, this.mSrpIdentity, this.mEncodedServerExtensions);
		}

		public IDictionary ReadServerExtensions()
		{
			if (this.mEncodedServerExtensions == null)
			{
				return null;
			}
			MemoryStream input = new MemoryStream(this.mEncodedServerExtensions, false);
			return TlsProtocol.ReadExtensions(input);
		}
	}
}
