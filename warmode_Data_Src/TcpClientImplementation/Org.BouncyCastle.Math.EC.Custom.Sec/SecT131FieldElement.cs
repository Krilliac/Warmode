using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT131FieldElement : ECFieldElement
	{
		protected readonly ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat192.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat192.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT131Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 131;
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
				return 131;
			}
		}

		public virtual int K1
		{
			get
			{
				return 2;
			}
		}

		public virtual int K2
		{
			get
			{
				return 3;
			}
		}

		public virtual int K3
		{
			get
			{
				return 8;
			}
		}

		public SecT131FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT131FieldElement", "x");
			}
			this.x = SecT131Field.FromBigInteger(x);
		}

		public SecT131FieldElement()
		{
			this.x = Nat192.Create64();
		}

		protected internal SecT131FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat192.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat192.Create64();
			SecT131Field.Add(this.x, ((SecT131FieldElement)b).x, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat192.Create64();
			SecT131Field.AddOne(this.x, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat192.Create64();
			SecT131Field.Multiply(this.x, ((SecT131FieldElement)b).x, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT131FieldElement)b).x;
			ulong[] array2 = ((SecT131FieldElement)x).x;
			ulong[] y3 = ((SecT131FieldElement)y).x;
			ulong[] array3 = Nat.Create64(5);
			SecT131Field.MultiplyAddToExt(array, y2, array3);
			SecT131Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat192.Create64();
			SecT131Field.Reduce(array3, z);
			return new SecT131FieldElement(z);
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
			ulong[] z = Nat192.Create64();
			SecT131Field.Square(this.x, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT131FieldElement)x).x;
			ulong[] y2 = ((SecT131FieldElement)y).x;
			ulong[] array3 = Nat.Create64(5);
			SecT131Field.SquareAddToExt(array, array3);
			SecT131Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat192.Create64();
			SecT131Field.Reduce(array3, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat192.Create64();
			SecT131Field.SquareN(this.x, pow, z);
			return new SecT131FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT131FieldElement(AbstractF2mCurve.Inverse(131, new int[]
			{
				2,
				3,
				8
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT131FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT131FieldElement);
		}

		public virtual bool Equals(SecT131FieldElement other)
		{
			return this == other || (other != null && Nat192.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 131832 ^ Arrays.GetHashCode(this.x, 0, 3);
		}
	}
}
