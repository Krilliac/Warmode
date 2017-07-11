using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class NafR2LMultiplier : AbstractECMultiplier
	{
		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			int[] array = WNafUtilities.GenerateCompactNaf(k);
			ECPoint eCPoint = p.Curve.Infinity;
			ECPoint eCPoint2 = p;
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i];
				int num3 = num2 >> 16;
				num += (num2 & 65535);
				eCPoint2 = eCPoint2.TimesPow2(num);
				eCPoint = eCPoint.Add((num3 < 0) ? eCPoint2.Negate() : eCPoint2);
				num = 1;
			}
			return eCPoint;
		}
	}
}
