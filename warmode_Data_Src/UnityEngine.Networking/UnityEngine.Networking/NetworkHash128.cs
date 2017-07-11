using System;

namespace UnityEngine.Networking
{
	[Serializable]
	public struct NetworkHash128
	{
		public byte i0;

		public byte i1;

		public byte i2;

		public byte i3;

		public byte i4;

		public byte i5;

		public byte i6;

		public byte i7;

		public byte i8;

		public byte i9;

		public byte i10;

		public byte i11;

		public byte i12;

		public byte i13;

		public byte i14;

		public byte i15;

		public void Reset()
		{
			this.i0 = 0;
			this.i1 = 0;
			this.i2 = 0;
			this.i3 = 0;
			this.i4 = 0;
			this.i5 = 0;
			this.i6 = 0;
			this.i7 = 0;
			this.i8 = 0;
			this.i9 = 0;
			this.i10 = 0;
			this.i11 = 0;
			this.i12 = 0;
			this.i13 = 0;
			this.i14 = 0;
			this.i15 = 0;
		}

		public bool IsValid()
		{
			return (this.i0 | this.i1 | this.i2 | this.i3 | this.i4 | this.i5 | this.i6 | this.i7 | this.i8 | this.i9 | this.i10 | this.i11 | this.i12 | this.i13 | this.i14 | this.i15) != 0;
		}

		private static int HexToNumber(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (int)(c - '0');
			}
			if (c >= 'a' && c <= 'f')
			{
				return (int)(c - 'a' + '\n');
			}
			if (c >= 'A' && c <= 'F')
			{
				return (int)(c - 'A' + '\n');
			}
			return 0;
		}

		public static NetworkHash128 Parse(string text)
		{
			int length = text.Length;
			if (length < 32)
			{
				string str = string.Empty;
				for (int i = 0; i < 32 - length; i++)
				{
					str += "0";
				}
				text = str + text;
			}
			NetworkHash128 result;
			result.i0 = (byte)(NetworkHash128.HexToNumber(text[0]) * 16 + NetworkHash128.HexToNumber(text[1]));
			result.i1 = (byte)(NetworkHash128.HexToNumber(text[2]) * 16 + NetworkHash128.HexToNumber(text[3]));
			result.i2 = (byte)(NetworkHash128.HexToNumber(text[4]) * 16 + NetworkHash128.HexToNumber(text[5]));
			result.i3 = (byte)(NetworkHash128.HexToNumber(text[6]) * 16 + NetworkHash128.HexToNumber(text[7]));
			result.i4 = (byte)(NetworkHash128.HexToNumber(text[8]) * 16 + NetworkHash128.HexToNumber(text[9]));
			result.i5 = (byte)(NetworkHash128.HexToNumber(text[10]) * 16 + NetworkHash128.HexToNumber(text[11]));
			result.i6 = (byte)(NetworkHash128.HexToNumber(text[12]) * 16 + NetworkHash128.HexToNumber(text[13]));
			result.i7 = (byte)(NetworkHash128.HexToNumber(text[14]) * 16 + NetworkHash128.HexToNumber(text[15]));
			result.i8 = (byte)(NetworkHash128.HexToNumber(text[16]) * 16 + NetworkHash128.HexToNumber(text[17]));
			result.i9 = (byte)(NetworkHash128.HexToNumber(text[18]) * 16 + NetworkHash128.HexToNumber(text[19]));
			result.i10 = (byte)(NetworkHash128.HexToNumber(text[20]) * 16 + NetworkHash128.HexToNumber(text[21]));
			result.i11 = (byte)(NetworkHash128.HexToNumber(text[22]) * 16 + NetworkHash128.HexToNumber(text[23]));
			result.i12 = (byte)(NetworkHash128.HexToNumber(text[24]) * 16 + NetworkHash128.HexToNumber(text[25]));
			result.i13 = (byte)(NetworkHash128.HexToNumber(text[26]) * 16 + NetworkHash128.HexToNumber(text[27]));
			result.i14 = (byte)(NetworkHash128.HexToNumber(text[28]) * 16 + NetworkHash128.HexToNumber(text[29]));
			result.i15 = (byte)(NetworkHash128.HexToNumber(text[30]) * 16 + NetworkHash128.HexToNumber(text[31]));
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0:x}{1:x}{2:x}{3:x}{4:x}{5:x}{6:x}{7:x}{8:x}{9:x}{10:x}{11:x}{12:x}{13:x}{14:x}{15:x}", new object[]
			{
				this.i0,
				this.i1,
				this.i2,
				this.i3,
				this.i4,
				this.i5,
				this.i6,
				this.i7,
				this.i8,
				this.i9,
				this.i10,
				this.i11,
				this.i12,
				this.i13,
				this.i14,
				this.i15
			});
		}
	}
}
