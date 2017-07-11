using System;
using UnityEngine;

namespace Facebook.Unity
{
	internal class ComponentFactory
	{
		internal enum IfNotExist
		{
			AddNew,
			ReturnNull
		}

		public const string GameObjectName = "UnityFacebookSDKPlugin";

		private static GameObject facebookGameObject;

		private static GameObject FacebookGameObject
		{
			get
			{
				if (ComponentFactory.facebookGameObject == null)
				{
					ComponentFactory.facebookGameObject = new GameObject("UnityFacebookSDKPlugin");
				}
				return ComponentFactory.facebookGameObject;
			}
		}

		public static T GetComponent<T>(ComponentFactory.IfNotExist ifNotExist = ComponentFactory.IfNotExist.AddNew) where T : MonoBehaviour
		{
			GameObject gameObject = ComponentFactory.FacebookGameObject;
			T t = gameObject.GetComponent<T>();
			if (t == null && ifNotExist == ComponentFactory.IfNotExist.AddNew)
			{
				t = gameObject.AddComponent<T>();
			}
			return t;
		}

		public static T AddComponent<T>() where T : MonoBehaviour
		{
			return ComponentFactory.FacebookGameObject.AddComponent<T>();
		}
	}
}
