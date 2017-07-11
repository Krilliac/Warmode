using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DeferredHash : TlsHandshakeHash, IDigest
	{
		protected const int BUFFERING_HASH_LIMIT = 4;

		protected TlsContext mContext;

		private DigestInputBuffer mBuf;

		private IDictionary mHashes;

		private int mPrfHashAlgorithm;

		public virtual string AlgorithmName
		{
			get
			{
				throw new InvalidOperationException("Use Fork() to get a definite IDigest");
			}
		}

		internal DeferredHash()
		{
			this.mBuf = new DigestInputBuffer();
			this.mHashes = Platform.CreateHashtable();
			this.mPrfHashAlgorithm = -1;
		}

		private DeferredHash(byte prfHashAlgorithm, IDigest prfHash)
		{
			this.mBuf = null;
			this.mHashes = Platform.CreateHashtable();
			this.mPrfHashAlgorithm = (int)prfHashAlgorithm;
			this.mHashes[prfHashAlgorithm] = prfHash;
		}

		public virtual void Init(TlsContext context)
		{
			this.mContext = context;
		}

		public virtual TlsHandshakeHash NotifyPrfDetermined()
		{
			int prfAlgorithm = this.mContext.SecurityParameters.PrfAlgorithm;
			if (prfAlgorithm == 0)
			{
				CombinedHash combinedHash = new CombinedHash();
				combinedHash.Init(this.mContext);
				this.mBuf.UpdateDigest(combinedHash);
				return combinedHash.NotifyPrfDetermined();
			}
			this.mPrfHashAlgorithm = (int)TlsUtilities.GetHashAlgorithmForPrfAlgorithm(prfAlgorithm);
			this.CheckTrackingHash((byte)this.mPrfHashAlgorithm);
			return this;
		}

		public virtual void TrackHashAlgorithm(byte hashAlgorithm)
		{
			if (this.mBuf == null)
			{
				throw new InvalidOperationException("Too late to track more hash algorithms");
			}
			this.CheckTrackingHash(hashAlgorithm);
		}

		public virtual void SealHashAlgorithms()
		{
			this.CheckStopBuffering();
		}

		public virtual TlsHandshakeHash StopTracking()
		{
			byte b = (byte)this.mPrfHashAlgorithm;
			IDigest digest = TlsUtilities.CloneHash(b, (IDigest)this.mHashes[b]);
			if (this.mBuf != null)
			{
				this.mBuf.UpdateDigest(digest);
			}
			DeferredHash deferredHash = new DeferredHash(b, digest);
			deferredHash.Init(this.mContext);
			return deferredHash;
		}

		public virtual IDigest ForkPrfHash()
		{
			this.CheckStopBuffering();
			byte b = (byte)this.mPrfHashAlgorithm;
			if (this.mBuf != null)
			{
				IDigest digest = TlsUtilities.CreateHash(b);
				this.mBuf.UpdateDigest(digest);
				return digest;
			}
			return TlsUtilities.CloneHash(b, (IDigest)this.mHashes[b]);
		}

		public virtual byte[] GetFinalHash(byte hashAlgorithm)
		{
			IDigest digest = (IDigest)this.mHashes[hashAlgorithm];
			if (digest == null)
			{
				throw new InvalidOperationException("HashAlgorithm " + hashAlgorithm + " is not being tracked");
			}
			digest = TlsUtilities.CloneHash(hashAlgorithm, digest);
			if (this.mBuf != null)
			{
				this.mBuf.UpdateDigest(digest);
			}
			return DigestUtilities.DoFinal(digest);
		}

		public virtual int GetByteLength()
		{
			throw new InvalidOperationException("Use Fork() to get a definite IDigest");
		}

		public virtual int GetDigestSize()
		{
			throw new InvalidOperationException("Use Fork() to get a definite IDigest");
		}

		public virtual void Update(byte input)
		{
			if (this.mBuf != null)
			{
				this.mBuf.WriteByte(input);
				return;
			}
			foreach (IDigest digest in this.mHashes.Values)
			{
				digest.Update(input);
			}
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			if (this.mBuf != null)
			{
				this.mBuf.Write(input, inOff, len);
				return;
			}
			foreach (IDigest digest in this.mHashes.Values)
			{
				digest.BlockUpdate(input, inOff, len);
			}
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			throw new InvalidOperationException("Use Fork() to get a definite IDigest");
		}

		public virtual void Reset()
		{
			if (this.mBuf != null)
			{
				this.mBuf.SetLength(0L);
				return;
			}
			foreach (IDigest digest in this.mHashes.Values)
			{
				digest.Reset();
			}
		}

		protected virtual void CheckStopBuffering()
		{
			if (this.mBuf != null && this.mHashes.Count <= 4)
			{
				foreach (IDigest d in this.mHashes.Values)
				{
					this.mBuf.UpdateDigest(d);
				}
				this.mBuf = null;
			}
		}

		protected virtual void CheckTrackingHash(byte hashAlgorithm)
		{
			if (!this.mHashes.Contains(hashAlgorithm))
			{
				IDigest value = TlsUtilities.CreateHash(hashAlgorithm);
				this.mHashes[hashAlgorithm] = value;
			}
		}
	}
}
