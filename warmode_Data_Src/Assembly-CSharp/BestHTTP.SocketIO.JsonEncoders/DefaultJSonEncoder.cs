using BestHTTP.JSON;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.JsonEncoders
{
	public sealed class DefaultJSonEncoder : IJsonEncoder
	{
		public List<object> Decode(string json)
		{
			return Json.Decode(json) as List<object>;
		}

		public string Encode(List<object> obj)
		{
			return Json.Encode(obj);
		}
	}
}
