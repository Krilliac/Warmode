using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal class DefiniteLengthInputStream : LimitedInputStream
	{
		private static readonly byte[] EmptyBytes = new byte[0];

		private readonly int _originalLength;

		private int _remaining;

		internal int Remaining
		{
			get
			{
				return this._remaining;
			}
		}

		internal DefiniteLengthInputStream(Stream inStream, int length) : base(inStream, length)
		{
			if (length < 0)
			{
				throw new ArgumentException("negative lengths not allowed", "length");
			}
			this._originalLength = length;
			this._remaining = length;
			if (length == 0)
			{
				this.SetParentEofDetect(true);
			}
		}

		public override int ReadByte()
		{
			if (this._remaining == 0)
			{
				return -1;
			}
			int num = this._in.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException(string.Concat(new object[]
				{
					"DEF length ",
					this._originalLength,
					" object truncated by ",
					this._remaining
				}));
			}
			if (--this._remaining == 0)
			{
				this.SetParentEofDetect(true);
			}
			return num;
		}

		public override int Read(byte[] buf, int off, int len)
		{
			if (this._remaining == 0)
			{
				return 0;
			}
			int count = Math.Min(len, this._remaining);
			int num = this._in.Read(buf, off, count);
			if (num < 1)
			{
				throw new EndOfStreamException(string.Concat(new object[]
				{
					"DEF length ",
					this._originalLength,
					" object truncated by ",
					this._remaining
				}));
			}
			if ((this._remaining -= num) == 0)
			{
				this.SetParentEofDetect(true);
			}
			return num;
		}

		internal void ReadAllIntoByteArray(byte[] buf)
		{
			if (this._remaining != buf.Length)
			{
				throw new ArgumentException("buffer length not right for data");
			}
			if ((this._remaining -= Streams.ReadFully(this._in, buf)) != 0)
			{
				throw new EndOfStreamException(string.Concat(new object[]
				{
					"DEF length ",
					this._originalLength,
					" object truncated by ",
					this._remaining
				}));
			}
			this.SetParentEofDetect(true);
		}

		internal byte[] ToArray()
		{
			if (this._remaining == 0)
			{
				return DefiniteLengthInputStream.EmptyBytes;
			}
			byte[] array = new byte[this._remaining];
			if ((this._remaining -= Streams.ReadFully(this._in, array)) != 0)
			{
				throw new EndOfStreamException(string.Concat(new object[]
				{
					"DEF length ",
					this._originalLength,
					" object truncated by ",
					this._remaining
				}));
			}
			this.SetParentEofDetect(true);
			return array;
		}
	}
}
