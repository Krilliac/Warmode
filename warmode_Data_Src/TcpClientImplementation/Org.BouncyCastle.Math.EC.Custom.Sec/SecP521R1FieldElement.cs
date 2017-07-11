using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP521R1FieldElement : ECFieldElement
	{
		public static readonly BigInteger Q = SecP521R1Curve.q;

		protected internal readonly uint[] x;

		public override bool IsZero
		{
			get
			{
				return Nat.IsZero(17, this.x);
			}
		}

		public override bool IsOne
		{
			get
			{
				return Nat.IsOne(17, this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecP521R1Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return SecP521R1FieldElement.Q.BitLength;
			}
		}

		public SecP521R1FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0 || x.CompareTo(SecP521R1FieldElement.Q) >= 0)
			{
				throw new ArgumentException("value invalid for SecP521R1FieldElement", "x");
			}
			this.x = SecP521R1Field.FromBigInteger(x);
		}

		public SecP521R1FieldElement()
		{
			this.x = Nat.Create(17);
		}

		protected internal SecP521R1FieldElement(uint[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return Nat.GetBit(this.x, 0) == 1u;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat.ToBigInteger(17, this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.Add(this.x, ((SecP521R1FieldElement)b).x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.AddOne(this.x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.Subtract(this.x, ((SecP521R1FieldElement)b).x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.Multiply(this.x, ((SecP521R1FieldElement)b).x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			uint[] z = Nat.Create(17);
			Mod.Invert(SecP521R1Field.P, ((SecP521R1FieldElement)b).x, z);
			SecP521R1Field.Multiply(z, this.x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Negate()
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.Negate(this.x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Square()
		{
			uint[] z = Nat.Create(17);
			SecP521R1Field.Square(this.x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			uint[] z = Nat.Create(17);
			Mod.Invert(SecP521R1Field.P, this.x, z);
			return new SecP521R1FieldElement(z);
		}

		public override ECFieldElement Sqrt()
		{
			uint[] array = this.x;
			if (Nat.IsZero(17, array) || Nat.IsOne(17, array))
			{
				return this;
			}
			uint[] z = Nat.Create(17);
			uint[] array2 = Nat.Create(17);
			SecP521R1Field.SquareN(array, 519, z);
			SecP521R1Field.Square(z, array2);
			if (!Nat.Eq(17, array, array2))
			{
				return null;
			}
			return new SecP521R1FieldElement(z);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecP521R1FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecP521R1FieldElement);
		}

		public virtual bool Equals(SecP521R1FieldElement other)
		{
			return this == other || (other != null && Nat.Eq(17, this.x, other.x));
		}

		public override int GetHashCode()
		{
			return SecP521R1FieldElement.Q.GetHashCode() ^ Arrays.GetHashCode(this.x, 0, 17);
		}
	}
}
