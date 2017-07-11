using System;
using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	public class KeyValuePairList
	{
		public List<KeyValuePair> Values
		{
			get;
			protected set;
		}

		public bool TryGet(string value, out KeyValuePair param)
		{
			param = null;
			for (int i = 0; i < this.Values.Count; i++)
			{
				if (string.CompareOrdinal(this.Values[i].Key, value) == 0)
				{
					param = this.Values[i];
					return true;
				}
			}
			return false;
		}

		public bool HasAny(string val1, string val2 = "")
		{
			for (int i = 0; i < this.Values.Count; i++)
			{
				if (string.CompareOrdinal(this.Values[i].Key, val1) == 0 || string.CompareOrdinal(this.Values[i].Key, val2) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
