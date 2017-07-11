using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Mobile.Android
{
	internal sealed class AndroidFacebook : MobileFacebook
	{
		private class JavaMethodCall<T> : MethodCall<T> where T : IResult
		{
			private AndroidFacebook androidImpl;

			public JavaMethodCall(AndroidFacebook androidImpl, string methodName) : base(androidImpl, methodName)
			{
				this.androidImpl = androidImpl;
			}

			public override void Call(MethodArguments args = null)
			{
				MethodArguments methodArguments;
				if (args == null)
				{
					methodArguments = new MethodArguments();
				}
				else
				{
					methodArguments = new MethodArguments(args);
				}
				if (base.Callback != null)
				{
					methodArguments.AddString("callback_id", this.androidImpl.CallbackManager.AddFacebookDelegate<T>(base.Callback));
				}
				this.androidImpl.CallFB(base.MethodName, methodArguments.ToJsonString());
			}
		}

		public const string LoginPermissionsKey = "scope";

		private bool limitEventUsage;

		private IAndroidJavaClass facebookJava;

		public string KeyHash
		{
			get;
			private set;
		}

		public override bool LimitEventUsage
		{
			get
			{
				return this.limitEventUsage;
			}
			set
			{
				this.limitEventUsage = value;
				this.CallFB("SetLimitEventUsage", value.ToString());
			}
		}

		public override string SDKName
		{
			get
			{
				return "FBAndroidSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return this.facebookJava.CallStatic<string>("GetSdkVersion");
			}
		}

		public AndroidFacebook() : this(new FBJavaClass(), new CallbackManager())
		{
		}

		public AndroidFacebook(IAndroidJavaClass facebookJavaClass, CallbackManager callbackManager) : base(callbackManager)
		{
			this.KeyHash = string.Empty;
			this.facebookJava = facebookJavaClass;
		}

		public void Init(string appId, HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			this.CallFB("SetUserAgentSuffix", string.Format(Constants.UnitySDKUserAgentSuffixLegacy, new object[0]));
			base.Init(hideUnityDelegate, onInitComplete);
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("appId", appId);
			AndroidFacebook.JavaMethodCall<IResult> javaMethodCall = new AndroidFacebook.JavaMethodCall<IResult>(this, "Init");
			javaMethodCall.Call(methodArguments);
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddCommaSeparatedList("scope", permissions);
			new AndroidFacebook.JavaMethodCall<ILoginResult>(this, "LoginWithReadPermissions")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddCommaSeparatedList("scope", permissions);
			new AndroidFacebook.JavaMethodCall<ILoginResult>(this, "LoginWithPublishPermissions")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void LogOut()
		{
			AndroidFacebook.JavaMethodCall<IResult> javaMethodCall = new AndroidFacebook.JavaMethodCall<IResult>(this, "Logout");
			javaMethodCall.Call(null);
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			base.ValidateAppRequestArgs(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("message", message);
			methodArguments.AddNullablePrimitive<OGActionType>("action_type", actionType);
			methodArguments.AddString("object_id", objectId);
			methodArguments.AddCommaSeparatedList("to", to);
			if (filters != null && filters.Any<object>())
			{
				string text = filters.First<object>() as string;
				if (text != null)
				{
					methodArguments.AddString("filters", text);
				}
			}
			methodArguments.AddNullablePrimitive<int>("max_recipients", maxRecipients);
			methodArguments.AddString("data", data);
			methodArguments.AddString("title", title);
			new AndroidFacebook.JavaMethodCall<IAppRequestResult>(this, "AppRequest")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddUri("appLinkUrl", appLinkUrl);
			methodArguments.AddUri("previewImageUrl", previewImageUrl);
			new AndroidFacebook.JavaMethodCall<IAppInviteResult>(this, "AppInvite")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddUri("content_url", contentURL);
			methodArguments.AddString("content_title", contentTitle);
			methodArguments.AddString("content_description", contentDescription);
			methodArguments.AddUri("photo_url", photoURL);
			new AndroidFacebook.JavaMethodCall<IShareResult>(this, "ShareLink")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("toId", toId);
			methodArguments.AddUri("link", link);
			methodArguments.AddString("linkName", linkName);
			methodArguments.AddString("linkCaption", linkCaption);
			methodArguments.AddString("linkDescription", linkDescription);
			methodArguments.AddUri("picture", picture);
			methodArguments.AddString("mediaSource", mediaSource);
			new AndroidFacebook.JavaMethodCall<IShareResult>(this, "FeedShare")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("name", name);
			methodArguments.AddString("description", description);
			methodArguments.AddString("privacy", privacy);
			new AndroidFacebook.JavaMethodCall<IGroupCreateResult>(this, "GameGroupCreate")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("id", id);
			new AndroidFacebook.JavaMethodCall<IGroupJoinResult>(this, "GameGroupJoin")
			{
				Callback = callback
			}.Call(methodArguments);
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			new AndroidFacebook.JavaMethodCall<IAppLinkResult>(this, "GetAppLink")
			{
				Callback = callback
			}.Call(null);
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("logEvent", logEvent);
			methodArguments.AddNullablePrimitive<float>("valueToSum", valueToSum);
			methodArguments.AddDictionary("parameters", parameters);
			AndroidFacebook.JavaMethodCall<IResult> javaMethodCall = new AndroidFacebook.JavaMethodCall<IResult>(this, "LogAppEvent");
			javaMethodCall.Call(methodArguments);
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddPrimative<float>("logPurchase", logPurchase);
			methodArguments.AddString("currency", currency);
			methodArguments.AddDictionary("parameters", parameters);
			AndroidFacebook.JavaMethodCall<IResult> javaMethodCall = new AndroidFacebook.JavaMethodCall<IResult>(this, "LogAppEvent");
			javaMethodCall.Call(methodArguments);
		}

		public override void ActivateApp(string appId)
		{
		}

		public override void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			MethodArguments args = new MethodArguments();
			new AndroidFacebook.JavaMethodCall<IAppLinkResult>(this, "FetchDeferredAppLinkData")
			{
				Callback = callback
			}.Call(args);
		}

		public override void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback)
		{
			new AndroidFacebook.JavaMethodCall<IAccessTokenRefreshResult>(this, "RefreshCurrentAccessToken")
			{
				Callback = callback
			}.Call(null);
		}

		protected override void SetShareDialogMode(ShareDialogMode mode)
		{
			this.CallFB("SetShareDialogMode", mode.ToString());
		}

		private void CallFB(string method, string args)
		{
			this.facebookJava.CallStatic(method, new object[]
			{
				args
			});
		}
	}
}
