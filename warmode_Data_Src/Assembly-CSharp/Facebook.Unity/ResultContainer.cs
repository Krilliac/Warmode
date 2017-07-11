using Facebook.MiniJSON;
using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class ResultContainer
	{
		private const string CanvasResponseKey = "response";

		public string RawResult
		{
			get;
			private set;
		}

		public IDictionary<string, object> ResultDictionary
		{
			get;
			set;
		}

		public ResultContainer(IDictionary<string, object> dictionary)
		{
			this.RawResult = dictionary.ToJson();
			this.ResultDictionary = dictionary;
			if (Constants.IsWeb)
			{
				this.ResultDictionary = this.GetWebFormattedResponseDictionary(this.ResultDictionary);
			}
		}

		public ResultContainer(string result)
		{
			this.RawResult = result;
			if (string.IsNullOrEmpty(result))
			{
				this.ResultDictionary = new Dictionary<string, object>();
			}
			else
			{
				this.ResultDictionary = (Json.Deserialize(result) as Dictionary<string, object>);
				if (Constants.IsWeb && this.ResultDictionary != null)
				{
					this.ResultDictionary = this.GetWebFormattedResponseDictionary(this.ResultDictionary);
				}
			}
		}

		private IDictionary<string, object> GetWebFormattedResponseDictionary(IDictionary<string, object> resultDictionary)
		{
			IDictionary<string, object> dictionary;
			if (resultDictionary.TryGetValue("response", out dictionary))
			{
				object value;
				if (resultDictionary.TryGetValue("callback_id", out value))
				{
					dictionary["callback_id"] = value;
				}
				return dictionary;
			}
			return resultDictionary;
		}
	}
}
