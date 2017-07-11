using System;

namespace Org.BouncyCastle.Math.EC
{
	public class FpPoint : AbstractFpPoint
	{
		public FpPoint(ECCurve curve, ECFieldElement x, ECFieldElement y) : this(curve, x, y, false)
		{
		}

		public FpPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : base(curve, x, y, withCompression)
		{
			if (x == null != (y == null))
			{
				throw new ArgumentException("Exactly one of the field elements is null");
			}
		}

		internal FpPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression) : base(curve, x, y, zs, withCompression)
		{
		}

		protected override ECPoint Detach()
		{
			return new FpPoint(null, this.AffineXCoord, this.AffineYCoord);
		}

		public override ECFieldElement GetZCoord(int index)
		{
			if (index == 1 && 4 == this.CurveCoordinateSystem)
			{
				return this.GetJacobianModifiedW();
			}
			return base.GetZCoord(index);
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
			if (this == b)
			{
				return this.Twice();
			}
			ECCurve curve = this.Curve;
			int coordinateSystem = curve.CoordinateSystem;
			ECFieldElement rawXCoord = base.RawXCoord;
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement rawXCoord2 = b.RawXCoord;
			ECFieldElement rawYCoord2 = b.RawYCoord;
			switch (coordinateSystem)
			{
			case 0:
			{
				ECFieldElement eCFieldElement = rawXCoord2.Subtract(rawXCoord);
				ECFieldElement eCFieldElement2 = rawYCoord2.Subtract(rawYCoord);
				if (!eCFieldElement.IsZero)
				{
					ECFieldElement eCFieldElement3 = eCFieldElement2.Divide(eCFieldElement);
					ECFieldElement eCFieldElement4 = eCFieldElement3.Square().Subtract(rawXCoord).Subtract(rawXCoord2);
					ECFieldElement y = eCFieldElement3.Multiply(rawXCoord.Subtract(eCFieldElement4)).Subtract(rawYCoord);
					return new FpPoint(this.Curve, eCFieldElement4, y, base.IsCompressed);
				}
				if (eCFieldElement2.IsZero)
				{
					return this.Twice();
				}
				return this.Curve.Infinity;
			}
			case 1:
			{
				ECFieldElement eCFieldElement5 = base.RawZCoords[0];
				ECFieldElement eCFieldElement6 = b.RawZCoords[0];
				bool isOne = eCFieldElement5.IsOne;
				bool isOne2 = eCFieldElement6.IsOne;
				ECFieldElement eCFieldElement7 = isOne ? rawYCoord2 : rawYCoord2.Multiply(eCFieldElement5);
				ECFieldElement eCFieldElement8 = isOne2 ? rawYCoord : rawYCoord.Multiply(eCFieldElement6);
				ECFieldElement eCFieldElement9 = eCFieldElement7.Subtract(eCFieldElement8);
				ECFieldElement eCFieldElement10 = isOne ? rawXCoord2 : rawXCoord2.Multiply(eCFieldElement5);
				ECFieldElement b2 = isOne2 ? rawXCoord : rawXCoord.Multiply(eCFieldElement6);
				ECFieldElement eCFieldElement11 = eCFieldElement10.Subtract(b2);
				if (!eCFieldElement11.IsZero)
				{
					ECFieldElement b3 = isOne ? eCFieldElement6 : (isOne2 ? eCFieldElement5 : eCFieldElement5.Multiply(eCFieldElement6));
					ECFieldElement eCFieldElement12 = eCFieldElement11.Square();
					ECFieldElement eCFieldElement13 = eCFieldElement12.Multiply(eCFieldElement11);
					ECFieldElement eCFieldElement14 = eCFieldElement12.Multiply(b2);
					ECFieldElement b4 = eCFieldElement9.Square().Multiply(b3).Subtract(eCFieldElement13).Subtract(this.Two(eCFieldElement14));
					ECFieldElement x = eCFieldElement11.Multiply(b4);
					ECFieldElement y2 = eCFieldElement14.Subtract(b4).MultiplyMinusProduct(eCFieldElement9, eCFieldElement8, eCFieldElement13);
					ECFieldElement eCFieldElement15 = eCFieldElement13.Multiply(b3);
					return new FpPoint(curve, x, y2, new ECFieldElement[]
					{
						eCFieldElement15
					}, base.IsCompressed);
				}
				if (eCFieldElement9.IsZero)
				{
					return this.Twice();
				}
				return curve.Infinity;
			}
			case 2:
			case 4:
			{
				ECFieldElement eCFieldElement16 = base.RawZCoords[0];
				ECFieldElement eCFieldElement17 = b.RawZCoords[0];
				bool isOne3 = eCFieldElement16.IsOne;
				ECFieldElement zSquared = null;
				ECFieldElement eCFieldElement22;
				ECFieldElement y3;
				ECFieldElement eCFieldElement23;
				if (!isOne3 && eCFieldElement16.Equals(eCFieldElement17))
				{
					ECFieldElement eCFieldElement18 = rawXCoord.Subtract(rawXCoord2);
					ECFieldElement eCFieldElement19 = rawYCoord.Subtract(rawYCoord2);
					if (eCFieldElement18.IsZero)
					{
						if (eCFieldElement19.IsZero)
						{
							return this.Twice();
						}
						return curve.Infinity;
					}
					else
					{
						ECFieldElement eCFieldElement20 = eCFieldElement18.Square();
						ECFieldElement eCFieldElement21 = rawXCoord.Multiply(eCFieldElement20);
						ECFieldElement b5 = rawXCoord2.Multiply(eCFieldElement20);
						ECFieldElement b6 = eCFieldElement21.Subtract(b5).Multiply(rawYCoord);
						eCFieldElement22 = eCFieldElement19.Square().Subtract(eCFieldElement21).Subtract(b5);
						y3 = eCFieldElement21.Subtract(eCFieldElement22).Multiply(eCFieldElement19).Subtract(b6);
						eCFieldElement23 = eCFieldElement18;
						if (isOne3)
						{
							zSquared = eCFieldElement20;
						}
						else
						{
							eCFieldElement23 = eCFieldElement23.Multiply(eCFieldElement16);
						}
					}
				}
				else
				{
					ECFieldElement b7;
					ECFieldElement b8;
					if (isOne3)
					{
						b7 = rawXCoord2;
						b8 = rawYCoord2;
					}
					else
					{
						ECFieldElement eCFieldElement24 = eCFieldElement16.Square();
						b7 = eCFieldElement24.Multiply(rawXCoord2);
						ECFieldElement eCFieldElement25 = eCFieldElement24.Multiply(eCFieldElement16);
						b8 = eCFieldElement25.Multiply(rawYCoord2);
					}
					bool isOne4 = eCFieldElement17.IsOne;
					ECFieldElement eCFieldElement26;
					ECFieldElement eCFieldElement27;
					if (isOne4)
					{
						eCFieldElement26 = rawXCoord;
						eCFieldElement27 = rawYCoord;
					}
					else
					{
						ECFieldElement eCFieldElement28 = eCFieldElement17.Square();
						eCFieldElement26 = eCFieldElement28.Multiply(rawXCoord);
						ECFieldElement eCFieldElement29 = eCFieldElement28.Multiply(eCFieldElement17);
						eCFieldElement27 = eCFieldElement29.Multiply(rawYCoord);
					}
					ECFieldElement eCFieldElement30 = eCFieldElement26.Subtract(b7);
					ECFieldElement eCFieldElement31 = eCFieldElement27.Subtract(b8);
					if (eCFieldElement30.IsZero)
					{
						if (eCFieldElement31.IsZero)
						{
							return this.Twice();
						}
						return curve.Infinity;
					}
					else
					{
						ECFieldElement eCFieldElement32 = eCFieldElement30.Square();
						ECFieldElement eCFieldElement33 = eCFieldElement32.Multiply(eCFieldElement30);
						ECFieldElement eCFieldElement34 = eCFieldElement32.Multiply(eCFieldElement26);
						eCFieldElement22 = eCFieldElement31.Square().Add(eCFieldElement33).Subtract(this.Two(eCFieldElement34));
						y3 = eCFieldElement34.Subtract(eCFieldElement22).MultiplyMinusProduct(eCFieldElement31, eCFieldElement33, eCFieldElement27);
						eCFieldElement23 = eCFieldElement30;
						if (!isOne3)
						{
							eCFieldElement23 = eCFieldElement23.Multiply(eCFieldElement16);
						}
						if (!isOne4)
						{
							eCFieldElement23 = eCFieldElement23.Multiply(eCFieldElement17);
						}
						if (eCFieldElement23 == eCFieldElement30)
						{
							zSquared = eCFieldElement32;
						}
					}
				}
				ECFieldElement[] zs;
				if (coordinateSystem == 4)
				{
					ECFieldElement eCFieldElement35 = this.CalculateJacobianModifiedW(eCFieldElement23, zSquared);
					zs = new ECFieldElement[]
					{
						eCFieldElement23,
						eCFieldElement35
					};
				}
				else
				{
					zs = new ECFieldElement[]
					{
						eCFieldElement23
					};
				}
				return new FpPoint(curve, eCFieldElement22, y3, zs, base.IsCompressed);
			}
			}
			throw new InvalidOperationException("unsupported coordinate system");
		}

		public override ECPoint Twice()
		{
			if (base.IsInfinity)
			{
				return this;
			}
			ECCurve curve = this.Curve;
			ECFieldElement rawYCoord = base.RawYCoord;
			if (rawYCoord.IsZero)
			{
				return curve.Infinity;
			}
			int coordinateSystem = curve.CoordinateSystem;
			ECFieldElement rawXCoord = base.RawXCoord;
			switch (coordinateSystem)
			{
			case 0:
			{
				ECFieldElement x = rawXCoord.Square();
				ECFieldElement eCFieldElement = this.Three(x).Add(this.Curve.A).Divide(this.Two(rawYCoord));
				ECFieldElement eCFieldElement2 = eCFieldElement.Square().Subtract(this.Two(rawXCoord));
				ECFieldElement y = eCFieldElement.Multiply(rawXCoord.Subtract(eCFieldElement2)).Subtract(rawYCoord);
				return new FpPoint(this.Curve, eCFieldElement2, y, base.IsCompressed);
			}
			case 1:
			{
				ECFieldElement eCFieldElement3 = base.RawZCoords[0];
				bool isOne = eCFieldElement3.IsOne;
				ECFieldElement eCFieldElement4 = curve.A;
				if (!eCFieldElement4.IsZero && !isOne)
				{
					eCFieldElement4 = eCFieldElement4.Multiply(eCFieldElement3.Square());
				}
				eCFieldElement4 = eCFieldElement4.Add(this.Three(rawXCoord.Square()));
				ECFieldElement eCFieldElement5 = isOne ? rawYCoord : rawYCoord.Multiply(eCFieldElement3);
				ECFieldElement eCFieldElement6 = isOne ? rawYCoord.Square() : eCFieldElement5.Multiply(rawYCoord);
				ECFieldElement x2 = rawXCoord.Multiply(eCFieldElement6);
				ECFieldElement eCFieldElement7 = this.Four(x2);
				ECFieldElement eCFieldElement8 = eCFieldElement4.Square().Subtract(this.Two(eCFieldElement7));
				ECFieldElement eCFieldElement9 = this.Two(eCFieldElement5);
				ECFieldElement x3 = eCFieldElement8.Multiply(eCFieldElement9);
				ECFieldElement eCFieldElement10 = this.Two(eCFieldElement6);
				ECFieldElement y2 = eCFieldElement7.Subtract(eCFieldElement8).Multiply(eCFieldElement4).Subtract(this.Two(eCFieldElement10.Square()));
				ECFieldElement x4 = isOne ? this.Two(eCFieldElement10) : eCFieldElement9.Square();
				ECFieldElement eCFieldElement11 = this.Two(x4).Multiply(eCFieldElement5);
				return new FpPoint(curve, x3, y2, new ECFieldElement[]
				{
					eCFieldElement11
				}, base.IsCompressed);
			}
			case 2:
			{
				ECFieldElement eCFieldElement12 = base.RawZCoords[0];
				bool isOne2 = eCFieldElement12.IsOne;
				ECFieldElement eCFieldElement13 = rawYCoord.Square();
				ECFieldElement x5 = eCFieldElement13.Square();
				ECFieldElement a = curve.A;
				ECFieldElement eCFieldElement14 = a.Negate();
				ECFieldElement eCFieldElement15;
				ECFieldElement eCFieldElement16;
				if (eCFieldElement14.ToBigInteger().Equals(BigInteger.ValueOf(3L)))
				{
					ECFieldElement b = isOne2 ? eCFieldElement12 : eCFieldElement12.Square();
					eCFieldElement15 = this.Three(rawXCoord.Add(b).Multiply(rawXCoord.Subtract(b)));
					eCFieldElement16 = this.Four(eCFieldElement13.Multiply(rawXCoord));
				}
				else
				{
					ECFieldElement x6 = rawXCoord.Square();
					eCFieldElement15 = this.Three(x6);
					if (isOne2)
					{
						eCFieldElement15 = eCFieldElement15.Add(a);
					}
					else if (!a.IsZero)
					{
						ECFieldElement eCFieldElement17 = isOne2 ? eCFieldElement12 : eCFieldElement12.Square();
						ECFieldElement eCFieldElement18 = eCFieldElement17.Square();
						if (eCFieldElement14.BitLength < a.BitLength)
						{
							eCFieldElement15 = eCFieldElement15.Subtract(eCFieldElement18.Multiply(eCFieldElement14));
						}
						else
						{
							eCFieldElement15 = eCFieldElement15.Add(eCFieldElement18.Multiply(a));
						}
					}
					eCFieldElement16 = this.Four(rawXCoord.Multiply(eCFieldElement13));
				}
				ECFieldElement eCFieldElement19 = eCFieldElement15.Square().Subtract(this.Two(eCFieldElement16));
				ECFieldElement y3 = eCFieldElement16.Subtract(eCFieldElement19).Multiply(eCFieldElement15).Subtract(this.Eight(x5));
				ECFieldElement eCFieldElement20 = this.Two(rawYCoord);
				if (!isOne2)
				{
					eCFieldElement20 = eCFieldElement20.Multiply(eCFieldElement12);
				}
				return new FpPoint(curve, eCFieldElement19, y3, new ECFieldElement[]
				{
					eCFieldElement20
				}, base.IsCompressed);
			}
			case 4:
				return this.TwiceJacobianModified(true);
			}
			throw new InvalidOperationException("unsupported coordinate system");
		}

		public override ECPoint TwicePlus(ECPoint b)
		{
			if (this == b)
			{
				return this.ThreeTimes();
			}
			if (base.IsInfinity)
			{
				return b;
			}
			if (b.IsInfinity)
			{
				return this.Twice();
			}
			ECFieldElement rawYCoord = base.RawYCoord;
			if (rawYCoord.IsZero)
			{
				return b;
			}
			ECCurve curve = this.Curve;
			int coordinateSystem = curve.CoordinateSystem;
			int num = coordinateSystem;
			if (num != 0)
			{
				if (num != 4)
				{
					return this.Twice().Add(b);
				}
				return this.TwiceJacobianModified(false).Add(b);
			}
			else
			{
				ECFieldElement rawXCoord = base.RawXCoord;
				ECFieldElement rawXCoord2 = b.RawXCoord;
				ECFieldElement rawYCoord2 = b.RawYCoord;
				ECFieldElement eCFieldElement = rawXCoord2.Subtract(rawXCoord);
				ECFieldElement eCFieldElement2 = rawYCoord2.Subtract(rawYCoord);
				if (eCFieldElement.IsZero)
				{
					if (eCFieldElement2.IsZero)
					{
						return this.ThreeTimes();
					}
					return this;
				}
				else
				{
					ECFieldElement eCFieldElement3 = eCFieldElement.Square();
					ECFieldElement b2 = eCFieldElement2.Square();
					ECFieldElement eCFieldElement4 = eCFieldElement3.Multiply(this.Two(rawXCoord).Add(rawXCoord2)).Subtract(b2);
					if (eCFieldElement4.IsZero)
					{
						return this.Curve.Infinity;
					}
					ECFieldElement eCFieldElement5 = eCFieldElement4.Multiply(eCFieldElement);
					ECFieldElement b3 = eCFieldElement5.Invert();
					ECFieldElement eCFieldElement6 = eCFieldElement4.Multiply(b3).Multiply(eCFieldElement2);
					ECFieldElement eCFieldElement7 = this.Two(rawYCoord).Multiply(eCFieldElement3).Multiply(eCFieldElement).Multiply(b3).Subtract(eCFieldElement6);
					ECFieldElement eCFieldElement8 = eCFieldElement7.Subtract(eCFieldElement6).Multiply(eCFieldElement6.Add(eCFieldElement7)).Add(rawXCoord2);
					ECFieldElement y = rawXCoord.Subtract(eCFieldElement8).Multiply(eCFieldElement7).Subtract(rawYCoord);
					return new FpPoint(this.Curve, eCFieldElement8, y, base.IsCompressed);
				}
			}
		}

		public override ECPoint ThreeTimes()
		{
			if (base.IsInfinity)
			{
				return this;
			}
			ECFieldElement rawYCoord = base.RawYCoord;
			if (rawYCoord.IsZero)
			{
				return this;
			}
			ECCurve curve = this.Curve;
			int coordinateSystem = curve.CoordinateSystem;
			int num = coordinateSystem;
			if (num != 0)
			{
				if (num != 4)
				{
					return this.Twice().Add(this);
				}
				return this.TwiceJacobianModified(false).Add(this);
			}
			else
			{
				ECFieldElement rawXCoord = base.RawXCoord;
				ECFieldElement eCFieldElement = this.Two(rawYCoord);
				ECFieldElement eCFieldElement2 = eCFieldElement.Square();
				ECFieldElement eCFieldElement3 = this.Three(rawXCoord.Square()).Add(this.Curve.A);
				ECFieldElement b = eCFieldElement3.Square();
				ECFieldElement eCFieldElement4 = this.Three(rawXCoord).Multiply(eCFieldElement2).Subtract(b);
				if (eCFieldElement4.IsZero)
				{
					return this.Curve.Infinity;
				}
				ECFieldElement eCFieldElement5 = eCFieldElement4.Multiply(eCFieldElement);
				ECFieldElement b2 = eCFieldElement5.Invert();
				ECFieldElement eCFieldElement6 = eCFieldElement4.Multiply(b2).Multiply(eCFieldElement3);
				ECFieldElement eCFieldElement7 = eCFieldElement2.Square().Multiply(b2).Subtract(eCFieldElement6);
				ECFieldElement eCFieldElement8 = eCFieldElement7.Subtract(eCFieldElement6).Multiply(eCFieldElement6.Add(eCFieldElement7)).Add(rawXCoord);
				ECFieldElement y = rawXCoord.Subtract(eCFieldElement8).Multiply(eCFieldElement7).Subtract(rawYCoord);
				return new FpPoint(this.Curve, eCFieldElement8, y, base.IsCompressed);
			}
		}

		public override ECPoint TimesPow2(int e)
		{
			if (e < 0)
			{
				throw new ArgumentException("cannot be negative", "e");
			}
			if (e == 0 || base.IsInfinity)
			{
				return this;
			}
			if (e == 1)
			{
				return this.Twice();
			}
			ECCurve curve = this.Curve;
			ECFieldElement eCFieldElement = base.RawYCoord;
			if (eCFieldElement.IsZero)
			{
				return curve.Infinity;
			}
			int coordinateSystem = curve.CoordinateSystem;
			ECFieldElement eCFieldElement2 = curve.A;
			ECFieldElement eCFieldElement3 = base.RawXCoord;
			ECFieldElement eCFieldElement4 = (base.RawZCoords.Length < 1) ? curve.FromBigInteger(BigInteger.One) : base.RawZCoords[0];
			if (!eCFieldElement4.IsOne)
			{
				switch (coordinateSystem)
				{
				case 1:
				{
					ECFieldElement eCFieldElement5 = eCFieldElement4.Square();
					eCFieldElement3 = eCFieldElement3.Multiply(eCFieldElement4);
					eCFieldElement = eCFieldElement.Multiply(eCFieldElement5);
					eCFieldElement2 = this.CalculateJacobianModifiedW(eCFieldElement4, eCFieldElement5);
					break;
				}
				case 2:
					eCFieldElement2 = this.CalculateJacobianModifiedW(eCFieldElement4, null);
					break;
				case 4:
					eCFieldElement2 = this.GetJacobianModifiedW();
					break;
				}
			}
			for (int i = 0; i < e; i++)
			{
				if (eCFieldElement.IsZero)
				{
					return curve.Infinity;
				}
				ECFieldElement x = eCFieldElement3.Square();
				ECFieldElement eCFieldElement6 = this.Three(x);
				ECFieldElement eCFieldElement7 = this.Two(eCFieldElement);
				ECFieldElement eCFieldElement8 = eCFieldElement7.Multiply(eCFieldElement);
				ECFieldElement eCFieldElement9 = this.Two(eCFieldElement3.Multiply(eCFieldElement8));
				ECFieldElement x2 = eCFieldElement8.Square();
				ECFieldElement eCFieldElement10 = this.Two(x2);
				if (!eCFieldElement2.IsZero)
				{
					eCFieldElement6 = eCFieldElement6.Add(eCFieldElement2);
					eCFieldElement2 = this.Two(eCFieldElement10.Multiply(eCFieldElement2));
				}
				eCFieldElement3 = eCFieldElement6.Square().Subtract(this.Two(eCFieldElement9));
				eCFieldElement = eCFieldElement6.Multiply(eCFieldElement9.Subtract(eCFieldElement3)).Subtract(eCFieldElement10);
				eCFieldElement4 = (eCFieldElement4.IsOne ? eCFieldElement7 : eCFieldElement7.Multiply(eCFieldElement4));
			}
			switch (coordinateSystem)
			{
			case 0:
			{
				ECFieldElement eCFieldElement11 = eCFieldElement4.Invert();
				ECFieldElement eCFieldElement12 = eCFieldElement11.Square();
				ECFieldElement b = eCFieldElement12.Multiply(eCFieldElement11);
				return new FpPoint(curve, eCFieldElement3.Multiply(eCFieldElement12), eCFieldElement.Multiply(b), base.IsCompressed);
			}
			case 1:
				eCFieldElement3 = eCFieldElement3.Multiply(eCFieldElement4);
				eCFieldElement4 = eCFieldElement4.Multiply(eCFieldElement4.Square());
				return new FpPoint(curve, eCFieldElement3, eCFieldElement, new ECFieldElement[]
				{
					eCFieldElement4
				}, base.IsCompressed);
			case 2:
				return new FpPoint(curve, eCFieldElement3, eCFieldElement, new ECFieldElement[]
				{
					eCFieldElement4
				}, base.IsCompressed);
			case 4:
				return new FpPoint(curve, eCFieldElement3, eCFieldElement, new ECFieldElement[]
				{
					eCFieldElement4,
					eCFieldElement2
				}, base.IsCompressed);
			}
			throw new InvalidOperationException("unsupported coordinate system");
		}

		protected virtual ECFieldElement Two(ECFieldElement x)
		{
			return x.Add(x);
		}

		protected virtual ECFieldElement Three(ECFieldElement x)
		{
			return this.Two(x).Add(x);
		}

		protected virtual ECFieldElement Four(ECFieldElement x)
		{
			return this.Two(this.Two(x));
		}

		protected virtual ECFieldElement Eight(ECFieldElement x)
		{
			return this.Four(this.Two(x));
		}

		protected virtual ECFieldElement DoubleProductFromSquares(ECFieldElement a, ECFieldElement b, ECFieldElement aSquared, ECFieldElement bSquared)
		{
			return a.Add(b).Square().Subtract(aSquared).Subtract(bSquared);
		}

		public override ECPoint Negate()
		{
			if (base.IsInfinity)
			{
				return this;
			}
			ECCurve curve = this.Curve;
			int coordinateSystem = curve.CoordinateSystem;
			if (coordinateSystem != 0)
			{
				return new FpPoint(curve, base.RawXCoord, base.RawYCoord.Negate(), base.RawZCoords, base.IsCompressed);
			}
			return new FpPoint(curve, base.RawXCoord, base.RawYCoord.Negate(), base.IsCompressed);
		}

		protected virtual ECFieldElement CalculateJacobianModifiedW(ECFieldElement Z, ECFieldElement ZSquared)
		{
			ECFieldElement a = this.Curve.A;
			if (a.IsZero || Z.IsOne)
			{
				return a;
			}
			if (ZSquared == null)
			{
				ZSquared = Z.Square();
			}
			ECFieldElement eCFieldElement = ZSquared.Square();
			ECFieldElement eCFieldElement2 = a.Negate();
			if (eCFieldElement2.BitLength < a.BitLength)
			{
				eCFieldElement = eCFieldElement.Multiply(eCFieldElement2).Negate();
			}
			else
			{
				eCFieldElement = eCFieldElement.Multiply(a);
			}
			return eCFieldElement;
		}

		protected virtual ECFieldElement GetJacobianModifiedW()
		{
			ECFieldElement[] rawZCoords = base.RawZCoords;
			ECFieldElement eCFieldElement = rawZCoords[1];
			if (eCFieldElement == null)
			{
				eCFieldElement = (rawZCoords[1] = this.CalculateJacobianModifiedW(rawZCoords[0], null));
			}
			return eCFieldElement;
		}

		protected virtual FpPoint TwiceJacobianModified(bool calculateW)
		{
			ECFieldElement rawXCoord = base.RawXCoord;
			ECFieldElement rawYCoord = base.RawYCoord;
			ECFieldElement eCFieldElement = base.RawZCoords[0];
			ECFieldElement jacobianModifiedW = this.GetJacobianModifiedW();
			ECFieldElement x = rawXCoord.Square();
			ECFieldElement eCFieldElement2 = this.Three(x).Add(jacobianModifiedW);
			ECFieldElement eCFieldElement3 = this.Two(rawYCoord);
			ECFieldElement eCFieldElement4 = eCFieldElement3.Multiply(rawYCoord);
			ECFieldElement eCFieldElement5 = this.Two(rawXCoord.Multiply(eCFieldElement4));
			ECFieldElement eCFieldElement6 = eCFieldElement2.Square().Subtract(this.Two(eCFieldElement5));
			ECFieldElement x2 = eCFieldElement4.Square();
			ECFieldElement eCFieldElement7 = this.Two(x2);
			ECFieldElement y = eCFieldElement2.Multiply(eCFieldElement5.Subtract(eCFieldElement6)).Subtract(eCFieldElement7);
			ECFieldElement eCFieldElement8 = calculateW ? this.Two(eCFieldElement7.Multiply(jacobianModifiedW)) : null;
			ECFieldElement eCFieldElement9 = eCFieldElement.IsOne ? eCFieldElement3 : eCFieldElement3.Multiply(eCFieldElement);
			return new FpPoint(this.Curve, eCFieldElement6, y, new ECFieldElement[]
			{
				eCFieldElement9,
				eCFieldElement8
			}, base.IsCompressed);
		}
	}
}
