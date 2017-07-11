using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsProcessableFile : CmsProcessable, CmsReadable
	{
		private const int DefaultBufSize = 32768;

		private readonly FileInfo _file;

		private readonly int _bufSize;

		public CmsProcessableFile(FileInfo file) : this(file, 32768)
		{
		}

		public CmsProcessableFile(FileInfo file, int bufSize)
		{
			this._file = file;
			this._bufSize = bufSize;
		}

		public virtual Stream GetInputStream()
		{
			return new FileStream(this._file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, this._bufSize);
		}

		public virtual void Write(Stream zOut)
		{
			Stream inputStream = this.GetInputStream();
			Streams.PipeAll(inputStream, zOut);
			inputStream.Close();
		}

		[Obsolete]
		public virtual object GetContent()
		{
			return this._file;
		}
	}
}
