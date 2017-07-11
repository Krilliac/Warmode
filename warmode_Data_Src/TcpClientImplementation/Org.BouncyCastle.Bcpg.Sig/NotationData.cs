using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class NotationData : SignatureSubpacket
	{
		public const int HeaderFlagLength = 4;

		public const int HeaderNameLength = 2;

		public const int HeaderValueLength = 2;

		public bool IsHumanReadable
		{
			get
			{
				return this.data[0] == 128;
			}
		}

		public NotationData(bool critical, byte[] data) : base(SignatureSubpacketTag.NotationData, critical, data)
		{
		}

		public NotationData(bool critical, bool humanReadable, string notationName, string notationValue) : base(SignatureSubpacketTag.NotationData, critical, NotationData.createData(humanReadable, notationName, notationValue))
		{
		}

		private static byte[] createData(bool humanReadable, string notationName, string notationValue)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(humanReadable ? 128 : 0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			byte[] bytes = Encoding.UTF8.GetBytes(notationName);
			int num = Math.Min(bytes.Length, 255);
			byte[] bytes2 = Encoding.UTF8.GetBytes(notationValue);
			int num2 = Math.Min(bytes2.Length, 255);
			memoryStream.WriteByte((byte)(num >> 8));
			memoryStream.WriteByte((byte)num);
			memoryStream.WriteByte((byte)(num2 >> 8));
			memoryStream.WriteByte((byte)num2);
			memoryStream.Write(bytes, 0, num);
			memoryStream.Write(bytes2, 0, num2);
			return memoryStream.ToArray();
		}

		public string GetNotationName()
		{
			int count = ((int)this.data[4] << 8) + (int)this.data[5];
			int index = 8;
			return Encoding.UTF8.GetString(this.data, index, count);
		}

		public string GetNotationValue()
		{
			int num = ((int)this.data[4] << 8) + (int)this.data[5];
			int count = ((int)this.data[6] << 8) + (int)this.data[7];
			int index = 8 + num;
			return Encoding.UTF8.GetString(this.data, index, count);
		}

		public byte[] GetNotationValueBytes()
		{
			int num = ((int)this.data[4] << 8) + (int)this.data[5];
			int num2 = ((int)this.data[6] << 8) + (int)this.data[7];
			int sourceIndex = 8 + num;
			byte[] array = new byte[num2];
			Array.Copy(this.data, sourceIndex, array, 0, num2);
			return array;
		}
	}
}
