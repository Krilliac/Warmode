using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class AppRequests : MenuBase
	{
		private string requestMessage = string.Empty;

		private string requestTo = string.Empty;

		private string requestFilter = string.Empty;

		private string requestExcludes = string.Empty;

		private string requestMax = string.Empty;

		private string requestData = string.Empty;

		private string requestTitle = string.Empty;

		private string requestObjectID = string.Empty;

		private int selectedAction;

		private string[] actionTypeStrings = new string[]
		{
			"NONE",
			OGActionType.SEND.ToString(),
			OGActionType.ASKFOR.ToString(),
			OGActionType.TURN.ToString()
		};

		protected override void GetGui()
		{
			if (base.Button("Select - Filter None"))
			{
				FacebookDelegate<IAppRequestResult> callback = new FacebookDelegate<IAppRequestResult>(base.HandleResult);
				FB.AppRequest("Test Message", null, null, null, null, string.Empty, string.Empty, callback);
			}
			if (base.Button("Select - Filter app_users"))
			{
				List<object> filters = new List<object>
				{
					"app_users"
				};
				FB.AppRequest("Test Message", null, filters, null, new int?(0), string.Empty, string.Empty, new FacebookDelegate<IAppRequestResult>(base.HandleResult));
			}
			if (base.Button("Select - Filter app_non_users"))
			{
				List<object> filters2 = new List<object>
				{
					"app_non_users"
				};
				FB.AppRequest("Test Message", null, filters2, null, new int?(0), string.Empty, string.Empty, new FacebookDelegate<IAppRequestResult>(base.HandleResult));
			}
			base.LabelAndTextField("Message: ", ref this.requestMessage);
			base.LabelAndTextField("To (optional): ", ref this.requestTo);
			base.LabelAndTextField("Filter (optional): ", ref this.requestFilter);
			base.LabelAndTextField("Exclude Ids (optional): ", ref this.requestExcludes);
			base.LabelAndTextField("Filters: ", ref this.requestExcludes);
			base.LabelAndTextField("Max Recipients (optional): ", ref this.requestMax);
			base.LabelAndTextField("Data (optional): ", ref this.requestData);
			base.LabelAndTextField("Title (optional): ", ref this.requestTitle);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Request Action (optional): ", base.LabelStyle, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(200f * base.ScaleFactor)
			});
			this.selectedAction = GUILayout.Toolbar(this.selectedAction, this.actionTypeStrings, base.ButtonStyle, new GUILayoutOption[]
			{
				GUILayout.MinHeight((float)ConsoleBase.ButtonHeight * base.ScaleFactor),
				GUILayout.MaxWidth((float)(ConsoleBase.MainWindowWidth - 150))
			});
			GUILayout.EndHorizontal();
			base.LabelAndTextField("Request Object ID (optional): ", ref this.requestObjectID);
			if (base.Button("Custom App Request"))
			{
				OGActionType? selectedOGActionType = this.GetSelectedOGActionType();
				if (selectedOGActionType.HasValue)
				{
					FB.AppRequest(this.requestMessage, selectedOGActionType.Value, this.requestObjectID, (this.requestTo == null) ? null : this.requestTo.Split(new char[]
					{
						','
					}), this.requestData, this.requestTitle, new FacebookDelegate<IAppRequestResult>(base.HandleResult));
				}
				else
				{
					FB.AppRequest(this.requestMessage, (!string.IsNullOrEmpty(this.requestTo)) ? this.requestTo.Split(new char[]
					{
						','
					}) : null, (!string.IsNullOrEmpty(this.requestFilter)) ? this.requestFilter.Split(new char[]
					{
						','
					}).OfType<object>().ToList<object>() : null, (!string.IsNullOrEmpty(this.requestExcludes)) ? this.requestExcludes.Split(new char[]
					{
						','
					}) : null, new int?((!string.IsNullOrEmpty(this.requestMax)) ? int.Parse(this.requestMax) : 0), this.requestData, this.requestTitle, new FacebookDelegate<IAppRequestResult>(base.HandleResult));
				}
			}
		}

		private OGActionType? GetSelectedOGActionType()
		{
			string a = this.actionTypeStrings[this.selectedAction];
			if (a == OGActionType.SEND.ToString())
			{
				return new OGActionType?(OGActionType.SEND);
			}
			if (a == OGActionType.ASKFOR.ToString())
			{
				return new OGActionType?(OGActionType.ASKFOR);
			}
			if (a == OGActionType.TURN.ToString())
			{
				return new OGActionType?(OGActionType.TURN);
			}
			return null;
		}
	}
}
