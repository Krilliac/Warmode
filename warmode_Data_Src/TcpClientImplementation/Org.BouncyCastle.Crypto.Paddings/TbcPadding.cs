using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Paddings
{
	public class TbcPadding : IBlockCipherPadding
	{
		public string PaddingName
		{
			get
			{
				return "TBC";
			}
		}

		public virtual void Init(SecureRandom random)
		{
		}

		public virtual int AddPadding(byte[] input, int inOff)
		{
			int result = input.Length - inOff;
			byte b;
			if (inOff > 0)
			{
				b = (((input[inOff - 1] & 1) == 0) ? 255 : 0);
			}
			else
			{
				b = (((input[input.Length - 1] & 1) == 0) ? 255 : 0);
			}
			while (inOff < input.Length)
			{
				input[inOff] = b;
				inOff++;
			}
			return result;
		}

		public virtual int PadCount(byte[] input)
		{
			byte b = input[input.Length - 1];
			int num = input.Length - 1;
			while (num > 0 && input[num - 1] == b)
			{
				num--;
			}
			return input.Length - num;
		}
	}
}
