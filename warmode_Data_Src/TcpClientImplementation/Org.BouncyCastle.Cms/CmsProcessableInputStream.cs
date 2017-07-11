using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsProcessableInputStream : CmsProcessable, CmsReadable
	{
		private Stream input;

		private bool used;

		public CmsProcessableInputStream(Stream input)
		{
			this.input = input;
		}

		public Stream GetInputStream()
		{
			this.CheckSingleUsage();
			return this.input;
		}

		public void Write(Stream output)
		{
			this.CheckSingleUsage();
			Streams.PipeAll(this.input, output);
			this.input.Close();
		}

		[Obsolete]
		public object GetContent()
		{
			return this.GetInputStream();
		}

		private void CheckSingleUsage()
		{
			lock (this)
			{
				if (this.used)
				{
					throw new InvalidOperationException("CmsProcessableInputStream can only be used once");
				}
				this.used = true;
			}
		}
	}
}
