using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SignatureSubpacket
	{
		private readonly SignatureSubpacketTag type;

		private readonly bool critical;

		internal readonly byte[] data;

		public SignatureSubpacketTag SubpacketType
		{
			get
			{
				return this.type;
			}
		}

		protected internal SignatureSubpacket(SignatureSubpacketTag type, bool critical, byte[] data)
		{
			this.type = type;
			this.critical = critical;
			this.data = data;
		}

		public bool IsCritical()
		{
			return this.critical;
		}

		public byte[] GetData()
		{
			return (byte[])this.data.Clone();
		}

		public void Encode(Stream os)
		{
			int num = this.data.Length + 1;
			if (num < 192)
			{
				os.WriteByte((byte)num);
			}
			else if (num <= 8383)
			{
				num -= 192;
				os.WriteByte((byte)((num >> 8 & 255) + 192));
				os.WriteByte((byte)num);
			}
			else
			{
				os.WriteByte(255);
				os.WriteByte((byte)(num >> 24));
				os.WriteByte((byte)(num >> 16));
				os.WriteByte((byte)(num >> 8));
				os.WriteByte((byte)num);
			}
			if (this.critical)
			{
				os.WriteByte((byte)((SignatureSubpacketTag)128 | this.type));
			}
			else
			{
				os.WriteByte((byte)this.type);
			}
			os.Write(this.data, 0, this.data.Length);
		}
	}
}
