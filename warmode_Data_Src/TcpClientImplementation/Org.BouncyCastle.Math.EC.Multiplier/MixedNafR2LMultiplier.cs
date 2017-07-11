using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class MixedNafR2LMultiplier : AbstractECMultiplier
	{
		protected readonly int additionCoord;

		protected readonly int doublingCoord;

		public MixedNafR2LMultiplier() : this(2, 4)
		{
		}

		public MixedNafR2LMultiplier(int additionCoord, int doublingCoord)
		{
			this.additionCoord = additionCoord;
			this.doublingCoord = doublingCoord;
		}

		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			ECCurve curve = p.Curve;
			ECCurve eCCurve = this.ConfigureCurve(curve, this.additionCoord);
			ECCurve eCCurve2 = this.ConfigureCurve(curve, this.doublingCoord);
			int[] array = WNafUtilities.GenerateCompactNaf(k);
			ECPoint eCPoint = eCCurve.Infinity;
			ECPoint eCPoint2 = eCCurve2.ImportPoint(p);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i];
				int num3 = num2 >> 16;
				num += (num2 & 65535);
				eCPoint2 = eCPoint2.TimesPow2(num);
				ECPoint eCPoint3 = eCCurve.ImportPoint(eCPoint2);
				if (num3 < 0)
				{
					eCPoint3 = eCPoint3.Negate();
				}
				eCPoint = eCPoint.Add(eCPoint3);
				num = 1;
			}
			return curve.ImportPoint(eCPoint);
		}

		protected virtual ECCurve ConfigureCurve(ECCurve c, int coord)
		{
			if (c.CoordinateSystem == coord)
			{
				return c;
			}
			if (!c.SupportsCoordinateSystem(coord))
			{
				throw new ArgumentException("Coordinate system " + coord + " not supported by this curve", "coord");
			}
			return c.Configure().SetCoordinateSystem(coord).Create();
		}
	}
}
