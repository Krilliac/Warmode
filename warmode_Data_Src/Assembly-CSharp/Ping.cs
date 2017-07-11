using System;
using UnityEngine;
using UnityEngine.UI;

public class Ping : MonoBehaviour
{
	public static global::Ping cs;

	public GameObject pingWindow;

	public Text pingText;

	private float tPing;

	private bool show = true;

	public void PostAwake()
	{
		global::Ping.cs = this;
		int num = 0;
		if (PlayerPrefs.HasKey("ShowPing"))
		{
			num = PlayerPrefs.GetInt("ShowPing");
		}
		if (num == 1)
		{
			this.ShowPing(true);
		}
		else
		{
			this.ShowPing(false);
		}
	}

	public void ShowPing(bool val)
	{
		this.show = val;
		this.pingWindow.SetActive(val);
	}

	public void SetPingText(float val)
	{
		if (!this.show)
		{
			return;
		}
		this.pingText.text = "Ping " + (val * 1000f).ToString("#");
	}

	private void Update()
	{
		if (!this.show)
		{
			return;
		}
		if (this.tPing + 0.5f > Time.time)
		{
			return;
		}
		this.tPing = Time.time;
		Client.cs.send_ping();
	}
}
