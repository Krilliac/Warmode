using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity.Editor.Dialogs
{
	internal class MockLoginDialog : EditorFacebookMockDialog
	{
		private string accessToken = string.Empty;

		protected override string DialogTitle
		{
			get
			{
				return "Mock Login Dialog";
			}
		}

		protected override void DoGui()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("User Access Token:", new GUILayoutOption[0]);
			this.accessToken = GUILayout.TextField(this.accessToken, GUI.skin.textArea, new GUILayoutOption[]
			{
				GUILayout.MinWidth(400f)
			});
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (GUILayout.Button("Find Access Token", new GUILayoutOption[0]))
			{
				Application.OpenURL(string.Format("https://developers.facebook.com/tools/accesstoken/?app_id={0}", FB.AppId));
			}
			GUILayout.Space(20f);
		}

		protected override void SendSuccessResult()
		{
			if (string.IsNullOrEmpty(this.accessToken))
			{
				this.SendErrorResult("Empty Access token string");
				return;
			}
			FB.API("/me?fields=id&access_token=" + this.accessToken, HttpMethod.GET, delegate(IGraphResult graphResult)
			{
				if (!string.IsNullOrEmpty(graphResult.Error))
				{
					this.SendErrorResult("Graph API error: " + graphResult.Error);
					return;
				}
				string facebookID = graphResult.ResultDictionary["id"] as string;
				FB.API("/me/permissions?access_token=" + this.accessToken, HttpMethod.GET, delegate(IGraphResult permResult)
				{
					if (!string.IsNullOrEmpty(permResult.Error))
					{
						this.SendErrorResult("Graph API error: " + permResult.Error);
						return;
					}
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					List<object> list3 = permResult.ResultDictionary["data"] as List<object>;
					using (List<object>.Enumerator enumerator = list3.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Dictionary<string, object> dictionary = (Dictionary<string, object>)enumerator.Current;
							if (dictionary["status"] as string == "granted")
							{
								list.Add(dictionary["permission"] as string);
							}
							else
							{
								list2.Add(dictionary["permission"] as string);
							}
						}
					}
					AccessToken accessToken = new AccessToken(this.accessToken, facebookID, DateTime.UtcNow.AddDays(60.0), list, new DateTime?(DateTime.UtcNow));
					IDictionary<string, object> dictionary2 = (IDictionary<string, object>)Json.Deserialize(accessToken.ToJson());
					dictionary2.Add("granted_permissions", list);
					dictionary2.Add("declined_permissions", list2);
					if (!string.IsNullOrEmpty(this.CallbackID))
					{
						dictionary2["callback_id"] = this.CallbackID;
					}
					if (this.Callback != null)
					{
						this.Callback(new ResultContainer(dictionary2));
					}
				}, null);
			}, null);
		}
	}
}
