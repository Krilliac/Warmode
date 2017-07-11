using Org.BouncyCastle.Bcpg.Attr;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class UserAttributeSubpacketsParser
	{
		private readonly Stream input;

		public UserAttributeSubpacketsParser(Stream input)
		{
			this.input = input;
		}

		public virtual UserAttributeSubpacket ReadPacket()
		{
			int num = this.input.ReadByte();
			if (num < 0)
			{
				return null;
			}
			bool forceLongLength = false;
			int num2;
			if (num < 192)
			{
				num2 = num;
			}
			else if (num <= 223)
			{
				num2 = (num - 192 << 8) + this.input.ReadByte() + 192;
			}
			else
			{
				if (num != 255)
				{
					throw new IOException("unrecognised length reading user attribute sub packet");
				}
				num2 = (this.input.ReadByte() << 24 | this.input.ReadByte() << 16 | this.input.ReadByte() << 8 | this.input.ReadByte());
				forceLongLength = true;
			}
			int num3 = this.input.ReadByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException("unexpected EOF reading user attribute sub packet");
			}
			byte[] array = new byte[num2 - 1];
			if (Streams.ReadFully(this.input, array) < array.Length)
			{
				throw new EndOfStreamException();
			}
			UserAttributeSubpacketTag userAttributeSubpacketTag = (UserAttributeSubpacketTag)num3;
			UserAttributeSubpacketTag userAttributeSubpacketTag2 = userAttributeSubpacketTag;
			if (userAttributeSubpacketTag2 == UserAttributeSubpacketTag.ImageAttribute)
			{
				return new ImageAttrib(forceLongLength, array);
			}
			return new UserAttributeSubpacket(userAttributeSubpacketTag, forceLongLength, array);
		}
	}
}
