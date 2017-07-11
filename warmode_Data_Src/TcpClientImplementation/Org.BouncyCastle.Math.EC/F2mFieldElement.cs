using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.EC
{
	public class F2mFieldElement : ECFieldElement
	{
		public const int Gnb = 1;

		public const int Tpb = 2;

		public const int Ppb = 3;

		private int representation;

		private int m;

		private int[] ks;

		private LongArray x;

		public override int BitLength
		{
			get
			{
				return this.x.Degree();
			}
		}

		public override bool IsOne
		{
			get
			{
				return this.x.IsOne();
			}
		}

		public override bool IsZero
		{
			get
			{
				return this.x.IsZero();
			}
		}

		public override string FieldName
		{
			get
			{
				return "F2m";
			}
		}

		public override int FieldSize
		{
			get
			{
				return this.m;
			}
		}

		public int Representation
		{
			get
			{
				return this.representation;
			}
		}

		public int M
		{
			get
			{
				return this.m;
			}
		}

		public int K1
		{
			get
			{
				return this.ks[0];
			}
		}

		public int K2
		{
			get
			{
				if (this.ks.Length < 2)
				{
					return 0;
				}
				return this.ks[1];
			}
		}

		public int K3
		{
			get
			{
				if (this.ks.Length < 3)
				{
					return 0;
				}
				return this.ks[2];
			}
		}

		public F2mFieldElement(int m, int k1, int k2, int k3, BigInteger x)
		{
			if (k2 == 0 && k3 == 0)
			{
				this.representation = 2;
				this.ks = new int[]
				{
					k1
				};
			}
			else
			{
				if (k2 >= k3)
				{
					throw new ArgumentException("k2 must be smaller than k3");
				}
				if (k2 <= 0)
				{
					throw new ArgumentException("k2 must be larger than 0");
				}
				this.representation = 3;
				this.ks = new int[]
				{
					k1,
					k2,
					k3
				};
			}
			this.m = m;
			this.x = new LongArray(x);
		}

		public F2mFieldElement(int m, int k, BigInteger x) : this(m, k, 0, 0, x)
		{
		}

		private F2mFieldElement(int m, int[] ks, LongArray x)
		{
			this.m = m;
			this.representation = ((ks.Length == 1) ? 2 : 3);
			this.ks = ks;
			this.x = x;
		}

		public override bool TestBitZero()
		{
			return this.x.TestBitZero();
		}

		public override BigInteger ToBigInteger()
		{
			return this.x.ToBigInteger();
		}

		public static void CheckFieldElements(ECFieldElement a, ECFieldElement b)
		{
			if (!(a is F2mFieldElement) || !(b is F2mFieldElement))
			{
				throw new ArgumentException("Field elements are not both instances of F2mFieldElement");
			}
			F2mFieldElement f2mFieldElement = (F2mFieldElement)a;
			F2mFieldElement f2mFieldElement2 = (F2mFieldElement)b;
			if (f2mFieldElement.representation != f2mFieldElement2.representation)
			{
				throw new ArgumentException("One of the F2m field elements has incorrect representation");
			}
			if (f2mFieldElement.m != f2mFieldElement2.m || !Arrays.AreEqual(f2mFieldElement.ks, f2mFieldElement2.ks))
			{
				throw new ArgumentException("Field elements are not elements of the same field F2m");
			}
		}

		public override ECFieldElement Add(ECFieldElement b)
		{
			LongArray longArray = this.x.Copy();
			F2mFieldElement f2mFieldElement = (F2mFieldElement)b;
			longArray.AddShiftedByWords(f2mFieldElement.x, 0);
			return new F2mFieldElement(this.m, this.ks, longArray);
		}

		public override ECFieldElement AddOne()
		{
			return new F2mFieldElement(this.m, this.ks, this.x.AddOne());
		}

		public override ECFieldElement Subtract(ECFieldElement b)
		{
			return this.Add(b);
		}

		public override ECFieldElement Multiply(ECFieldElement b)
		{
			return new F2mFieldElement(this.m, this.ks, this.x.ModMultiply(((F2mFieldElement)b).x, this.m, this.ks));
		}

		public override ECFieldElement MultiplyMinusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			return this.MultiplyPlusProduct(b, x, y);
		}

		public override ECFieldElement MultiplyPlusProduct(ECFieldElement b, ECFieldElement x, ECFieldElement y)
		{
			LongArray longArray = this.x;
			LongArray longArray2 = ((F2mFieldElement)b).x;
			LongArray longArray3 = ((F2mFieldElement)x).x;
			LongArray other = ((F2mFieldElement)y).x;
			LongArray longArray4 = longArray.Multiply(longArray2, this.m, this.ks);
			LongArray other2 = longArray3.Multiply(other, this.m, this.ks);
			if (longArray4 == longArray || longArray4 == longArray2)
			{
				longArray4 = longArray4.Copy();
			}
			longArray4.AddShiftedByWords(other2, 0);
			longArray4.Reduce(this.m, this.ks);
			return new F2mFieldElement(this.m, this.ks, longArray4);
		}

		public override ECFieldElement Divide(ECFieldElement b)
		{
			ECFieldElement b2 = b.Invert();
			return this.Multiply(b2);
		}

		public override ECFieldElement Negate()
		{
			return this;
		}

		public override ECFieldElement Square()
		{
			return new F2mFieldElement(this.m, this.ks, this.x.ModSquare(this.m, this.ks));
		}

		public override ECFieldElement SquareMinusProduct(ECFieldElement x, ECFieldElement y)
		{
			return this.SquarePlusProduct(x, y);
		}

		public override ECFieldElement SquarePlusProduct(ECFieldElement x, ECFieldElement y)
		{
			LongArray longArray = this.x;
			LongArray longArray2 = ((F2mFieldElement)x).x;
			LongArray other = ((F2mFieldElement)y).x;
			LongArray longArray3 = longArray.Square(this.m, this.ks);
			LongArray other2 = longArray2.Multiply(other, this.m, this.ks);
			if (longArray3 == longArray)
			{
				longArray3 = longArray3.Copy();
			}
			longArray3.AddShiftedByWords(other2, 0);
			longArray3.Reduce(this.m, this.ks);
			return new F2mFieldElement(this.m, this.ks, longArray3);
		}

		public override ECFieldElement SquarePow(int pow)
		{
			if (pow >= 1)
			{
				return new F2mFieldElement(this.m, this.ks, this.x.ModSquareN(pow, this.m, this.ks));
			}
			return this;
		}

		public override ECFieldElement Invert()
		{
			return new F2mFieldElement(this.m, this.ks, this.x.ModInverse(this.m, this.ks));
		}

		public override ECFieldElement Sqrt()
		{
			if (!this.x.IsZero() && !this.x.IsOne())
			{
				return this.SquarePow(this.m - 1);
			}
			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			F2mFieldElement f2mFieldElement = obj as F2mFieldElement;
			return f2mFieldElement != null && this.Equals(f2mFieldElement);
		}

		public virtual bool Equals(F2mFieldElement other)
		{
			return this.m == other.m && this.representation == other.representation && Arrays.AreEqual(this.ks, other.ks) && this.x.Equals(other.x);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.m ^ Arrays.GetHashCode(this.ks);
		}
	}
}
