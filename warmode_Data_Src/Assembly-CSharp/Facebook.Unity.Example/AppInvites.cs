using System;

namespace Facebook.Unity.Example
{
	internal class AppInvites : MenuBase
	{
		protected override void GetGui()
		{
			if (base.Button("Android Invite"))
			{
				base.Status = "Logged FB.AppEvent";
				FacebookDelegate<IAppInviteResult> callback = new FacebookDelegate<IAppInviteResult>(base.HandleResult);
				FB.Mobile.AppInvite(new Uri("https://fb.me/892708710750483"), null, callback);
			}
			if (base.Button("Android Invite With Custom Image"))
			{
				base.Status = "Logged FB.AppEvent";
				FB.Mobile.AppInvite(new Uri("https://fb.me/892708710750483"), new Uri("http://i.imgur.com/zkYlB.jpg"), new FacebookDelegate<IAppInviteResult>(base.HandleResult));
			}
			if (base.Button("iOS Invite"))
			{
				base.Status = "Logged FB.AppEvent";
				FacebookDelegate<IAppInviteResult> callback = new FacebookDelegate<IAppInviteResult>(base.HandleResult);
				FB.Mobile.AppInvite(new Uri("https://fb.me/810530068992919"), null, callback);
			}
			if (base.Button("iOS Invite With Custom Image"))
			{
				base.Status = "Logged FB.AppEvent";
				FB.Mobile.AppInvite(new Uri("https://fb.me/810530068992919"), new Uri("http://i.imgur.com/zkYlB.jpg"), new FacebookDelegate<IAppInviteResult>(base.HandleResult));
			}
		}
	}
}
