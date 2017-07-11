using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Bcpg
{
	public class LiteralDataPacket : InputStreamPacket
	{
		private int format;

		private byte[] fileName;

		private long modDate;

		public int Format
		{
			get
			{
				return this.format;
			}
		}

		public long ModificationTime
		{
			get
			{
				return this.modDate;
			}
		}

		public string FileName
		{
			get
			{
				return Strings.FromUtf8ByteArray(this.fileName);
			}
		}

		internal LiteralDataPacket(BcpgInputStream bcpgIn) : base(bcpgIn)
		{
			this.format = bcpgIn.ReadByte();
			int num = bcpgIn.ReadByte();
			this.fileName = new byte[num];
			for (int num2 = 0; num2 != num; num2++)
			{
				this.fileName[num2] = (byte)bcpgIn.ReadByte();
			}
			this.modDate = (long)((ulong)(bcpgIn.ReadByte() << 24 | bcpgIn.ReadByte() << 16 | bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte()) * 1000uL);
		}

		public byte[] GetRawFileName()
		{
			return Arrays.Clone(this.fileName);
		}
	}
}
