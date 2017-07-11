using Org.BouncyCastle.Math.EC.Abc;
using Org.BouncyCastle.Math.Field;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class AbstractF2mCurve : ECCurve
	{
		private BigInteger[] si;

		public virtual bool IsKoblitz
		{
			get
			{
				return this.m_order != null && this.m_cofactor != null && this.m_b.IsOne && (this.m_a.IsZero || this.m_a.IsOne);
			}
		}

		public static BigInteger Inverse(int m, int[] ks, BigInteger x)
		{
			return new LongArray(x).ModInverse(m, ks).ToBigInteger();
		}

		private static IFiniteField BuildField(int m, int k1, int k2, int k3)
		{
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
				return FiniteFields.GetBinaryExtensionField(new int[]
				{
					0,
					k1,
					m
				});
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
				return FiniteFields.GetBinaryExtensionField(new int[]
				{
					0,
					k1,
					k2,
					k3,
					m
				});
			}
		}

		protected AbstractF2mCurve(int m, int k1, int k2, int k3) : base(AbstractF2mCurve.BuildField(m, k1, k2, k3))
		{
		}

		[Obsolete("Per-point compression property will be removed")]
		public override ECPoint CreatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			ECFieldElement eCFieldElement = this.FromBigInteger(x);
			ECFieldElement eCFieldElement2 = this.FromBigInteger(y);
			switch (this.CoordinateSystem)
			{
			case 5:
			case 6:
				if (eCFieldElement.IsZero)
				{
					if (!eCFieldElement2.Square().Equals(this.B))
					{
						throw new ArgumentException();
					}
				}
				else
				{
					eCFieldElement2 = eCFieldElement2.Divide(eCFieldElement).Add(eCFieldElement);
				}
				break;
			}
			return this.CreateRawPoint(eCFieldElement, eCFieldElement2, withCompression);
		}

		internal virtual BigInteger[] GetSi()
		{
			if (this.si == null)
			{
				lock (this)
				{
					if (this.si == null)
					{
						this.si = Tnaf.GetSi(this);
					}
				}
			}
			return this.si;
		}
	}
}
