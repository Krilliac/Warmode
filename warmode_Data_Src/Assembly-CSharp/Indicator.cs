using System;
using UnityEngine;

public class Indicator : MonoBehaviour
{
	private GameObject goPlayer;

	private Texture2D tIndicator;

	private static float[] time = new float[4];

	private static Vector3[] pos = new Vector3[4];

	private Color a = new Color(1f, 1f, 1f, 0.3f);

	private Vector2 v = Vector2.zero;

	private Rect r;

	public void PostAwake()
	{
		this.goPlayer = GameObject.Find("LocalPlayer");
		this.tIndicator = TEX.GetTextureByName("indicator");
		this.OnResize();
	}

	public void OnResize()
	{
		this.r = new Rect((float)Screen.width / 2f - GUI2.YRES(200f), (float)Screen.height / 2f - GUI2.YRES(200f), GUI2.YRES(400f), GUI2.YRES(400f));
		this.v = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
	}

	public static void SetIndicator(float x, float y, float z)
	{
		int num = 0;
		float num2 = Indicator.time[0];
		for (int i = 1; i < 4; i++)
		{
			if (Indicator.time[i] < num2)
			{
				num2 = Indicator.time[i];
				num = i;
			}
		}
		Indicator.time[num] = Time.time + 1f;
		Indicator.pos[num] = new Vector3(x, y, z);
	}

	private void OnGUI()
	{
		GUI.color = this.a;
		for (int i = 0; i < 4; i++)
		{
			this.DrawIndicator(i);
		}
		GUI.color = Color.white;
	}

	private void DrawIndicator(int i)
	{
		if (Time.time > Indicator.time[i])
		{
			return;
		}
		float angle = Indicator.AngleSigned(Camera.main.transform.forward, Indicator.pos[i] - this.goPlayer.transform.position, this.goPlayer.transform.up);
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(angle, this.v);
		GUI.DrawTexture(this.r, this.tIndicator);
		GUI.matrix = matrix;
	}

	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}
}
