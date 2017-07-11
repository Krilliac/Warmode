using System;

namespace Org.BouncyCastle.Bcpg
{
	public class Crc24
	{
		private const int Crc24Init = 11994318;

		private const int Crc24Poly = 25578747;

		private int crc = 11994318;

		public int Value
		{
			get
			{
				return this.crc;
			}
		}

		public void Update(int b)
		{
			this.crc ^= b << 16;
			for (int i = 0; i < 8; i++)
			{
				this.crc <<= 1;
				if ((this.crc & 16777216) != 0)
				{
					this.crc ^= 25578747;
				}
			}
		}

		[Obsolete("Use 'Value' property instead")]
		public int GetValue()
		{
			return this.crc;
		}

		public void Reset()
		{
			this.crc = 11994318;
		}
	}
}
