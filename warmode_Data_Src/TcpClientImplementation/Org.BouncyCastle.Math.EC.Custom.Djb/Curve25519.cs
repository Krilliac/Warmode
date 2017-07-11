using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Djb
{
	internal class Curve25519 : AbstractFpCurve
	{
		private const int Curve25519_DEFAULT_COORDS = 4;

		public static readonly BigInteger q = Nat256.ToBigInteger(Curve25519Field.P);

		protected readonly Curve25519Point m_infinity;

		public virtual BigInteger Q
		{
			get
			{
				return Curve25519.q;
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
				return Curve25519.q.BitLength;
			}
		}

		public Curve25519() : base(Curve25519.q)
		{
			this.m_infinity = new Curve25519Point(this, null, null);
			this.m_a = this.FromBigInteger(new BigInteger(1, Hex.Decode("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA984914A144")));
			this.m_b = this.FromBigInteger(new BigInteger(1, Hex.Decode("7B425ED097B425ED097B425ED097B425ED097B425ED097B4260B5E9C7710C864")));
			this.m_order = new BigInteger(1, Hex.Decode("1000000000000000000000000000000014DEF9DEA2F79CD65812631A5CF5D3ED"));
			this.m_cofactor = BigInteger.ValueOf(8L);
			this.m_coord = 4;
		}

		protected override ECCurve CloneCurve()
		{
			return new Curve25519();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			return coord == 4;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new Curve25519FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new Curve25519Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new Curve25519Point(this, x, y, zs, withCompression);
		}
	}
}
