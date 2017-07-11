using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	internal abstract class FacebookBase : IFacebook, IFacebookImplementation, IFacebookResultHandler
	{
		private InitDelegate onInitCompleteDelegate;

		private HideUnityDelegate onHideUnityDelegate;

		public abstract bool LimitEventUsage
		{
			get;
			set;
		}

		public abstract string SDKName
		{
			get;
		}

		public abstract string SDKVersion
		{
			get;
		}

		public virtual string SDKUserAgent
		{
			get
			{
				return Utilities.GetUserAgent(this.SDKName, this.SDKVersion);
			}
		}

		public bool LoggedIn
		{
			get
			{
				AccessToken currentAccessToken = AccessToken.CurrentAccessToken;
				return currentAccessToken != null && currentAccessToken.ExpirationTime > DateTime.UtcNow;
			}
		}

		public bool Initialized
		{
			get;
			private set;
		}

		protected CallbackManager CallbackManager
		{
			get;
			private set;
		}

		protected FacebookBase(CallbackManager callbackManager)
		{
			this.CallbackManager = callbackManager;
		}

		public virtual void Init(HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			this.onHideUnityDelegate = hideUnityDelegate;
			this.onInitCompleteDelegate = onInitComplete;
		}

		public abstract void LogInWithPublishPermissions(IEnumerable<string> scope, FacebookDelegate<ILoginResult> callback);

		public abstract void LogInWithReadPermissions(IEnumerable<string> scope, FacebookDelegate<ILoginResult> callback);

		public virtual void LogOut()
		{
			AccessToken.CurrentAccessToken = null;
		}

		public void AppRequest(string message, IEnumerable<string> to = null, IEnumerable<object> filters = null, IEnumerable<string> excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null)
		{
			this.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
		}

		public abstract void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback);

		public abstract void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback);

		public abstract void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback);

		public void API(string query, HttpMethod method, IDictionary<string, string> formData, FacebookDelegate<IGraphResult> callback)
		{
			IDictionary<string, string> arg_19_0;
			if (formData != null)
			{
				IDictionary<string, string> dictionary = this.CopyByValue(formData);
				arg_19_0 = dictionary;
			}
			else
			{
				arg_19_0 = new Dictionary<string, string>();
			}
			IDictionary<string, string> dictionary2 = arg_19_0;
			if (!dictionary2.ContainsKey("access_token") && !query.Contains("access_token="))
			{
				dictionary2["access_token"] = ((!FB.IsLoggedIn) ? string.Empty : AccessToken.CurrentAccessToken.TokenString);
			}
			AsyncRequestString.Request(this.GetGraphUrl(query), method, dictionary2, callback);
		}

		public void API(string query, HttpMethod method, WWWForm formData, FacebookDelegate<IGraphResult> callback)
		{
			if (formData == null)
			{
				formData = new WWWForm();
			}
			string value = (AccessToken.CurrentAccessToken == null) ? string.Empty : AccessToken.CurrentAccessToken.TokenString;
			formData.AddField("access_token", value);
			AsyncRequestString.Request(this.GetGraphUrl(query), method, formData, callback);
		}

		public abstract void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback);

		public abstract void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback);

		public abstract void ActivateApp(string appId = null);

		public abstract void GetAppLink(FacebookDelegate<IAppLinkResult> callback);

		public abstract void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters);

		public abstract void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters);

		public virtual void OnHideUnity(bool isGameShown)
		{
			if (this.onHideUnityDelegate != null)
			{
				this.onHideUnityDelegate(isGameShown);
			}
		}

		public virtual void OnInitComplete(ResultContainer resultContainer)
		{
			this.Initialized = true;
			FacebookDelegate<ILoginResult> callback = delegate(ILoginResult result)
			{
				if (this.onInitCompleteDelegate != null)
				{
					this.onInitCompleteDelegate();
				}
			};
			resultContainer.ResultDictionary["callback_id"] = this.CallbackManager.AddFacebookDelegate<ILoginResult>(callback);
			this.OnLoginComplete(resultContainer);
		}

		public abstract void OnLoginComplete(ResultContainer resultContainer);

		public void OnLogoutComplete(ResultContainer resultContainer)
		{
			AccessToken.CurrentAccessToken = null;
		}

		public abstract void OnGetAppLinkComplete(ResultContainer resultContainer);

		public abstract void OnGroupCreateComplete(ResultContainer resultContainer);

		public abstract void OnGroupJoinComplete(ResultContainer resultContainer);

		public abstract void OnAppRequestsComplete(ResultContainer resultContainer);

		public abstract void OnShareLinkComplete(ResultContainer resultContainer);

		protected void ValidateAppRequestArgs(string message, OGActionType? actionType, string objectId, IEnumerable<string> to = null, IEnumerable<object> filters = null, IEnumerable<string> excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException("message", "message cannot be null or empty!");
			}
			if (!string.IsNullOrEmpty(objectId) && !(actionType == OGActionType.ASKFOR) && !(actionType == OGActionType.SEND))
			{
				throw new ArgumentNullException("objectId", "Object ID must be set if and only if action type is SEND or ASKFOR");
			}
			if (!actionType.HasValue && !string.IsNullOrEmpty(objectId))
			{
				throw new ArgumentNullException("actionType", "You cannot provide an objectId without an actionType");
			}
		}

		protected void OnAuthResponse(LoginResult result)
		{
			if (result.AccessToken != null)
			{
				AccessToken.CurrentAccessToken = result.AccessToken;
			}
			this.CallbackManager.OnFacebookResponse(result);
		}

		private IDictionary<string, string> CopyByValue(IDictionary<string, string> data)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(data.Count);
			foreach (KeyValuePair<string, string> current in data)
			{
				dictionary[current.Key] = ((current.Value == null) ? null : new string(current.Value.ToCharArray()));
			}
			return dictionary;
		}

		private Uri GetGraphUrl(string query)
		{
			if (!string.IsNullOrEmpty(query) && query.StartsWith("/"))
			{
				query = query.Substring(1);
			}
			return new Uri(Constants.GraphUrl, query);
		}
	}
}
