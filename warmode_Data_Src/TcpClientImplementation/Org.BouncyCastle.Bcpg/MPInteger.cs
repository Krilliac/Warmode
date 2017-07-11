using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class MPInteger : BcpgObject
	{
		private readonly BigInteger val;

		public BigInteger Value
		{
			get
			{
				return this.val;
			}
		}

		public MPInteger(BcpgInputStream bcpgIn)
		{
			if (bcpgIn == null)
			{
				throw new ArgumentNullException("bcpgIn");
			}
			int num = bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte();
			byte[] array = new byte[(num + 7) / 8];
			bcpgIn.ReadFully(array);
			this.val = new BigInteger(1, array);
		}

		public MPInteger(BigInteger val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("val");
			}
			if (val.SignValue < 0)
			{
				throw new ArgumentException("Values must be positive", "val");
			}
			this.val = val;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WriteShort((short)this.val.BitLength);
			bcpgOut.Write(this.val.ToByteArrayUnsigned());
		}

		internal static void Encode(BcpgOutputStream bcpgOut, BigInteger val)
		{
			bcpgOut.WriteShort((short)val.BitLength);
			bcpgOut.Write(val.ToByteArrayUnsigned());
		}
	}
}
