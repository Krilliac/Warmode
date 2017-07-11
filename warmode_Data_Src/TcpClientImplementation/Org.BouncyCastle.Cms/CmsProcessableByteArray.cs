using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsProcessableByteArray : CmsProcessable, CmsReadable
	{
		private readonly byte[] bytes;

		public CmsProcessableByteArray(byte[] bytes)
		{
			this.bytes = bytes;
		}

		public Stream GetInputStream()
		{
			return new MemoryStream(this.bytes, false);
		}

		public virtual void Write(Stream zOut)
		{
			zOut.Write(this.bytes, 0, this.bytes.Length);
		}

		public virtual object GetContent()
		{
			return this.bytes.Clone();
		}
	}
}
