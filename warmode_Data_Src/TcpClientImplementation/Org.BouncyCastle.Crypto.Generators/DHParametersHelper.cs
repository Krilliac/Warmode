using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	internal class DHParametersHelper
	{
		private static readonly BigInteger Six = BigInteger.ValueOf(6L);

		private static readonly int[][] primeLists = BigInteger.primeLists;

		private static readonly int[] primeProducts = BigInteger.primeProducts;

		private static readonly BigInteger[] BigPrimeProducts = DHParametersHelper.ConstructBigPrimeProducts(DHParametersHelper.primeProducts);

		private static BigInteger[] ConstructBigPrimeProducts(int[] primeProducts)
		{
			BigInteger[] array = new BigInteger[primeProducts.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = BigInteger.ValueOf((long)primeProducts[i]);
			}
			return array;
		}

		internal static BigInteger[] GenerateSafePrimes(int size, int certainty, SecureRandom random)
		{
			int num = size - 1;
			int num2 = size >> 2;
			BigInteger bigInteger;
			BigInteger bigInteger2;
			if (size <= 32)
			{
				while (true)
				{
					bigInteger = new BigInteger(num, 2, random);
					bigInteger2 = bigInteger.ShiftLeft(1).Add(BigInteger.One);
					if (bigInteger2.IsProbablePrime(certainty))
					{
						if (certainty <= 2 || bigInteger.IsProbablePrime(certainty - 2))
						{
							break;
						}
					}
				}
			}
			else
			{
				while (true)
				{
					bigInteger = new BigInteger(num, 0, random);
					while (true)
					{
						IL_51:
						for (int i = 0; i < DHParametersHelper.primeLists.Length; i++)
						{
							int num3 = bigInteger.Remainder(DHParametersHelper.BigPrimeProducts[i]).IntValue;
							if (i == 0)
							{
								int num4 = num3 % 3;
								if (num4 != 2)
								{
									int num5 = 2 * num4 + 2;
									bigInteger = bigInteger.Add(BigInteger.ValueOf((long)num5));
									num3 = (num3 + num5) % DHParametersHelper.primeProducts[i];
								}
							}
							int[] array = DHParametersHelper.primeLists[i];
							for (int j = 0; j < array.Length; j++)
							{
								int num6 = array[j];
								int num7 = num3 % num6;
								if (num7 == 0 || num7 == num6 >> 1)
								{
									bigInteger = bigInteger.Add(DHParametersHelper.Six);
									goto IL_51;
								}
							}
						}
						break;
					}
					if (bigInteger.BitLength == num && bigInteger.RabinMillerTest(2, random))
					{
						bigInteger2 = bigInteger.ShiftLeft(1).Add(BigInteger.One);
						if (bigInteger2.RabinMillerTest(certainty, random) && (certainty <= 2 || bigInteger.RabinMillerTest(certainty - 2, random)) && WNafUtilities.GetNafWeight(bigInteger2) >= num2)
						{
							break;
						}
					}
				}
			}
			return new BigInteger[]
			{
				bigInteger2,
				bigInteger
			};
		}

		internal static BigInteger SelectGenerator(BigInteger p, BigInteger q, SecureRandom random)
		{
			BigInteger max = p.Subtract(BigInteger.Two);
			BigInteger bigInteger2;
			do
			{
				BigInteger bigInteger = BigIntegers.CreateRandomInRange(BigInteger.Two, max, random);
				bigInteger2 = bigInteger.ModPow(BigInteger.Two, p);
			}
			while (bigInteger2.Equals(BigInteger.One));
			return bigInteger2;
		}
	}
}
