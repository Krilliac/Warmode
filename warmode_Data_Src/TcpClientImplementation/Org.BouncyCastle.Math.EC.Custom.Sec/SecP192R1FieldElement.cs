using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192R1FieldElement : ECFieldElement
	{
		public static readonly BigInteger Q = SecP192R1Curve.q;

		protected internal readonly uint[] x;

		public override bool IsZero
		{
			get
			{
				return Nat192.IsZero(this.x);
			}
		}

		public override bool IsOne
		{
			get
			{
				return Nat192.IsOne(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecP192R1Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return SecP192R1FieldElement.Q.BitLength;
			}
		}

		public SecP192R1FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(SecP192R1FieldElement.Q) >= 0)
			{
				throw new ArgumentException("value invalid for SecP192R1FieldElement", "x");
			}
			this.x = SecP192R1Field.FromBigInteger(x);
		}

		public SecP192R1FieldElement()
		{
			this.x = Nat192.Create();
		}

		protected internal SecP192R1FieldElement(uint[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return Nat192.GetBit(this.x, 0) == 1u;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat192.ToBigInteger(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.Add(this.x, ((SecP192R1FieldElement)b).x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.AddOne(this.x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.Subtract(this.x, ((SecP192R1FieldElement)b).x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.Multiply(this.x, ((SecP192R1FieldElement)b).x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			Mod.Invert(SecP192R1Field.P, ((SecP192R1FieldElement)b).x, z);
			SecP192R1Field.Multiply(z, this.x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Negate()
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.Negate(this.x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Square()
		{
			uint[] z = Nat192.Create();
			SecP192R1Field.Square(this.x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			uint[] z = Nat192.Create();
			Mod.Invert(SecP192R1Field.P, this.x, z);
			return new SecP192R1FieldElement(z);
		}

		public override ECFieldElement Sqrt()
		{
			uint[] y = this.x;
			if (Nat192.IsZero(y) || Nat192.IsOne(y))
			{
				return this;
			}
			uint[] array = Nat192.Create();
			uint[] array2 = Nat192.Create();
			SecP192R1Field.Square(y, array);
			SecP192R1Field.Multiply(array, y, array);
			SecP192R1Field.SquareN(array, 2, array2);
			SecP192R1Field.Multiply(array2, array, array2);
			SecP192R1Field.SquareN(array2, 4, array);
			SecP192R1Field.Multiply(array, array2, array);
			SecP192R1Field.SquareN(array, 8, array2);
			SecP192R1Field.Multiply(array2, array, array2);
			SecP192R1Field.SquareN(array2, 16, array);
			SecP192R1Field.Multiply(array, array2, array);
			SecP192R1Field.SquareN(array, 32, array2);
			SecP192R1Field.Multiply(array2, array, array2);
			SecP192R1Field.SquareN(array2, 64, array);
			SecP192R1Field.Multiply(array, array2, array);
			SecP192R1Field.SquareN(array, 62, array);
			SecP192R1Field.Square(array, array2);
			if (!Nat192.Eq(y, array2))
			{
				return null;
			}
			return new SecP192R1FieldElement(array);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecP192R1FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecP192R1FieldElement);
		}

		public virtual bool Equals(SecP192R1FieldElement other)
		{
			return this == other || (other != null && Nat192.Eq(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return SecP192R1FieldElement.Q.GetHashCode() ^ Arrays.GetHashCode(this.x, 0, 6);
		}
	}
}
