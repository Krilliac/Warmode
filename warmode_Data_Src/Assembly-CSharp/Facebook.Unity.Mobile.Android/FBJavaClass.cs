using System;

namespace Facebook.Unity.Mobile.Android
{
	internal class FBJavaClass : IAndroidJavaClass
	{
		private class AndroidJNIHelper
		{
			public static bool Debug
			{
				get;
				set;
			}
		}

		private class AndroidJavaClass
		{
			public AndroidJavaClass(string mock)
			{
			}

			public T CallStatic<T>(string method)
			{
				return default(T);
			}

			public void CallStatic(string method, params object[] args)
			{
			}
		}

		private const string FacebookJavaClassName = "com.facebook.unity.FB";

		private FBJavaClass.AndroidJavaClass facebookJavaClass = new FBJavaClass.AndroidJavaClass("com.facebook.unity.FB");

		public T CallStatic<T>(string methodName)
		{
			return this.facebookJavaClass.CallStatic<T>(methodName);
		}

		public void CallStatic(string methodName, params object[] args)
		{
			this.facebookJavaClass.CallStatic(methodName, args);
		}
	}
}
