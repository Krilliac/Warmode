using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class WNafL2RMultiplier : AbstractECMultiplier
	{
		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			int num = Math.Max(2, Math.Min(16, this.GetWindowSize(k.BitLength)));
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.Precompute(p, num, true);
			ECPoint[] preComp = wNafPreCompInfo.PreComp;
			ECPoint[] preCompNeg = wNafPreCompInfo.PreCompNeg;
			int[] array = WNafUtilities.GenerateCompactWindowNaf(num, k);
			ECPoint eCPoint = p.Curve.Infinity;
			int i = array.Length;
			if (i > 1)
			{
				int num2 = array[--i];
				int num3 = num2 >> 16;
				int num4 = num2 & 65535;
				int num5 = Math.Abs(num3);
				ECPoint[] array2 = (num3 < 0) ? preCompNeg : preComp;
				if (num5 << 2 < 1 << num)
				{
					int num6 = (int)LongArray.BitLengths[num5];
					int num7 = num - num6;
					int num8 = num5 ^ 1 << num6 - 1;
					int num9 = (1 << num - 1) - 1;
					int num10 = (num8 << num7) + 1;
					eCPoint = array2[num9 >> 1].Add(array2[num10 >> 1]);
					num4 -= num7;
				}
				else
				{
					eCPoint = array2[num5 >> 1];
				}
				eCPoint = eCPoint.TimesPow2(num4);
			}
			while (i > 0)
			{
				int num11 = array[--i];
				int num12 = num11 >> 16;
				int e = num11 & 65535;
				int num13 = Math.Abs(num12);
				ECPoint[] array3 = (num12 < 0) ? preCompNeg : preComp;
				ECPoint b = array3[num13 >> 1];
				eCPoint = eCPoint.TwicePlus(b);
				eCPoint = eCPoint.TimesPow2(e);
			}
			return eCPoint;
		}

		protected virtual int GetWindowSize(int bits)
		{
			return WNafUtilities.GetWindowSize(bits);
		}
	}
}
