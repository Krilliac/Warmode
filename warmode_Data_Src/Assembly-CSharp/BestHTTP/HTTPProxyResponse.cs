using System;
using System.IO;

namespace BestHTTP
{
	public class HTTPProxyResponse : HTTPResponse
	{
		internal HTTPProxyResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache) : base(request, stream, isStreamed, isFromCache)
		{
		}

		internal override bool Receive(int forceReadRawContentLength = -1, bool readPayloadData = false)
		{
			return base.Receive(forceReadRawContentLength, false);
		}
	}
}
