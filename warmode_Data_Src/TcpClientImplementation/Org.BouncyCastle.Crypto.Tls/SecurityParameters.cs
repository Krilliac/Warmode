using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SecurityParameters
	{
		internal int entity = -1;

		internal int cipherSuite = -1;

		internal byte compressionAlgorithm;

		internal int prfAlgorithm = -1;

		internal int verifyDataLength = -1;

		internal byte[] masterSecret;

		internal byte[] clientRandom;

		internal byte[] serverRandom;

		internal byte[] sessionHash;

		internal byte[] pskIdentity;

		internal byte[] srpIdentity;

		internal short maxFragmentLength = -1;

		internal bool truncatedHMac;

		internal bool encryptThenMac;

		internal bool extendedMasterSecret;

		public virtual int Entity
		{
			get
			{
				return this.entity;
			}
		}

		public virtual int CipherSuite
		{
			get
			{
				return this.cipherSuite;
			}
		}

		public byte CompressionAlgorithm
		{
			get
			{
				return this.compressionAlgorithm;
			}
		}

		public virtual int PrfAlgorithm
		{
			get
			{
				return this.prfAlgorithm;
			}
		}

		public virtual int VerifyDataLength
		{
			get
			{
				return this.verifyDataLength;
			}
		}

		public virtual byte[] MasterSecret
		{
			get
			{
				return this.masterSecret;
			}
		}

		public virtual byte[] ClientRandom
		{
			get
			{
				return this.clientRandom;
			}
		}

		public virtual byte[] ServerRandom
		{
			get
			{
				return this.serverRandom;
			}
		}

		public virtual byte[] SessionHash
		{
			get
			{
				return this.sessionHash;
			}
		}

		public virtual byte[] PskIdentity
		{
			get
			{
				return this.pskIdentity;
			}
		}

		public virtual byte[] SrpIdentity
		{
			get
			{
				return this.srpIdentity;
			}
		}

		internal virtual void Clear()
		{
			if (this.masterSecret != null)
			{
				Arrays.Fill(this.masterSecret, 0);
				this.masterSecret = null;
			}
		}
	}
}
