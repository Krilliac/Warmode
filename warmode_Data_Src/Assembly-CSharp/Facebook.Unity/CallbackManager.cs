using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class CallbackManager
	{
		private IDictionary<string, object> facebookDelegates = new Dictionary<string, object>();

		private int nextAsyncId;

		public string AddFacebookDelegate<T>(FacebookDelegate<T> callback) where T : IResult
		{
			if (callback == null)
			{
				return null;
			}
			this.nextAsyncId++;
			this.facebookDelegates.Add(this.nextAsyncId.ToString(), callback);
			return this.nextAsyncId.ToString();
		}

		public void OnFacebookResponse(IInternalResult result)
		{
			if (result == null || result.CallbackId == null)
			{
				return;
			}
			object callback;
			if (this.facebookDelegates.TryGetValue(result.CallbackId, out callback))
			{
				CallbackManager.CallCallback(callback, result);
				this.facebookDelegates.Remove(result.CallbackId);
			}
		}

		private static void CallCallback(object callback, IResult result)
		{
			if (callback == null || result == null)
			{
				return;
			}
			if (CallbackManager.TryCallCallback<IAppRequestResult>(callback, result) || CallbackManager.TryCallCallback<IShareResult>(callback, result) || CallbackManager.TryCallCallback<IGroupCreateResult>(callback, result) || CallbackManager.TryCallCallback<IGroupJoinResult>(callback, result) || CallbackManager.TryCallCallback<IPayResult>(callback, result) || CallbackManager.TryCallCallback<IAppInviteResult>(callback, result) || CallbackManager.TryCallCallback<IAppLinkResult>(callback, result) || CallbackManager.TryCallCallback<ILoginResult>(callback, result) || CallbackManager.TryCallCallback<IAccessTokenRefreshResult>(callback, result))
			{
				return;
			}
			throw new NotSupportedException("Unexpected result type: " + callback.GetType().FullName);
		}

		private static bool TryCallCallback<T>(object callback, IResult result) where T : IResult
		{
			FacebookDelegate<T> facebookDelegate = callback as FacebookDelegate<T>;
			if (facebookDelegate != null)
			{
				facebookDelegate((T)((object)result));
				return true;
			}
			return false;
		}
	}
}
