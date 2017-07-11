using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class SipHash : IMac
	{
		protected readonly int c;

		protected readonly int d;

		protected long k0;

		protected long k1;

		protected long v0;

		protected long v1;

		protected long v2;

		protected long v3;

		protected long m;

		protected int wordPos;

		protected int wordCount;

		public virtual string AlgorithmName
		{
			get
			{
				return string.Concat(new object[]
				{
					"SipHash-",
					this.c,
					"-",
					this.d
				});
			}
		}

		public SipHash() : this(2, 4)
		{
		}

		public SipHash(int c, int d)
		{
			this.c = c;
			this.d = d;
		}

		public virtual int GetMacSize()
		{
			return 8;
		}

		public virtual void Init(ICipherParameters parameters)
		{
			KeyParameter keyParameter = parameters as KeyParameter;
			if (keyParameter == null)
			{
				throw new ArgumentException("must be an instance of KeyParameter", "parameters");
			}
			byte[] key = keyParameter.GetKey();
			if (key.Length != 16)
			{
				throw new ArgumentException("must be a 128-bit key", "parameters");
			}
			this.k0 = (long)Pack.LE_To_UInt64(key, 0);
			this.k1 = (long)Pack.LE_To_UInt64(key, 8);
			this.Reset();
		}

		public virtual void Update(byte input)
		{
			this.m = (long)((ulong)this.m >> 8 | (ulong)input << 56);
			if (++this.wordPos == 8)
			{
				this.ProcessMessageWord();
				this.wordPos = 0;
			}
		}

		public virtual void BlockUpdate(byte[] input, int offset, int length)
		{
			int i = 0;
			int num = length & -8;
			if (this.wordPos == 0)
			{
				while (i < num)
				{
					this.m = (long)Pack.LE_To_UInt64(input, offset + i);
					this.ProcessMessageWord();
					i += 8;
				}
				while (i < length)
				{
					this.m = (long)((ulong)this.m >> 8 | (ulong)input[offset + i] << 56);
					i++;
				}
				this.wordPos = length - num;
				return;
			}
			int num2 = this.wordPos << 3;
			while (i < num)
			{
				ulong num3 = Pack.LE_To_UInt64(input, offset + i);
				this.m = (long)(num3 << num2 | (ulong)this.m >> -num2);
				this.ProcessMessageWord();
				this.m = (long)num3;
				i += 8;
			}
			while (i < length)
			{
				this.m = (long)((ulong)this.m >> 8 | (ulong)input[offset + i] << 56);
				if (++this.wordPos == 8)
				{
					this.ProcessMessageWord();
					this.wordPos = 0;
				}
				i++;
			}
		}

		public virtual long DoFinal()
		{
			this.m = (long)((ulong)this.m >> (7 - this.wordPos << 3));
			this.m = (long)((ulong)this.m >> 8);
			this.m |= (long)((this.wordCount << 3) + this.wordPos) << 56;
			this.ProcessMessageWord();
			this.v2 ^= 255L;
			this.ApplySipRounds(this.d);
			long result = this.v0 ^ this.v1 ^ this.v2 ^ this.v3;
			this.Reset();
			return result;
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			long n = this.DoFinal();
			Pack.UInt64_To_LE((ulong)n, output, outOff);
			return 8;
		}

		public virtual void Reset()
		{
			this.v0 = (this.k0 ^ 8317987319222330741L);
			this.v1 = (this.k1 ^ 7237128888997146477L);
			this.v2 = (this.k0 ^ 7816392313619706465L);
			this.v3 = (this.k1 ^ 8387220255154660723L);
			this.m = 0L;
			this.wordPos = 0;
			this.wordCount = 0;
		}

		protected virtual void ProcessMessageWord()
		{
			this.wordCount++;
			this.v3 ^= this.m;
			this.ApplySipRounds(this.c);
			this.v0 ^= this.m;
		}

		protected virtual void ApplySipRounds(int n)
		{
			long num = this.v0;
			long num2 = this.v1;
			long num3 = this.v2;
			long num4 = this.v3;
			for (int i = 0; i < n; i++)
			{
				num += num2;
				num3 += num4;
				num2 = SipHash.RotateLeft(num2, 13);
				num4 = SipHash.RotateLeft(num4, 16);
				num2 ^= num;
				num4 ^= num3;
				num = SipHash.RotateLeft(num, 32);
				num3 += num2;
				num += num4;
				num2 = SipHash.RotateLeft(num2, 17);
				num4 = SipHash.RotateLeft(num4, 21);
				num2 ^= num3;
				num4 ^= num;
				num3 = SipHash.RotateLeft(num3, 32);
			}
			this.v0 = num;
			this.v1 = num2;
			this.v2 = num3;
			this.v3 = num4;
		}

		protected static long RotateLeft(long x, int n)
		{
			return x << n | (long)((ulong)x >> -n);
		}
	}
}
