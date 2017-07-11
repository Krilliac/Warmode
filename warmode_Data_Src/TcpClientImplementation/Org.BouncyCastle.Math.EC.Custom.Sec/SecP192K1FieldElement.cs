using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192K1FieldElement : ECFieldElement
	{
		public static readonly BigInteger Q = SecP192K1Curve.q;

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
				return "SecP192K1Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return SecP192K1FieldElement.Q.BitLength;
			}
		}

		public SecP192K1FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(SecP192K1FieldElement.Q) >= 0)
			{
				throw new ArgumentException("value invalid for SecP192K1FieldElement", "x");
			}
			this.x = SecP192K1Field.FromBigInteger(x);
		}

		public SecP192K1FieldElement()
		{
			this.x = Nat192.Create();
		}

		protected internal SecP192K1FieldElement(uint[] x)
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
			SecP192K1Field.Add(this.x, ((SecP192K1FieldElement)b).x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			uint[] z = Nat192.Create();
			SecP192K1Field.AddOne(this.x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			SecP192K1Field.Subtract(this.x, ((SecP192K1FieldElement)b).x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			SecP192K1Field.Multiply(this.x, ((SecP192K1FieldElement)b).x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			uint[] z = Nat192.Create();
			Mod.Invert(SecP192K1Field.P, ((SecP192K1FieldElement)b).x, z);
			SecP192K1Field.Multiply(z, this.x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Negate()
		{
			uint[] z = Nat192.Create();
			SecP192K1Field.Negate(this.x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Square()
		{
			uint[] z = Nat192.Create();
			SecP192K1Field.Square(this.x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			uint[] z = Nat192.Create();
			Mod.Invert(SecP192K1Field.P, this.x, z);
			return new SecP192K1FieldElement(z);
		}

		public override ECFieldElement Sqrt()
		{
			uint[] y = this.x;
			if (Nat192.IsZero(y) || Nat192.IsOne(y))
			{
				return this;
			}
			uint[] array = Nat192.Create();
			SecP192K1Field.Square(y, array);
			SecP192K1Field.Multiply(array, y, array);
			uint[] array2 = Nat192.Create();
			SecP192K1Field.Square(array, array2);
			SecP192K1Field.Multiply(array2, y, array2);
			uint[] array3 = Nat192.Create();
			SecP192K1Field.SquareN(array2, 3, array3);
			SecP192K1Field.Multiply(array3, array2, array3);
			uint[] array4 = array3;
			SecP192K1Field.SquareN(array3, 2, array4);
			SecP192K1Field.Multiply(array4, array, array4);
			uint[] array5 = array;
			SecP192K1Field.SquareN(array4, 8, array5);
			SecP192K1Field.Multiply(array5, array4, array5);
			uint[] array6 = array4;
			SecP192K1Field.SquareN(array5, 3, array6);
			SecP192K1Field.Multiply(array6, array2, array6);
			uint[] array7 = Nat192.Create();
			SecP192K1Field.SquareN(array6, 16, array7);
			SecP192K1Field.Multiply(array7, array5, array7);
			uint[] array8 = array5;
			SecP192K1Field.SquareN(array7, 35, array8);
			SecP192K1Field.Multiply(array8, array7, array8);
			uint[] z = array7;
			SecP192K1Field.SquareN(array8, 70, z);
			SecP192K1Field.Multiply(z, array8, z);
			uint[] array9 = array8;
			SecP192K1Field.SquareN(z, 19, array9);
			SecP192K1Field.Multiply(array9, array6, array9);
			uint[] z2 = array9;
			SecP192K1Field.SquareN(z2, 20, z2);
			SecP192K1Field.Multiply(z2, array6, z2);
			SecP192K1Field.SquareN(z2, 4, z2);
			SecP192K1Field.Multiply(z2, array2, z2);
			SecP192K1Field.SquareN(z2, 6, z2);
			SecP192K1Field.Multiply(z2, array2, z2);
			SecP192K1Field.Square(z2, z2);
			uint[] array10 = array2;
			SecP192K1Field.Square(z2, array10);
			if (!Nat192.Eq(y, array10))
			{
				return null;
			}
			return new SecP192K1FieldElement(z2);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecP192K1FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecP192K1FieldElement);
		}

		public virtual bool Equals(SecP192K1FieldElement other)
		{
			return this == other || (other != null && Nat192.Eq(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return SecP192K1FieldElement.Q.GetHashCode() ^ Arrays.GetHashCode(this.x, 0, 6);
		}
	}
}
