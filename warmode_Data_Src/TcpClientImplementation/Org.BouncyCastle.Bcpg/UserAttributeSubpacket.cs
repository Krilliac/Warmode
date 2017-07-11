using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class UserAttributeSubpacket
	{
		internal readonly UserAttributeSubpacketTag type;

		private readonly bool longLength;

		protected readonly byte[] data;

		public virtual UserAttributeSubpacketTag SubpacketType
		{
			get
			{
				return this.type;
			}
		}

		protected internal UserAttributeSubpacket(UserAttributeSubpacketTag type, byte[] data) : this(type, false, data)
		{
		}

		protected internal UserAttributeSubpacket(UserAttributeSubpacketTag type, bool forceLongLength, byte[] data)
		{
			this.type = type;
			this.longLength = forceLongLength;
			this.data = data;
		}

		public virtual byte[] GetData()
		{
			return this.data;
		}

		public virtual void Encode(Stream os)
		{
			int num = this.data.Length + 1;
			if (num < 192 && !this.longLength)
			{
				os.WriteByte((byte)num);
			}
			else if (num <= 8383 && !this.longLength)
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
			os.WriteByte((byte)this.type);
			os.Write(this.data, 0, this.data.Length);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			UserAttributeSubpacket userAttributeSubpacket = obj as UserAttributeSubpacket;
			return userAttributeSubpacket != null && this.type == userAttributeSubpacket.type && Arrays.AreEqual(this.data, userAttributeSubpacket.data);
		}

		public override int GetHashCode()
		{
			return this.type.GetHashCode() ^ Arrays.GetHashCode(this.data);
		}
	}
}
