using System;
using System.Text;

namespace BestHTTP.Forms
{
	public class HTTPFieldData
	{
		public string Name
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string MimeType
		{
			get;
			set;
		}

		public Encoding Encoding
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public byte[] Binary
		{
			get;
			set;
		}

		public byte[] Payload
		{
			get
			{
				if (this.Binary != null)
				{
					return this.Binary;
				}
				if (this.Encoding == null)
				{
					this.Encoding = Encoding.UTF8;
				}
				byte[] bytes = this.Encoding.GetBytes(this.Text);
				this.Binary = bytes;
				return bytes;
			}
		}
	}
}
