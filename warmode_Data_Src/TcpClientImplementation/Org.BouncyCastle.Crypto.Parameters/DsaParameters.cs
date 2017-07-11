using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DsaParameters : ICipherParameters
	{
		private readonly BigInteger p;

		private readonly BigInteger q;

		private readonly BigInteger g;

		private readonly DsaValidationParameters validation;

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

		public BigInteger G
		{
			get
			{
				return this.g;
			}
		}

		public DsaValidationParameters ValidationParameters
		{
			get
			{
				return this.validation;
			}
		}

		public DsaParameters(BigInteger p, BigInteger q, BigInteger g) : this(p, q, g, null)
		{
		}

		public DsaParameters(BigInteger p, BigInteger q, BigInteger g, DsaValidationParameters parameters)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			this.p = p;
			this.q = q;
			this.g = g;
			this.validation = parameters;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DsaParameters dsaParameters = obj as DsaParameters;
			return dsaParameters != null && this.Equals(dsaParameters);
		}

		protected bool Equals(DsaParameters other)
		{
			return this.p.Equals(other.p) && this.q.Equals(other.q) && this.g.Equals(other.g);
		}

		public override int GetHashCode()
		{
			return this.p.GetHashCode() ^ this.q.GetHashCode() ^ this.g.GetHashCode();
		}
	}
}
