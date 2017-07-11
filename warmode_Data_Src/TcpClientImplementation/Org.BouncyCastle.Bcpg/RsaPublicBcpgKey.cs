using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class RsaPublicBcpgKey : BcpgObject, IBcpgKey
	{
		private readonly MPInteger n;

		private readonly MPInteger e;

		public BigInteger PublicExponent
		{
			get
			{
				return this.e.Value;
			}
		}

		public BigInteger Modulus
		{
			get
			{
				return this.n.Value;
			}
		}

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public RsaPublicBcpgKey(BcpgInputStream bcpgIn)
		{
			this.n = new MPInteger(bcpgIn);
			this.e = new MPInteger(bcpgIn);
		}

		public RsaPublicBcpgKey(BigInteger n, BigInteger e)
		{
			this.n = new MPInteger(n);
			this.e = new MPInteger(e);
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
				this.n,
				this.e
			});
		}
	}
}
