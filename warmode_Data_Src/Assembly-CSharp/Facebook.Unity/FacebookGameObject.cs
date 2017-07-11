using System;
using UnityEngine;

namespace Facebook.Unity
{
	internal abstract class FacebookGameObject : MonoBehaviour, IFacebookCallbackHandler
	{
		public IFacebookImplementation Facebook
		{
			get;
			set;
		}

		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			AccessToken.CurrentAccessToken = null;
			this.OnAwake();
		}

		public void OnInitComplete(string message)
		{
			this.Facebook.OnInitComplete(new ResultContainer(message));
		}

		public void OnLoginComplete(string message)
		{
			this.Facebook.OnLoginComplete(new ResultContainer(message));
		}

		public void OnLogoutComplete(string message)
		{
			this.Facebook.OnLogoutComplete(new ResultContainer(message));
		}

		public void OnGetAppLinkComplete(string message)
		{
			this.Facebook.OnGetAppLinkComplete(new ResultContainer(message));
		}

		public void OnGroupCreateComplete(string message)
		{
			this.Facebook.OnGroupCreateComplete(new ResultContainer(message));
		}

		public void OnGroupJoinComplete(string message)
		{
			this.Facebook.OnGroupJoinComplete(new ResultContainer(message));
		}

		public void OnAppRequestsComplete(string message)
		{
			this.Facebook.OnAppRequestsComplete(new ResultContainer(message));
		}

		public void OnShareLinkComplete(string message)
		{
			this.Facebook.OnShareLinkComplete(new ResultContainer(message));
		}

		protected virtual void OnAwake()
		{
		}
	}
}
