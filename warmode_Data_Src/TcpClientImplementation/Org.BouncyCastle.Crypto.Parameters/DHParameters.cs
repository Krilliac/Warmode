using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHParameters : ICipherParameters
	{
		private const int DefaultMinimumLength = 160;

		private readonly BigInteger p;

		private readonly BigInteger g;

		private readonly BigInteger q;

		private readonly BigInteger j;

		private readonly int m;

		private readonly int l;

		private readonly DHValidationParameters validation;

		public BigInteger P
		{
			get
			{
				return this.p;
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q;
			}
		}

		public BigInteger J
		{
			get
			{
				return this.j;
			}
		}

		public int M
		{
			get
			{
				return this.m;
			}
		}

		public int L
		{
			get
			{
				return this.l;
			}
		}

		public DHValidationParameters ValidationParameters
		{
			get
			{
				return this.validation;
			}
		}

		private static int GetDefaultMParam(int lParam)
		{
			if (lParam == 0)
			{
				return 160;
			}
			return Math.Min(lParam, 160);
		}

		public DHParameters(BigInteger p, BigInteger g) : this(p, g, null, 0)
		{
		}

		public DHParameters(BigInteger p, BigInteger g, BigInteger q) : this(p, g, q, 0)
		{
		}

		public DHParameters(BigInteger p, BigInteger g, BigInteger q, int l) : this(p, g, q, DHParameters.GetDefaultMParam(l), l, null, null)
		{
		}

		public DHParameters(BigInteger p, BigInteger g, BigInteger q, int m, int l) : this(p, g, q, m, l, null, null)
		{
		}

		public DHParameters(BigInteger p, BigInteger g, BigInteger q, BigInteger j, DHValidationParameters validation) : this(p, g, q, 160, 0, j, validation)
		{
		}

		public DHParameters(BigInteger p, BigInteger g, BigInteger q, int m, int l, BigInteger j, DHValidationParameters validation)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			if (!p.TestBit(0))
			{
				throw new ArgumentException("field must be an odd prime", "p");
			}
			if (g.CompareTo(BigInteger.Two) < 0 || g.CompareTo(p.Subtract(BigInteger.Two)) > 0)
			{
				throw new ArgumentException("generator must in the range [2, p - 2]", "g");
			}
			if (q != null && q.BitLength >= p.BitLength)
			{
				throw new ArgumentException("q too big to be a factor of (p-1)", "q");
			}
			if (m >= p.BitLength)
			{
				throw new ArgumentException("m value must be < bitlength of p", "m");
			}
			if (l != 0)
			{
				if (l >= p.BitLength)
				{
					throw new ArgumentException("when l value specified, it must be less than bitlength(p)", "l");
				}
				if (l < m)
				{
					throw new ArgumentException("when l value specified, it may not be less than m value", "l");
				}
			}
			if (j != null && j.CompareTo(BigInteger.Two) < 0)
			{
				throw new ArgumentException("subgroup factor must be >= 2", "j");
			}
			this.p = p;
			this.g = g;
			this.q = q;
			this.m = m;
			this.l = l;
			this.j = j;
			this.validation = validation;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHParameters dHParameters = obj as DHParameters;
			return dHParameters != null && this.Equals(dHParameters);
		}

		protected bool Equals(DHParameters other)
		{
			return this.p.Equals(other.p) && this.g.Equals(other.g) && object.Equals(this.q, other.q);
		}

		public override int GetHashCode()
		{
			int num = this.p.GetHashCode() ^ this.g.GetHashCode();
			if (this.q != null)
			{
				num ^= this.q.GetHashCode();
			}
			return num;
		}
	}
}
