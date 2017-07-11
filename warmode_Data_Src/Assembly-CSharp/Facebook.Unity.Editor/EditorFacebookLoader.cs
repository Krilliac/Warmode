using System;

namespace Facebook.Unity.Editor
{
	internal class EditorFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				EditorFacebookGameObject component = ComponentFactory.GetComponent<EditorFacebookGameObject>(ComponentFactory.IfNotExist.AddNew);
				component.Facebook = new EditorFacebook();
				return component;
			}
		}
	}
}
