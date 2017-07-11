using System;

namespace Org.BouncyCastle.Utilities.Zlib
{
	internal sealed class Adler32
	{
		private const int BASE = 65521;

		private const int NMAX = 5552;

		internal long adler32(long adler, byte[] buf, int index, int len)
		{
			if (buf == null)
			{
				return 1L;
			}
			long num = adler & 65535L;
			long num2 = adler >> 16 & 65535L;
			while (len > 0)
			{
				int i = (len < 5552) ? len : 5552;
				len -= i;
				while (i >= 16)
				{
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					num += (long)(buf[index++] & 255);
					num2 += num;
					i -= 16;
				}
				if (i != 0)
				{
					do
					{
						num += (long)(buf[index++] & 255);
						num2 += num;
					}
					while (--i != 0);
				}
				num %= 65521L;
				num2 %= 65521L;
			}
			return num2 << 16 | num;
		}
	}
}
