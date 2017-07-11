using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192K1Curve : AbstractFpCurve
	{
		private const int SECP192K1_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFEE37"));

		protected readonly SecP192K1Point m_infinity;

		public virtual BigInteger Q
		{
			get
			{
				return SecP192K1Curve.q;
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
				return SecP192K1Curve.q.BitLength;
			}
		}

		public SecP192K1Curve() : base(SecP192K1Curve.q)
		{
			this.m_infinity = new SecP192K1Point(this, null, null);
			this.m_a = this.FromBigInteger(BigInteger.Zero);
			this.m_b = this.FromBigInteger(BigInteger.ValueOf(3L));
			this.m_order = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFE26F2FC170F69466A74DEFD8D"));
			this.m_cofactor = BigInteger.One;
			this.m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP192K1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			return coord == 2;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecP192K1FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP192K1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP192K1Point(this, x, y, zs, withCompression);
		}
	}
}
