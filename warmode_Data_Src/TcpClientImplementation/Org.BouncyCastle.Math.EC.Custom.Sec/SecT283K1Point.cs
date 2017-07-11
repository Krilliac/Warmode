using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT283K1Point : AbstractF2mPoint
	{
		public override ECFieldElement YCoord
		{
			get
			{
				ECFieldElement rawXCoord = base.RawXCoord;
				ECFieldElement rawYCoord = base.RawYCoord;
				if (base.IsInfinity || rawXCoord.IsZero)
				{
					return rawYCoord;
				}
				ECFieldElement eCFieldElement = rawYCoord.Add(rawXCoord).Multiply(rawXCoord);
				ECFieldElement eCFieldElement2 = base.RawZCoords[0];
				if (!eCFieldElement2.IsOne)
				{
					eCFieldElement = eCFieldElement.Divide(eCFieldElement2);
				}
				return eCFieldElement;
			}
		}

		protected internal override bool CompressionYTilde
		{
			get
			{
				ECFieldElement rawXCoord = base.RawXCoord;
				if (rawXCoord.IsZero)
				{
					return false;
				}
				ECFieldElement rawYCoord = base.RawYCoord;
				return rawYCoord.TestBitZero() != rawXCoord.TestBitZero();
			}
		}

		public SecT283K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y) : this(curve, x, y, false)
		{
		}

		public SecT283K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : base(curve, x, y, withCompression)
		{
			if (x == null != (y == null))
			{
				throw new ArgumentException("Exactly one of the field elements is null");
			}
		}

		internal SecT283K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression) : base(curve, x, y, zs, withCompression)
		{
		}

		protected override ECPoint Detach()
		{
			return new SecT283K1Point(null, this.AffineXCoord, this.AffineYCoord);
		}

		public override ECPoint Add(ECPoint b)
		{
			if (base.IsInfinity)
			{
				return b;
			}
			if (b.IsInfinity)
			{
				return this;
			}
			ECCurve curve = this.Curve;
			ECFieldElement eCFieldElement = base.RawXCoord;
			ECFieldElement rawXCoord = b.RawXCoord;
			if (eCFieldElement.IsZero)
			{
				if (rawXCoord.IsZero)
				{
					return curve.Infinity;
				}
				return b.Add(this);
			}
			else
			{
				ECFieldElement rawYCoord = base.RawYCoord;
				ECFieldElement eCFieldElement2 = base.RawZCoords[0];
				ECFieldElement rawYCoord2 = b.RawYCoord;
				ECFieldElement eCFieldElement3 = b.RawZCoords[0];
				bool isOne = eCFieldElement2.IsOne;
				ECFieldElement eCFieldElement4 = rawXCoord;
				ECFieldElement eCFieldElement5 = rawYCoord2;
				if (!isOne)
				{
					eCFieldElement4 = eCFieldElement4.Multiply(eCFieldElement2);
					eCFieldElement5 = eCFieldElement5.Multiply(eCFieldElement2);
				}
				bool isOne2 = eCFieldElement3.IsOne;
				ECFieldElement eCFieldElement6 = eCFieldElement;
				ECFieldElement eCFieldElement7 = rawYCoord;
				if (!isOne2)
				{
					eCFieldElement6 = eCFieldElement6.Multiply(eCFieldElement3);
					eCFieldElement7 = eCFieldElement7.Multiply(eCFieldElement3);
				}
				ECFieldElement eCFieldElement8 = eCFieldElement7.Add(eCFieldElement5);
				ECFieldElement eCFieldElement9 = eCFieldElement6.Add(eCFieldElement4);
				if (!eCFieldElement9.IsZero)
				{
					ECFieldElement eCFieldElement11;
					ECFieldElement y;
					ECFieldElement eCFieldElement13;
					if (rawXCoord.IsZero)
					{
						ECPoint eCPoint = this.Normalize();
						eCFieldElement = eCPoint.XCoord;
						ECFieldElement yCoord = eCPoint.YCoord;
						ECFieldElement b2 = rawYCoord2;
						ECFieldElement eCFieldElement10 = yCoord.Add(b2).Divide(eCFieldElement);
						eCFieldElement11 = eCFieldElement10.Square().Add(eCFieldElement10).Add(eCFieldElement);
						if (eCFieldElement11.IsZero)
						{
							return new SecT283K1Point(curve, eCFieldElement11, curve.B, base.IsCompressed);
						}
						ECFieldElement eCFieldElement12 = eCFieldElement10.Multiply(eCFieldElement.Add(eCFieldElement11)).Add(eCFieldElement11).Add(yCoord);
						y = eCFieldElement12.Divide(eCFieldElement11).Add(eCFieldElement11);
						eCFieldElement13 = curve.FromBigInteger(BigInteger.One);
					}
					else
					{
						eCFieldElement9 = eCFieldElement9.Square();
						ECFieldElement eCFieldElement14 = eCFieldElement8.Multiply(eCFieldElement6);
						ECFieldElement eCFieldElement15 = eCFieldElement8.Multiply(eCFieldElement4);
						eCFieldElement11 = eCFieldElement14.Multiply(eCFieldElement15);
						if (eCFieldElement11.IsZero)
						{
							return new SecT283K1Point(curve, eCFieldElement11, curve.B, base.IsCompressed);
						}
						ECFieldElement eCFieldElement16 = eCFieldElement8.Multiply(eCFieldElement9);
						if (!isOne2)
						{
							eCFieldElement16 = eCFieldElement16.Multiply(eCFieldElement3);
						}
						y = eCFieldElement15.Add(eCFieldElement9).SquarePlusProduct(eCFieldElement16, rawYCoord.Add(eCFieldElement2));
						eCFieldElement13 = eCFieldElement16;
						if (!isOne)
						{
							eCFieldElement13 = eCFieldElement13.Multiply(eCFieldElement2);
						}
					}
					return new SecT283K1Point(curve, eCFieldElement11, y, new ECFieldElement[]
					{
						eCFieldElement13
					}, base.IsCompressed);
				}
				if (eCFieldElement8.IsZero)
				{
					return this.Twice();
				}
				return curve.Infinity;
			}
		}

		public override ECPoint Twice()
		{
			if (base.IsInfinity)
			{
				return this;
			}
			ECCurve curve = this.Curve;
			ECFieldElement rawXCoord = base.RawXCoord;
			if (rawXCoord.IsZero)
			{
				return curve.Infinity;
			}
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement eCFieldElement = base.RawZCoords[0];
			bool isOne = eCFieldElement.IsOne;
			ECFieldElement eCFieldElement2 = isOne ? eCFieldElement : eCFieldElement.Square();
			ECFieldElement eCFieldElement3;
			if (isOne)
			{
				eCFieldElement3 = rawYCoord.Square().Add(rawYCoord);
			}
			else
			{
				eCFieldElement3 = rawYCoord.Add(eCFieldElement).Multiply(rawYCoord);
			}
			if (eCFieldElement3.IsZero)
			{
				return new SecT283K1Point(curve, eCFieldElement3, curve.B, base.IsCompressed);
			}
			ECFieldElement eCFieldElement4 = eCFieldElement3.Square();
			ECFieldElement eCFieldElement5 = isOne ? eCFieldElement3 : eCFieldElement3.Multiply(eCFieldElement2);
			ECFieldElement eCFieldElement6 = rawYCoord.Add(rawXCoord).Square();
			ECFieldElement b = isOne ? eCFieldElement : eCFieldElement2.Square();
			ECFieldElement y = eCFieldElement6.Add(eCFieldElement3).Add(eCFieldElement2).Multiply(eCFieldElement6).Add(b).Add(eCFieldElement4).Add(eCFieldElement5);
			return new SecT283K1Point(curve, eCFieldElement4, y, new ECFieldElement[]
			{
				eCFieldElement5
			}, base.IsCompressed);
		}

		public override ECPoint TwicePlus(ECPoint b)
		{
			if (base.IsInfinity)
			{
				return b;
			}
			if (b.IsInfinity)
			{
				return this.Twice();
			}
			ECCurve curve = this.Curve;
			ECFieldElement rawXCoord = base.RawXCoord;
			if (rawXCoord.IsZero)
			{
				return b;
			}
			ECFieldElement rawXCoord2 = b.RawXCoord;
			ECFieldElement eCFieldElement = b.RawZCoords[0];
			if (rawXCoord2.IsZero || !eCFieldElement.IsOne)
			{
				return this.Twice().Add(b);
			}
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement eCFieldElement2 = base.RawZCoords[0];
			ECFieldElement rawYCoord2 = b.RawYCoord;
			ECFieldElement x = rawXCoord.Square();
			ECFieldElement eCFieldElement3 = rawYCoord.Square();
			ECFieldElement eCFieldElement4 = eCFieldElement2.Square();
			ECFieldElement b2 = rawYCoord.Multiply(eCFieldElement2);
			ECFieldElement b3 = eCFieldElement3.Add(b2);
			ECFieldElement eCFieldElement5 = rawYCoord2.AddOne();
			ECFieldElement eCFieldElement6 = eCFieldElement5.Multiply(eCFieldElement4).Add(eCFieldElement3).MultiplyPlusProduct(b3, x, eCFieldElement4);
			ECFieldElement eCFieldElement7 = rawXCoord2.Multiply(eCFieldElement4);
			ECFieldElement eCFieldElement8 = eCFieldElement7.Add(b3).Square();
			if (eCFieldElement8.IsZero)
			{
				if (eCFieldElement6.IsZero)
				{
					return b.Twice();
				}
				return curve.Infinity;
			}
			else
			{
				if (eCFieldElement6.IsZero)
				{
					return new SecT283K1Point(curve, eCFieldElement6, curve.B, base.IsCompressed);
				}
				ECFieldElement x2 = eCFieldElement6.Square().Multiply(eCFieldElement7);
				ECFieldElement eCFieldElement9 = eCFieldElement6.Multiply(eCFieldElement8).Multiply(eCFieldElement4);
				ECFieldElement y = eCFieldElement6.Add(eCFieldElement8).Square().MultiplyPlusProduct(b3, eCFieldElement5, eCFieldElement9);
				return new SecT283K1Point(curve, x2, y, new ECFieldElement[]
				{
					eCFieldElement9
				}, base.IsCompressed);
			}
		}

		public override ECPoint Negate()
		{
			if (base.IsInfinity)
			{
				return this;
			}
			ECFieldElement rawXCoord = base.RawXCoord;
			if (rawXCoord.IsZero)
			{
				return this;
			}
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement eCFieldElement = base.RawZCoords[0];
			return new SecT283K1Point(this.Curve, rawXCoord, rawYCoord.Add(eCFieldElement), new ECFieldElement[]
			{
				eCFieldElement
			}, base.IsCompressed);
		}
	}
}
