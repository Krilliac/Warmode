using System;

namespace Facebook.Unity.Mobile.IOS
{
	internal class IOSFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				IOSFacebookGameObject component = ComponentFactory.GetComponent<IOSFacebookGameObject>(ComponentFactory.IfNotExist.AddNew);
				if (component.Facebook == null)
				{
					component.Facebook = new IOSFacebook();
				}
				return component;
			}
		}
	}
}
