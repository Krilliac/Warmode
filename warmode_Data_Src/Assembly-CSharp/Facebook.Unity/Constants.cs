using System;
using System.Globalization;
using UnityEngine;

namespace Facebook.Unity
{
	internal static class Constants
	{
		public const string CallbackIdKey = "callback_id";

		public const string AccessTokenKey = "access_token";

		public const string UrlKey = "url";

		public const string RefKey = "ref";

		public const string ExtrasKey = "extras";

		public const string TargetUrlKey = "target_url";

		public const string CancelledKey = "cancelled";

		public const string ErrorKey = "error";

		public const string OnPayCompleteMethodName = "OnPayComplete";

		public const string OnShareCompleteMethodName = "OnShareLinkComplete";

		public const string OnAppRequestsCompleteMethodName = "OnAppRequestsComplete";

		public const string OnGroupCreateCompleteMethodName = "OnGroupCreateComplete";

		public const string OnGroupJoinCompleteMethodName = "OnJoinGroupComplete";

		public const string GraphApiVersion = "v2.6";

		public const string GraphUrlFormat = "https://graph.{0}/{1}/";

		public const string UserLikesPermission = "user_likes";

		public const string EmailPermission = "email";

		public const string PublishActionsPermission = "publish_actions";

		public const string PublishPagesPermission = "publish_pages";

		private static FacebookUnityPlatform? currentPlatform;

		public static Uri GraphUrl
		{
			get
			{
				string uriString = string.Format(CultureInfo.InvariantCulture, "https://graph.{0}/{1}/", new object[]
				{
					FB.FacebookDomain,
					FB.GraphApiVersion
				});
				return new Uri(uriString);
			}
		}

		public static string GraphApiUserAgent
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
				{
					FB.FacebookImpl.SDKUserAgent,
					Constants.UnitySDKUserAgent
				});
			}
		}

		public static bool IsMobile
		{
			get
			{
				return Constants.CurrentPlatform == FacebookUnityPlatform.Android || Constants.CurrentPlatform == FacebookUnityPlatform.IOS;
			}
		}

		public static bool IsEditor
		{
			get
			{
				return false;
			}
		}

		public static bool IsWeb
		{
			get
			{
				return Constants.CurrentPlatform == FacebookUnityPlatform.WebGL || Constants.CurrentPlatform == FacebookUnityPlatform.WebPlayer;
			}
		}

		public static string UnitySDKUserAgentSuffixLegacy
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "Unity.{0}", new object[]
				{
					FacebookSdkVersion.Build
				});
			}
		}

		public static string UnitySDKUserAgent
		{
			get
			{
				return Utilities.GetUserAgent("FBUnitySDK", FacebookSdkVersion.Build);
			}
		}

		public static bool DebugMode
		{
			get
			{
				return Debug.isDebugBuild;
			}
		}

		public static FacebookUnityPlatform CurrentPlatform
		{
			get
			{
				if (!Constants.currentPlatform.HasValue)
				{
					Constants.currentPlatform = new FacebookUnityPlatform?(Constants.GetCurrentPlatform());
				}
				return Constants.currentPlatform.Value;
			}
			set
			{
				Constants.currentPlatform = new FacebookUnityPlatform?(value);
			}
		}

		private static FacebookUnityPlatform GetCurrentPlatform()
		{
			RuntimePlatform platform = Application.platform;
			switch (platform)
			{
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				return FacebookUnityPlatform.WebPlayer;
			case RuntimePlatform.OSXDashboardPlayer:
			case (RuntimePlatform)6:
			case RuntimePlatform.WindowsEditor:
				IL_26:
				if (platform == RuntimePlatform.Android)
				{
					return FacebookUnityPlatform.Android;
				}
				if (platform != RuntimePlatform.WebGLPlayer)
				{
					return FacebookUnityPlatform.Unknown;
				}
				return FacebookUnityPlatform.WebGL;
			case RuntimePlatform.IPhonePlayer:
				return FacebookUnityPlatform.IOS;
			}
			goto IL_26;
		}
	}
}
