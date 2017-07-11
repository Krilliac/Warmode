using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT571FieldElement : ECFieldElement
	{
		protected readonly ulong[] x;

		public override bool IsOne
		{
			get
			{
				return Nat576.IsOne64(this.x);
			}
		}

		public override bool IsZero
		{
			get
			{
				return Nat576.IsZero64(this.x);
			}
		}

		public override string FieldName
		{
			get
			{
				return "SecT571Field";
			}
		}

		public override int FieldSize
		{
			get
			{
				return 571;
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
				return 571;
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
				return 5;
			}
		}

		public virtual int K3
		{
			get
			{
				return 10;
			}
		}

		public SecT571FieldElement(BigInteger x)
		{
			if (x == null || x.SignValue < 0)
			{
				throw new ArgumentException("value invalid for SecT571FieldElement", "x");
			}
			this.x = SecT571Field.FromBigInteger(x);
		}

		public SecT571FieldElement()
		{
			this.x = Nat576.Create64();
		}

		protected internal SecT571FieldElement(ulong[] x)
		{
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return (this.x[0] & 1uL) != 0uL;
		}

		public override BigInteger ToBigInteger()
		{
			return Nat576.ToBigInteger64(this.x);
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			ulong[] z = Nat576.Create64();
			SecT571Field.Add(this.x, ((SecT571FieldElement)b).x, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement AddOne()
		{
			ulong[] z = Nat576.Create64();
			SecT571Field.AddOne(this.x, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			ulong[] z = Nat576.Create64();
			SecT571Field.Multiply(this.x, ((SecT571FieldElement)b).x, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] y2 = ((SecT571FieldElement)b).x;
			ulong[] array2 = ((SecT571FieldElement)x).x;
			ulong[] y3 = ((SecT571FieldElement)y).x;
			ulong[] array3 = Nat576.CreateExt64();
			SecT571Field.MultiplyAddToExt(array, y2, array3);
			SecT571Field.MultiplyAddToExt(array2, y3, array3);
			ulong[] z = Nat576.Create64();
			SecT571Field.Reduce(array3, z);
			return new SecT571FieldElement(z);
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
			ulong[] z = Nat576.Create64();
			SecT571Field.Square(this.x, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			ulong[] array = this.x;
			ulong[] array2 = ((SecT571FieldElement)x).x;
			ulong[] y2 = ((SecT571FieldElement)y).x;
			ulong[] array3 = Nat576.CreateExt64();
			SecT571Field.SquareAddToExt(array, array3);
			SecT571Field.MultiplyAddToExt(array2, y2, array3);
			ulong[] z = Nat576.Create64();
			SecT571Field.Reduce(array3, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow < 1)
			{
				return this;
			}
			ulong[] z = Nat576.Create64();
			SecT571Field.SquareN(this.x, pow, z);
			return new SecT571FieldElement(z);
		}

		public override ECFieldElement Invert()
		{
			return new SecT571FieldElement(AbstractF2mCurve.Inverse(571, new int[]
			{
				2,
				5,
				10
			}, this.ToBigInteger()));
		}

		public override ECFieldElement Sqrt()
		{
			return this.SquarePow(this.M - 1);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SecT571FieldElement);
		}

		public override bool Equals(ECFieldElement other)
		{
			return this.Equals(other as SecT571FieldElement);
		}

		public virtual bool Equals(SecT571FieldElement other)
		{
			return this == other || (other != null && Nat576.Eq64(this.x, other.x));
		}

		public override int GetHashCode()
		{
			return 5711052 ^ Arrays.GetHashCode(this.x, 0, 9);
		}
	}
}
