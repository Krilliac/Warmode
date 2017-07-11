using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class DsaPublicBcpgKey : BcpgObject, IBcpgKey
	{
		private readonly MPInteger p;

		private readonly MPInteger q;

		private readonly MPInteger g;

		private readonly MPInteger y;

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g.Value;
			}
		}

		public BigInteger P
		{
			get
			{
				return this.p.Value;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q.Value;
			}
		}

		public BigInteger Y
		{
			get
			{
				return this.y.Value;
			}
		}

		public DsaPublicBcpgKey(BcpgInputStream bcpgIn)
		{
			this.p = new MPInteger(bcpgIn);
			this.q = new MPInteger(bcpgIn);
			this.g = new MPInteger(bcpgIn);
			this.y = new MPInteger(bcpgIn);
		}

		public DsaPublicBcpgKey(BigInteger p, BigInteger q, BigInteger g, BigInteger y)
		{
			this.p = new MPInteger(p);
			this.q = new MPInteger(q);
			this.g = new MPInteger(g);
			this.y = new MPInteger(y);
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
				this.p,
				this.q,
				this.g,
				this.y
			});
		}
	}
}
