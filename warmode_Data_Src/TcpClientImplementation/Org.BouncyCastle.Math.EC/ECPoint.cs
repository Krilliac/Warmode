using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class ECPoint
	{
		protected static ECFieldElement[] EMPTY_ZS = new ECFieldElement[0];

		protected internal readonly ECCurve m_curve;

		protected internal readonly ECFieldElement m_x;

		protected internal readonly ECFieldElement m_y;

		protected internal readonly ECFieldElement[] m_zs;

		protected internal readonly bool m_withCompression;

		protected internal IDictionary m_preCompTable;

		public virtual ECCurve Curve
		{
			get
			{
				return this.m_curve;
			}
		}

		protected virtual int CurveCoordinateSystem
		{
			get
			{
				if (this.m_curve != null)
				{
					return this.m_curve.CoordinateSystem;
				}
				return 0;
			}
		}

		[Obsolete("Use AffineXCoord, or Normalize() and XCoord, instead")]
		public virtual ECFieldElement X
		{
			get
			{
				return this.Normalize().XCoord;
			}
		}

		[Obsolete("Use AffineYCoord, or Normalize() and YCoord, instead")]
		public virtual ECFieldElement Y
		{
			get
			{
				return this.Normalize().YCoord;
			}
		}

		public virtual ECFieldElement AffineXCoord
		{
			get
			{
				this.CheckNormalized();
				return this.XCoord;
			}
		}

		public virtual ECFieldElement AffineYCoord
		{
			get
			{
				this.CheckNormalized();
				return this.YCoord;
			}
		}

		public virtual ECFieldElement XCoord
		{
			get
			{
				return this.m_x;
			}
		}

		public virtual ECFieldElement YCoord
		{
			get
			{
				return this.m_y;
			}
		}

		protected internal ECFieldElement RawXCoord
		{
			get
			{
				return this.m_x;
			}
		}

		protected internal ECFieldElement RawYCoord
		{
			get
			{
				return this.m_y;
			}
		}

		protected internal ECFieldElement[] RawZCoords
		{
			get
			{
				return this.m_zs;
			}
		}

		public bool IsInfinity
		{
			get
			{
				return this.m_x == null && this.m_y == null;
			}
		}

		public bool IsCompressed
		{
			get
			{
				return this.m_withCompression;
			}
		}

		protected internal abstract bool CompressionYTilde
		{
			get;
		}

		protected static ECFieldElement[] GetInitialZCoords(ECCurve curve)
		{
			int num = (curve == null) ? 0 : curve.CoordinateSystem;
			int num2 = num;
			if (num2 == 0 || num2 == 5)
			{
				return ECPoint.EMPTY_ZS;
			}
			ECFieldElement eCFieldElement = curve.FromBigInteger(BigInteger.One);
			switch (num)
			{
			case 1:
			case 2:
			case 6:
				return new ECFieldElement[]
				{
					eCFieldElement
				};
			case 3:
				return new ECFieldElement[]
				{
					eCFieldElement,
					eCFieldElement,
					eCFieldElement
				};
			case 4:
				return new ECFieldElement[]
				{
					eCFieldElement,
					curve.A
				};
			}
			throw new ArgumentException("unknown coordinate system");
		}

		protected ECPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression) : this(curve, x, y, ECPoint.GetInitialZCoords(curve), withCompression)
		{
		}

		internal ECPoint(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			this.m_curve = curve;
			this.m_x = x;
			this.m_y = y;
			this.m_zs = zs;
			this.m_withCompression = withCompression;
		}

		protected internal bool SatisfiesCofactor()
		{
			BigInteger cofactor = this.Curve.Cofactor;
			return cofactor == null || cofactor.Equals(BigInteger.One) || !ECAlgorithms.ReferenceMultiply(this, cofactor).IsInfinity;
		}

		protected abstract bool SatisfiesCurveEquation();

		public ECPoint GetDetachedPoint()
		{
			return this.Normalize().Detach();
		}

		protected abstract ECPoint Detach();

		public virtual ECFieldElement GetZCoord(int index)
		{
			if (index >= 0 && index < this.m_zs.Length)
			{
				return this.m_zs[index];
			}
			return null;
		}

		public virtual ECFieldElement[] GetZCoords()
		{
			int num = this.m_zs.Length;
			if (num == 0)
			{
				return this.m_zs;
			}
			ECFieldElement[] array = new ECFieldElement[num];
			Array.Copy(this.m_zs, 0, array, 0, num);
			return array;
		}

		protected virtual void CheckNormalized()
		{
			if (!this.IsNormalized())
			{
				throw new InvalidOperationException("point not in normal form");
			}
		}

		public virtual bool IsNormalized()
		{
			int curveCoordinateSystem = this.CurveCoordinateSystem;
			return curveCoordinateSystem == 0 || curveCoordinateSystem == 5 || this.IsInfinity || this.RawZCoords[0].IsOne;
		}

		public virtual ECPoint Normalize()
		{
			if (this.IsInfinity)
			{
				return this;
			}
			int curveCoordinateSystem = this.CurveCoordinateSystem;
			if (curveCoordinateSystem == 0 || curveCoordinateSystem == 5)
			{
				return this;
			}
			ECFieldElement eCFieldElement = this.RawZCoords[0];
			if (eCFieldElement.IsOne)
			{
				return this;
			}
			return this.Normalize(eCFieldElement.Invert());
		}

		internal virtual ECPoint Normalize(ECFieldElement zInv)
		{
			switch (this.CurveCoordinateSystem)
			{
			case 1:
			case 6:
				return this.CreateScaledPoint(zInv, zInv);
			case 2:
			case 3:
			case 4:
			{
				ECFieldElement eCFieldElement = zInv.Square();
				ECFieldElement sy = eCFieldElement.Multiply(zInv);
				return this.CreateScaledPoint(eCFieldElement, sy);
			}
			}
			throw new InvalidOperationException("not a projective coordinate system");
		}

		protected virtual ECPoint CreateScaledPoint(ECFieldElement sx, ECFieldElement sy)
		{
			return this.Curve.CreateRawPoint(this.RawXCoord.Multiply(sx), this.RawYCoord.Multiply(sy), this.IsCompressed);
		}

		public bool IsValid()
		{
			if (this.IsInfinity)
			{
				return true;
			}
			ECCurve curve = this.Curve;
			if (curve != null)
			{
				if (!this.SatisfiesCurveEquation())
				{
					return false;
				}
				if (!this.SatisfiesCofactor())
				{
					return false;
				}
			}
			return true;
		}

		public virtual ECPoint ScaleX(ECFieldElement scale)
		{
			if (!this.IsInfinity)
			{
				return this.Curve.CreateRawPoint(this.RawXCoord.Multiply(scale), this.RawYCoord, this.RawZCoords, this.IsCompressed);
			}
			return this;
		}

		public virtual ECPoint ScaleY(ECFieldElement scale)
		{
			if (!this.IsInfinity)
			{
				return this.Curve.CreateRawPoint(this.RawXCoord, this.RawYCoord.Multiply(scale), this.RawZCoords, this.IsCompressed);
			}
			return this;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ECPoint);
		}

		public virtual bool Equals(ECPoint other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			ECCurve curve = this.Curve;
			ECCurve curve2 = other.Curve;
			bool flag = null == curve;
			bool flag2 = null == curve2;
			bool isInfinity = this.IsInfinity;
			bool isInfinity2 = other.IsInfinity;
			if (isInfinity || isInfinity2)
			{
				return isInfinity && isInfinity2 && (flag || flag2 || curve.Equals(curve2));
			}
			ECPoint eCPoint = this;
			ECPoint eCPoint2 = other;
			if (!flag || !flag2)
			{
				if (flag)
				{
					eCPoint2 = eCPoint2.Normalize();
				}
				else if (flag2)
				{
					eCPoint = eCPoint.Normalize();
				}
				else
				{
					if (!curve.Equals(curve2))
					{
						return false;
					}
					ECPoint[] array = new ECPoint[]
					{
						this,
						curve.ImportPoint(eCPoint2)
					};
					curve.NormalizeAll(array);
					eCPoint = array[0];
					eCPoint2 = array[1];
				}
			}
			return eCPoint.XCoord.Equals(eCPoint2.XCoord) && eCPoint.YCoord.Equals(eCPoint2.YCoord);
		}

		public override int GetHashCode()
		{
			ECCurve curve = this.Curve;
			int num = (curve == null) ? 0 : (~curve.GetHashCode());
			if (!this.IsInfinity)
			{
				ECPoint eCPoint = this.Normalize();
				num ^= eCPoint.XCoord.GetHashCode() * 17;
				num ^= eCPoint.YCoord.GetHashCode() * 257;
			}
			return num;
		}

		public override string ToString()
		{
			if (this.IsInfinity)
			{
				return "INF";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			stringBuilder.Append(this.RawXCoord);
			stringBuilder.Append(',');
			stringBuilder.Append(this.RawYCoord);
			for (int i = 0; i < this.m_zs.Length; i++)
			{
				stringBuilder.Append(',');
				stringBuilder.Append(this.m_zs[i]);
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public virtual byte[] GetEncoded()
		{
			return this.GetEncoded(this.m_withCompression);
		}

		public abstract byte[] GetEncoded(bool compressed);

		public abstract ECPoint Add(ECPoint b);

		public abstract ECPoint Subtract(ECPoint b);

		public abstract ECPoint Negate();

		public virtual ECPoint TimesPow2(int e)
		{
			if (e < 0)
			{
				throw new ArgumentException("cannot be negative", "e");
			}
			ECPoint eCPoint = this;
			while (--e >= 0)
			{
				eCPoint = eCPoint.Twice();
			}
			return eCPoint;
		}

		public abstract ECPoint Twice();

		public abstract ECPoint Multiply(BigInteger b);

		public virtual ECPoint TwicePlus(ECPoint b)
		{
			return this.Twice().Add(b);
		}

		public virtual ECPoint ThreeTimes()
		{
			return this.TwicePlus(this);
		}
	}
}
