using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public abstract class WNafUtilities
	{
		public static readonly string PRECOMP_NAME = "bc_wnaf";

		private static readonly int[] DEFAULT_WINDOW_SIZE_CUTOFFS = new int[]
		{
			13,
			41,
			121,
			337,
			897,
			2305
		};

		private static readonly byte[] EMPTY_BYTES = new byte[0];

		private static readonly int[] EMPTY_INTS = new int[0];

		private static readonly ECPoint[] EMPTY_POINTS = new ECPoint[0];

		public static int[] GenerateCompactNaf(BigInteger k)
		{
			if (k.BitLength >> 16 != 0)
			{
				throw new ArgumentException("must have bitlength < 2^16", "k");
			}
			if (k.SignValue == 0)
			{
				return WNafUtilities.EMPTY_INTS;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			int bitLength = bigInteger.BitLength;
			int[] array = new int[bitLength >> 1];
			BigInteger bigInteger2 = bigInteger.Xor(k);
			int num = bitLength - 1;
			int num2 = 0;
			int num3 = 0;
			for (int i = 1; i < num; i++)
			{
				if (!bigInteger2.TestBit(i))
				{
					num3++;
				}
				else
				{
					int num4 = k.TestBit(i) ? -1 : 1;
					array[num2++] = (num4 << 16 | num3);
					num3 = 1;
					i++;
				}
			}
			array[num2++] = (65536 | num3);
			if (array.Length > num2)
			{
				array = WNafUtilities.Trim(array, num2);
			}
			return array;
		}

		public static int[] GenerateCompactWindowNaf(int width, BigInteger k)
		{
			if (width == 2)
			{
				return WNafUtilities.GenerateCompactNaf(k);
			}
			if (width < 2 || width > 16)
			{
				throw new ArgumentException("must be in the range [2, 16]", "width");
			}
			if (k.BitLength >> 16 != 0)
			{
				throw new ArgumentException("must have bitlength < 2^16", "k");
			}
			if (k.SignValue == 0)
			{
				return WNafUtilities.EMPTY_INTS;
			}
			int[] array = new int[k.BitLength / width + 1];
			int num = 1 << width;
			int num2 = num - 1;
			int num3 = num >> 1;
			bool flag = false;
			int num4 = 0;
			int i = 0;
			while (i <= k.BitLength)
			{
				if (k.TestBit(i) == flag)
				{
					i++;
				}
				else
				{
					k = k.ShiftRight(i);
					int num5 = k.IntValue & num2;
					if (flag)
					{
						num5++;
					}
					flag = ((num5 & num3) != 0);
					if (flag)
					{
						num5 -= num;
					}
					int num6 = (num4 > 0) ? (i - 1) : i;
					array[num4++] = (num5 << 16 | num6);
					i = width;
				}
			}
			if (array.Length > num4)
			{
				array = WNafUtilities.Trim(array, num4);
			}
			return array;
		}

		public static byte[] GenerateJsf(BigInteger g, BigInteger h)
		{
			int num = Math.Max(g.BitLength, h.BitLength) + 1;
			byte[] array = new byte[num];
			BigInteger bigInteger = g;
			BigInteger bigInteger2 = h;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while ((num3 | num4) != 0 || bigInteger.BitLength > num5 || bigInteger2.BitLength > num5)
			{
				int num6 = (int)(((uint)bigInteger.IntValue >> num5) + (uint)num3 & 7u);
				int num7 = (int)(((uint)bigInteger2.IntValue >> num5) + (uint)num4 & 7u);
				int num8 = num6 & 1;
				if (num8 != 0)
				{
					num8 -= (num6 & 2);
					if (num6 + num8 == 4 && (num7 & 3) == 2)
					{
						num8 = -num8;
					}
				}
				int num9 = num7 & 1;
				if (num9 != 0)
				{
					num9 -= (num7 & 2);
					if (num7 + num9 == 4 && (num6 & 3) == 2)
					{
						num9 = -num9;
					}
				}
				if (num3 << 1 == 1 + num8)
				{
					num3 ^= 1;
				}
				if (num4 << 1 == 1 + num9)
				{
					num4 ^= 1;
				}
				if (++num5 == 30)
				{
					num5 = 0;
					bigInteger = bigInteger.ShiftRight(30);
					bigInteger2 = bigInteger2.ShiftRight(30);
				}
				array[num2++] = (byte)(num8 << 4 | (num9 & 15));
			}
			if (array.Length > num2)
			{
				array = WNafUtilities.Trim(array, num2);
			}
			return array;
		}

		public static byte[] GenerateNaf(BigInteger k)
		{
			if (k.SignValue == 0)
			{
				return WNafUtilities.EMPTY_BYTES;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			int num = bigInteger.BitLength - 1;
			byte[] array = new byte[num];
			BigInteger bigInteger2 = bigInteger.Xor(k);
			for (int i = 1; i < num; i++)
			{
				if (bigInteger2.TestBit(i))
				{
					array[i - 1] = (byte)(k.TestBit(i) ? -1 : 1);
					i++;
				}
			}
			array[num - 1] = 1;
			return array;
		}

		public static byte[] GenerateWindowNaf(int width, BigInteger k)
		{
			if (width == 2)
			{
				return WNafUtilities.GenerateNaf(k);
			}
			if (width < 2 || width > 8)
			{
				throw new ArgumentException("must be in the range [2, 8]", "width");
			}
			if (k.SignValue == 0)
			{
				return WNafUtilities.EMPTY_BYTES;
			}
			byte[] array = new byte[k.BitLength + 1];
			int num = 1 << width;
			int num2 = num - 1;
			int num3 = num >> 1;
			bool flag = false;
			int num4 = 0;
			int i = 0;
			while (i <= k.BitLength)
			{
				if (k.TestBit(i) == flag)
				{
					i++;
				}
				else
				{
					k = k.ShiftRight(i);
					int num5 = k.IntValue & num2;
					if (flag)
					{
						num5++;
					}
					flag = ((num5 & num3) != 0);
					if (flag)
					{
						num5 -= num;
					}
					num4 += ((num4 > 0) ? (i - 1) : i);
					array[num4++] = (byte)num5;
					i = width;
				}
			}
			if (array.Length > num4)
			{
				array = WNafUtilities.Trim(array, num4);
			}
			return array;
		}

		public static int GetNafWeight(BigInteger k)
		{
			if (k.SignValue == 0)
			{
				return 0;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			BigInteger bigInteger2 = bigInteger.Xor(k);
			return bigInteger2.BitCount;
		}

		public static WNafPreCompInfo GetWNafPreCompInfo(ECPoint p)
		{
			return WNafUtilities.GetWNafPreCompInfo(p.Curve.GetPreCompInfo(p, WNafUtilities.PRECOMP_NAME));
		}

		public static WNafPreCompInfo GetWNafPreCompInfo(PreCompInfo preCompInfo)
		{
			if (preCompInfo != null && preCompInfo is WNafPreCompInfo)
			{
				return (WNafPreCompInfo)preCompInfo;
			}
			return new WNafPreCompInfo();
		}

		public static int GetWindowSize(int bits)
		{
			return WNafUtilities.GetWindowSize(bits, WNafUtilities.DEFAULT_WINDOW_SIZE_CUTOFFS);
		}

		public static int GetWindowSize(int bits, int[] windowSizeCutoffs)
		{
			int num = 0;
			while (num < windowSizeCutoffs.Length && bits >= windowSizeCutoffs[num])
			{
				num++;
			}
			return num + 2;
		}

		public static ECPoint MapPointWithPrecomp(ECPoint p, int width, bool includeNegated, ECPointMap pointMap)
		{
			ECCurve curve = p.Curve;
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.Precompute(p, width, includeNegated);
			ECPoint eCPoint = pointMap.Map(p);
			WNafPreCompInfo wNafPreCompInfo2 = WNafUtilities.GetWNafPreCompInfo(curve.GetPreCompInfo(eCPoint, WNafUtilities.PRECOMP_NAME));
			ECPoint twice = wNafPreCompInfo.Twice;
			if (twice != null)
			{
				ECPoint twice2 = pointMap.Map(twice);
				wNafPreCompInfo2.Twice = twice2;
			}
			ECPoint[] preComp = wNafPreCompInfo.PreComp;
			ECPoint[] array = new ECPoint[preComp.Length];
			for (int i = 0; i < preComp.Length; i++)
			{
				array[i] = pointMap.Map(preComp[i]);
			}
			wNafPreCompInfo2.PreComp = array;
			if (includeNegated)
			{
				ECPoint[] array2 = new ECPoint[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[j].Negate();
				}
				wNafPreCompInfo2.PreCompNeg = array2;
			}
			curve.SetPreCompInfo(eCPoint, WNafUtilities.PRECOMP_NAME, wNafPreCompInfo2);
			return eCPoint;
		}

		public static WNafPreCompInfo Precompute(ECPoint p, int width, bool includeNegated)
		{
			ECCurve curve = p.Curve;
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.GetWNafPreCompInfo(curve.GetPreCompInfo(p, WNafUtilities.PRECOMP_NAME));
			int num = 0;
			int num2 = 1 << Math.Max(0, width - 2);
			ECPoint[] array = wNafPreCompInfo.PreComp;
			if (array == null)
			{
				array = WNafUtilities.EMPTY_POINTS;
			}
			else
			{
				num = array.Length;
			}
			if (num < num2)
			{
				array = WNafUtilities.ResizeTable(array, num2);
				if (num2 == 1)
				{
					array[0] = p.Normalize();
				}
				else
				{
					int i = num;
					if (i == 0)
					{
						array[0] = p;
						i = 1;
					}
					ECFieldElement eCFieldElement = null;
					if (num2 == 2)
					{
						array[1] = p.ThreeTimes();
					}
					else
					{
						ECPoint eCPoint = wNafPreCompInfo.Twice;
						ECPoint eCPoint2 = array[i - 1];
						if (eCPoint == null)
						{
							eCPoint = array[0].Twice();
							wNafPreCompInfo.Twice = eCPoint;
							if (ECAlgorithms.IsFpCurve(curve) && curve.FieldSize >= 64)
							{
								switch (curve.CoordinateSystem)
								{
								case 2:
								case 3:
								case 4:
								{
									eCFieldElement = eCPoint.GetZCoord(0);
									eCPoint = curve.CreatePoint(eCPoint.XCoord.ToBigInteger(), eCPoint.YCoord.ToBigInteger());
									ECFieldElement eCFieldElement2 = eCFieldElement.Square();
									ECFieldElement scale = eCFieldElement2.Multiply(eCFieldElement);
									eCPoint2 = eCPoint2.ScaleX(eCFieldElement2).ScaleY(scale);
									if (num == 0)
									{
										array[0] = eCPoint2;
									}
									break;
								}
								}
							}
						}
						while (i < num2)
						{
							eCPoint2 = (array[i++] = eCPoint2.Add(eCPoint));
						}
					}
					curve.NormalizeAll(array, num, num2 - num, eCFieldElement);
				}
			}
			wNafPreCompInfo.PreComp = array;
			if (includeNegated)
			{
				ECPoint[] array2 = wNafPreCompInfo.PreCompNeg;
				int j;
				if (array2 == null)
				{
					j = 0;
					array2 = new ECPoint[num2];
				}
				else
				{
					j = array2.Length;
					if (j < num2)
					{
						array2 = WNafUtilities.ResizeTable(array2, num2);
					}
				}
				while (j < num2)
				{
					array2[j] = array[j].Negate();
					j++;
				}
				wNafPreCompInfo.PreCompNeg = array2;
			}
			curve.SetPreCompInfo(p, WNafUtilities.PRECOMP_NAME, wNafPreCompInfo);
			return wNafPreCompInfo;
		}

		private static byte[] Trim(byte[] a, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(a, 0, array, 0, array.Length);
			return array;
		}

		private static int[] Trim(int[] a, int length)
		{
			int[] array = new int[length];
			Array.Copy(a, 0, array, 0, array.Length);
			return array;
		}

		private static ECPoint[] ResizeTable(ECPoint[] a, int length)
		{
			ECPoint[] array = new ECPoint[length];
			Array.Copy(a, 0, array, 0, a.Length);
			return array;
		}
	}
}
