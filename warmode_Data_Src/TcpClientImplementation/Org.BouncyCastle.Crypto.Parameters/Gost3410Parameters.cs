using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410Parameters : ICipherParameters
	{
		private readonly BigInteger p;

		private readonly BigInteger q;

		private readonly BigInteger a;

		private readonly Gost3410ValidationParameters validation;

		public BigInteger P
		{
			get
			{
				return this.p;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q;
			}
		}

		public BigInteger A
		{
			get
			{
				return this.a;
			}
		}

		public Gost3410ValidationParameters ValidationParameters
		{
			get
			{
				return this.validation;
			}
		}

		public Gost3410Parameters(BigInteger p, BigInteger q, BigInteger a) : this(p, q, a, null)
		{
		}

		public Gost3410Parameters(BigInteger p, BigInteger q, BigInteger a, Gost3410ValidationParameters validation)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			if (a == null)
			{
				throw new ArgumentNullException("a");
			}
			this.p = p;
			this.q = q;
			this.a = a;
			this.validation = validation;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			Gost3410Parameters gost3410Parameters = obj as Gost3410Parameters;
			return gost3410Parameters != null && this.Equals(gost3410Parameters);
		}

		protected bool Equals(Gost3410Parameters other)
		{
			return this.p.Equals(other.p) && this.q.Equals(other.q) && this.a.Equals(other.a);
		}

		public override int GetHashCode()
		{
			return this.p.GetHashCode() ^ this.q.GetHashCode() ^ this.a.GetHashCode();
		}
	}
}
