using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal abstract class LimitedInputStream : BaseInputStream
	{
		protected readonly Stream _in;

		private int _limit;

		internal LimitedInputStream(Stream inStream, int limit)
		{
			this._in = inStream;
			this._limit = limit;
		}

		internal virtual int GetRemaining()
		{
			return this._limit;
		}

		protected virtual void SetParentEofDetect(bool on)
		{
			if (this._in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)this._in).SetEofOn00(on);
			}
		}
	}
}
