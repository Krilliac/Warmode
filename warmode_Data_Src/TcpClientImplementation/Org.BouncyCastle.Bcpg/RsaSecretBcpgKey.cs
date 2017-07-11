using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class RsaSecretBcpgKey : BcpgObject, IBcpgKey
	{
		private readonly MPInteger d;

		private readonly MPInteger p;

		private readonly MPInteger q;

		private readonly MPInteger u;

		private readonly BigInteger expP;

		private readonly BigInteger expQ;

		private readonly BigInteger crt;

		public BigInteger Modulus
		{
			get
			{
				return this.p.Value.Multiply(this.q.Value);
			}
		}

		public BigInteger PrivateExponent
		{
			get
			{
				return this.d.Value;
			}
		}

		public BigInteger PrimeP
		{
			get
			{
				return this.p.Value;
			}
		}

		public BigInteger PrimeQ
		{
			get
			{
				return this.q.Value;
			}
		}

		public BigInteger PrimeExponentP
		{
			get
			{
				return this.expP;
			}
		}

		public BigInteger PrimeExponentQ
		{
			get
			{
				return this.expQ;
			}
		}

		public BigInteger CrtCoefficient
		{
			get
			{
				return this.crt;
			}
		}

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public RsaSecretBcpgKey(BcpgInputStream bcpgIn)
		{
			this.d = new MPInteger(bcpgIn);
			this.p = new MPInteger(bcpgIn);
			this.q = new MPInteger(bcpgIn);
			this.u = new MPInteger(bcpgIn);
			this.expP = this.d.Value.Remainder(this.p.Value.Subtract(BigInteger.One));
			this.expQ = this.d.Value.Remainder(this.q.Value.Subtract(BigInteger.One));
			this.crt = this.q.Value.ModInverse(this.p.Value);
		}

		public RsaSecretBcpgKey(BigInteger d, BigInteger p, BigInteger q)
		{
			int num = p.CompareTo(q);
			if (num >= 0)
			{
				if (num == 0)
				{
					throw new ArgumentException("p and q cannot be equal");
				}
				BigInteger bigInteger = p;
				p = q;
				q = bigInteger;
			}
			this.d = new MPInteger(d);
			this.p = new MPInteger(p);
			this.q = new MPInteger(q);
			this.u = new MPInteger(p.ModInverse(q));
			this.expP = d.Remainder(p.Subtract(BigInteger.One));
			this.expQ = d.Remainder(q.Subtract(BigInteger.One));
			this.crt = q.ModInverse(p);
		}

		public override byte[] GetEncoded()
		{
			byte[] result;
			try
			{
				result = base.GetEncoded();
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WriteObjects(new BcpgObject[]
			{
				this.d,
				this.p,
				this.q,
				this.u
			});
		}
	}
}
