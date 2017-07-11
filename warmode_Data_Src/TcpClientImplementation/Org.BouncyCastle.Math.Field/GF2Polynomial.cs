using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.Field
{
	internal class GF2Polynomial : IPolynomial
	{
		protected readonly int[] exponents;

		public virtual int Degree
		{
			get
			{
				return this.exponents[this.exponents.Length - 1];
			}
		}

		internal GF2Polynomial(int[] exponents)
		{
			this.exponents = Arrays.Clone(exponents);
		}

		public virtual int[] GetExponentsPresent()
		{
			return Arrays.Clone(this.exponents);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			GF2Polynomial gF2Polynomial = obj as GF2Polynomial;
			return gF2Polynomial != null && Arrays.AreEqual(this.exponents, gF2Polynomial.exponents);
		}

		public override int GetHashCode()
		{
			return Arrays.GetHashCode(this.exponents);
		}
	}
}
