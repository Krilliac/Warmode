using Org.BouncyCastle.Asn1.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class DerOutputStream : FilterStream
	{
		public DerOutputStream(Stream os) : base(os)
		{
		}

		private void WriteLength(int length)
		{
			if (length > 127)
			{
				int num = 1;
				uint num2 = (uint)length;
				while ((num2 >>= 8) != 0u)
				{
					num++;
				}
				this.WriteByte((byte)(num | 128));
				for (int i = (num - 1) * 8; i >= 0; i -= 8)
				{
					this.WriteByte((byte)(length >> i));
				}
				return;
			}
			this.WriteByte((byte)length);
		}

		internal void WriteEncoded(int tag, byte[] bytes)
		{
			this.WriteByte((byte)tag);
			this.WriteLength(bytes.Length);
			this.Write(bytes, 0, bytes.Length);
		}

		internal void WriteEncoded(int tag, byte[] bytes, int offset, int length)
		{
			this.WriteByte((byte)tag);
			this.WriteLength(length);
			this.Write(bytes, offset, length);
		}

		internal void WriteTag(int flags, int tagNo)
		{
			if (tagNo < 31)
			{
				this.WriteByte((byte)(flags | tagNo));
				return;
			}
			this.WriteByte((byte)(flags | 31));
			if (tagNo < 128)
			{
				this.WriteByte((byte)tagNo);
				return;
			}
			byte[] array = new byte[5];
			int num = array.Length;
			array[--num] = (byte)(tagNo & 127);
			do
			{
				tagNo >>= 7;
				array[--num] = (byte)((tagNo & 127) | 128);
			}
			while (tagNo > 127);
			this.Write(array, num, array.Length - num);
		}

		internal void WriteEncoded(int flags, int tagNo, byte[] bytes)
		{
			this.WriteTag(flags, tagNo);
			this.WriteLength(bytes.Length);
			this.Write(bytes, 0, bytes.Length);
		}

		protected void WriteNull()
		{
			this.WriteByte(5);
			this.WriteByte(0);
		}

		[Obsolete("Use version taking an Asn1Encodable arg instead")]
		public virtual void WriteObject(object obj)
		{
			if (obj == null)
			{
				this.WriteNull();
				return;
			}
			if (obj is Asn1Object)
			{
				((Asn1Object)obj).Encode(this);
				return;
			}
			if (obj is Asn1Encodable)
			{
				((Asn1Encodable)obj).ToAsn1Object().Encode(this);
				return;
			}
			throw new IOException("object not Asn1Object");
		}

		public virtual void WriteObject(Asn1Encodable obj)
		{
			if (obj == null)
			{
				this.WriteNull();
				return;
			}
			obj.ToAsn1Object().Encode(this);
		}

		public virtual void WriteObject(Asn1Object obj)
		{
			if (obj == null)
			{
				this.WriteNull();
				return;
			}
			obj.Encode(this);
		}
	}
}
