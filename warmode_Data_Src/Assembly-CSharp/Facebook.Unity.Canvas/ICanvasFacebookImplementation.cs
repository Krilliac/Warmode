using System;

namespace Facebook.Unity.Canvas
{
	internal interface ICanvasFacebookImplementation : ICanvasFacebook, ICanvasFacebookResultHandler, IFacebook, IFacebookResultHandler, IPayFacebook
	{
	}
}
