using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP160R1Curve : AbstractFpCurve
	{
		private const int SecP160R1_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFF"));

		protected readonly SecP160R1Point m_infinity;

		public virtual BigInteger Q
		{
			get
			{
				return SecP160R1Curve.q;
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
				return SecP160R1Curve.q.BitLength;
			}
		}

		public SecP160R1Curve() : base(SecP160R1Curve.q)
		{
			this.m_infinity = new SecP160R1Point(this, null, null);
			this.m_a = this.FromBigInteger(new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFC")));
			this.m_b = this.FromBigInteger(new BigInteger(1, Hex.Decode("1C97BEFC54BD7A8B65ACF89F81D4D4ADC565FA45")));
			this.m_order = new BigInteger(1, Hex.Decode("0100000000000000000001F4C8F927AED3CA752257"));
			this.m_cofactor = BigInteger.One;
			this.m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP160R1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			return coord == 2;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecP160R1FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP160R1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP160R1Point(this, x, y, zs, withCompression);
		}
	}
}
