using System;

namespace Facebook.Unity.Canvas
{
	internal interface ICanvasFacebookResultHandler : IFacebookResultHandler
	{
		void OnPayComplete(ResultContainer resultContainer);

		void OnFacebookAuthResponseChange(ResultContainer resultContainer);

		void OnUrlResponse(string message);

		void OnHideUnity(bool hide);
	}
}
