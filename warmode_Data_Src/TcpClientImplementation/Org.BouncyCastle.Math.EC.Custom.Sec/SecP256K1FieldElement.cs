using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP256K1FieldElement : ECFieldElement
	{
		public static readonly BigInteger Q = SecP256K1Curve.q;

		protected internal readonly uint[] x;

		public override bool IsZero
		{
			get
			{
				return Nat256.IsZero(this.x);
			}
		}

		public override bool IsOne
		{
			get
			{
				return Nat256.IsOne(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecP256K1Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return SecP256K1FieldElement.Q.BitLength;
			}
		}

		public SecP256K1FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(SecP256K1FieldElement.Q) >= 0)
			{
				throw new ArgumentException("value invalid for SecP256K1FieldElement", "x");
			}
			this.x = SecP256K1Field.FromBigInteger(x);
		}

		public SecP256K1FieldElement()
		{
			this.x = Nat256.Create();
		}

		protected internal SecP256K1FieldElement(uint[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return Nat256.GetBit(this.x, 0) == 1u;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat256.ToBigInteger(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.Add(this.x, ((SecP256K1FieldElement)b).x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.AddOne(this.x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.Subtract(this.x, ((SecP256K1FieldElement)b).x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.Multiply(this.x, ((SecP256K1FieldElement)b).x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			uint[] z = Nat256.Create();
			Mod.Invert(SecP256K1Field.P, ((SecP256K1FieldElement)b).x, z);
			SecP256K1Field.Multiply(z, this.x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Negate()
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.Negate(this.x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Square()
		{
			uint[] z = Nat256.Create();
			SecP256K1Field.Square(this.x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			uint[] z = Nat256.Create();
			Mod.Invert(SecP256K1Field.P, this.x, z);
			return new SecP256K1FieldElement(z);
		}

		public override ECFieldElement Sqrt()
		{
			uint[] y = this.x;
			if (Nat256.IsZero(y) || Nat256.IsOne(y))
			{
				return this;
			}
			uint[] array = Nat256.Create();
			SecP256K1Field.Square(y, array);
			SecP256K1Field.Multiply(array, y, array);
			uint[] array2 = Nat256.Create();
			SecP256K1Field.Square(array, array2);
			SecP256K1Field.Multiply(array2, y, array2);
			uint[] array3 = Nat256.Create();
			SecP256K1Field.SquareN(array2, 3, array3);
			SecP256K1Field.Multiply(array3, array2, array3);
			uint[] array4 = array3;
			SecP256K1Field.SquareN(array3, 3, array4);
			SecP256K1Field.Multiply(array4, array2, array4);
			uint[] array5 = array4;
			SecP256K1Field.SquareN(array4, 2, array5);
			SecP256K1Field.Multiply(array5, array, array5);
			uint[] array6 = Nat256.Create();
			SecP256K1Field.SquareN(array5, 11, array6);
			SecP256K1Field.Multiply(array6, array5, array6);
			uint[] array7 = array5;
			SecP256K1Field.SquareN(array6, 22, array7);
			SecP256K1Field.Multiply(array7, array6, array7);
			uint[] array8 = Nat256.Create();
			SecP256K1Field.SquareN(array7, 44, array8);
			SecP256K1Field.Multiply(array8, array7, array8);
			uint[] z = Nat256.Create();
			SecP256K1Field.SquareN(array8, 88, z);
			SecP256K1Field.Multiply(z, array8, z);
			uint[] z2 = array8;
			SecP256K1Field.SquareN(z, 44, z2);
			SecP256K1Field.Multiply(z2, array7, z2);
			uint[] array9 = array7;
			SecP256K1Field.SquareN(z2, 3, array9);
			SecP256K1Field.Multiply(array9, array2, array9);
			uint[] z3 = array9;
			SecP256K1Field.SquareN(z3, 23, z3);
			SecP256K1Field.Multiply(z3, array6, z3);
			SecP256K1Field.SquareN(z3, 6, z3);
			SecP256K1Field.Multiply(z3, array, z3);
			SecP256K1Field.SquareN(z3, 2, z3);
			uint[] array10 = array;
			SecP256K1Field.Square(z3, array10);
			if (!Nat256.Eq(y, array10))
			{
				return null;
			}
			return new SecP256K1FieldElement(z3);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecP256K1FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecP256K1FieldElement);
		}

		public virtual bool Equals(SecP256K1FieldElement other)
		{
			return this == other || (other != null && Nat256.Eq(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return SecP256K1FieldElement.Q.GetHashCode() ^ Arrays.GetHashCode(this.x, 0, 8);
		}
	}
}
