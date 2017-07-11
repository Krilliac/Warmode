using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class ElGamalPublicBcpgKey : BcpgObject, IBcpgKey
	{
		internal MPInteger p;

		internal MPInteger g;

		internal MPInteger y;

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public BigInteger P
		{
			get
			{
				return this.p.Value;
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g.Value;
			}
		}

		public BigInteger Y
		{
			get
			{
				return this.y.Value;
			}
		}

		public ElGamalPublicBcpgKey(BcpgInputStream bcpgIn)
		{
			this.p = new MPInteger(bcpgIn);
			this.g = new MPInteger(bcpgIn);
			this.y = new MPInteger(bcpgIn);
		}

		public ElGamalPublicBcpgKey(BigInteger p, BigInteger g, BigInteger y)
		{
			this.p = new MPInteger(p);
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
				this.g,
				this.y
			});
		}
	}
}
