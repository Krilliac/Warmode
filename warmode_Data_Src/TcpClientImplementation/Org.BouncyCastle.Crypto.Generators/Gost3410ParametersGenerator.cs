using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Gost3410ParametersGenerator
	{
		private int size;

		private int typeproc;

		private SecureRandom init_random;

		public void Init(int size, int typeProcedure, SecureRandom random)
		{
			this.size = size;
			this.typeproc = typeProcedure;
			this.init_random = random;
		}

		private int procedure_A(int x0, int c, BigInteger[] pq, int size)
		{
			while (x0 < 0 || x0 > 65536)
			{
				x0 = this.init_random.NextInt() / 32768;
			}
			while (c < 0 || c > 65536 || c / 2 == 0)
			{
				c = this.init_random.NextInt() / 32768 + 1;
			}
			BigInteger value = BigInteger.ValueOf((long)c);
			BigInteger val = BigInteger.ValueOf(19381L);
			BigInteger[] array = new BigInteger[]
			{
				BigInteger.ValueOf((long)x0)
			};
			int[] array2 = new int[]
			{
				size
			};
			int num = 0;
			int num2 = 0;
			while (array2[num2] >= 17)
			{
				int[] array3 = new int[array2.Length + 1];
				Array.Copy(array2, 0, array3, 0, array2.Length);
				array2 = new int[array3.Length];
				Array.Copy(array3, 0, array2, 0, array3.Length);
				array2[num2 + 1] = array2[num2] / 2;
				num = num2 + 1;
				num2++;
			}
			BigInteger[] array4 = new BigInteger[num + 1];
			array4[num] = new BigInteger("8003", 16);
			int num3 = num - 1;
			for (int i = 0; i < num; i++)
			{
				int num4 = array2[num3] / 16;
				while (true)
				{
					BigInteger[] array5 = new BigInteger[array.Length];
					Array.Copy(array, 0, array5, 0, array.Length);
					array = new BigInteger[num4 + 1];
					Array.Copy(array5, 0, array, 0, array5.Length);
					for (int j = 0; j < num4; j++)
					{
						array[j + 1] = array[j].Multiply(val).Add(value).Mod(BigInteger.Two.Pow(16));
					}
					BigInteger bigInteger = BigInteger.Zero;
					for (int k = 0; k < num4; k++)
					{
						bigInteger = bigInteger.Add(array[k].ShiftLeft(16 * k));
					}
					array[0] = array[num4];
					BigInteger bigInteger2 = BigInteger.One.ShiftLeft(array2[num3] - 1).Divide(array4[num3 + 1]).Add(bigInteger.ShiftLeft(array2[num3] - 1).Divide(array4[num3 + 1].ShiftLeft(16 * num4)));
					if (bigInteger2.TestBit(0))
					{
						bigInteger2 = bigInteger2.Add(BigInteger.One);
					}
					while (true)
					{
						BigInteger bigInteger3 = bigInteger2.Multiply(array4[num3 + 1]);
						if (bigInteger3.BitLength > array2[num3])
						{
							break;
						}
						array4[num3] = bigInteger3.Add(BigInteger.One);
						if (BigInteger.Two.ModPow(bigInteger3, array4[num3]).CompareTo(BigInteger.One) == 0 && BigInteger.Two.ModPow(bigInteger2, array4[num3]).CompareTo(BigInteger.One) != 0)
						{
							goto IL_27C;
						}
						bigInteger2 = bigInteger2.Add(BigInteger.Two);
					}
				}
				IL_27C:
				if (--num3 < 0)
				{
					pq[0] = array4[0];
					pq[1] = array4[1];
					return array[0].IntValue;
				}
			}
			return array[0].IntValue;
		}

		private long procedure_Aa(long x0, long c, BigInteger[] pq, int size)
		{
			while (x0 < 0L || x0 > 4294967296L)
			{
				x0 = (long)(this.init_random.NextInt() * 2);
			}
			while (c < 0L || c > 4294967296L || c / 2L == 0L)
			{
				c = (long)(this.init_random.NextInt() * 2 + 1);
			}
			BigInteger value = BigInteger.ValueOf(c);
			BigInteger val = BigInteger.ValueOf(97781173L);
			BigInteger[] array = new BigInteger[]
			{
				BigInteger.ValueOf(x0)
			};
			int[] array2 = new int[]
			{
				size
			};
			int num = 0;
			int num2 = 0;
			while (array2[num2] >= 33)
			{
				int[] array3 = new int[array2.Length + 1];
				Array.Copy(array2, 0, array3, 0, array2.Length);
				array2 = new int[array3.Length];
				Array.Copy(array3, 0, array2, 0, array3.Length);
				array2[num2 + 1] = array2[num2] / 2;
				num = num2 + 1;
				num2++;
			}
			BigInteger[] array4 = new BigInteger[num + 1];
			array4[num] = new BigInteger("8000000B", 16);
			int num3 = num - 1;
			for (int i = 0; i < num; i++)
			{
				int num4 = array2[num3] / 32;
				while (true)
				{
					BigInteger[] array5 = new BigInteger[array.Length];
					Array.Copy(array, 0, array5, 0, array.Length);
					array = new BigInteger[num4 + 1];
					Array.Copy(array5, 0, array, 0, array5.Length);
					for (int j = 0; j < num4; j++)
					{
						array[j + 1] = array[j].Multiply(val).Add(value).Mod(BigInteger.Two.Pow(32));
					}
					BigInteger bigInteger = BigInteger.Zero;
					for (int k = 0; k < num4; k++)
					{
						bigInteger = bigInteger.Add(array[k].ShiftLeft(32 * k));
					}
					array[0] = array[num4];
					BigInteger bigInteger2 = BigInteger.One.ShiftLeft(array2[num3] - 1).Divide(array4[num3 + 1]).Add(bigInteger.ShiftLeft(array2[num3] - 1).Divide(array4[num3 + 1].ShiftLeft(32 * num4)));
					if (bigInteger2.TestBit(0))
					{
						bigInteger2 = bigInteger2.Add(BigInteger.One);
					}
					while (true)
					{
						BigInteger bigInteger3 = bigInteger2.Multiply(array4[num3 + 1]);
						if (bigInteger3.BitLength > array2[num3])
						{
							break;
						}
						array4[num3] = bigInteger3.Add(BigInteger.One);
						if (BigInteger.Two.ModPow(bigInteger3, array4[num3]).CompareTo(BigInteger.One) == 0 && BigInteger.Two.ModPow(bigInteger2, array4[num3]).CompareTo(BigInteger.One) != 0)
						{
							goto IL_281;
						}
						bigInteger2 = bigInteger2.Add(BigInteger.Two);
					}
				}
				IL_281:
				if (--num3 < 0)
				{
					pq[0] = array4[0];
					pq[1] = array4[1];
					return array[0].LongValue;
				}
			}
			return array[0].LongValue;
		}

		private void procedure_B(int x0, int c, BigInteger[] pq)
		{
			while (x0 < 0 || x0 > 65536)
			{
				x0 = this.init_random.NextInt() / 32768;
			}
			while (c < 0 || c > 65536 || c / 2 == 0)
			{
				c = this.init_random.NextInt() / 32768 + 1;
			}
			BigInteger[] array = new BigInteger[2];
			BigInteger value = BigInteger.ValueOf((long)c);
			BigInteger val = BigInteger.ValueOf(19381L);
			x0 = this.procedure_A(x0, c, array, 256);
			BigInteger bigInteger = array[0];
			x0 = this.procedure_A(x0, c, array, 512);
			BigInteger val2 = array[0];
			BigInteger[] array2 = new BigInteger[65];
			array2[0] = BigInteger.ValueOf((long)x0);
			BigInteger bigInteger2 = bigInteger.Multiply(val2);
			BigInteger bigInteger6;
			while (true)
			{
				for (int i = 0; i < 64; i++)
				{
					array2[i + 1] = array2[i].Multiply(val).Add(value).Mod(BigInteger.Two.Pow(16));
				}
				BigInteger bigInteger3 = BigInteger.Zero;
				for (int j = 0; j < 64; j++)
				{
					bigInteger3 = bigInteger3.Add(array2[j].ShiftLeft(16 * j));
				}
				array2[0] = array2[64];
				BigInteger bigInteger4 = BigInteger.One.ShiftLeft(1023).Divide(bigInteger2).Add(bigInteger3.ShiftLeft(1023).Divide(bigInteger2.ShiftLeft(1024)));
				if (bigInteger4.TestBit(0))
				{
					bigInteger4 = bigInteger4.Add(BigInteger.One);
				}
				while (true)
				{
					BigInteger bigInteger5 = bigInteger2.Multiply(bigInteger4);
					if (bigInteger5.BitLength > 1024)
					{
						break;
					}
					bigInteger6 = bigInteger5.Add(BigInteger.One);
					if (BigInteger.Two.ModPow(bigInteger5, bigInteger6).CompareTo(BigInteger.One) == 0 && BigInteger.Two.ModPow(bigInteger.Multiply(bigInteger4), bigInteger6).CompareTo(BigInteger.One) != 0)
					{
						goto Block_11;
					}
					bigInteger4 = bigInteger4.Add(BigInteger.Two);
				}
			}
			Block_11:
			pq[0] = bigInteger6;
			pq[1] = bigInteger;
		}

		private void procedure_Bb(long x0, long c, BigInteger[] pq)
		{
			while (x0 < 0L || x0 > 4294967296L)
			{
				x0 = (long)(this.init_random.NextInt() * 2);
			}
			while (c < 0L || c > 4294967296L || c / 2L == 0L)
			{
				c = (long)(this.init_random.NextInt() * 2 + 1);
			}
			BigInteger[] array = new BigInteger[2];
			BigInteger value = BigInteger.ValueOf(c);
			BigInteger val = BigInteger.ValueOf(97781173L);
			x0 = this.procedure_Aa(x0, c, array, 256);
			BigInteger bigInteger = array[0];
			x0 = this.procedure_Aa(x0, c, array, 512);
			BigInteger val2 = array[0];
			BigInteger[] array2 = new BigInteger[33];
			array2[0] = BigInteger.ValueOf(x0);
			BigInteger bigInteger2 = bigInteger.Multiply(val2);
			BigInteger bigInteger6;
			while (true)
			{
				for (int i = 0; i < 32; i++)
				{
					array2[i + 1] = array2[i].Multiply(val).Add(value).Mod(BigInteger.Two.Pow(32));
				}
				BigInteger bigInteger3 = BigInteger.Zero;
				for (int j = 0; j < 32; j++)
				{
					bigInteger3 = bigInteger3.Add(array2[j].ShiftLeft(32 * j));
				}
				array2[0] = array2[32];
				BigInteger bigInteger4 = BigInteger.One.ShiftLeft(1023).Divide(bigInteger2).Add(bigInteger3.ShiftLeft(1023).Divide(bigInteger2.ShiftLeft(1024)));
				if (bigInteger4.TestBit(0))
				{
					bigInteger4 = bigInteger4.Add(BigInteger.One);
				}
				while (true)
				{
					BigInteger bigInteger5 = bigInteger2.Multiply(bigInteger4);
					if (bigInteger5.BitLength > 1024)
					{
						break;
					}
					bigInteger6 = bigInteger5.Add(BigInteger.One);
					if (BigInteger.Two.ModPow(bigInteger5, bigInteger6).CompareTo(BigInteger.One) == 0 && BigInteger.Two.ModPow(bigInteger.Multiply(bigInteger4), bigInteger6).CompareTo(BigInteger.One) != 0)
					{
						goto Block_11;
					}
					bigInteger4 = bigInteger4.Add(BigInteger.Two);
				}
			}
			Block_11:
			pq[0] = bigInteger6;
			pq[1] = bigInteger;
		}

		private BigInteger procedure_C(BigInteger p, BigInteger q)
		{
			BigInteger bigInteger = p.Subtract(BigInteger.One);
			BigInteger e = bigInteger.Divide(q);
			BigInteger bigInteger3;
			while (true)
			{
				BigInteger bigInteger2 = new BigInteger(p.BitLength, this.init_random);
				if (bigInteger2.CompareTo(BigInteger.One) > 0 && bigInteger2.CompareTo(bigInteger) < 0)
				{
					bigInteger3 = bigInteger2.ModPow(e, p);
					if (bigInteger3.CompareTo(BigInteger.One) != 0)
					{
						break;
					}
				}
			}
			return bigInteger3;
		}

		public Gost3410Parameters GenerateParameters()
		{
			BigInteger[] array = new BigInteger[2];
			BigInteger p;
			BigInteger q;
			BigInteger a;
			if (this.typeproc == 1)
			{
				int x = this.init_random.NextInt();
				int c = this.init_random.NextInt();
				int num = this.size;
				if (num != 512)
				{
					if (num != 1024)
					{
						throw new ArgumentException("Ooops! key size 512 or 1024 bit.");
					}
					this.procedure_B(x, c, array);
				}
				else
				{
					this.procedure_A(x, c, array, 512);
				}
				p = array[0];
				q = array[1];
				a = this.procedure_C(p, q);
				return new Gost3410Parameters(p, q, a, new Gost3410ValidationParameters(x, c));
			}
			long num2 = this.init_random.NextLong();
			long num3 = this.init_random.NextLong();
			int num4 = this.size;
			if (num4 != 512)
			{
				if (num4 != 1024)
				{
					throw new InvalidOperationException("Ooops! key size 512 or 1024 bit.");
				}
				this.procedure_Bb(num2, num3, array);
			}
			else
			{
				this.procedure_Aa(num2, num3, array, 512);
			}
			p = array[0];
			q = array[1];
			a = this.procedure_C(p, q);
			return new Gost3410Parameters(p, q, a, new Gost3410ValidationParameters(num2, num3));
		}
	}
}
