using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class DsaParametersGenerator
	{
		private IDigest digest;

		private int L;

		private int N;

		private int certainty;

		private SecureRandom random;

		private bool use186_3;

		private int usageIndex;

		public DsaParametersGenerator() : this(new Sha1Digest())
		{
		}

		public DsaParametersGenerator(IDigest digest)
		{
			this.digest = digest;
		}

		public virtual void Init(int size, int certainty, SecureRandom random)
		{
			if (!DsaParametersGenerator.IsValidDsaStrength(size))
			{
				throw new ArgumentException("size must be from 512 - 1024 and a multiple of 64", "size");
			}
			this.use186_3 = false;
			this.L = size;
			this.N = DsaParametersGenerator.GetDefaultN(size);
			this.certainty = certainty;
			this.random = random;
		}

		public virtual void Init(DsaParameterGenerationParameters parameters)
		{
			this.use186_3 = true;
			this.L = parameters.L;
			this.N = parameters.N;
			this.certainty = parameters.Certainty;
			this.random = parameters.Random;
			this.usageIndex = parameters.UsageIndex;
			if (this.L < 1024 || this.L > 3072 || this.L % 1024 != 0)
			{
				throw new ArgumentException("Values must be between 1024 and 3072 and a multiple of 1024", "L");
			}
			if (this.L == 1024 && this.N != 160)
			{
				throw new ArgumentException("N must be 160 for L = 1024");
			}
			if (this.L == 2048 && this.N != 224 && this.N != 256)
			{
				throw new ArgumentException("N must be 224 or 256 for L = 2048");
			}
			if (this.L == 3072 && this.N != 256)
			{
				throw new ArgumentException("N must be 256 for L = 3072");
			}
			if (this.digest.GetDigestSize() * 8 < this.N)
			{
				throw new InvalidOperationException("Digest output size too small for value of N");
			}
		}

		public virtual DsaParameters GenerateParameters()
		{
			if (!this.use186_3)
			{
				return this.GenerateParameters_FIPS186_2();
			}
			return this.GenerateParameters_FIPS186_3();
		}

		protected virtual DsaParameters GenerateParameters_FIPS186_2()
		{
			byte[] array = new byte[20];
			byte[] array2 = new byte[20];
			byte[] array3 = new byte[20];
			byte[] array4 = new byte[20];
			int num = (this.L - 1) / 160;
			byte[] array5 = new byte[this.L / 8];
			if (!(this.digest is Sha1Digest))
			{
				throw new InvalidOperationException("can only use SHA-1 for generating FIPS 186-2 parameters");
			}
			BigInteger bigInteger;
			int i;
			BigInteger bigInteger4;
			while (true)
			{
				this.random.NextBytes(array);
				DsaParametersGenerator.Hash(this.digest, array, array2);
				Array.Copy(array, 0, array3, 0, array.Length);
				DsaParametersGenerator.Inc(array3);
				DsaParametersGenerator.Hash(this.digest, array3, array3);
				for (int num2 = 0; num2 != array4.Length; num2++)
				{
					array4[num2] = (array2[num2] ^ array3[num2]);
				}
				byte[] expr_B6_cp_0 = array4;
				int expr_B6_cp_1 = 0;
				expr_B6_cp_0[expr_B6_cp_1] |= 128;
				byte[] expr_D0_cp_0 = array4;
				int expr_D0_cp_1 = 19;
				expr_D0_cp_0[expr_D0_cp_1] |= 1;
				bigInteger = new BigInteger(1, array4);
				if (bigInteger.IsProbablePrime(this.certainty))
				{
					byte[] array6 = Arrays.Clone(array);
					DsaParametersGenerator.Inc(array6);
					for (i = 0; i < 4096; i++)
					{
						for (int j = 0; j < num; j++)
						{
							DsaParametersGenerator.Inc(array6);
							DsaParametersGenerator.Hash(this.digest, array6, array2);
							Array.Copy(array2, 0, array5, array5.Length - (j + 1) * array2.Length, array2.Length);
						}
						DsaParametersGenerator.Inc(array6);
						DsaParametersGenerator.Hash(this.digest, array6, array2);
						Array.Copy(array2, array2.Length - (array5.Length - num * array2.Length), array5, 0, array5.Length - num * array2.Length);
						byte[] expr_18F_cp_0 = array5;
						int expr_18F_cp_1 = 0;
						expr_18F_cp_0[expr_18F_cp_1] |= 128;
						BigInteger bigInteger2 = new BigInteger(1, array5);
						BigInteger bigInteger3 = bigInteger2.Mod(bigInteger.ShiftLeft(1));
						bigInteger4 = bigInteger2.Subtract(bigInteger3.Subtract(BigInteger.One));
						if (bigInteger4.BitLength == this.L && bigInteger4.IsProbablePrime(this.certainty))
						{
							goto Block_6;
						}
					}
				}
			}
			Block_6:
			BigInteger g = this.CalculateGenerator_FIPS186_2(bigInteger4, bigInteger, this.random);
			return new DsaParameters(bigInteger4, bigInteger, g, new DsaValidationParameters(array, i));
		}

		protected virtual BigInteger CalculateGenerator_FIPS186_2(BigInteger p, BigInteger q, SecureRandom r)
		{
			BigInteger e = p.Subtract(BigInteger.One).Divide(q);
			BigInteger max = p.Subtract(BigInteger.Two);
			BigInteger bigInteger2;
			do
			{
				BigInteger bigInteger = BigIntegers.CreateRandomInRange(BigInteger.Two, max, r);
				bigInteger2 = bigInteger.ModPow(e, p);
			}
			while (bigInteger2.BitLength <= 1);
			return bigInteger2;
		}

		protected virtual DsaParameters GenerateParameters_FIPS186_3()
		{
			IDigest digest = this.digest;
			int num = digest.GetDigestSize() * 8;
			int n = this.N;
			byte[] array = new byte[n / 8];
			int num2 = (this.L - 1) / num;
			int n2 = (this.L - 1) % num;
			byte[] array2 = new byte[digest.GetDigestSize()];
			BigInteger bigInteger2;
			int i;
			BigInteger bigInteger7;
			while (true)
			{
				this.random.NextBytes(array);
				DsaParametersGenerator.Hash(digest, array, array2);
				BigInteger bigInteger = new BigInteger(1, array2).Mod(BigInteger.One.ShiftLeft(this.N - 1));
				bigInteger2 = BigInteger.One.ShiftLeft(this.N - 1).Add(bigInteger).Add(BigInteger.One).Subtract(bigInteger.Mod(BigInteger.Two));
				if (bigInteger2.IsProbablePrime(this.certainty))
				{
					byte[] array3 = Arrays.Clone(array);
					int num3 = 4 * this.L;
					for (i = 0; i < num3; i++)
					{
						BigInteger bigInteger3 = BigInteger.Zero;
						int j = 0;
						int num4 = 0;
						while (j <= num2)
						{
							DsaParametersGenerator.Inc(array3);
							DsaParametersGenerator.Hash(digest, array3, array2);
							BigInteger bigInteger4 = new BigInteger(1, array2);
							if (j == num2)
							{
								bigInteger4 = bigInteger4.Mod(BigInteger.One.ShiftLeft(n2));
							}
							bigInteger3 = bigInteger3.Add(bigInteger4.ShiftLeft(num4));
							j++;
							num4 += num;
						}
						BigInteger bigInteger5 = bigInteger3.Add(BigInteger.One.ShiftLeft(this.L - 1));
						BigInteger bigInteger6 = bigInteger5.Mod(bigInteger2.ShiftLeft(1));
						bigInteger7 = bigInteger5.Subtract(bigInteger6.Subtract(BigInteger.One));
						if (bigInteger7.BitLength == this.L && bigInteger7.IsProbablePrime(this.certainty))
						{
							goto Block_5;
						}
					}
				}
			}
			Block_5:
			if (this.usageIndex >= 0)
			{
				BigInteger bigInteger8 = this.CalculateGenerator_FIPS186_3_Verifiable(digest, bigInteger7, bigInteger2, array, this.usageIndex);
				if (bigInteger8 != null)
				{
					return new DsaParameters(bigInteger7, bigInteger2, bigInteger8, new DsaValidationParameters(array, i, this.usageIndex));
				}
			}
			BigInteger g = this.CalculateGenerator_FIPS186_3_Unverifiable(bigInteger7, bigInteger2, this.random);
			return new DsaParameters(bigInteger7, bigInteger2, g, new DsaValidationParameters(array, i));
		}

		protected virtual BigInteger CalculateGenerator_FIPS186_3_Unverifiable(BigInteger p, BigInteger q, SecureRandom r)
		{
			return this.CalculateGenerator_FIPS186_2(p, q, r);
		}

		protected virtual BigInteger CalculateGenerator_FIPS186_3_Verifiable(IDigest d, BigInteger p, BigInteger q, byte[] seed, int index)
		{
			BigInteger e = p.Subtract(BigInteger.One).Divide(q);
			byte[] array = Hex.Decode("6767656E");
			byte[] array2 = new byte[seed.Length + array.Length + 1 + 2];
			Array.Copy(seed, 0, array2, 0, seed.Length);
			Array.Copy(array, 0, array2, seed.Length, array.Length);
			array2[array2.Length - 3] = (byte)index;
			byte[] array3 = new byte[d.GetDigestSize()];
			for (int i = 1; i < 65536; i++)
			{
				DsaParametersGenerator.Inc(array2);
				DsaParametersGenerator.Hash(d, array2, array3);
				BigInteger bigInteger = new BigInteger(1, array3);
				BigInteger bigInteger2 = bigInteger.ModPow(e, p);
				if (bigInteger2.CompareTo(BigInteger.Two) >= 0)
				{
					return bigInteger2;
				}
			}
			return null;
		}

		private static bool IsValidDsaStrength(int strength)
		{
			return strength >= 512 && strength <= 1024 && strength % 64 == 0;
		}

		protected static void Hash(IDigest d, byte[] input, byte[] output)
		{
			d.BlockUpdate(input, 0, input.Length);
			d.DoFinal(output, 0);
		}

		private static int GetDefaultN(int L)
		{
			if (L <= 1024)
			{
				return 160;
			}
			return 256;
		}

		protected static void Inc(byte[] buf)
		{
			for (int i = buf.Length - 1; i >= 0; i--)
			{
				byte b = buf[i] + 1;
				buf[i] = b;
				if (b != 0)
				{
					return;
				}
			}
		}
	}
}
