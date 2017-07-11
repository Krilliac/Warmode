using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class HexEncoder : IEncoder
	{
		protected readonly byte[] encodingTable = new byte[]
		{
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			97,
			98,
			99,
			100,
			101,
			102
		};

		protected readonly byte[] decodingTable = new byte[128];

		protected void InitialiseDecodingTable()
		{
			Arrays.Fill(this.decodingTable, 255);
			for (int i = 0; i < this.encodingTable.Length; i++)
			{
				this.decodingTable[(int)this.encodingTable[i]] = (byte)i;
			}
			this.decodingTable[65] = this.decodingTable[97];
			this.decodingTable[66] = this.decodingTable[98];
			this.decodingTable[67] = this.decodingTable[99];
			this.decodingTable[68] = this.decodingTable[100];
			this.decodingTable[69] = this.decodingTable[101];
			this.decodingTable[70] = this.decodingTable[102];
		}

		public HexEncoder()
		{
			this.InitialiseDecodingTable();
		}

		public int Encode(byte[] data, int off, int length, Stream outStream)
		{
			for (int i = off; i < off + length; i++)
			{
				int num = (int)data[i];
				outStream.WriteByte(this.encodingTable[num >> 4]);
				outStream.WriteByte(this.encodingTable[num & 15]);
			}
			return length * 2;
		}

		private static bool Ignore(char c)
		{
			return c == '\n' || c == '\r' || c == '\t' || c == ' ';
		}

		public int Decode(byte[] data, int off, int length, Stream outStream)
		{
			int num = 0;
			int num2 = off + length;
			while (num2 > off && HexEncoder.Ignore((char)data[num2 - 1]))
			{
				num2--;
			}
			int i = off;
			while (i < num2)
			{
				while (i < num2 && HexEncoder.Ignore((char)data[i]))
				{
					i++;
				}
				byte b = this.decodingTable[(int)data[i++]];
				while (i < num2 && HexEncoder.Ignore((char)data[i]))
				{
					i++;
				}
				byte b2 = this.decodingTable[(int)data[i++]];
				if ((b | b2) >= 128)
				{
					throw new IOException("invalid characters encountered in Hex data");
				}
				outStream.WriteByte((byte)((int)b << 4 | (int)b2));
				num++;
			}
			return num;
		}

		public int DecodeString(string data, Stream outStream)
		{
			int num = 0;
			int num2 = data.Length;
			while (num2 > 0 && HexEncoder.Ignore(data[num2 - 1]))
			{
				num2--;
			}
			int i = 0;
			while (i < num2)
			{
				while (i < num2 && HexEncoder.Ignore(data[i]))
				{
					i++;
				}
				byte b = this.decodingTable[(int)data[i++]];
				while (i < num2 && HexEncoder.Ignore(data[i]))
				{
					i++;
				}
				byte b2 = this.decodingTable[(int)data[i++]];
				if ((b | b2) >= 128)
				{
					throw new IOException("invalid characters encountered in Hex data");
				}
				outStream.WriteByte((byte)((int)b << 4 | (int)b2));
				num++;
			}
			return num;
		}
	}
}
