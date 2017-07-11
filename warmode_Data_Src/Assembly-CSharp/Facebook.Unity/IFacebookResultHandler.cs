using System;

namespace Facebook.Unity
{
	internal interface IFacebookResultHandler
	{
		void OnInitComplete(ResultContainer resultContainer);

		void OnLoginComplete(ResultContainer resultContainer);

		void OnLogoutComplete(ResultContainer resultContainer);

		void OnGetAppLinkComplete(ResultContainer resultContainer);

		void OnGroupCreateComplete(ResultContainer resultContainer);

		void OnGroupJoinComplete(ResultContainer resultContainer);

		void OnAppRequestsComplete(ResultContainer resultContainer);

		void OnShareLinkComplete(ResultContainer resultContainer);
	}
}
