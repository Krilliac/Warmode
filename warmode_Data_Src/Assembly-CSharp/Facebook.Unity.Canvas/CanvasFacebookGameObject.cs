using System;
using UnityEngine;

namespace Facebook.Unity.Canvas
{
	internal class CanvasFacebookGameObject : FacebookGameObject, ICanvasFacebookCallbackHandler, IFacebookCallbackHandler
	{
		protected ICanvasFacebookImplementation CanvasFacebookImpl
		{
			get
			{
				return (ICanvasFacebookImplementation)base.Facebook;
			}
		}

		public void OnPayComplete(string result)
		{
			this.CanvasFacebookImpl.OnPayComplete(new ResultContainer(result));
		}

		public void OnFacebookAuthResponseChange(string message)
		{
			this.CanvasFacebookImpl.OnFacebookAuthResponseChange(new ResultContainer(message));
		}

		public void OnUrlResponse(string message)
		{
			this.CanvasFacebookImpl.OnUrlResponse(message);
		}

		public void OnHideUnity(bool hide)
		{
			this.CanvasFacebookImpl.OnHideUnity(hide);
		}

		protected override void OnAwake()
		{
			GameObject gameObject = new GameObject("FacebookJsBridge");
			gameObject.AddComponent<JsBridge>();
			gameObject.transform.parent = base.gameObject.transform;
		}
	}
}
