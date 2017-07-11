using Facebook.MiniJSON;
using Facebook.Unity.Canvas;
using Facebook.Unity.Mobile;
using System;
using System.Collections.Generic;

namespace Facebook.Unity.Editor
{
	internal class EditorFacebook : FacebookBase, ICanvasFacebook, ICanvasFacebookImplementation, ICanvasFacebookResultHandler, IFacebook, IFacebookResultHandler, IPayFacebook, IMobileFacebook, IMobileFacebookImplementation, IMobileFacebookResultHandler
	{
		private const string WarningMessage = "You are using the facebook SDK in the Unity Editor. Behavior may not be the same as when used on iOS, Android, or Web.";

		private const string AccessTokenKey = "com.facebook.unity.editor.accesstoken";

		private IEditorWrapper editorWrapper;

		public override bool LimitEventUsage
		{
			get;
			set;
		}

		public ShareDialogMode ShareDialogMode
		{
			get;
			set;
		}

		public override string SDKName
		{
			get
			{
				return "FBUnityEditorSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return FacebookSdkVersion.Build;
			}
		}

		private static IFacebookCallbackHandler EditorGameObject
		{
			get
			{
				return ComponentFactory.GetComponent<EditorFacebookGameObject>(ComponentFactory.IfNotExist.AddNew);
			}
		}

		public EditorFacebook(IEditorWrapper wrapper, CallbackManager callbackManager) : base(callbackManager)
		{
			this.editorWrapper = wrapper;
		}

		public EditorFacebook() : this(new EditorWrapper(EditorFacebook.EditorGameObject), new CallbackManager())
		{
		}

		public override void Init(HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			FacebookLogger.Warn("You are using the facebook SDK in the Unity Editor. Behavior may not be the same as when used on iOS, Android, or Web.");
			base.Init(hideUnityDelegate, onInitComplete);
			this.editorWrapper.Init();
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			this.LogInWithPublishPermissions(permissions, callback);
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			this.editorWrapper.ShowLoginMockDialog(new Utilities.Callback<ResultContainer>(this.OnLoginComplete), base.CallbackManager.AddFacebookDelegate<ILoginResult>(callback), permissions.ToCommaSeparateList());
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			this.editorWrapper.ShowAppRequestMockDialog(new Utilities.Callback<ResultContainer>(this.OnAppRequestsComplete), base.CallbackManager.AddFacebookDelegate<IAppRequestResult>(callback));
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			this.editorWrapper.ShowMockShareDialog(new Utilities.Callback<ResultContainer>(this.OnShareLinkComplete), "ShareLink", base.CallbackManager.AddFacebookDelegate<IShareResult>(callback));
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			this.editorWrapper.ShowMockShareDialog(new Utilities.Callback<ResultContainer>(this.OnShareLinkComplete), "FeedShare", base.CallbackManager.AddFacebookDelegate<IShareResult>(callback));
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			this.editorWrapper.ShowGameGroupCreateMockDialog(new Utilities.Callback<ResultContainer>(this.OnGroupCreateComplete), base.CallbackManager.AddFacebookDelegate<IGroupCreateResult>(callback));
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			this.editorWrapper.ShowGameGroupJoinMockDialog(new Utilities.Callback<ResultContainer>(this.OnGroupJoinComplete), base.CallbackManager.AddFacebookDelegate<IGroupJoinResult>(callback));
		}

		public override void ActivateApp(string appId)
		{
			FacebookLogger.Info("This only needs to be called for iOS or Android.");
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["url"] = "mockurl://testing.url";
			dictionary["callback_id"] = base.CallbackManager.AddFacebookDelegate<IAppLinkResult>(callback);
			this.OnGetAppLinkComplete(new ResultContainer(dictionary));
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			FacebookLogger.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			FacebookLogger.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback)
		{
			this.editorWrapper.ShowAppInviteMockDialog(new Utilities.Callback<ResultContainer>(this.OnAppInviteComplete), base.CallbackManager.AddFacebookDelegate<IAppInviteResult>(callback));
		}

		public void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["url"] = "mockurl://testing.url";
			dictionary["ref"] = "mock ref";
			dictionary["extras"] = new Dictionary<string, object>
			{
				{
					"mock extra key",
					"mock extra value"
				}
			};
			dictionary["target_url"] = "mocktargeturl://mocktarget.url";
			dictionary["callback_id"] = base.CallbackManager.AddFacebookDelegate<IAppLinkResult>(callback);
			this.OnFetchDeferredAppLinkComplete(new ResultContainer(dictionary));
		}

		public void Pay(string product, string action, int quantity, int? quantityMin, int? quantityMax, string requestId, string pricepointId, string testCurrency, FacebookDelegate<IPayResult> callback)
		{
			this.editorWrapper.ShowPayMockDialog(new Utilities.Callback<ResultContainer>(this.OnPayComplete), base.CallbackManager.AddFacebookDelegate<IPayResult>(callback));
		}

		public void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback)
		{
			if (callback == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"callback_id",
					base.CallbackManager.AddFacebookDelegate<IAccessTokenRefreshResult>(callback)
				}
			};
			if (AccessToken.CurrentAccessToken == null)
			{
				dictionary["error"] = "No current access token";
			}
			else
			{
				IDictionary<string, object> source = (IDictionary<string, object>)Json.Deserialize(AccessToken.CurrentAccessToken.ToJson());
				dictionary.AddAllKVPFrom(source);
			}
			this.OnRefreshCurrentAccessTokenComplete(new ResultContainer(dictionary));
		}

		public override void OnAppRequestsComplete(ResultContainer resultContainer)
		{
			AppRequestResult result = new AppRequestResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGetAppLinkComplete(ResultContainer resultContainer)
		{
			AppLinkResult result = new AppLinkResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupCreateComplete(ResultContainer resultContainer)
		{
			GroupCreateResult result = new GroupCreateResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupJoinComplete(ResultContainer resultContainer)
		{
			GroupJoinResult result = new GroupJoinResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnLoginComplete(ResultContainer resultContainer)
		{
			LoginResult result = new LoginResult(resultContainer);
			base.OnAuthResponse(result);
		}

		public override void OnShareLinkComplete(ResultContainer resultContainer)
		{
			ShareResult result = new ShareResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnAppInviteComplete(ResultContainer resultContainer)
		{
			AppInviteResult result = new AppInviteResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnFetchDeferredAppLinkComplete(ResultContainer resultContainer)
		{
			AppLinkResult result = new AppLinkResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnPayComplete(ResultContainer resultContainer)
		{
			PayResult result = new PayResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnRefreshCurrentAccessTokenComplete(ResultContainer resultContainer)
		{
			AccessTokenRefreshResult result = new AccessTokenRefreshResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnFacebookAuthResponseChange(ResultContainer resultContainer)
		{
			throw new NotSupportedException();
		}

		public void OnUrlResponse(string message)
		{
			throw new NotSupportedException();
		}
	}
}
