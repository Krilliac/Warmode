using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class ConsoleBase : MonoBehaviour
	{
		private const int DpiScalingFactor = 160;

		private static Stack<string> menuStack = new Stack<string>();

		private string status = "Ready";

		private string lastResponse = string.Empty;

		private Vector2 scrollPosition = Vector2.zero;

		private float? scaleFactor;

		private GUIStyle textStyle;

		private GUIStyle buttonStyle;

		private GUIStyle textInputStyle;

		private GUIStyle labelStyle;

		protected static int ButtonHeight
		{
			get
			{
				return (!Constants.IsMobile) ? 24 : 60;
			}
		}

		protected static int MainWindowWidth
		{
			get
			{
				return (!Constants.IsMobile) ? 700 : (Screen.width - 30);
			}
		}

		protected static int MainWindowFullWidth
		{
			get
			{
				return (!Constants.IsMobile) ? 760 : Screen.width;
			}
		}

		protected static int MarginFix
		{
			get
			{
				return (!Constants.IsMobile) ? 48 : 0;
			}
		}

		protected static Stack<string> MenuStack
		{
			get
			{
				return ConsoleBase.menuStack;
			}
			set
			{
				ConsoleBase.menuStack = value;
			}
		}

		protected string Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		protected Texture2D LastResponseTexture
		{
			get;
			set;
		}

		protected string LastResponse
		{
			get
			{
				return this.lastResponse;
			}
			set
			{
				this.lastResponse = value;
			}
		}

		protected Vector2 ScrollPosition
		{
			get
			{
				return this.scrollPosition;
			}
			set
			{
				this.scrollPosition = value;
			}
		}

		protected float ScaleFactor
		{
			get
			{
				if (!this.scaleFactor.HasValue)
				{
					this.scaleFactor = new float?(Screen.dpi / 160f);
				}
				return this.scaleFactor.Value;
			}
		}

		protected int FontSize
		{
			get
			{
				return (int)Math.Round((double)(this.ScaleFactor * 16f));
			}
		}

		protected GUIStyle TextStyle
		{
			get
			{
				if (this.textStyle == null)
				{
					this.textStyle = new GUIStyle(GUI.skin.textArea);
					this.textStyle.alignment = TextAnchor.UpperLeft;
					this.textStyle.wordWrap = true;
					this.textStyle.padding = new RectOffset(10, 10, 10, 10);
					this.textStyle.stretchHeight = true;
					this.textStyle.stretchWidth = false;
					this.textStyle.fontSize = this.FontSize;
				}
				return this.textStyle;
			}
		}

		protected GUIStyle ButtonStyle
		{
			get
			{
				if (this.buttonStyle == null)
				{
					this.buttonStyle = new GUIStyle(GUI.skin.button);
					this.buttonStyle.fontSize = this.FontSize;
				}
				return this.buttonStyle;
			}
		}

		protected GUIStyle TextInputStyle
		{
			get
			{
				if (this.textInputStyle == null)
				{
					this.textInputStyle = new GUIStyle(GUI.skin.textField);
					this.textInputStyle.fontSize = this.FontSize;
				}
				return this.textInputStyle;
			}
		}

		protected GUIStyle LabelStyle
		{
			get
			{
				if (this.labelStyle == null)
				{
					this.labelStyle = new GUIStyle(GUI.skin.label);
					this.labelStyle.fontSize = this.FontSize;
				}
				return this.labelStyle;
			}
		}

		protected virtual void Awake()
		{
			Application.targetFrameRate = 60;
		}

		protected bool Button(string label)
		{
			return GUILayout.Button(label, this.ButtonStyle, new GUILayoutOption[]
			{
				GUILayout.MinHeight((float)ConsoleBase.ButtonHeight * this.ScaleFactor),
				GUILayout.MaxWidth((float)ConsoleBase.MainWindowWidth)
			});
		}

		protected void LabelAndTextField(string label, ref string text)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, this.LabelStyle, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(200f * this.ScaleFactor)
			});
			text = GUILayout.TextField(text, this.TextInputStyle, new GUILayoutOption[]
			{
				GUILayout.MaxWidth((float)(ConsoleBase.MainWindowWidth - 150))
			});
			GUILayout.EndHorizontal();
		}

		protected bool IsHorizontalLayout()
		{
			return true;
		}

		protected void SwitchMenu(Type menuClass)
		{
			ConsoleBase.menuStack.Push(base.GetType().Name);
			Application.LoadLevel(menuClass.Name);
		}

		protected void GoBack()
		{
			if (ConsoleBase.menuStack.Any<string>())
			{
				Application.LoadLevel(ConsoleBase.menuStack.Pop());
			}
		}
	}
}
