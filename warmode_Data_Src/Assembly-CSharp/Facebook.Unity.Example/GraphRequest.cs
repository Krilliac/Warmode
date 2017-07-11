using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class GraphRequest : MenuBase
	{
		private string apiQuery = string.Empty;

		private Texture2D profilePic;

		protected override void GetGui()
		{
			bool enabled = GUI.enabled;
			GUI.enabled = (enabled && FB.IsLoggedIn);
			if (base.Button("Basic Request - Me"))
			{
				FB.API("/me", HttpMethod.GET, new FacebookDelegate<IGraphResult>(base.HandleResult), null);
			}
			if (base.Button("Retrieve Profile Photo"))
			{
				FB.API("/me/picture", HttpMethod.GET, new FacebookDelegate<IGraphResult>(this.ProfilePhotoCallback), null);
			}
			if (base.Button("Take and Upload screenshot"))
			{
				base.StartCoroutine(this.TakeScreenshot());
			}
			base.LabelAndTextField("Request", ref this.apiQuery);
			if (base.Button("Custom Request"))
			{
				FB.API(this.apiQuery, HttpMethod.GET, new FacebookDelegate<IGraphResult>(base.HandleResult), null);
			}
			if (this.profilePic != null)
			{
				GUILayout.Box(this.profilePic, new GUILayoutOption[0]);
			}
			GUI.enabled = enabled;
		}

		private void ProfilePhotoCallback(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
			{
				this.profilePic = result.Texture;
			}
			base.HandleResult(result);
		}

		[DebuggerHidden]
		private IEnumerator TakeScreenshot()
		{
			GraphRequest.<TakeScreenshot>c__Iterator16 <TakeScreenshot>c__Iterator = new GraphRequest.<TakeScreenshot>c__Iterator16();
			<TakeScreenshot>c__Iterator.<>f__this = this;
			return <TakeScreenshot>c__Iterator;
		}
	}
}
