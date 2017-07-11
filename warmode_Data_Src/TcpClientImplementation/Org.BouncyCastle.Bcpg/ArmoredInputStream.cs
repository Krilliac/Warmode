using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Bcpg
{
	public class ArmoredInputStream : BaseInputStream
	{
		private static readonly byte[] decodingTable;

		private Stream input;

		private bool start = true;

		private int[] outBuf = new int[3];

		private int bufPtr = 3;

		private Crc24 crc = new Crc24();

		private bool crcFound;

		private bool hasHeaders = true;

		private string header;

		private bool newLineFound;

		private bool clearText;

		private bool restart;

		private IList headerList = Platform.CreateArrayList();

		private int lastC;

		private bool isEndOfStream;

		static ArmoredInputStream()
		{
			ArmoredInputStream.decodingTable = new byte[128];
			for (int i = 65; i <= 90; i++)
			{
				ArmoredInputStream.decodingTable[i] = (byte)(i - 65);
			}
			for (int j = 97; j <= 122; j++)
			{
				ArmoredInputStream.decodingTable[j] = (byte)(j - 97 + 26);
			}
			for (int k = 48; k <= 57; k++)
			{
				ArmoredInputStream.decodingTable[k] = (byte)(k - 48 + 52);
			}
			ArmoredInputStream.decodingTable[43] = 62;
			ArmoredInputStream.decodingTable[47] = 63;
		}

		private int Decode(int in0, int in1, int in2, int in3, int[] result)
		{
			if (in3 < 0)
			{
				throw new EndOfStreamException("unexpected end of file in armored stream.");
			}
			int num;
			int num2;
			if (in2 == 61)
			{
				num = (int)(ArmoredInputStream.decodingTable[in0] & 255);
				num2 = (int)(ArmoredInputStream.decodingTable[in1] & 255);
				result[2] = ((num << 2 | num2 >> 4) & 255);
				return 2;
			}
			int num3;
			if (in3 == 61)
			{
				num = (int)ArmoredInputStream.decodingTable[in0];
				num2 = (int)ArmoredInputStream.decodingTable[in1];
				num3 = (int)ArmoredInputStream.decodingTable[in2];
				result[1] = ((num << 2 | num2 >> 4) & 255);
				result[2] = ((num2 << 4 | num3 >> 2) & 255);
				return 1;
			}
			num = (int)ArmoredInputStream.decodingTable[in0];
			num2 = (int)ArmoredInputStream.decodingTable[in1];
			num3 = (int)ArmoredInputStream.decodingTable[in2];
			int num4 = (int)ArmoredInputStream.decodingTable[in3];
			result[0] = ((num << 2 | num2 >> 4) & 255);
			result[1] = ((num2 << 4 | num3 >> 2) & 255);
			result[2] = ((num3 << 6 | num4) & 255);
			return 0;
		}

		public ArmoredInputStream(Stream input) : this(input, true)
		{
		}

		public ArmoredInputStream(Stream input, bool hasHeaders)
		{
			this.input = input;
			this.hasHeaders = hasHeaders;
			if (hasHeaders)
			{
				this.ParseHeaders();
			}
			this.start = false;
		}

		private bool ParseHeaders()
		{
			this.header = null;
			int num = 0;
			bool flag = false;
			this.headerList = Platform.CreateArrayList();
			if (this.restart)
			{
				flag = true;
			}
			else
			{
				int num2;
				while ((num2 = this.input.ReadByte()) >= 0)
				{
					if (num2 == 45 && (num == 0 || num == 10 || num == 13))
					{
						flag = true;
						break;
					}
					num = num2;
				}
			}
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder("-");
				bool flag2 = false;
				bool flag3 = false;
				if (this.restart)
				{
					stringBuilder.Append('-');
				}
				int num2;
				while ((num2 = this.input.ReadByte()) >= 0)
				{
					if (num == 13 && num2 == 10)
					{
						flag3 = true;
					}
					if ((flag2 && num != 13 && num2 == 10) || (flag2 && num2 == 13))
					{
						break;
					}
					if (num2 == 13 || (num != 13 && num2 == 10))
					{
						string text = stringBuilder.ToString();
						if (text.Trim().Length < 1)
						{
							break;
						}
						this.headerList.Add(text);
						stringBuilder.Length = 0;
					}
					if (num2 != 10 && num2 != 13)
					{
						stringBuilder.Append((char)num2);
						flag2 = false;
					}
					else if (num2 == 13 || (num != 13 && num2 == 10))
					{
						flag2 = true;
					}
					num = num2;
				}
				if (flag3)
				{
					this.input.ReadByte();
				}
			}
			if (this.headerList.Count > 0)
			{
				this.header = (string)this.headerList[0];
			}
			this.clearText = "-----BEGIN PGP SIGNED MESSAGE-----".Equals(this.header);
			this.newLineFound = true;
			return flag;
		}

		public bool IsClearText()
		{
			return this.clearText;
		}

		public bool IsEndOfStream()
		{
			return this.isEndOfStream;
		}

		public string GetArmorHeaderLine()
		{
			return this.header;
		}

		public string[] GetArmorHeaders()
		{
			if (this.headerList.Count <= 1)
			{
				return null;
			}
			string[] array = new string[this.headerList.Count - 1];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = (string)this.headerList[num + 1];
			}
			return array;
		}

		private int ReadIgnoreSpace()
		{
			int num;
			do
			{
				num = this.input.ReadByte();
			}
			while (num == 32 || num == 9);
			return num;
		}

		private int ReadIgnoreWhitespace()
		{
			int num;
			do
			{
				num = this.input.ReadByte();
			}
			while (num == 32 || num == 9 || num == 13 || num == 10);
			return num;
		}

		private int ReadByteClearText()
		{
			int num = this.input.ReadByte();
			if (num == 13 || (num == 10 && this.lastC != 13))
			{
				this.newLineFound = true;
			}
			else if (this.newLineFound && num == 45)
			{
				num = this.input.ReadByte();
				if (num == 45)
				{
					this.clearText = false;
					this.start = true;
					this.restart = true;
				}
				else
				{
					num = this.input.ReadByte();
				}
				this.newLineFound = false;
			}
			else if (num != 10 && this.lastC != 13)
			{
				this.newLineFound = false;
			}
			this.lastC = num;
			if (num < 0)
			{
				this.isEndOfStream = true;
			}
			return num;
		}

		private int ReadClearText(byte[] buffer, int offset, int count)
		{
			int i = offset;
			try
			{
				int num = offset + count;
				while (i < num)
				{
					int num2 = this.ReadByteClearText();
					if (num2 == -1)
					{
						break;
					}
					buffer[i++] = (byte)num2;
				}
			}
			catch (IOException ex)
			{
				if (i == offset)
				{
					throw ex;
				}
			}
			return i - offset;
		}

		private int DoReadByte()
		{
			if (this.bufPtr > 2 || this.crcFound)
			{
				int num = this.ReadIgnoreSpace();
				if (num == 10 || num == 13)
				{
					num = this.ReadIgnoreWhitespace();
					if (num == 61)
					{
						this.bufPtr = this.Decode(this.ReadIgnoreSpace(), this.ReadIgnoreSpace(), this.ReadIgnoreSpace(), this.ReadIgnoreSpace(), this.outBuf);
						if (this.bufPtr != 0)
						{
							throw new IOException("no crc found in armored message.");
						}
						this.crcFound = true;
						int num2 = (this.outBuf[0] & 255) << 16 | (this.outBuf[1] & 255) << 8 | (this.outBuf[2] & 255);
						if (num2 != this.crc.Value)
						{
							throw new IOException("crc check failed in armored message.");
						}
						return this.ReadByte();
					}
					else if (num == 45)
					{
						while ((num = this.input.ReadByte()) >= 0 && num != 10 && num != 13)
						{
						}
						if (!this.crcFound)
						{
							throw new IOException("crc check not found.");
						}
						this.crcFound = false;
						this.start = true;
						this.bufPtr = 3;
						if (num < 0)
						{
							this.isEndOfStream = true;
						}
						return -1;
					}
				}
				if (num < 0)
				{
					this.isEndOfStream = true;
					return -1;
				}
				this.bufPtr = this.Decode(num, this.ReadIgnoreSpace(), this.ReadIgnoreSpace(), this.ReadIgnoreSpace(), this.outBuf);
			}
			return this.outBuf[this.bufPtr++];
		}

		public override int ReadByte()
		{
			if (this.start)
			{
				if (this.hasHeaders)
				{
					this.ParseHeaders();
				}
				this.crc.Reset();
				this.start = false;
			}
			if (this.clearText)
			{
				return this.ReadByteClearText();
			}
			int num = this.DoReadByte();
			this.crc.Update(num);
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.start && count > 0)
			{
				if (this.hasHeaders)
				{
					this.ParseHeaders();
				}
				this.start = false;
			}
			if (this.clearText)
			{
				return this.ReadClearText(buffer, offset, count);
			}
			int i = offset;
			try
			{
				int num = offset + count;
				while (i < num)
				{
					int num2 = this.DoReadByte();
					this.crc.Update(num2);
					if (num2 == -1)
					{
						break;
					}
					buffer[i++] = (byte)num2;
				}
			}
			catch (IOException ex)
			{
				if (i == offset)
				{
					throw ex;
				}
			}
			return i - offset;
		}

		public override void Close()
		{
			this.input.Close();
			base.Close();
		}
	}
}
