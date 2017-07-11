using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT283FieldElement : ECFieldElement
	{
		protected readonly ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat320.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat320.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT283Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 283;
			}
		}

		public virtual int Representation
		{
			get
			{
				return 3;
			}
		}

		public virtual int M
		{
			get
			{
				return 283;
			}
		}

		public virtual int K1
		{
			get
			{
				return 5;
			}
		}

		public virtual int K2
		{
			get
			{
				return 7;
			}
		}

		public virtual int K3
		{
			get
			{
				return 12;
			}
		}

		public SecT283FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT283FieldElement", "x");
			}
			this.x = SecT283Field.FromBigInteger(x);
		}

		public SecT283FieldElement()
		{
			this.x = Nat320.Create64();
		}

		protected internal SecT283FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat320.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat320.Create64();
			SecT283Field.Add(this.x, ((SecT283FieldElement)b).x, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat320.Create64();
			SecT283Field.AddOne(this.x, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat320.Create64();
			SecT283Field.Multiply(this.x, ((SecT283FieldElement)b).x, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT283FieldElement)b).x;
			ulong[] array2 = ((SecT283FieldElement)x).x;
			ulong[] y3 = ((SecT283FieldElement)y).x;
			ulong[] array3 = Nat.Create64(9);
			SecT283Field.MultiplyAddToExt(array, y2, array3);
			SecT283Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat320.Create64();
			SecT283Field.Reduce(array3, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			return this.Multiply(b.Invert());
		}

		public override ECFieldElement Negate()
		{
			return this;
		}

		public override ECFieldElement Square()
		{
			ulong[] z = Nat320.Create64();
			SecT283Field.Square(this.x, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT283FieldElement)x).x;
			ulong[] y2 = ((SecT283FieldElement)y).x;
			ulong[] array3 = Nat.Create64(9);
			SecT283Field.SquareAddToExt(array, array3);
			SecT283Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat320.Create64();
			SecT283Field.Reduce(array3, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat320.Create64();
			SecT283Field.SquareN(this.x, pow, z);
			return new SecT283FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT283FieldElement(AbstractF2mCurve.Inverse(283, new int[]
			{
				5,
				7,
				12
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT283FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT283FieldElement);
		}

		public virtual bool Equals(SecT283FieldElement other)
		{
			return this == other || (other != null && Nat320.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 2831275 ^ Arrays.GetHashCode(this.x, 0, 5);
		}
	}
}
