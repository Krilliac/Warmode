using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP128R1FieldElement : ECFieldElement
	{
		public static readonly BigInteger Q = SecP128R1Curve.q;

		protected internal readonly uint[] x;

		public override bool IsZero
		{
			get
			{
				return Nat128.IsZero(this.x);
			}
		}

		public override bool IsOne
		{
			get
			{
				return Nat128.IsOne(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecP128R1Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return SecP128R1FieldElement.Q.BitLength;
			}
		}

		public SecP128R1FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(SecP128R1FieldElement.Q) >= 0)
			{
				throw new ArgumentException("value invalid for SecP128R1FieldElement", "x");
			}
			this.x = SecP128R1Field.FromBigInteger(x);
		}

		public SecP128R1FieldElement()
		{
			this.x = Nat128.Create();
		}

		protected internal SecP128R1FieldElement(uint[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return Nat128.GetBit(this.x, 0) == 1u;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat128.ToBigInteger(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.Add(this.x, ((SecP128R1FieldElement)b).x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.AddOne(this.x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.Subtract(this.x, ((SecP128R1FieldElement)b).x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.Multiply(this.x, ((SecP128R1FieldElement)b).x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			uint[] z = Nat128.Create();
			Mod.Invert(SecP128R1Field.P, ((SecP128R1FieldElement)b).x, z);
			SecP128R1Field.Multiply(z, this.x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Negate()
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.Negate(this.x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Square()
		{
			uint[] z = Nat128.Create();
			SecP128R1Field.Square(this.x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			uint[] z = Nat128.Create();
			Mod.Invert(SecP128R1Field.P, this.x, z);
			return new SecP128R1FieldElement(z);
		}

		public override ECFieldElement Sqrt()
		{
			uint[] y = this.x;
			if (Nat128.IsZero(y) || Nat128.IsOne(y))
			{
				return this;
			}
			uint[] array = Nat128.Create();
			SecP128R1Field.Square(y, array);
			SecP128R1Field.Multiply(array, y, array);
			uint[] array2 = Nat128.Create();
			SecP128R1Field.SquareN(array, 2, array2);
			SecP128R1Field.Multiply(array2, array, array2);
			uint[] array3 = Nat128.Create();
			SecP128R1Field.SquareN(array2, 4, array3);
			SecP128R1Field.Multiply(array3, array2, array3);
			uint[] array4 = array2;
			SecP128R1Field.SquareN(array3, 2, array4);
			SecP128R1Field.Multiply(array4, array, array4);
			uint[] z = array;
			SecP128R1Field.SquareN(array4, 10, z);
			SecP128R1Field.Multiply(z, array4, z);
			uint[] array5 = array3;
			SecP128R1Field.SquareN(z, 10, array5);
			SecP128R1Field.Multiply(array5, array4, array5);
			uint[] array6 = array4;
			SecP128R1Field.Square(array5, array6);
			SecP128R1Field.Multiply(array6, y, array6);
			uint[] z2 = array6;
			SecP128R1Field.SquareN(z2, 95, z2);
			uint[] array7 = array5;
			SecP128R1Field.Square(z2, array7);
			if (!Nat128.Eq(y, array7))
			{
				return null;
			}
			return new SecP128R1FieldElement(z2);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecP128R1FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecP128R1FieldElement);
		}

		public virtual bool Equals(SecP128R1FieldElement other)
		{
			return this == other || (other != null && Nat128.Eq(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return SecP128R1FieldElement.Q.GetHashCode() ^ Arrays.GetHashCode(this.x, 0, 4);
		}
	}
}
