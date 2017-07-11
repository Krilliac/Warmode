using Org.BouncyCastle.Math.EC.Endo;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Math.Field;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public class ECAlgorithms
	{
		public static bool IsF2mCurve(ECCurve c)
		{
			IFiniteField field = c.Field;
			return field.Dimension > 1 && field.Characteristic.Equals(BigInteger.Two) && field is IPolynomialExtensionField;
		}

		public static bool IsFpCurve(ECCurve c)
		{
			return c.Field.Dimension == 1;
		}

		public static ECPoint SumOfMultiplies(ECPoint[] ps, BigInteger[] ks)
		{
			if (ps == null || ks == null || ps.Length != ks.Length || ps.Length < 1)
			{
				throw new ArgumentException("point and scalar arrays should be non-null, and of equal, non-zero, length");
			}
			int num = ps.Length;
			switch (num)
			{
			case 1:
				return ps[0].Multiply(ks[0]);
			case 2:
				return ECAlgorithms.SumOfTwoMultiplies(ps[0], ks[0], ps[1], ks[1]);
			default:
			{
				ECPoint eCPoint = ps[0];
				ECCurve curve = eCPoint.Curve;
				ECPoint[] array = new ECPoint[num];
				array[0] = eCPoint;
				for (int i = 1; i < num; i++)
				{
					array[i] = ECAlgorithms.ImportPoint(curve, ps[i]);
				}
				GlvEndomorphism glvEndomorphism = curve.GetEndomorphism() as GlvEndomorphism;
				if (glvEndomorphism != null)
				{
					return ECAlgorithms.ValidatePoint(ECAlgorithms.ImplSumOfMultipliesGlv(array, ks, glvEndomorphism));
				}
				return ECAlgorithms.ValidatePoint(ECAlgorithms.ImplSumOfMultiplies(array, ks));
			}
			}
		}

		public static ECPoint SumOfTwoMultiplies(ECPoint P, BigInteger a, ECPoint Q, BigInteger b)
		{
			ECCurve curve = P.Curve;
			Q = ECAlgorithms.ImportPoint(curve, Q);
			if (curve is F2mCurve)
			{
				F2mCurve f2mCurve = (F2mCurve)curve;
				if (f2mCurve.IsKoblitz)
				{
					return ECAlgorithms.ValidatePoint(P.Multiply(a).Add(Q.Multiply(b)));
				}
			}
			GlvEndomorphism glvEndomorphism = curve.GetEndomorphism() as GlvEndomorphism;
			if (glvEndomorphism != null)
			{
				return ECAlgorithms.ValidatePoint(ECAlgorithms.ImplSumOfMultipliesGlv(new ECPoint[]
				{
					P,
					Q
				}, new BigInteger[]
				{
					a,
					b
				}, glvEndomorphism));
			}
			return ECAlgorithms.ValidatePoint(ECAlgorithms.ImplShamirsTrickWNaf(P, a, Q, b));
		}

		public static ECPoint ShamirsTrick(ECPoint P, BigInteger k, ECPoint Q, BigInteger l)
		{
			ECCurve curve = P.Curve;
			Q = ECAlgorithms.ImportPoint(curve, Q);
			return ECAlgorithms.ValidatePoint(ECAlgorithms.ImplShamirsTrickJsf(P, k, Q, l));
		}

		public static ECPoint ImportPoint(ECCurve c, ECPoint p)
		{
			ECCurve curve = p.Curve;
			if (!c.Equals(curve))
			{
				throw new ArgumentException("Point must be on the same curve");
			}
			return c.ImportPoint(p);
		}

		public static void MontgomeryTrick(ECFieldElement[] zs, int off, int len)
		{
			ECAlgorithms.MontgomeryTrick(zs, off, len, null);
		}

		public static void MontgomeryTrick(ECFieldElement[] zs, int off, int len, ECFieldElement scale)
		{
			ECFieldElement[] array = new ECFieldElement[len];
			array[0] = zs[off];
			int i = 0;
			while (++i < len)
			{
				array[i] = array[i - 1].Multiply(zs[off + i]);
			}
			i--;
			if (scale != null)
			{
				array[i] = array[i].Multiply(scale);
			}
			ECFieldElement eCFieldElement = array[i].Invert();
			while (i > 0)
			{
				int num = off + i--;
				ECFieldElement b = zs[num];
				zs[num] = array[i].Multiply(eCFieldElement);
				eCFieldElement = eCFieldElement.Multiply(b);
			}
			zs[off] = eCFieldElement;
		}

		public static ECPoint ReferenceMultiply(ECPoint p, BigInteger k)
		{
			BigInteger bigInteger = k.Abs();
			ECPoint eCPoint = p.Curve.Infinity;
			int bitLength = bigInteger.BitLength;
			if (bitLength > 0)
			{
				if (bigInteger.TestBit(0))
				{
					eCPoint = p;
				}
				for (int i = 1; i < bitLength; i++)
				{
					p = p.Twice();
					if (bigInteger.TestBit(i))
					{
						eCPoint = eCPoint.Add(p);
					}
				}
			}
			if (k.SignValue >= 0)
			{
				return eCPoint;
			}
			return eCPoint.Negate();
		}

		public static ECPoint ValidatePoint(ECPoint p)
		{
			if (!p.IsValid())
			{
				throw new ArgumentException("Invalid point", "p");
			}
			return p;
		}

		internal static ECPoint ImplShamirsTrickJsf(ECPoint P, BigInteger k, ECPoint Q, BigInteger l)
		{
			ECCurve curve = P.Curve;
			ECPoint infinity = curve.Infinity;
			ECPoint eCPoint = P.Add(Q);
			ECPoint eCPoint2 = P.Subtract(Q);
			ECPoint[] array = new ECPoint[]
			{
				Q,
				eCPoint2,
				P,
				eCPoint
			};
			curve.NormalizeAll(array);
			ECPoint[] array2 = new ECPoint[]
			{
				array[3].Negate(),
				array[2].Negate(),
				array[1].Negate(),
				array[0].Negate(),
				infinity,
				array[0],
				array[1],
				array[2],
				array[3]
			};
			byte[] array3 = WNafUtilities.GenerateJsf(k, l);
			ECPoint eCPoint3 = infinity;
			int num = array3.Length;
			while (--num >= 0)
			{
				int num2 = (int)array3[num];
				int num3 = num2 << 24 >> 28;
				int num4 = num2 << 28 >> 28;
				int num5 = 4 + num3 * 3 + num4;
				eCPoint3 = eCPoint3.TwicePlus(array2[num5]);
			}
			return eCPoint3;
		}

		internal static ECPoint ImplShamirsTrickWNaf(ECPoint P, BigInteger k, ECPoint Q, BigInteger l)
		{
			bool flag = k.SignValue < 0;
			bool flag2 = l.SignValue < 0;
			k = k.Abs();
			l = l.Abs();
			int width = Math.Max(2, Math.Min(16, WNafUtilities.GetWindowSize(k.BitLength)));
			int width2 = Math.Max(2, Math.Min(16, WNafUtilities.GetWindowSize(l.BitLength)));
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.Precompute(P, width, true);
			WNafPreCompInfo wNafPreCompInfo2 = WNafUtilities.Precompute(Q, width2, true);
			ECPoint[] preCompP = flag ? wNafPreCompInfo.PreCompNeg : wNafPreCompInfo.PreComp;
			ECPoint[] preCompQ = flag2 ? wNafPreCompInfo2.PreCompNeg : wNafPreCompInfo2.PreComp;
			ECPoint[] preCompNegP = flag ? wNafPreCompInfo.PreComp : wNafPreCompInfo.PreCompNeg;
			ECPoint[] preCompNegQ = flag2 ? wNafPreCompInfo2.PreComp : wNafPreCompInfo2.PreCompNeg;
			byte[] wnafP = WNafUtilities.GenerateWindowNaf(width, k);
			byte[] wnafQ = WNafUtilities.GenerateWindowNaf(width2, l);
			return ECAlgorithms.ImplShamirsTrickWNaf(preCompP, preCompNegP, wnafP, preCompQ, preCompNegQ, wnafQ);
		}

		internal static ECPoint ImplShamirsTrickWNaf(ECPoint P, BigInteger k, ECPointMap pointMapQ, BigInteger l)
		{
			bool flag = k.SignValue < 0;
			bool flag2 = l.SignValue < 0;
			k = k.Abs();
			l = l.Abs();
			int width = Math.Max(2, Math.Min(16, WNafUtilities.GetWindowSize(Math.Max(k.BitLength, l.BitLength))));
			ECPoint p = WNafUtilities.MapPointWithPrecomp(P, width, true, pointMapQ);
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.GetWNafPreCompInfo(P);
			WNafPreCompInfo wNafPreCompInfo2 = WNafUtilities.GetWNafPreCompInfo(p);
			ECPoint[] preCompP = flag ? wNafPreCompInfo.PreCompNeg : wNafPreCompInfo.PreComp;
			ECPoint[] preCompQ = flag2 ? wNafPreCompInfo2.PreCompNeg : wNafPreCompInfo2.PreComp;
			ECPoint[] preCompNegP = flag ? wNafPreCompInfo.PreComp : wNafPreCompInfo.PreCompNeg;
			ECPoint[] preCompNegQ = flag2 ? wNafPreCompInfo2.PreComp : wNafPreCompInfo2.PreCompNeg;
			byte[] wnafP = WNafUtilities.GenerateWindowNaf(width, k);
			byte[] wnafQ = WNafUtilities.GenerateWindowNaf(width, l);
			return ECAlgorithms.ImplShamirsTrickWNaf(preCompP, preCompNegP, wnafP, preCompQ, preCompNegQ, wnafQ);
		}

		private static ECPoint ImplShamirsTrickWNaf(ECPoint[] preCompP, ECPoint[] preCompNegP, byte[] wnafP, ECPoint[] preCompQ, ECPoint[] preCompNegQ, byte[] wnafQ)
		{
			int num = Math.Max(wnafP.Length, wnafQ.Length);
			ECCurve curve = preCompP[0].Curve;
			ECPoint infinity = curve.Infinity;
			ECPoint eCPoint = infinity;
			int num2 = 0;
			for (int i = num - 1; i >= 0; i--)
			{
				int num3 = (int)((i < wnafP.Length) ? ((sbyte)wnafP[i]) : 0);
				int num4 = (int)((i < wnafQ.Length) ? ((sbyte)wnafQ[i]) : 0);
				if ((num3 | num4) == 0)
				{
					num2++;
				}
				else
				{
					ECPoint eCPoint2 = infinity;
					if (num3 != 0)
					{
						int num5 = Math.Abs(num3);
						ECPoint[] array = (num3 < 0) ? preCompNegP : preCompP;
						eCPoint2 = eCPoint2.Add(array[num5 >> 1]);
					}
					if (num4 != 0)
					{
						int num6 = Math.Abs(num4);
						ECPoint[] array2 = (num4 < 0) ? preCompNegQ : preCompQ;
						eCPoint2 = eCPoint2.Add(array2[num6 >> 1]);
					}
					if (num2 > 0)
					{
						eCPoint = eCPoint.TimesPow2(num2);
						num2 = 0;
					}
					eCPoint = eCPoint.TwicePlus(eCPoint2);
				}
			}
			if (num2 > 0)
			{
				eCPoint = eCPoint.TimesPow2(num2);
			}
			return eCPoint;
		}

		internal static ECPoint ImplSumOfMultiplies(ECPoint[] ps, BigInteger[] ks)
		{
			int num = ps.Length;
			bool[] array = new bool[num];
			WNafPreCompInfo[] array2 = new WNafPreCompInfo[num];
			byte[][] array3 = new byte[num][];
			for (int i = 0; i < num; i++)
			{
				BigInteger bigInteger = ks[i];
				array[i] = (bigInteger.SignValue < 0);
				bigInteger = bigInteger.Abs();
				int width = Math.Max(2, Math.Min(16, WNafUtilities.GetWindowSize(bigInteger.BitLength)));
				array2[i] = WNafUtilities.Precompute(ps[i], width, true);
				array3[i] = WNafUtilities.GenerateWindowNaf(width, bigInteger);
			}
			return ECAlgorithms.ImplSumOfMultiplies(array, array2, array3);
		}

		internal static ECPoint ImplSumOfMultipliesGlv(ECPoint[] ps, BigInteger[] ks, GlvEndomorphism glvEndomorphism)
		{
			BigInteger order = ps[0].Curve.Order;
			int num = ps.Length;
			BigInteger[] array = new BigInteger[num << 1];
			int i = 0;
			int num2 = 0;
			while (i < num)
			{
				BigInteger[] array2 = glvEndomorphism.DecomposeScalar(ks[i].Mod(order));
				array[num2++] = array2[0];
				array[num2++] = array2[1];
				i++;
			}
			ECPointMap pointMap = glvEndomorphism.PointMap;
			if (glvEndomorphism.HasEfficientPointMap)
			{
				return ECAlgorithms.ImplSumOfMultiplies(ps, pointMap, array);
			}
			ECPoint[] array3 = new ECPoint[num << 1];
			int j = 0;
			int num3 = 0;
			while (j < num)
			{
				ECPoint eCPoint = ps[j];
				ECPoint eCPoint2 = pointMap.Map(eCPoint);
				array3[num3++] = eCPoint;
				array3[num3++] = eCPoint2;
				j++;
			}
			return ECAlgorithms.ImplSumOfMultiplies(array3, array);
		}

		internal static ECPoint ImplSumOfMultiplies(ECPoint[] ps, ECPointMap pointMap, BigInteger[] ks)
		{
			int num = ps.Length;
			int num2 = num << 1;
			bool[] array = new bool[num2];
			WNafPreCompInfo[] array2 = new WNafPreCompInfo[num2];
			byte[][] array3 = new byte[num2][];
			for (int i = 0; i < num; i++)
			{
				int num3 = i << 1;
				int num4 = num3 + 1;
				BigInteger bigInteger = ks[num3];
				array[num3] = (bigInteger.SignValue < 0);
				bigInteger = bigInteger.Abs();
				BigInteger bigInteger2 = ks[num4];
				array[num4] = (bigInteger2.SignValue < 0);
				bigInteger2 = bigInteger2.Abs();
				int width = Math.Max(2, Math.Min(16, WNafUtilities.GetWindowSize(Math.Max(bigInteger.BitLength, bigInteger2.BitLength))));
				ECPoint p = ps[i];
				ECPoint p2 = WNafUtilities.MapPointWithPrecomp(p, width, true, pointMap);
				array2[num3] = WNafUtilities.GetWNafPreCompInfo(p);
				array2[num4] = WNafUtilities.GetWNafPreCompInfo(p2);
				array3[num3] = WNafUtilities.GenerateWindowNaf(width, bigInteger);
				array3[num4] = WNafUtilities.GenerateWindowNaf(width, bigInteger2);
			}
			return ECAlgorithms.ImplSumOfMultiplies(array, array2, array3);
		}

		private static ECPoint ImplSumOfMultiplies(bool[] negs, WNafPreCompInfo[] infos, byte[][] wnafs)
		{
			int num = 0;
			int num2 = wnafs.Length;
			for (int i = 0; i < num2; i++)
			{
				num = Math.Max(num, wnafs[i].Length);
			}
			ECCurve curve = infos[0].PreComp[0].Curve;
			ECPoint infinity = curve.Infinity;
			ECPoint eCPoint = infinity;
			int num3 = 0;
			for (int j = num - 1; j >= 0; j--)
			{
				ECPoint eCPoint2 = infinity;
				for (int k = 0; k < num2; k++)
				{
					byte[] array = wnafs[k];
					int num4 = (int)((j < array.Length) ? ((sbyte)array[j]) : 0);
					if (num4 != 0)
					{
						int num5 = Math.Abs(num4);
						WNafPreCompInfo wNafPreCompInfo = infos[k];
						ECPoint[] array2 = (num4 < 0 == negs[k]) ? wNafPreCompInfo.PreComp : wNafPreCompInfo.PreCompNeg;
						eCPoint2 = eCPoint2.Add(array2[num5 >> 1]);
					}
				}
				if (eCPoint2 == infinity)
				{
					num3++;
				}
				else
				{
					if (num3 > 0)
					{
						eCPoint = eCPoint.TimesPow2(num3);
						num3 = 0;
					}
					eCPoint = eCPoint.TwicePlus(eCPoint2);
				}
			}
			if (num3 > 0)
			{
				eCPoint = eCPoint.TimesPow2(num3);
			}
			return eCPoint;
		}
	}
}
