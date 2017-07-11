using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class AppLinkResult : ResultBase, IAppLinkResult, IResult
	{
		public string Url
		{
			get;
			private set;
		}

		public string TargetUrl
		{
			get;
			private set;
		}

		public string Ref
		{
			get;
			private set;
		}

		public IDictionary<string, object> Extras
		{
			get;
			private set;
		}

		public AppLinkResult(ResultContainer resultContainer) : base(resultContainer)
		{
			if (this.ResultDictionary != null)
			{
				string url;
				if (this.ResultDictionary.TryGetValue("url", out url))
				{
					this.Url = url;
				}
				string targetUrl;
				if (this.ResultDictionary.TryGetValue("target_url", out targetUrl))
				{
					this.TargetUrl = targetUrl;
				}
				string @ref;
				if (this.ResultDictionary.TryGetValue("ref", out @ref))
				{
					this.Ref = @ref;
				}
				IDictionary<string, object> extras;
				if (this.ResultDictionary.TryGetValue("extras", out extras))
				{
					this.Extras = extras;
				}
			}
		}

		public override string ToString()
		{
			return Utilities.FormatToString(base.ToString(), base.GetType().Name, new Dictionary<string, string>
			{
				{
					"Url",
					this.Url
				},
				{
					"TargetUrl",
					this.TargetUrl
				},
				{
					"Ref",
					this.Ref
				},
				{
					"Extras",
					this.Extras.ToJson()
				}
			});
		}
	}
}
