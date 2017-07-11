using System;
using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	public sealed class WWWAuthenticateHeaderParser : KeyValuePairList
	{
		public WWWAuthenticateHeaderParser(string headerValue)
		{
			base.Values = this.ParseQuotedHeader(headerValue);
		}

		private List<KeyValuePair> ParseQuotedHeader(string str)
		{
			List<KeyValuePair> list = new List<KeyValuePair>();
			if (str != null)
			{
				int i = 0;
				string key = str.Read(ref i, (char ch) => !char.IsWhiteSpace(ch) && !char.IsControl(ch), true).TrimAndLower();
				list.Add(new KeyValuePair(key));
				while (i < str.Length)
				{
					string key2 = str.Read(ref i, '=', true).TrimAndLower();
					KeyValuePair keyValuePair = new KeyValuePair(key2);
					str.SkipWhiteSpace(ref i);
					keyValuePair.Value = str.ReadQuotedText(ref i);
					list.Add(keyValuePair);
				}
			}
			return list;
		}
	}
}
