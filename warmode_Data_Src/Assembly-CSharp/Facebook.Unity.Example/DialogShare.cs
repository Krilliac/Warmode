using System;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class DialogShare : MenuBase
	{
		private string shareLink = "https://developers.facebook.com/";

		private string shareTitle = "Link Title";

		private string shareDescription = "Link Description";

		private string shareImage = "http://i.imgur.com/j4M7vCO.jpg";

		private string feedTo = string.Empty;

		private string feedLink = "https://developers.facebook.com/";

		private string feedTitle = "Test Title";

		private string feedCaption = "Test Caption";

		private string feedDescription = "Test Description";

		private string feedImage = "http://i.imgur.com/zkYlB.jpg";

		private string feedMediaSource = string.Empty;

		protected override bool ShowDialogModeSelector()
		{
			return false;
		}

		protected override void GetGui()
		{
			bool enabled = GUI.enabled;
			if (base.Button("Share - Link"))
			{
				FacebookDelegate<IShareResult> callback = new FacebookDelegate<IShareResult>(base.HandleResult);
				FB.ShareLink(new Uri("https://developers.facebook.com/"), string.Empty, string.Empty, null, callback);
			}
			if (base.Button("Share - Link Photo"))
			{
				FB.ShareLink(new Uri("https://developers.facebook.com/"), "Link Share", "Look I'm sharing a link", new Uri("http://i.imgur.com/j4M7vCO.jpg"), new FacebookDelegate<IShareResult>(base.HandleResult));
			}
			base.LabelAndTextField("Link", ref this.shareLink);
			base.LabelAndTextField("Title", ref this.shareTitle);
			base.LabelAndTextField("Description", ref this.shareDescription);
			base.LabelAndTextField("Image", ref this.shareImage);
			if (base.Button("Share - Custom"))
			{
				FB.ShareLink(new Uri(this.shareLink), this.shareTitle, this.shareDescription, new Uri(this.shareImage), new FacebookDelegate<IShareResult>(base.HandleResult));
			}
			GUI.enabled = (enabled && (!Constants.IsEditor || (Constants.IsEditor && FB.IsLoggedIn)));
			if (base.Button("Feed Share - No To"))
			{
				FB.FeedShare(string.Empty, new Uri("https://developers.facebook.com/"), "Test Title", "Test caption", "Test Description", new Uri("http://i.imgur.com/zkYlB.jpg"), string.Empty, new FacebookDelegate<IShareResult>(base.HandleResult));
			}
			base.LabelAndTextField("To", ref this.feedTo);
			base.LabelAndTextField("Link", ref this.feedLink);
			base.LabelAndTextField("Title", ref this.feedTitle);
			base.LabelAndTextField("Caption", ref this.feedCaption);
			base.LabelAndTextField("Description", ref this.feedDescription);
			base.LabelAndTextField("Image", ref this.feedImage);
			base.LabelAndTextField("Media Source", ref this.feedMediaSource);
			if (base.Button("Feed Share - Custom"))
			{
				FB.FeedShare(this.feedTo, (!string.IsNullOrEmpty(this.feedLink)) ? new Uri(this.feedLink) : null, this.feedTitle, this.feedCaption, this.feedDescription, (!string.IsNullOrEmpty(this.feedImage)) ? new Uri(this.feedImage) : null, this.feedMediaSource, new FacebookDelegate<IShareResult>(base.HandleResult));
			}
			GUI.enabled = enabled;
		}
	}
}
