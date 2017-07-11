using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class GroupCreateResult : ResultBase, IGroupCreateResult, IResult
	{
		public const string IDKey = "id";

		public string GroupId
		{
			get;
			private set;
		}

		public GroupCreateResult(ResultContainer resultContainer) : base(resultContainer)
		{
			string groupId;
			if (this.ResultDictionary != null && this.ResultDictionary.TryGetValue("id", out groupId))
			{
				this.GroupId = groupId;
			}
		}

		public override string ToString()
		{
			return Utilities.FormatToString(base.ToString(), base.GetType().Name, new Dictionary<string, string>
			{
				{
					"GroupId",
					this.GroupId
				}
			});
		}
	}
}
