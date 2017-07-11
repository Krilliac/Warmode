using BestHTTP.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;

namespace BestHTTP
{
	internal static class HTTPProtocolFactory
	{
		public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			if (protocol != SupportedProtocols.WebSocket)
			{
				return new HTTPResponse(request, stream, isStreamed, isFromCache);
			}
			return new WebSocketResponse(request, stream, isStreamed, isFromCache);
		}

		public static SupportedProtocols GetProtocolFromUri(Uri uri)
		{
			string text = uri.Scheme.ToLowerInvariant();
			string text2 = text;
			if (text2 != null)
			{
				if (HTTPProtocolFactory.<>f__switch$map3 == null)
				{
					HTTPProtocolFactory.<>f__switch$map3 = new Dictionary<string, int>(2)
					{
						{
							"ws",
							0
						},
						{
							"wss",
							0
						}
					};
				}
				int num;
				if (HTTPProtocolFactory.<>f__switch$map3.TryGetValue(text2, out num))
				{
					if (num == 0)
					{
						return SupportedProtocols.WebSocket;
					}
				}
			}
			return SupportedProtocols.HTTP;
		}

		public static bool IsSecureProtocol(Uri uri)
		{
			string text = uri.Scheme.ToLowerInvariant();
			string text2 = text;
			if (text2 != null)
			{
				if (HTTPProtocolFactory.<>f__switch$map4 == null)
				{
					HTTPProtocolFactory.<>f__switch$map4 = new Dictionary<string, int>(2)
					{
						{
							"https",
							0
						},
						{
							"wss",
							0
						}
					};
				}
				int num;
				if (HTTPProtocolFactory.<>f__switch$map4.TryGetValue(text2, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
