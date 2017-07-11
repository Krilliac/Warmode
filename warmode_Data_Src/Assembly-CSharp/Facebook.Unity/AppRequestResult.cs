using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class AppRequestResult : ResultBase, IAppRequestResult, IResult
	{
		public const string RequestIDKey = "request";

		public const string ToKey = "to";

		public string RequestID
		{
			get;
			private set;
		}

		public IEnumerable<string> To
		{
			get;
			private set;
		}

		public AppRequestResult(ResultContainer resultContainer) : base(resultContainer)
		{
			if (this.ResultDictionary != null)
			{
				string requestID;
				if (this.ResultDictionary.TryGetValue("request", out requestID))
				{
					this.RequestID = requestID;
				}
				string text;
				IEnumerable<object> enumerable;
				if (this.ResultDictionary.TryGetValue("to", out text))
				{
					this.To = text.Split(new char[]
					{
						','
					});
				}
				else if (this.ResultDictionary.TryGetValue("to", out enumerable))
				{
					List<string> list = new List<string>();
					foreach (object current in enumerable)
					{
						string text2 = current as string;
						if (text2 != null)
						{
							list.Add(text2);
						}
					}
					this.To = list;
				}
			}
		}

		public override string ToString()
		{
			return Utilities.FormatToString(base.ToString(), base.GetType().Name, new Dictionary<string, string>
			{
				{
					"RequestID",
					this.RequestID
				},
				{
					"To",
					(this.To == null) ? null : this.To.ToCommaSeparateList()
				}
			});
		}
	}
}
