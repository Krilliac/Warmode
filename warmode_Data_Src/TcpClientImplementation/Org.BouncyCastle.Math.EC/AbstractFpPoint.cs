using System;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class AbstractFpPoint : ECPointBase
	{
		protected internal override bool CompressionYTilde
		{
			get
			{
				return this.AffineYCoord.TestBitZero();
			}
		}

		protected AbstractFpPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : base(curve, x, y, withCompression)
		{
		}

		protected AbstractFpPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression) : base(curve, x, y, zs, withCompression)
		{
		}

		protected override bool SatisfiesCurveEquation()
		{
			ECFieldElement rawXCoord = base.RawXCoord;
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement eCFieldElement = this.Curve.A;
			ECFieldElement eCFieldElement2 = this.Curve.B;
			ECFieldElement eCFieldElement3 = rawYCoord.Square();
			switch (this.CurveCoordinateSystem)
			{
			case 0:
				break;
			case 1:
			{
				ECFieldElement eCFieldElement4 = base.RawZCoords[0];
				if (!eCFieldElement4.IsOne)
				{
					ECFieldElement b = eCFieldElement4.Square();
					ECFieldElement b2 = eCFieldElement4.Multiply(b);
					eCFieldElement3 = eCFieldElement3.Multiply(eCFieldElement4);
					eCFieldElement = eCFieldElement.Multiply(b);
					eCFieldElement2 = eCFieldElement2.Multiply(b2);
				}
				break;
			}
			case 2:
			case 3:
			case 4:
			{
				ECFieldElement eCFieldElement5 = base.RawZCoords[0];
				if (!eCFieldElement5.IsOne)
				{
					ECFieldElement eCFieldElement6 = eCFieldElement5.Square();
					ECFieldElement b3 = eCFieldElement6.Square();
					ECFieldElement b4 = eCFieldElement6.Multiply(b3);
					eCFieldElement = eCFieldElement.Multiply(b3);
					eCFieldElement2 = eCFieldElement2.Multiply(b4);
				}
				break;
			}
			default:
				throw new InvalidOperationException("unsupported coordinate system");
			}
			ECFieldElement other = rawXCoord.Square().Add(eCFieldElement).Multiply(rawXCoord).Add(eCFieldElement2);
			return eCFieldElement3.Equals(other);
		}

		public override ECPoint Subtract(ECPoint b)
		{
			if (b.IsInfinity)
			{
				return this;
			}
			return this.Add(b.Negate());
		}
	}
}
