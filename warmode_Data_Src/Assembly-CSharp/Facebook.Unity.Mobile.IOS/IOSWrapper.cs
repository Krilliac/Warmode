using System;

namespace Facebook.Unity.Mobile.IOS
{
	internal class IOSWrapper : IIOSWrapper
	{
		public void Init(string appId, bool frictionlessRequests, string urlSuffix, string unityUserAgentSuffix)
		{
			IOSWrapper.IOSInit(appId, frictionlessRequests, urlSuffix, unityUserAgentSuffix);
		}

		public void LogInWithReadPermissions(int requestId, string scope)
		{
			IOSWrapper.IOSLogInWithReadPermissions(requestId, scope);
		}

		public void LogInWithPublishPermissions(int requestId, string scope)
		{
			IOSWrapper.IOSLogInWithPublishPermissions(requestId, scope);
		}

		public void LogOut()
		{
			IOSWrapper.IOSLogOut();
		}

		public void SetShareDialogMode(int mode)
		{
			IOSWrapper.IOSSetShareDialogMode(mode);
		}

		public void ShareLink(int requestId, string contentURL, string contentTitle, string contentDescription, string photoURL)
		{
			IOSWrapper.IOSShareLink(requestId, contentURL, contentTitle, contentDescription, photoURL);
		}

		public void FeedShare(int requestId, string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, string mediaSource)
		{
			IOSWrapper.IOSFeedShare(requestId, toId, link, linkName, linkCaption, linkDescription, picture, mediaSource);
		}

		public void AppRequest(int requestId, string message, string actionType, string objectId, string[] to = null, int toLength = 0, string filters = "", string[] excludeIds = null, int excludeIdsLength = 0, bool hasMaxRecipients = false, int maxRecipients = 0, string data = "", string title = "")
		{
			IOSWrapper.IOSAppRequest(requestId, message, actionType, objectId, to, toLength, filters, excludeIds, excludeIdsLength, hasMaxRecipients, maxRecipients, data, title);
		}

		public void AppInvite(int requestId, string appLinkUrl, string previewImageUrl)
		{
			IOSWrapper.IOSAppInvite(requestId, appLinkUrl, previewImageUrl);
		}

		public void CreateGameGroup(int requestId, string name, string description, string privacy)
		{
			IOSWrapper.IOSCreateGameGroup(requestId, name, description, privacy);
		}

		public void JoinGameGroup(int requestId, string groupId)
		{
			IOSWrapper.IOSJoinGameGroup(requestId, groupId);
		}

		public void FBSettingsActivateApp(string appId)
		{
			IOSWrapper.IOSFBSettingsActivateApp(appId);
		}

		public void LogAppEvent(string logEvent, double valueToSum, int numParams, string[] paramKeys, string[] paramVals)
		{
			IOSWrapper.IOSFBAppEventsLogEvent(logEvent, valueToSum, numParams, paramKeys, paramVals);
		}

		public void LogPurchaseAppEvent(double logPurchase, string currency, int numParams, string[] paramKeys, string[] paramVals)
		{
			IOSWrapper.IOSFBAppEventsLogPurchase(logPurchase, currency, numParams, paramKeys, paramVals);
		}

		public void FBAppEventsSetLimitEventUsage(bool limitEventUsage)
		{
			IOSWrapper.IOSFBAppEventsSetLimitEventUsage(limitEventUsage);
		}

		public void GetAppLink(int requestId)
		{
			IOSWrapper.IOSGetAppLink(requestId);
		}

		public string FBSdkVersion()
		{
			return IOSWrapper.IOSFBSdkVersion();
		}

		public void FetchDeferredAppLink(int requestId)
		{
			IOSWrapper.IOSFetchDeferredAppLink(requestId);
		}

		public void RefreshCurrentAccessToken(int requestId)
		{
			IOSWrapper.IOSRefreshCurrentAccessToken(requestId);
		}

		private static void IOSInit(string appId, bool frictionlessRequests, string urlSuffix, string unityUserAgentSuffix)
		{
		}

		private static void IOSLogInWithReadPermissions(int requestId, string scope)
		{
		}

		private static void IOSLogInWithPublishPermissions(int requestId, string scope)
		{
		}

		private static void IOSLogOut()
		{
		}

		private static void IOSSetShareDialogMode(int mode)
		{
		}

		private static void IOSShareLink(int requestId, string contentURL, string contentTitle, string contentDescription, string photoURL)
		{
		}

		private static void IOSFeedShare(int requestId, string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, string mediaSource)
		{
		}

		private static void IOSAppRequest(int requestId, string message, string actionType, string objectId, string[] to = null, int toLength = 0, string filters = "", string[] excludeIds = null, int excludeIdsLength = 0, bool hasMaxRecipients = false, int maxRecipients = 0, string data = "", string title = "")
		{
		}

		private static void IOSAppInvite(int requestId, string appLinkUrl, string previewImageUrl)
		{
		}

		private static void IOSCreateGameGroup(int requestId, string name, string description, string privacy)
		{
		}

		private static void IOSJoinGameGroup(int requestId, string groupId)
		{
		}

		private static void IOSFBSettingsActivateApp(string appId)
		{
		}

		private static void IOSFBAppEventsLogEvent(string logEvent, double valueToSum, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		private static void IOSFBAppEventsLogPurchase(double logPurchase, string currency, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		private static void IOSFBAppEventsSetLimitEventUsage(bool limitEventUsage)
		{
		}

		private static void IOSGetAppLink(int requestId)
		{
		}

		private static string IOSFBSdkVersion()
		{
			return "NONE";
		}

		private static void IOSFetchDeferredAppLink(int requestId)
		{
		}

		private static void IOSRefreshCurrentAccessToken(int requestId)
		{
		}
	}
}
