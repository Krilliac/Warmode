using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	internal class GraphResult : ResultBase, IGraphResult, IResult
	{
		public IList<object> ResultList
		{
			get;
			private set;
		}

		public Texture2D Texture
		{
			get;
			private set;
		}

		internal GraphResult(WWW result) : base(new ResultContainer(result.text), result.error, false)
		{
			this.Init(this.RawResult);
			if (result.error == null)
			{
				this.Texture = result.texture;
			}
		}

		private void Init(string rawResult)
		{
			if (string.IsNullOrEmpty(rawResult))
			{
				return;
			}
			object obj = Json.Deserialize(this.RawResult);
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				this.ResultDictionary = dictionary;
				return;
			}
			IList<object> list = obj as IList<object>;
			if (list != null)
			{
				this.ResultList = list;
				return;
			}
		}
	}
}
