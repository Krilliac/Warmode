using System;
using UnityEngine;

public class FPS : MonoBehaviour
{
	private bool show;

	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		GUI.color = Color.black;
		float num = 1f / Time.deltaTime;
		GUI.Label(new Rect(8f, (float)(Screen.height - 20), 200f, 20f), "FPS = " + num);
		GUI.color = Color.white;
	}
}
