using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DigestInputBuffer : MemoryStream
	{
		private class DigStream : BaseOutputStream
		{
			private readonly IDigest d;

			internal DigStream(IDigest d)
			{
				this.d = d;
			}

			public override void WriteByte(byte b)
			{
				this.d.Update(b);
			}

			public override void Write(byte[] buf, int off, int len)
			{
				this.d.BlockUpdate(buf, off, len);
			}
		}

		internal void UpdateDigest(IDigest d)
		{
			this.WriteTo(new DigestInputBuffer.DigStream(d));
		}
	}
}
