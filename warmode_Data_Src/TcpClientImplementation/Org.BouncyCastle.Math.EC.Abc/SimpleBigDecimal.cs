using System;
using System.Text;

namespace Org.BouncyCastle.Math.EC.Abc
{
	internal class SimpleBigDecimal
	{
		private readonly BigInteger bigInt;

		private readonly int scale;

		public int IntValue
		{
			get
			{
				return this.Floor().IntValue;
			}
		}

		public long LongValue
		{
			get
			{
				return this.Floor().LongValue;
			}
		}

		public int Scale
		{
			get
			{
				return this.scale;
			}
		}

		public static SimpleBigDecimal GetInstance(BigInteger val, int scale)
		{
			return new SimpleBigDecimal(val.ShiftLeft(scale), scale);
		}

		public SimpleBigDecimal(BigInteger bigInt, int scale)
		{
			if (scale < 0)
			{
				throw new ArgumentException("scale may not be negative");
			}
			this.bigInt = bigInt;
			this.scale = scale;
		}

		private SimpleBigDecimal(SimpleBigDecimal limBigDec)
		{
			this.bigInt = limBigDec.bigInt;
			this.scale = limBigDec.scale;
		}

		private void CheckScale(SimpleBigDecimal b)
		{
			if (this.scale != b.scale)
			{
				throw new ArgumentException("Only SimpleBigDecimal of same scale allowed in arithmetic operations");
			}
		}

		public SimpleBigDecimal AdjustScale(int newScale)
		{
			if (newScale < 0)
			{
				throw new ArgumentException("scale may not be negative");
			}
			if (newScale == this.scale)
			{
				return this;
			}
			return new SimpleBigDecimal(this.bigInt.ShiftLeft(newScale - this.scale), newScale);
		}

		public SimpleBigDecimal Add(SimpleBigDecimal b)
		{
			this.CheckScale(b);
			return new SimpleBigDecimal(this.bigInt.Add(b.bigInt), this.scale);
		}

		public SimpleBigDecimal Add(BigInteger b)
		{
			return new SimpleBigDecimal(this.bigInt.Add(b.ShiftLeft(this.scale)), this.scale);
		}

		public SimpleBigDecimal Negate()
		{
			return new SimpleBigDecimal(this.bigInt.Negate(), this.scale);
		}

		public SimpleBigDecimal Subtract(SimpleBigDecimal b)
		{
			return this.Add(b.Negate());
		}

		public SimpleBigDecimal Subtract(BigInteger b)
		{
			return new SimpleBigDecimal(this.bigInt.Subtract(b.ShiftLeft(this.scale)), this.scale);
		}

		public SimpleBigDecimal Multiply(SimpleBigDecimal b)
		{
			this.CheckScale(b);
			return new SimpleBigDecimal(this.bigInt.Multiply(b.bigInt), this.scale + this.scale);
		}

		public SimpleBigDecimal Multiply(BigInteger b)
		{
			return new SimpleBigDecimal(this.bigInt.Multiply(b), this.scale);
		}

		public SimpleBigDecimal Divide(SimpleBigDecimal b)
		{
			this.CheckScale(b);
			BigInteger bigInteger = this.bigInt.ShiftLeft(this.scale);
			return new SimpleBigDecimal(bigInteger.Divide(b.bigInt), this.scale);
		}

		public SimpleBigDecimal Divide(BigInteger b)
		{
			return new SimpleBigDecimal(this.bigInt.Divide(b), this.scale);
		}

		public SimpleBigDecimal ShiftLeft(int n)
		{
			return new SimpleBigDecimal(this.bigInt.ShiftLeft(n), this.scale);
		}

		public int CompareTo(SimpleBigDecimal val)
		{
			this.CheckScale(val);
			return this.bigInt.CompareTo(val.bigInt);
		}

		public int CompareTo(BigInteger val)
		{
			return this.bigInt.CompareTo(val.ShiftLeft(this.scale));
		}

		public BigInteger Floor()
		{
			return this.bigInt.ShiftRight(this.scale);
		}

		public BigInteger Round()
		{
			SimpleBigDecimal simpleBigDecimal = new SimpleBigDecimal(BigInteger.One, 1);
			return this.Add(simpleBigDecimal.AdjustScale(this.scale)).Floor();
		}

		public override string ToString()
		{
			if (this.scale == 0)
			{
				return this.bigInt.ToString();
			}
			BigInteger bigInteger = this.Floor();
			BigInteger bigInteger2 = this.bigInt.Subtract(bigInteger.ShiftLeft(this.scale));
			if (this.bigInt.SignValue < 0)
			{
				bigInteger2 = BigInteger.One.ShiftLeft(this.scale).Subtract(bigInteger2);
			}
			if (bigInteger.SignValue == -1 && !bigInteger2.Equals(BigInteger.Zero))
			{
				bigInteger = bigInteger.Add(BigInteger.One);
			}
			string value = bigInteger.ToString();
			char[] array = new char[this.scale];
			string text = bigInteger2.ToString(2);
			int length = text.Length;
			int num = this.scale - length;
			for (int i = 0; i < num; i++)
			{
				array[i] = '0';
			}
			for (int j = 0; j < length; j++)
			{
				array[num + j] = text[j];
			}
			string value2 = new string(array);
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Append(".");
			stringBuilder.Append(value2);
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			SimpleBigDecimal simpleBigDecimal = obj as SimpleBigDecimal;
			return simpleBigDecimal != null && this.bigInt.Equals(simpleBigDecimal.bigInt) && this.scale == simpleBigDecimal.scale;
		}

		public override int GetHashCode()
		{
			return this.bigInt.GetHashCode() ^ this.scale;
		}
	}
}
