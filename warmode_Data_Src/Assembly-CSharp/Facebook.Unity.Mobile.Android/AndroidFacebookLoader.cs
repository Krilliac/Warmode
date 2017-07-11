using System;

namespace Facebook.Unity.Mobile.Android
{
	internal class AndroidFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				AndroidFacebookGameObject component = ComponentFactory.GetComponent<AndroidFacebookGameObject>(ComponentFactory.IfNotExist.AddNew);
				if (component.Facebook == null)
				{
					component.Facebook = new AndroidFacebook();
				}
				return component;
			}
		}
	}
}
