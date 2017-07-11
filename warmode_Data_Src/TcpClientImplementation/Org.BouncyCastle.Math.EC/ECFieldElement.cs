using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class ECFieldElement
	{
		public abstract string FieldName
		{
			get;
		}

		public abstract int FieldSize
		{
			get;
		}

		public virtual int BitLength
		{
			get
			{
				return this.ToBigInteger().BitLength;
			}
		}

		public virtual bool IsOne
		{
			get
			{
				return this.BitLength == 1;
			}
		}

		public virtual bool IsZero
		{
			get
			{
				return 0 == this.ToBigInteger().SignValue;
			}
		}

		public abstract BigInteger ToBigInteger();

		public abstract ECFieldElement Add(ECFieldElement b);

		public abstract ECFieldElement AddOne();

		public abstract ECFieldElement Subtract(ECFieldElement b);

		public abstract ECFieldElement Multiply(ECFieldElement b);

		public abstract ECFieldElement Divide(ECFieldElement b);

		public abstract ECFieldElement Negate();

		public abstract ECFieldElement Square();

		public abstract ECFieldElement Invert();

		public abstract ECFieldElement Sqrt();

		public virtual ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.Multiply(b).Subtract(x.Multiply(y));
		}

		public virtual ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.Multiply(b).Add(x.Multiply(y));
		}

		public virtual ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.Square().Subtract(x.Multiply(y));
		}

		public virtual ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.Square().Add(x.Multiply(y));
		}

		public virtual ECFieldElement SquarePow(int pow)
		{
			ECFieldElement eCFieldElement = this;
			for (int i = 0; i < pow; i++)
			{
				eCFieldElement = eCFieldElement.Square();
			}
			return eCFieldElement;
		}

		public virtual bool TestBitZero()
		{
			return this.ToBigInteger().TestBit(0);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ECFieldElement);
		}

		public virtual bool Equals(ECFieldElement other)
		{
			return this == other || (other != null && this.ToBigInteger().Equals(other.ToBigInteger()));
		}

		public override int GetHashCode()
		{
			return this.ToBigInteger().GetHashCode();
		}

		public override string ToString()
		{
			return this.ToBigInteger().ToString(16);
		}

		public virtual byte[] GetEncoded()
		{
			return BigIntegers.AsUnsignedByteArray((this.FieldSize + 7) / 8, this.ToBigInteger());
		}
	}
}
