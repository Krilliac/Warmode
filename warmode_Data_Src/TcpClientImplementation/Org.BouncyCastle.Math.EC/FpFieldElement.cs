using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public class FpFieldElement : ECFieldElement
	{
		private readonly BigInteger q;

		private readonly BigInteger r;

		private readonly BigInteger x;

		public override string FieldName
		{
			get
			{
				return "Fp";
			}
		}

		public override int FieldSize
		{
			get
			{
				return this.q.BitLength;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q;
			}
		}

		internal static BigInteger CalculateResidue(BigInteger p)
		{
			int bitLength = p.BitLength;
			if (bitLength >= 96)
			{
				BigInteger bigInteger = p.ShiftRight(bitLength - 64);
				if (bigInteger.LongValue == -1L)
				{
					return BigInteger.One.ShiftLeft(bitLength).Subtract(p);
				}
				if ((bitLength & 7) == 0)
				{
					return BigInteger.One.ShiftLeft(bitLength << 1).Divide(p).Negate();
				}
			}
			return null;
		}

		[Obsolete("Use ECCurve.FromBigInteger to construct field elements")]
		public FpFieldElement(BigInteger q, BigInteger x) : this(q, FpFieldElement.CalculateResidue(q), x)
		{
		}

		internal FpFieldElement(BigInteger q, BigInteger r, BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(q) >= 0)
			{
				throw new ArgumentException("value invalid in Fp field element", "x");
			}
			this.q = q;
			this.r = r;
			this.x = x;
		}

		public override BigInteger ToBigInteger()
		{
			return this.x;
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			return new FpFieldElement(this.q, this.r, this.ModAdd(this.x, b.ToBigInteger()));
		}

		public override ECFieldElement AddOne()
		{
			BigInteger bigInteger = this.x.Add(BigInteger.One);
			if (bigInteger.CompareTo(this.q) == 0)
			{
				bigInteger = BigInteger.Zero;
			}
			return new FpFieldElement(this.q, this.r, bigInteger);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return new FpFieldElement(this.q, this.r, this.ModSubtract(this.x, b.ToBigInteger()));
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			return new FpFieldElement(this.q, this.r, this.ModMult(this.x, b.ToBigInteger()));
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			BigInteger bigInteger = this.x;
			BigInteger val = b.ToBigInteger();
			BigInteger bigInteger2 = x.ToBigInteger();
			BigInteger val2 = y.ToBigInteger();
			BigInteger bigInteger3 = bigInteger.Multiply(val);
			BigInteger n = bigInteger2.Multiply(val2);
			return new FpFieldElement(this.q, this.r, this.ModReduce(bigInteger3.Subtract(n)));
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			BigInteger bigInteger = this.x;
			BigInteger val = b.ToBigInteger();
			BigInteger bigInteger2 = x.ToBigInteger();
			BigInteger val2 = y.ToBigInteger();
			BigInteger bigInteger3 = bigInteger.Multiply(val);
			BigInteger value = bigInteger2.Multiply(val2);
			BigInteger bigInteger4 = bigInteger3.Add(value);
			if (this.r != null && this.r.SignValue < 0 && bigInteger4.BitLength > this.q.BitLength << 1)
			{
				bigInteger4 = bigInteger4.Subtract(this.q.ShiftLeft(this.q.BitLength));
			}
			return new FpFieldElement(this.q, this.r, this.ModReduce(bigInteger4));
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			return new FpFieldElement(this.q, this.r, this.ModMult(this.x, this.ModInverse(b.ToBigInteger())));
		}

		public override ECFieldElement Negate()
		{
			if (this.x.SignValue != 0)
			{
				return new FpFieldElement(this.q, this.r, this.q.Subtract(this.x));
			}
			return this;
		}

		public override ECFieldElement Square()
		{
			return new FpFieldElement(this.q, this.r, this.ModMult(this.x, this.x));
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			BigInteger bigInteger = this.x;
			BigInteger bigInteger2 = x.ToBigInteger();
			BigInteger val = y.ToBigInteger();
			BigInteger bigInteger3 = bigInteger.Multiply(bigInteger);
			BigInteger n = bigInteger2.Multiply(val);
			return new FpFieldElement(this.q, this.r, this.ModReduce(bigInteger3.Subtract(n)));
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			BigInteger bigInteger = this.x;
			BigInteger bigInteger2 = x.ToBigInteger();
			BigInteger val = y.ToBigInteger();
			BigInteger bigInteger3 = bigInteger.Multiply(bigInteger);
			BigInteger value = bigInteger2.Multiply(val);
			BigInteger bigInteger4 = bigInteger3.Add(value);
			if (this.r != null && this.r.SignValue < 0 && bigInteger4.BitLength > this.q.BitLength << 1)
			{
				bigInteger4 = bigInteger4.Subtract(this.q.ShiftLeft(this.q.BitLength));
			}
			return new FpFieldElement(this.q, this.r, this.ModReduce(bigInteger4));
		}

		public override ECFieldElement Invert()
		{
			return new FpFieldElement(this.q, this.r, this.ModInverse(this.x));
		}

		public override ECFieldElement Sqrt()
		{
			if (this.IsZero || this.IsOne)
			{
				return this;
			}
			if (!this.q.TestBit(0))
			{
				throw Platform.CreateNotImplementedException("even value of q");
			}
			if (this.q.TestBit(1))
			{
				BigInteger e = this.q.ShiftRight(2).Add(BigInteger.One);
				return this.CheckSqrt(new FpFieldElement(this.q, this.r, this.x.ModPow(e, this.q)));
			}
			if (this.q.TestBit(2))
			{
				BigInteger bigInteger = this.x.ModPow(this.q.ShiftRight(3), this.q);
				BigInteger x = this.ModMult(bigInteger, this.x);
				BigInteger bigInteger2 = this.ModMult(x, bigInteger);
				if (bigInteger2.Equals(BigInteger.One))
				{
					return this.CheckSqrt(new FpFieldElement(this.q, this.r, x));
				}
				BigInteger x2 = BigInteger.Two.ModPow(this.q.ShiftRight(2), this.q);
				BigInteger bigInteger3 = this.ModMult(x, x2);
				return this.CheckSqrt(new FpFieldElement(this.q, this.r, bigInteger3));
			}
			else
			{
				BigInteger bigInteger4 = this.q.ShiftRight(1);
				if (!this.x.ModPow(bigInteger4, this.q).Equals(BigInteger.One))
				{
					return null;
				}
				BigInteger bigInteger5 = this.x;
				BigInteger bigInteger6 = this.ModDouble(this.ModDouble(bigInteger5));
				BigInteger k = bigInteger4.Add(BigInteger.One);
				BigInteger obj = this.q.Subtract(BigInteger.One);
				Random random = new Random();
				BigInteger bigInteger9;
				while (true)
				{
					BigInteger bigInteger7 = new BigInteger(this.q.BitLength, random);
					if (bigInteger7.CompareTo(this.q) < 0 && this.ModReduce(bigInteger7.Multiply(bigInteger7).Subtract(bigInteger6)).ModPow(bigInteger4, this.q).Equals(obj))
					{
						BigInteger[] array = this.LucasSequence(bigInteger7, bigInteger5, k);
						BigInteger bigInteger8 = array[0];
						bigInteger9 = array[1];
						if (this.ModMult(bigInteger9, bigInteger9).Equals(bigInteger6))
						{
							break;
						}
						if (!bigInteger8.Equals(BigInteger.One) && !bigInteger8.Equals(obj))
						{
							goto Block_11;
						}
					}
				}
				return new FpFieldElement(this.q, this.r, this.ModHalfAbs(bigInteger9));
				Block_11:
				return null;
			}
		}

		private ECFieldElement CheckSqrt(ECFieldElement z)
		{
			if (!z.Square().Equals(this))
			{
				return null;
			}
			return z;
		}

		private BigInteger[] LucasSequence(BigInteger P, BigInteger Q, BigInteger k)
		{
			int bitLength = k.BitLength;
			int lowestSetBit = k.GetLowestSetBit();
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = BigInteger.Two;
			BigInteger bigInteger3 = P;
			BigInteger bigInteger4 = BigInteger.One;
			BigInteger bigInteger5 = BigInteger.One;
			for (int i = bitLength - 1; i >= lowestSetBit + 1; i--)
			{
				bigInteger4 = this.ModMult(bigInteger4, bigInteger5);
				if (k.TestBit(i))
				{
					bigInteger5 = this.ModMult(bigInteger4, Q);
					bigInteger = this.ModMult(bigInteger, bigInteger3);
					bigInteger2 = this.ModReduce(bigInteger3.Multiply(bigInteger2).Subtract(P.Multiply(bigInteger4)));
					bigInteger3 = this.ModReduce(bigInteger3.Multiply(bigInteger3).Subtract(bigInteger5.ShiftLeft(1)));
				}
				else
				{
					bigInteger5 = bigInteger4;
					bigInteger = this.ModReduce(bigInteger.Multiply(bigInteger2).Subtract(bigInteger4));
					bigInteger3 = this.ModReduce(bigInteger3.Multiply(bigInteger2).Subtract(P.Multiply(bigInteger4)));
					bigInteger2 = this.ModReduce(bigInteger2.Multiply(bigInteger2).Subtract(bigInteger4.ShiftLeft(1)));
				}
			}
			bigInteger4 = this.ModMult(bigInteger4, bigInteger5);
			bigInteger5 = this.ModMult(bigInteger4, Q);
			bigInteger = this.ModReduce(bigInteger.Multiply(bigInteger2).Subtract(bigInteger4));
			bigInteger2 = this.ModReduce(bigInteger3.Multiply(bigInteger2).Subtract(P.Multiply(bigInteger4)));
			bigInteger4 = this.ModMult(bigInteger4, bigInteger5);
			for (int j = 1; j <= lowestSetBit; j++)
			{
				bigInteger = this.ModMult(bigInteger, bigInteger2);
				bigInteger2 = this.ModReduce(bigInteger2.Multiply(bigInteger2).Subtract(bigInteger4.ShiftLeft(1)));
				bigInteger4 = this.ModMult(bigInteger4, bigInteger4);
			}
			return new BigInteger[]
			{
				bigInteger,
				bigInteger2
			};
		}

		protected virtual BigInteger ModAdd(BigInteger x1, BigInteger x2)
		{
			BigInteger bigInteger = x1.Add(x2);
			if (bigInteger.CompareTo(this.q) >= 0)
			{
				bigInteger = bigInteger.Subtract(this.q);
			}
			return bigInteger;
		}

		protected virtual BigInteger ModDouble(BigInteger x)
		{
			BigInteger bigInteger = x.ShiftLeft(1);
			if (bigInteger.CompareTo(this.q) >= 0)
			{
				bigInteger = bigInteger.Subtract(this.q);
			}
			return bigInteger;
		}

		protected virtual BigInteger ModHalf(BigInteger x)
		{
			if (x.TestBit(0))
			{
				x = this.q.Add(x);
			}
			return x.ShiftRight(1);
		}

		protected virtual BigInteger ModHalfAbs(BigInteger x)
		{
			if (x.TestBit(0))
			{
				x = this.q.Subtract(x);
			}
			return x.ShiftRight(1);
		}

		protected virtual BigInteger ModInverse(BigInteger x)
		{
			int fieldSize = this.FieldSize;
			int len = fieldSize + 31 >> 5;
			uint[] p = Nat.FromBigInteger(fieldSize, this.q);
			uint[] array = Nat.FromBigInteger(fieldSize, x);
			uint[] z = Nat.Create(len);
			Mod.Invert(p, array, z);
			return Nat.ToBigInteger(len, z);
		}

		protected virtual BigInteger ModMult(BigInteger x1, BigInteger x2)
		{
			return this.ModReduce(x1.Multiply(x2));
		}

		protected virtual BigInteger ModReduce(BigInteger x)
		{
			if (this.r == null)
			{
				x = x.Mod(this.q);
			}
			else
			{
				bool flag = x.SignValue < 0;
				if (flag)
				{
					x = x.Abs();
				}
				int bitLength = this.q.BitLength;
				if (this.r.SignValue > 0)
				{
					BigInteger n = BigInteger.One.ShiftLeft(bitLength);
					bool flag2 = this.r.Equals(BigInteger.One);
					while (x.BitLength > bitLength + 1)
					{
						BigInteger bigInteger = x.ShiftRight(bitLength);
						BigInteger value = x.Remainder(n);
						if (!flag2)
						{
							bigInteger = bigInteger.Multiply(this.r);
						}
						x = bigInteger.Add(value);
					}
				}
				else
				{
					int num = (bitLength - 1 & 31) + 1;
					BigInteger bigInteger2 = this.r.Negate();
					BigInteger bigInteger3 = bigInteger2.Multiply(x.ShiftRight(bitLength - num));
					BigInteger bigInteger4 = bigInteger3.ShiftRight(bitLength + num);
					BigInteger bigInteger5 = bigInteger4.Multiply(this.q);
					BigInteger bigInteger6 = BigInteger.One.ShiftLeft(bitLength + num);
					bigInteger5 = bigInteger5.Remainder(bigInteger6);
					x = x.Remainder(bigInteger6);
					x = x.Subtract(bigInteger5);
					if (x.SignValue < 0)
					{
						x = x.Add(bigInteger6);
					}
				}
				while (x.CompareTo(this.q) >= 0)
				{
					x = x.Subtract(this.q);
				}
				if (flag && x.SignValue != 0)
				{
					x = this.q.Subtract(x);
				}
			}
			return x;
		}

		protected virtual BigInteger ModSubtract(BigInteger x1, BigInteger x2)
		{
			BigInteger bigInteger = x1.Subtract(x2);
			if (bigInteger.SignValue < 0)
			{
				bigInteger = bigInteger.Add(this.q);
			}
			return bigInteger;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			FpFieldElement fpFieldElement = obj as FpFieldElement;
			return fpFieldElement != null && this.Equals(fpFieldElement);
		}

		public virtual bool Equals(FpFieldElement other)
		{
			return this.q.Equals(other.q) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.q.GetHashCode() ^ base.GetHashCode();
		}
	}
}
