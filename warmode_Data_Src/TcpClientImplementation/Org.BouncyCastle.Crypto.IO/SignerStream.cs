using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.IO
{
	public class SignerStream : Stream
	{
		protected readonly Stream stream;

		protected readonly ISigner inSigner;

		protected readonly ISigner outSigner;

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

		public SignerStream(Stream stream, ISigner readSigner, ISigner writeSigner)
		{
			this.stream = stream;
			this.inSigner = readSigner;
			this.outSigner = writeSigner;
		}

		public virtual ISigner ReadSigner()
		{
			return this.inSigner;
		}

		public virtual ISigner WriteSigner()
		{
			return this.outSigner;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this.stream.Read(buffer, offset, count);
			if (this.inSigner != null && num > 0)
			{
				this.inSigner.BlockUpdate(buffer, offset, num);
			}
			return num;
		}

		public override int ReadByte()
		{
			int num = this.stream.ReadByte();
			if (this.inSigner != null && num >= 0)
			{
				this.inSigner.Update((byte)num);
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.outSigner != null && count > 0)
			{
				this.outSigner.BlockUpdate(buffer, offset, count);
			}
			this.stream.Write(buffer, offset, count);
		}

		public override void WriteByte(byte b)
		{
			if (this.outSigner != null)
			{
				this.outSigner.Update(b);
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
