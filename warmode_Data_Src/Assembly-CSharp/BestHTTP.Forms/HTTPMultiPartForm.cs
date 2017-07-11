using BestHTTP.Extensions;
using System;
using System.IO;

namespace BestHTTP.Forms
{
	public sealed class HTTPMultiPartForm : HTTPFormBase
	{
		private string Boundary;

		private byte[] CachedData;

		public HTTPMultiPartForm()
		{
			this.Boundary = this.GetHashCode().ToString("X");
		}

		public override void PrepareRequest(HTTPRequest request)
		{
			request.SetHeader("Content-Type", "multipart/form-data; boundary=\"" + this.Boundary + "\"");
		}

		public override byte[] GetData()
		{
			if (this.CachedData != null)
			{
				return this.CachedData;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				for (int i = 0; i < base.Fields.Count; i++)
				{
					HTTPFieldData hTTPFieldData = base.Fields[i];
					memoryStream.WriteLine("--" + this.Boundary);
					memoryStream.WriteLine("Content-Disposition: form-data; name=\"" + hTTPFieldData.Name + "\"" + (string.IsNullOrEmpty(hTTPFieldData.FileName) ? string.Empty : ("; filename=\"" + hTTPFieldData.FileName + "\"")));
					if (!string.IsNullOrEmpty(hTTPFieldData.MimeType))
					{
						memoryStream.WriteLine("Content-Type: " + hTTPFieldData.MimeType);
					}
					memoryStream.WriteLine("Content-Length: " + hTTPFieldData.Payload.Length.ToString());
					memoryStream.WriteLine();
					memoryStream.Write(hTTPFieldData.Payload, 0, hTTPFieldData.Payload.Length);
					memoryStream.Write(HTTPRequest.EOL, 0, HTTPRequest.EOL.Length);
				}
				memoryStream.WriteLine("--" + this.Boundary + "--");
				base.IsChanged = false;
				result = (this.CachedData = memoryStream.ToArray());
			}
			return result;
		}
	}
}
