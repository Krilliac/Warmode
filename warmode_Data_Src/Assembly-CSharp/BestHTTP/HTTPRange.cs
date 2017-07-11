using System;

namespace BestHTTP
{
	public sealed class HTTPRange
	{
		public int FirstBytePos
		{
			get;
			private set;
		}

		public int LastBytePos
		{
			get;
			private set;
		}

		public int ContentLength
		{
			get;
			private set;
		}

		public bool IsValid
		{
			get;
			private set;
		}

		internal HTTPRange()
		{
			this.ContentLength = -1;
			this.IsValid = false;
		}

		internal HTTPRange(int contentLength)
		{
			this.ContentLength = contentLength;
			this.IsValid = false;
		}

		internal HTTPRange(int fbp, int lbp, int contentLength)
		{
			this.FirstBytePos = fbp;
			this.LastBytePos = lbp;
			this.ContentLength = contentLength;
			this.IsValid = (this.FirstBytePos <= this.LastBytePos && this.ContentLength > this.LastBytePos);
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}/{2} (valid: {3})", new object[]
			{
				this.FirstBytePos,
				this.LastBytePos,
				this.ContentLength,
				this.IsValid
			});
		}
	}
}
