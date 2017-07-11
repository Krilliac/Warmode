using System;
using UnityEngine;

namespace Facebook.Unity.Canvas
{
	internal class JsBridge : MonoBehaviour
	{
		private ICanvasFacebookCallbackHandler facebook;

		public void Start()
		{
			this.facebook = ComponentFactory.GetComponent<CanvasFacebookGameObject>(ComponentFactory.IfNotExist.ReturnNull);
		}

		public void OnLoginComplete(string responseJsonData = "")
		{
			this.facebook.OnLoginComplete(responseJsonData);
		}

		public void OnFacebookAuthResponseChange(string responseJsonData = "")
		{
			this.facebook.OnFacebookAuthResponseChange(responseJsonData);
		}

		public void OnPayComplete(string responseJsonData = "")
		{
			this.facebook.OnPayComplete(responseJsonData);
		}

		public void OnAppRequestsComplete(string responseJsonData = "")
		{
			this.facebook.OnAppRequestsComplete(responseJsonData);
		}

		public void OnShareLinkComplete(string responseJsonData = "")
		{
			this.facebook.OnShareLinkComplete(responseJsonData);
		}

		public void OnGroupCreateComplete(string responseJsonData = "")
		{
			this.facebook.OnGroupCreateComplete(responseJsonData);
		}

		public void OnJoinGroupComplete(string responseJsonData = "")
		{
			this.facebook.OnGroupJoinComplete(responseJsonData);
		}

		public void OnFacebookFocus(string state)
		{
			this.facebook.OnHideUnity(state != "hide");
		}

		public void OnInitComplete(string responseJsonData = "")
		{
			this.facebook.OnInitComplete(responseJsonData);
		}

		public void OnUrlResponse(string url = "")
		{
			this.facebook.OnUrlResponse(url);
		}
	}
}
