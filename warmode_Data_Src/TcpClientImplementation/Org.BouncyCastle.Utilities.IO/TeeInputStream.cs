using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
	public class TeeInputStream : BaseInputStream
	{
		private readonly Stream input;

		private readonly Stream tee;

		public TeeInputStream(Stream input, Stream tee)
		{
			this.input = input;
			this.tee = tee;
		}

		public override void Close()
		{
			this.input.Close();
			this.tee.Close();
		}

		public override int Read(byte[] buf, int off, int len)
		{
			int num = this.input.Read(buf, off, len);
			if (num > 0)
			{
				this.tee.Write(buf, off, num);
			}
			return num;
		}

		public override int ReadByte()
		{
			int num = this.input.ReadByte();
			if (num >= 0)
			{
				this.tee.WriteByte((byte)num);
			}
			return num;
		}
	}
}
