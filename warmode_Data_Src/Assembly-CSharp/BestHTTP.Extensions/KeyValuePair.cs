using System;

namespace BestHTTP.Extensions
{
	public sealed class KeyValuePair
	{
		public string Key
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public KeyValuePair(string key)
		{
			this.Key = key;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Value))
			{
				return this.Key + '=' + this.Value;
			}
			return this.Key;
		}
	}
}
