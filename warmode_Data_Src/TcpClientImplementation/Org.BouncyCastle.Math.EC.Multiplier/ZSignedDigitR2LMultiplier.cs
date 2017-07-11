using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class ZSignedDigitR2LMultiplier : AbstractECMultiplier
	{
		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			ECPoint eCPoint = p.Curve.Infinity;
			int bitLength = k.BitLength;
			int lowestSetBit = k.GetLowestSetBit();
			ECPoint eCPoint2 = p.TimesPow2(lowestSetBit);
			int num = lowestSetBit;
			while (++num < bitLength)
			{
				eCPoint = eCPoint.Add(k.TestBit(num) ? eCPoint2 : eCPoint2.Negate());
				eCPoint2 = eCPoint2.Twice();
			}
			return eCPoint.Add(eCPoint2);
		}
	}
}
