using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.JsonEncoders
{
	public interface IJsonEncoder
	{
		List<object> Decode(string json);

		string Encode(List<object> obj);
	}
}
