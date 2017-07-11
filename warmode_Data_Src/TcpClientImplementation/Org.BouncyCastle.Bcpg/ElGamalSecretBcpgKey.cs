using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class ElGamalSecretBcpgKey : BcpgObject, IBcpgKey
	{
		internal MPInteger x;

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public BigInteger X
		{
			get
			{
				return this.x.Value;
			}
		}

		public ElGamalSecretBcpgKey(BcpgInputStream bcpgIn)
		{
			this.x = new MPInteger(bcpgIn);
		}

		public ElGamalSecretBcpgKey(BigInteger x)
		{
			this.x = new MPInteger(x);
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
			bcpgOut.WriteObject(this.x);
		}
	}
}
