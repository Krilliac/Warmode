using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Paddings
{
	public class ISO10126d2Padding : IBlockCipherPadding
	{
		private SecureRandom random;

		public string PaddingName
		{
			get
			{
				return "ISO10126-2";
			}
		}

		public void Init(SecureRandom random)
		{
			this.random = ((random != null) ? random : new SecureRandom());
		}

		public int AddPadding(byte[] input, int inOff)
		{
			byte b = (byte)(input.Length - inOff);
			while (inOff < input.Length - 1)
			{
				input[inOff] = (byte)this.random.NextInt();
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
