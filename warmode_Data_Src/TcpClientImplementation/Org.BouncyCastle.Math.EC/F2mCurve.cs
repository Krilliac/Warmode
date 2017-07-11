using Org.BouncyCastle.Math.EC.Multiplier;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public class F2mCurve : AbstractF2mCurve
	{
		private const int F2M_DEFAULT_COORDS = 6;

		private readonly int m;

		private readonly int k1;

		private readonly int k2;

		private readonly int k3;

		protected readonly F2mPoint m_infinity;

		public override int FieldSize
		{
			get
			{
				return this.m;
			}
		}

		public override ECPoint Infinity
		{
			get
			{
				return this.m_infinity;
			}
		}

		public int M
		{
			get
			{
				return this.m;
			}
		}

		public int K1
		{
			get
			{
				return this.k1;
			}
		}

		public int K2
		{
			get
			{
				return this.k2;
			}
		}

		public int K3
		{
			get
			{
				return this.k3;
			}
		}

		[Obsolete("Use 'Order' property instead")]
		public BigInteger N
		{
			get
			{
				return this.m_order;
			}
		}

		[Obsolete("Use 'Cofactor' property instead")]
		public BigInteger H
		{
			get
			{
				return this.m_cofactor;
			}
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b) : this(m, k, 0, 0, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor) : this(m, k, 0, 0, a, b, order, cofactor)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b) : this(m, k1, k2, k3, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor) : base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			this.m_order = order;
			this.m_cofactor = cofactor;
			this.m_infinity = new F2mPoint(this, null, null);
			if (k1 == 0)
			{
				throw new ArgumentException("k1 must be > 0");
			}
			if (k2 == 0)
			{
				if (k3 != 0)
				{
					throw new ArgumentException("k3 must be 0 if k2 == 0");
				}
			}
			else
			{
				if (k2 <= k1)
				{
					throw new ArgumentException("k2 must be > k1");
				}
				if (k3 <= k2)
				{
					throw new ArgumentException("k3 must be > k2");
				}
			}
			this.m_a = this.FromBigInteger(a);
			this.m_b = this.FromBigInteger(b);
			this.m_coord = 6;
		}

		protected F2mCurve(int m, int k1, int k2, int k3, ECFieldElement a, ECFieldElement b, BigInteger order, BigInteger cofactor) : base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			this.m_order = order;
			this.m_cofactor = cofactor;
			this.m_infinity = new F2mPoint(this, null, null);
			this.m_a = a;
			this.m_b = b;
			this.m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new F2mCurve(this.m, this.k1, this.k2, this.k3, this.m_a, this.m_b, this.m_order, this.m_cofactor);
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			switch (coord)
			{
			case 0:
			case 1:
				break;
			default:
				if (coord != 6)
				{
					return false;
				}
				break;
			}
			return true;
		}

		protected override ECMultiplier CreateDefaultMultiplier()
		{
			if (this.IsKoblitz)
			{
				return new WTauNafMultiplier();
			}
			return base.CreateDefaultMultiplier();
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new F2mFieldElement(this.m, this.k1, this.k2, this.k3, x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new F2mPoint(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new F2mPoint(this, x, y, zs, withCompression);
		}

		protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
		{
			ECFieldElement eCFieldElement = this.FromBigInteger(X1);
			ECFieldElement eCFieldElement2 = null;
			if (eCFieldElement.IsZero)
			{
				eCFieldElement2 = this.m_b.Sqrt();
			}
			else
			{
				ECFieldElement beta = eCFieldElement.Square().Invert().Multiply(this.B).Add(this.A).Add(eCFieldElement);
				ECFieldElement eCFieldElement3 = this.SolveQuadradicEquation(beta);
				if (eCFieldElement3 != null)
				{
					if (eCFieldElement3.TestBitZero() != (yTilde == 1))
					{
						eCFieldElement3 = eCFieldElement3.AddOne();
					}
					switch (this.CoordinateSystem)
					{
					case 5:
					case 6:
						eCFieldElement2 = eCFieldElement3.Add(eCFieldElement);
						break;
					default:
						eCFieldElement2 = eCFieldElement3.Multiply(eCFieldElement);
						break;
					}
				}
			}
			if (eCFieldElement2 == null)
			{
				throw new ArgumentException("Invalid point compression");
			}
			return this.CreateRawPoint(eCFieldElement, eCFieldElement2, true);
		}

		private ECFieldElement SolveQuadradicEquation(ECFieldElement beta)
		{
			if (beta.IsZero)
			{
				return beta;
			}
			ECFieldElement eCFieldElement = this.FromBigInteger(BigInteger.Zero);
			Random random = new Random();
			while (true)
			{
				ECFieldElement b = this.FromBigInteger(new BigInteger(this.m, random));
				ECFieldElement eCFieldElement2 = eCFieldElement;
				ECFieldElement eCFieldElement3 = beta;
				for (int i = 1; i < this.m; i++)
				{
					ECFieldElement eCFieldElement4 = eCFieldElement3.Square();
					eCFieldElement2 = eCFieldElement2.Square().Add(eCFieldElement4.Multiply(b));
					eCFieldElement3 = eCFieldElement4.Add(beta);
				}
				if (!eCFieldElement3.IsZero)
				{
					break;
				}
				ECFieldElement eCFieldElement5 = eCFieldElement2.Square().Add(eCFieldElement2);
				if (!eCFieldElement5.IsZero)
				{
					return eCFieldElement2;
				}
			}
			return null;
		}

		public bool IsTrinomial()
		{
			return this.k2 == 0 && this.k3 == 0;
		}
	}
}
