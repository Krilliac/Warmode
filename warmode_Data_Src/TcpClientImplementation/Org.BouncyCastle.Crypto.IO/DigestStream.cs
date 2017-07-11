using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.IO
{
	public class DigestStream : Stream
	{
		protected readonly Stream stream;

		protected readonly IDigest inDigest;

		protected readonly IDigest outDigest;

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Position;
			}
			set
			{
				this.stream.Position = value;
			}
		}

		public DigestStream(Stream stream, IDigest readDigest, IDigest writeDigest)
		{
			this.stream = stream;
			this.inDigest = readDigest;
			this.outDigest = writeDigest;
		}

		public virtual IDigest ReadDigest()
		{
			return this.inDigest;
		}

		public virtual IDigest WriteDigest()
		{
			return this.outDigest;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this.stream.Read(buffer, offset, count);
			if (this.inDigest != null && num > 0)
			{
				this.inDigest.BlockUpdate(buffer, offset, num);
			}
			return num;
		}

		public override int ReadByte()
		{
			int num = this.stream.ReadByte();
			if (this.inDigest != null && num >= 0)
			{
				this.inDigest.Update((byte)num);
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.outDigest != null && count > 0)
			{
				this.outDigest.BlockUpdate(buffer, offset, count);
			}
			this.stream.Write(buffer, offset, count);
		}

		public override void WriteByte(byte b)
		{
			if (this.outDigest != null)
			{
				this.outDigest.Update(b);
			}
			this.stream.WriteByte(b);
		}

		public override void Close()
		{
			this.stream.Close();
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, origin);
		}

		public override void SetLength(long length)
		{
			this.stream.SetLength(length);
		}
	}
}
