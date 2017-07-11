using System;

namespace Facebook.Unity.Editor
{
	internal interface IEditorWrapper
	{
		void Init();

		void ShowLoginMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId, string permissions);

		void ShowAppRequestMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId);

		void ShowGameGroupCreateMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId);

		void ShowGameGroupJoinMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId);

		void ShowAppInviteMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId);

		void ShowPayMockDialog(Utilities.Callback<ResultContainer> callback, string callbackId);

		void ShowMockShareDialog(Utilities.Callback<ResultContainer> callback, string subTitle, string callbackId);
	}
}
