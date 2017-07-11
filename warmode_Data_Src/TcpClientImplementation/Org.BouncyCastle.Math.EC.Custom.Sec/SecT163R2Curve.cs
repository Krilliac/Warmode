using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT163R2Curve : AbstractF2mCurve
	{
		private const int SecT163R2_DEFAULT_COORDS = 6;

		protected readonly SecT163R2Point m_infinity;

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
				return 163;
			}
		}

		public override bool IsKoblitz
		{
			get
			{
				return false;
			}
		}

		public virtual int M
		{
			get
			{
				return 163;
			}
		}

		public virtual bool IsTrinomial
		{
			get
			{
				return false;
			}
		}

		public virtual int K1
		{
			get
			{
				return 3;
			}
		}

		public virtual int K2
		{
			get
			{
				return 6;
			}
		}

		public virtual int K3
		{
			get
			{
				return 7;
			}
		}

		public SecT163R2Curve() : base(163, 3, 6, 7)
		{
			this.m_infinity = new SecT163R2Point(this, null, null);
			this.m_a = this.FromBigInteger(BigInteger.One);
			this.m_b = this.FromBigInteger(new BigInteger(1, Hex.Decode("020A601907B8C953CA1481EB10512F78744A3205FD")));
			this.m_order = new BigInteger(1, Hex.Decode("040000000000000000000292FE77E70C12A4234C33"));
			this.m_cofactor = BigInteger.Two;
			this.m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT163R2Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			return coord == 6;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecT163FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT163R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT163R2Point(this, x, y, zs, withCompression);
		}

		protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
		{
			ECFieldElement eCFieldElement = this.FromBigInteger(X1);
			ECFieldElement eCFieldElement2 = null;
			if (eCFieldElement.IsZero)
			{
				eCFieldElement2 = this.B.Sqrt();
			}
			else
			{
				ECFieldElement beta = eCFieldElement.Square().Invert().Multiply(this.B).Add(this.A).Add(eCFieldElement);
				ECFieldElement eCFieldElement3 = this.SolveQuadraticEquation(beta);
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

		private ECFieldElement SolveQuadraticEquation(ECFieldElement beta)
		{
			if (beta.IsZero)
			{
				return beta;
			}
			ECFieldElement eCFieldElement = this.FromBigInteger(BigInteger.Zero);
			Random random = new Random();
			while (true)
			{
				ECFieldElement b = this.FromBigInteger(new BigInteger(163, random));
				ECFieldElement eCFieldElement2 = eCFieldElement;
				ECFieldElement eCFieldElement3 = beta;
				for (int i = 1; i < 163; i++)
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
	}
}
