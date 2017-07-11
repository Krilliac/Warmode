using System;
using UnityEngine;

public class Main_ : MonoBehaviour
{
	public static Texture2D black;

	public static Texture2D white;

	public static Texture2D yellow;

	private Texture2D warlogo;

	private GUIStyle gui_style;

	private bool show = true;

	private void Awake()
	{
		Application.runInBackground = true;
		Main_.black = new Texture2D(1, 1);
		Main_.black.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
		Main_.black.Apply();
		Main_.white = new Texture2D(1, 1);
		Main_.white.SetPixel(0, 0, new Color(1f, 1f, 1f, 1f));
		Main_.white.Apply();
		Main_.yellow = new Texture2D(1, 1);
		Main_.yellow.SetPixel(0, 0, new Color(1f, 1f, 0f, 1f));
		Main_.yellow.Apply();
		this.warlogo = (Resources.Load("GUI/warlogo") as Texture2D);
		this.gui_style = new GUIStyle();
		this.gui_style.font = (Resources.Load("Fonts/Ubuntu-B") as Font);
		this.gui_style.fontSize = 26;
		this.gui_style.normal.textColor = new Color(255f, 255f, 255f, 255f);
		this.gui_style.alignment = TextAnchor.MiddleRight;
	}

	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		GUI.DrawTexture(new Rect(0f, 8f, (float)Screen.width, 40f), Main_.black);
		GUI.DrawTexture(new Rect((float)(Screen.width - 256 - 8), 12f, 256f, 32f), this.warlogo);
	}

	public void LoadEnd()
	{
		this.show = false;
	}
}
