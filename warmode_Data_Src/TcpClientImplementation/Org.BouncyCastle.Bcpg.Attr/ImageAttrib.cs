using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg.Attr
{
	public class ImageAttrib : UserAttributeSubpacket
	{
		public enum Format : byte
		{
			Jpeg = 1
		}

		private static readonly byte[] Zeroes = new byte[12];

		private int hdrLength;

		private int _version;

		private int _encoding;

		private byte[] imageData;

		public virtual int Version
		{
			get
			{
				return this._version;
			}
		}

		public virtual int Encoding
		{
			get
			{
				return this._encoding;
			}
		}

		public ImageAttrib(byte[] data) : this(false, data)
		{
		}

		public ImageAttrib(bool forceLongLength, byte[] data) : base(UserAttributeSubpacketTag.ImageAttribute, forceLongLength, data)
		{
			this.hdrLength = ((int)(data[1] & 255) << 8 | (int)(data[0] & 255));
			this._version = (int)(data[2] & 255);
			this._encoding = (int)(data[3] & 255);
			this.imageData = new byte[data.Length - this.hdrLength];
			Array.Copy(data, this.hdrLength, this.imageData, 0, this.imageData.Length);
		}

		public ImageAttrib(ImageAttrib.Format imageType, byte[] imageData) : this(ImageAttrib.ToByteArray(imageType, imageData))
		{
		}

		private static byte[] ToByteArray(ImageAttrib.Format imageType, byte[] imageData)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(16);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(1);
			memoryStream.WriteByte((byte)imageType);
			memoryStream.Write(ImageAttrib.Zeroes, 0, ImageAttrib.Zeroes.Length);
			memoryStream.Write(imageData, 0, imageData.Length);
			return memoryStream.ToArray();
		}

		public virtual byte[] GetImageData()
		{
			return this.imageData;
		}
	}
}
