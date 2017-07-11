using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Paddings
{
	public class X923Padding : IBlockCipherPadding
	{
		private SecureRandom random;

		public string PaddingName
		{
			get
			{
				return "X9.23";
			}
		}

		public void Init(SecureRandom random)
		{
			this.random = random;
		}

		public int AddPadding(byte[] input, int inOff)
		{
			byte b = (byte)(input.Length - inOff);
			while (inOff < input.Length - 1)
			{
				if (this.random == null)
				{
					input[inOff] = 0;
				}
				else
				{
					input[inOff] = (byte)this.random.NextInt();
				}
				inOff++;
			}
			input[inOff] = b;
			return (int)b;
		}

		public int PadCount(byte[] input)
		{
			int num = (int)(input[input.Length - 1] & 255);
			if (num > input.Length)
			{
				throw new InvalidCipherTextException("pad block corrupted");
			}
			return num;
		}
	}
}
