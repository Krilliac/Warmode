using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT409FieldElement : ECFieldElement
	{
		protected ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat448.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat448.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT409Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 409;
			}
		}

		public virtual int Representation
		{
			get
			{
				return 2;
			}
		}

		public virtual int M
		{
			get
			{
				return 409;
			}
		}

		public virtual int K1
		{
			get
			{
				return 87;
			}
		}

		public virtual int K2
		{
			get
			{
				return 0;
			}
		}

		public virtual int K3
		{
			get
			{
				return 0;
			}
		}

		public SecT409FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT409FieldElement", "x");
			}
			this.x = SecT409Field.FromBigInteger(x);
		}

		public SecT409FieldElement()
		{
			this.x = Nat448.Create64();
		}

		protected internal SecT409FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat448.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat448.Create64();
			SecT409Field.Add(this.x, ((SecT409FieldElement)b).x, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat448.Create64();
			SecT409Field.AddOne(this.x, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat448.Create64();
			SecT409Field.Multiply(this.x, ((SecT409FieldElement)b).x, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT409FieldElement)b).x;
			ulong[] array2 = ((SecT409FieldElement)x).x;
			ulong[] y3 = ((SecT409FieldElement)y).x;
			ulong[] array3 = Nat.Create64(13);
			SecT409Field.MultiplyAddToExt(array, y2, array3);
			SecT409Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat448.Create64();
			SecT409Field.Reduce(array3, z);
			return new SecT409FieldElement(z);
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
			ulong[] z = Nat448.Create64();
			SecT409Field.Square(this.x, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT409FieldElement)x).x;
			ulong[] y2 = ((SecT409FieldElement)y).x;
			ulong[] array3 = Nat.Create64(13);
			SecT409Field.SquareAddToExt(array, array3);
			SecT409Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat448.Create64();
			SecT409Field.Reduce(array3, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat448.Create64();
			SecT409Field.SquareN(this.x, pow, z);
			return new SecT409FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT409FieldElement(AbstractF2mCurve.Inverse(409, new int[]
			{
				87
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT409FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT409FieldElement);
		}

		public virtual bool Equals(SecT409FieldElement other)
		{
			return this == other || (other != null && Nat448.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 4090087 ^ Arrays.GetHashCode(this.x, 0, 7);
		}
	}
}
