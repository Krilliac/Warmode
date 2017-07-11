using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity.Editor
{
	internal abstract class EditorFacebookMockDialog : MonoBehaviour
	{
		private Rect modalRect;

		private GUIStyle modalStyle;

		public Utilities.Callback<ResultContainer> Callback
		{
			protected get;
			set;
		}

		public string CallbackID
		{
			protected get;
			set;
		}

		protected abstract string DialogTitle
		{
			get;
		}

		public void Start()
		{
			this.modalRect = new Rect(10f, 10f, (float)(Screen.width - 20), (float)(Screen.height - 20));
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1f));
			texture2D.Apply();
			this.modalStyle = new GUIStyle();
			this.modalStyle.normal.background = texture2D;
		}

		public void OnGUI()
		{
			GUI.ModalWindow(this.GetHashCode(), this.modalRect, new GUI.WindowFunction(this.OnGUIDialog), this.DialogTitle, this.modalStyle);
		}

		protected abstract void DoGui();

		protected abstract void SendSuccessResult();

		protected virtual void SendCancelResult()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["cancelled"] = true;
			if (!string.IsNullOrEmpty(this.CallbackID))
			{
				dictionary["callback_id"] = this.CallbackID;
			}
			this.Callback(new ResultContainer(dictionary.ToJson()));
		}

		protected virtual void SendErrorResult(string errorMessage)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["error"] = errorMessage;
			if (!string.IsNullOrEmpty(this.CallbackID))
			{
				dictionary["callback_id"] = this.CallbackID;
			}
			this.Callback(new ResultContainer(dictionary.ToJson()));
		}

		private void OnGUIDialog(int windowId)
		{
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Warning! Mock dialog responses will NOT match production dialogs", new GUILayoutOption[0]);
			GUILayout.Label("Test your app on one of the supported platforms", new GUILayoutOption[0]);
			this.DoGui();
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUIContent content = new GUIContent("Send Success");
			Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button);
			if (GUI.Button(rect, content))
			{
				this.SendSuccessResult();
				UnityEngine.Object.Destroy(this);
			}
			GUIContent content2 = new GUIContent("Send Cancel");
			Rect rect2 = GUILayoutUtility.GetRect(content2, GUI.skin.button);
			if (GUI.Button(rect2, content2, GUI.skin.button))
			{
				this.SendCancelResult();
				UnityEngine.Object.Destroy(this);
			}
			GUIContent content3 = new GUIContent("Send Error");
			Rect rect3 = GUILayoutUtility.GetRect(content2, GUI.skin.button);
			if (GUI.Button(rect3, content3, GUI.skin.button))
			{
				this.SendErrorResult("Error: Error button pressed");
				UnityEngine.Object.Destroy(this);
			}
			GUILayout.EndHorizontal();
		}
	}
}
