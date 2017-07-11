using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class HMacDsaKCalculator : IDsaKCalculator
	{
		private readonly HMac hMac;

		private readonly byte[] K;

		private readonly byte[] V;

		private BigInteger n;

		public virtual bool IsDeterministic
		{
			get
			{
				return true;
			}
		}

		public HMacDsaKCalculator(IDigest digest)
		{
			this.hMac = new HMac(digest);
			this.V = new byte[this.hMac.GetMacSize()];
			this.K = new byte[this.hMac.GetMacSize()];
		}

		public virtual void Init(BigInteger n, SecureRandom random)
		{
			throw new InvalidOperationException("Operation not supported");
		}

		public void Init(BigInteger n, BigInteger d, byte[] message)
		{
			this.n = n;
			Arrays.Fill(this.V, 1);
			Arrays.Fill(this.K, 0);
			byte[] array = new byte[(n.BitLength + 7) / 8];
			byte[] array2 = BigIntegers.AsUnsignedByteArray(d);
			Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			byte[] array3 = new byte[(n.BitLength + 7) / 8];
			BigInteger bigInteger = this.BitsToInt(message);
			if (bigInteger.CompareTo(n) >= 0)
			{
				bigInteger = bigInteger.Subtract(n);
			}
			byte[] array4 = BigIntegers.AsUnsignedByteArray(bigInteger);
			Array.Copy(array4, 0, array3, array3.Length - array4.Length, array4.Length);
			this.hMac.Init(new KeyParameter(this.K));
			this.hMac.BlockUpdate(this.V, 0, this.V.Length);
			this.hMac.Update(0);
			this.hMac.BlockUpdate(array, 0, array.Length);
			this.hMac.BlockUpdate(array3, 0, array3.Length);
			this.hMac.DoFinal(this.K, 0);
			this.hMac.Init(new KeyParameter(this.K));
			this.hMac.BlockUpdate(this.V, 0, this.V.Length);
			this.hMac.DoFinal(this.V, 0);
			this.hMac.BlockUpdate(this.V, 0, this.V.Length);
			this.hMac.Update(1);
			this.hMac.BlockUpdate(array, 0, array.Length);
			this.hMac.BlockUpdate(array3, 0, array3.Length);
			this.hMac.DoFinal(this.K, 0);
			this.hMac.Init(new KeyParameter(this.K));
			this.hMac.BlockUpdate(this.V, 0, this.V.Length);
			this.hMac.DoFinal(this.V, 0);
		}

		public virtual BigInteger NextK()
		{
			byte[] array = new byte[(this.n.BitLength + 7) / 8];
			BigInteger bigInteger;
			while (true)
			{
				int num;
				for (int i = 0; i < array.Length; i += num)
				{
					this.hMac.BlockUpdate(this.V, 0, this.V.Length);
					this.hMac.DoFinal(this.V, 0);
					num = Math.Min(array.Length - i, this.V.Length);
					Array.Copy(this.V, 0, array, i, num);
				}
				bigInteger = this.BitsToInt(array);
				if (bigInteger.SignValue > 0 && bigInteger.CompareTo(this.n) < 0)
				{
					break;
				}
				this.hMac.BlockUpdate(this.V, 0, this.V.Length);
				this.hMac.Update(0);
				this.hMac.DoFinal(this.K, 0);
				this.hMac.Init(new KeyParameter(this.K));
				this.hMac.BlockUpdate(this.V, 0, this.V.Length);
				this.hMac.DoFinal(this.V, 0);
			}
			return bigInteger;
		}

		private BigInteger BitsToInt(byte[] t)
		{
			BigInteger bigInteger = new BigInteger(1, t);
			if (t.Length * 8 > this.n.BitLength)
			{
				bigInteger = bigInteger.ShiftRight(t.Length * 8 - this.n.BitLength);
			}
			return bigInteger;
		}
	}
}
