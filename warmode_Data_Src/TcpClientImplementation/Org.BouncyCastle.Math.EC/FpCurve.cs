using System;

namespace Org.BouncyCastle.Math.EC
{
	public class FpCurve : AbstractFpCurve
	{
		private const int FP_DEFAULT_COORDS = 4;

		protected readonly BigInteger m_q;

		protected readonly BigInteger m_r;

		protected readonly FpPoint m_infinity;

		public virtual BigInteger Q
		{
			get
			{
				return this.m_q;
			}
		}

		public override ECPoint Infinity
		{
			get
			{
				return this.m_infinity;
			}
		}

		public override int FieldSize
		{
			get
			{
				return this.m_q.BitLength;
			}
		}

		public FpCurve(BigInteger q, BigInteger a, BigInteger b) : this(q, a, b, null, null)
		{
		}

		public FpCurve(BigInteger q, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor) : base(q)
		{
			this.m_q = q;
			this.m_r = FpFieldElement.CalculateResidue(q);
			this.m_infinity = new FpPoint(this, null, null);
			this.m_a = this.FromBigInteger(a);
			this.m_b = this.FromBigInteger(b);
			this.m_order = order;
			this.m_cofactor = cofactor;
			this.m_coord = 4;
		}

		protected FpCurve(BigInteger q, BigInteger r, ECFieldElement a, ECFieldElement b) : this(q, r, a, b, null, null)
		{
		}

		protected FpCurve(BigInteger q, BigInteger r, ECFieldElement a, ECFieldElement b, BigInteger order, BigInteger cofactor) : base(q)
		{
			this.m_q = q;
			this.m_r = r;
			this.m_infinity = new FpPoint(this, null, null);
			this.m_a = a;
			this.m_b = b;
			this.m_order = order;
			this.m_cofactor = cofactor;
			this.m_coord = 4;
		}

		protected override ECCurve CloneCurve()
		{
			return new FpCurve(this.m_q, this.m_r, this.m_a, this.m_b, this.m_order, this.m_cofactor);
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			switch (coord)
			{
			case 0:
			case 1:
			case 2:
			case 4:
				return true;
			}
			return false;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new FpFieldElement(this.m_q, this.m_r, x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new FpPoint(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new FpPoint(this, x, y, zs, withCompression);
		}

		public override ECPoint ImportPoint(ECPoint p)
		{
			if (this != p.Curve && this.CoordinateSystem == 2 && !p.IsInfinity)
			{
				switch (p.Curve.CoordinateSystem)
				{
				case 2:
				case 3:
				case 4:
					return new FpPoint(this, this.FromBigInteger(p.RawXCoord.ToBigInteger()), this.FromBigInteger(p.RawYCoord.ToBigInteger()), new ECFieldElement[]
					{
						this.FromBigInteger(p.GetZCoord(0).ToBigInteger())
					}, p.IsCompressed);
				}
			}
			return base.ImportPoint(p);
		}
	}
}
