using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT163K1Point : AbstractF2mPoint
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

		public SecT163K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y) : this(curve, x, y, false)
		{
		}

		public SecT163K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : base(curve, x, y, withCompression)
		{
			if (x == null != (y == null))
			{
				throw new ArgumentException("Exactly one of the field elements is null");
			}
		}

		internal SecT163K1Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression) : base(curve, x, y, zs, withCompression)
		{
		}

		protected override ECPoint Detach()
		{
			return new SecT163K1Point(null, this.AffineXCoord, this.AffineYCoord);
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
						eCFieldElement11 = eCFieldElement10.Square().Add(eCFieldElement10).Add(eCFieldElement).AddOne();
						if (eCFieldElement11.IsZero)
						{
							return new SecT163K1Point(curve, eCFieldElement11, curve.B, base.IsCompressed);
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
							return new SecT163K1Point(curve, eCFieldElement11, curve.B, base.IsCompressed);
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
					return new SecT163K1Point(curve, eCFieldElement11, y, new ECFieldElement[]
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
			ECFieldElement b = isOne ? rawYCoord : rawYCoord.Multiply(eCFieldElement);
			ECFieldElement b2 = isOne ? eCFieldElement : eCFieldElement.Square();
			ECFieldElement eCFieldElement2 = rawYCoord.Square().Add(b).Add(b2);
			if (eCFieldElement2.IsZero)
			{
				return new SecT163K1Point(curve, eCFieldElement2, curve.B, base.IsCompressed);
			}
			ECFieldElement eCFieldElement3 = eCFieldElement2.Square();
			ECFieldElement eCFieldElement4 = isOne ? eCFieldElement2 : eCFieldElement2.Multiply(b2);
			ECFieldElement eCFieldElement5 = rawYCoord.Add(rawXCoord).Square();
			ECFieldElement y = eCFieldElement5.Add(eCFieldElement2).Add(b2).Multiply(eCFieldElement5).Add(eCFieldElement3);
			return new SecT163K1Point(curve, eCFieldElement3, y, new ECFieldElement[]
			{
				eCFieldElement4
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
			ECFieldElement b2 = rawYCoord.Square();
			ECFieldElement eCFieldElement3 = eCFieldElement2.Square();
			ECFieldElement b3 = rawYCoord.Multiply(eCFieldElement2);
			ECFieldElement b4 = eCFieldElement3.Add(b2).Add(b3);
			ECFieldElement x2 = rawYCoord2.AddOne();
			ECFieldElement eCFieldElement4 = rawYCoord2.Multiply(eCFieldElement3).Add(b2).MultiplyPlusProduct(b4, x, eCFieldElement3);
			ECFieldElement eCFieldElement5 = rawXCoord2.Multiply(eCFieldElement3);
			ECFieldElement eCFieldElement6 = eCFieldElement5.Add(b4).Square();
			if (eCFieldElement6.IsZero)
			{
				if (eCFieldElement4.IsZero)
				{
					return b.Twice();
				}
				return curve.Infinity;
			}
			else
			{
				if (eCFieldElement4.IsZero)
				{
					return new SecT163K1Point(curve, eCFieldElement4, curve.B, base.IsCompressed);
				}
				ECFieldElement x3 = eCFieldElement4.Square().Multiply(eCFieldElement5);
				ECFieldElement eCFieldElement7 = eCFieldElement4.Multiply(eCFieldElement6).Multiply(eCFieldElement3);
				ECFieldElement y = eCFieldElement4.Add(eCFieldElement6).Square().MultiplyPlusProduct(b4, x2, eCFieldElement7);
				return new SecT163K1Point(curve, x3, y, new ECFieldElement[]
				{
					eCFieldElement7
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
			return new SecT163K1Point(this.Curve, rawXCoord, rawYCoord.Add(eCFieldElement), new ECFieldElement[]
			{
				eCFieldElement
			}, base.IsCompressed);
		}
	}
}
