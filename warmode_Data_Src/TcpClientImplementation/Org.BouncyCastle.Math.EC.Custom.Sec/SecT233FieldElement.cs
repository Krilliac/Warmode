using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT233FieldElement : ECFieldElement
	{
		protected readonly ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat256.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat256.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT233Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 233;
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
				return 233;
			}
		}

		public virtual int K1
		{
			get
			{
				return 74;
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

		public SecT233FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT233FieldElement", "x");
			}
			this.x = SecT233Field.FromBigInteger(x);
		}

		public SecT233FieldElement()
		{
			this.x = Nat256.Create64();
		}

		protected internal SecT233FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat256.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat256.Create64();
			SecT233Field.Add(this.x, ((SecT233FieldElement)b).x, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat256.Create64();
			SecT233Field.AddOne(this.x, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat256.Create64();
			SecT233Field.Multiply(this.x, ((SecT233FieldElement)b).x, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT233FieldElement)b).x;
			ulong[] array2 = ((SecT233FieldElement)x).x;
			ulong[] y3 = ((SecT233FieldElement)y).x;
			ulong[] array3 = Nat256.CreateExt64();
			SecT233Field.MultiplyAddToExt(array, y2, array3);
			SecT233Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat256.Create64();
			SecT233Field.Reduce(array3, z);
			return new SecT233FieldElement(z);
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
			ulong[] z = Nat256.Create64();
			SecT233Field.Square(this.x, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT233FieldElement)x).x;
			ulong[] y2 = ((SecT233FieldElement)y).x;
			ulong[] array3 = Nat256.CreateExt64();
			SecT233Field.SquareAddToExt(array, array3);
			SecT233Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat256.Create64();
			SecT233Field.Reduce(array3, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat256.Create64();
			SecT233Field.SquareN(this.x, pow, z);
			return new SecT233FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT233FieldElement(AbstractF2mCurve.Inverse(233, new int[]
			{
				74
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT233FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT233FieldElement);
		}

		public virtual bool Equals(SecT233FieldElement other)
		{
			return this == other || (other != null && Nat256.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 2330074 ^ Arrays.GetHashCode(this.x, 0, 4);
		}
	}
}
