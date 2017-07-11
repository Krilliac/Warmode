using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT113FieldElement : ECFieldElement
	{
		protected internal readonly ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat128.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat128.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT113Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 113;
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
				return 113;
			}
		}

		public virtual int K1
		{
			get
			{
				return 9;
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

		public SecT113FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT113FieldElement", "x");
			}
			this.x = SecT113Field.FromBigInteger(x);
		}

		public SecT113FieldElement()
		{
			this.x = Nat128.Create64();
		}

		protected internal SecT113FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat128.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat128.Create64();
			SecT113Field.Add(this.x, ((SecT113FieldElement)b).x, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat128.Create64();
			SecT113Field.AddOne(this.x, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat128.Create64();
			SecT113Field.Multiply(this.x, ((SecT113FieldElement)b).x, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT113FieldElement)b).x;
			ulong[] array2 = ((SecT113FieldElement)x).x;
			ulong[] y3 = ((SecT113FieldElement)y).x;
			ulong[] array3 = Nat128.CreateExt64();
			SecT113Field.MultiplyAddToExt(array, y2, array3);
			SecT113Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat128.Create64();
			SecT113Field.Reduce(array3, z);
			return new SecT113FieldElement(z);
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
			ulong[] z = Nat128.Create64();
			SecT113Field.Square(this.x, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT113FieldElement)x).x;
			ulong[] y2 = ((SecT113FieldElement)y).x;
			ulong[] array3 = Nat128.CreateExt64();
			SecT113Field.SquareAddToExt(array, array3);
			SecT113Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat128.Create64();
			SecT113Field.Reduce(array3, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat128.Create64();
			SecT113Field.SquareN(this.x, pow, z);
			return new SecT113FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT113FieldElement(AbstractF2mCurve.Inverse(113, new int[]
			{
				9
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT113FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT113FieldElement);
		}

		public virtual bool Equals(SecT113FieldElement other)
		{
			return this == other || (other != null && Nat128.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 113009 ^ Arrays.GetHashCode(this.x, 0, 2);
		}
	}
}
