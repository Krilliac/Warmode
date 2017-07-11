using LitJson;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.JsonEncoders
{
	public sealed class LitJsonEncoder : IJsonEncoder
	{
		public List<object> Decode(string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ToObject<List<object>>(reader);
		}

		public string Encode(List<object> obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}
	}
}
