using System;
using UnityEngine;

public class vp_SimpleCrosshair : MonoBehaviour
{
	public Texture m_ImageCrosshair;

	private void OnGUI()
	{
		if (this.m_ImageCrosshair != null)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.8f);
			GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - (float)this.m_ImageCrosshair.width * 0.5f, (float)Screen.height * 0.5f - (float)this.m_ImageCrosshair.height * 0.5f, (float)this.m_ImageCrosshair.width, (float)this.m_ImageCrosshair.height), this.m_ImageCrosshair);
			GUI.color = Color.white;
		}
	}
}
