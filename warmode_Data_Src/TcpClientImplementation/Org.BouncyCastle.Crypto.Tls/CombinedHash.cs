using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class CombinedHash : TlsHandshakeHash, IDigest
	{
		protected TlsContext mContext;

		protected IDigest mMd5;

		protected IDigest mSha1;

		public virtual string AlgorithmName
		{
			get
			{
				return this.mMd5.AlgorithmName + " and " + this.mSha1.AlgorithmName;
			}
		}

		internal CombinedHash()
		{
			this.mMd5 = TlsUtilities.CreateHash(1);
			this.mSha1 = TlsUtilities.CreateHash(2);
		}

		internal CombinedHash(CombinedHash t)
		{
			this.mContext = t.mContext;
			this.mMd5 = TlsUtilities.CloneHash(1, t.mMd5);
			this.mSha1 = TlsUtilities.CloneHash(2, t.mSha1);
		}

		public virtual void Init(TlsContext context)
		{
			this.mContext = context;
		}

		public virtual TlsHandshakeHash NotifyPrfDetermined()
		{
			return this;
		}

		public virtual void TrackHashAlgorithm(byte hashAlgorithm)
		{
			throw new InvalidOperationException("CombinedHash only supports calculating the legacy PRF for handshake hash");
		}

		public virtual void SealHashAlgorithms()
		{
		}

		public virtual TlsHandshakeHash StopTracking()
		{
			return new CombinedHash(this);
		}

		public virtual IDigest ForkPrfHash()
		{
			return new CombinedHash(this);
		}

		public virtual byte[] GetFinalHash(byte hashAlgorithm)
		{
			throw new InvalidOperationException("CombinedHash doesn't support multiple hashes");
		}

		public virtual int GetByteLength()
		{
			return Math.Max(this.mMd5.GetByteLength(), this.mSha1.GetByteLength());
		}

		public virtual int GetDigestSize()
		{
			return this.mMd5.GetDigestSize() + this.mSha1.GetDigestSize();
		}

		public virtual void Update(byte input)
		{
			this.mMd5.Update(input);
			this.mSha1.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			this.mMd5.BlockUpdate(input, inOff, len);
			this.mSha1.BlockUpdate(input, inOff, len);
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			if (this.mContext != null && TlsUtilities.IsSsl(this.mContext))
			{
				this.Ssl3Complete(this.mMd5, Ssl3Mac.IPAD, Ssl3Mac.OPAD, 48);
				this.Ssl3Complete(this.mSha1, Ssl3Mac.IPAD, Ssl3Mac.OPAD, 40);
			}
			int num = this.mMd5.DoFinal(output, outOff);
			int num2 = this.mSha1.DoFinal(output, outOff + num);
			return num + num2;
		}

		public virtual void Reset()
		{
			this.mMd5.Reset();
			this.mSha1.Reset();
		}

		protected virtual void Ssl3Complete(IDigest d, byte[] ipad, byte[] opad, int padLength)
		{
			byte[] masterSecret = this.mContext.SecurityParameters.masterSecret;
			d.BlockUpdate(masterSecret, 0, masterSecret.Length);
			d.BlockUpdate(ipad, 0, padLength);
			byte[] array = DigestUtilities.DoFinal(d);
			d.BlockUpdate(masterSecret, 0, masterSecret.Length);
			d.BlockUpdate(opad, 0, padLength);
			d.BlockUpdate(array, 0, array.Length);
		}
	}
}
