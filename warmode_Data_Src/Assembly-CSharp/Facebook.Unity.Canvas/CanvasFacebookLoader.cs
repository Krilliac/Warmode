using System;

namespace Facebook.Unity.Canvas
{
	internal class CanvasFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				CanvasFacebookGameObject component = ComponentFactory.GetComponent<CanvasFacebookGameObject>(ComponentFactory.IfNotExist.AddNew);
				if (component.Facebook == null)
				{
					component.Facebook = new CanvasFacebook();
				}
				return component;
			}
		}
	}
}
