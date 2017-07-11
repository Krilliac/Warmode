using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	public class FacebookSettings : ScriptableObject
	{
		[Serializable]
		public class UrlSchemes
		{
			[SerializeField]
			private List<string> list;

			public List<string> Schemes
			{
				get
				{
					return this.list;
				}
				set
				{
					this.list = value;
				}
			}

			public UrlSchemes(List<string> schemes = null)
			{
				this.list = ((schemes != null) ? schemes : new List<string>());
			}
		}

		private const string FacebookSettingsAssetName = "FacebookSettings";

		private const string FacebookSettingsPath = "FacebookSDK/SDK/Resources";

		private const string FacebookSettingsAssetExtension = ".asset";

		private static FacebookSettings instance;

		[SerializeField]
		private int selectedAppIndex;

		[SerializeField]
		private List<string> appIds = new List<string>
		{
			"0"
		};

		[SerializeField]
		private List<string> appLabels = new List<string>
		{
			"App Name"
		};

		[SerializeField]
		private bool cookie = true;

		[SerializeField]
		private bool logging = true;

		[SerializeField]
		private bool status = true;

		[SerializeField]
		private bool xfbml;

		[SerializeField]
		private bool frictionlessRequests = true;

		[SerializeField]
		private string iosURLSuffix = string.Empty;

		[SerializeField]
		private List<FacebookSettings.UrlSchemes> appLinkSchemes = new List<FacebookSettings.UrlSchemes>
		{
			new FacebookSettings.UrlSchemes(null)
		};

		public static int SelectedAppIndex
		{
			get
			{
				return FacebookSettings.Instance.selectedAppIndex;
			}
			set
			{
				if (FacebookSettings.Instance.selectedAppIndex != value)
				{
					FacebookSettings.Instance.selectedAppIndex = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static List<string> AppIds
		{
			get
			{
				return FacebookSettings.Instance.appIds;
			}
			set
			{
				if (FacebookSettings.Instance.appIds != value)
				{
					FacebookSettings.Instance.appIds = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static List<string> AppLabels
		{
			get
			{
				return FacebookSettings.Instance.appLabels;
			}
			set
			{
				if (FacebookSettings.Instance.appLabels != value)
				{
					FacebookSettings.Instance.appLabels = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static string AppId
		{
			get
			{
				return FacebookSettings.AppIds[FacebookSettings.SelectedAppIndex];
			}
		}

		public static bool IsValidAppId
		{
			get
			{
				return FacebookSettings.AppId != null && FacebookSettings.AppId.Length > 0 && !FacebookSettings.AppId.Equals("0");
			}
		}

		public static bool Cookie
		{
			get
			{
				return FacebookSettings.Instance.cookie;
			}
			set
			{
				if (FacebookSettings.Instance.cookie != value)
				{
					FacebookSettings.Instance.cookie = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static bool Logging
		{
			get
			{
				return FacebookSettings.Instance.logging;
			}
			set
			{
				if (FacebookSettings.Instance.logging != value)
				{
					FacebookSettings.Instance.logging = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static bool Status
		{
			get
			{
				return FacebookSettings.Instance.status;
			}
			set
			{
				if (FacebookSettings.Instance.status != value)
				{
					FacebookSettings.Instance.status = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static bool Xfbml
		{
			get
			{
				return FacebookSettings.Instance.xfbml;
			}
			set
			{
				if (FacebookSettings.Instance.xfbml != value)
				{
					FacebookSettings.Instance.xfbml = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static string IosURLSuffix
		{
			get
			{
				return FacebookSettings.Instance.iosURLSuffix;
			}
			set
			{
				if (FacebookSettings.Instance.iosURLSuffix != value)
				{
					FacebookSettings.Instance.iosURLSuffix = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static string ChannelUrl
		{
			get
			{
				return "/channel.html";
			}
		}

		public static bool FrictionlessRequests
		{
			get
			{
				return FacebookSettings.Instance.frictionlessRequests;
			}
			set
			{
				if (FacebookSettings.Instance.frictionlessRequests != value)
				{
					FacebookSettings.Instance.frictionlessRequests = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		public static List<FacebookSettings.UrlSchemes> AppLinkSchemes
		{
			get
			{
				return FacebookSettings.Instance.appLinkSchemes;
			}
			set
			{
				if (FacebookSettings.Instance.appLinkSchemes != value)
				{
					FacebookSettings.Instance.appLinkSchemes = value;
					FacebookSettings.DirtyEditor();
				}
			}
		}

		private static FacebookSettings Instance
		{
			get
			{
				if (FacebookSettings.instance == null)
				{
					FacebookSettings.instance = (Resources.Load("FacebookSettings") as FacebookSettings);
					if (FacebookSettings.instance == null)
					{
						FacebookSettings.instance = ScriptableObject.CreateInstance<FacebookSettings>();
					}
				}
				return FacebookSettings.instance;
			}
		}

		public static void SettingsChanged()
		{
			FacebookSettings.DirtyEditor();
		}

		private static void DirtyEditor()
		{
		}
	}
}
