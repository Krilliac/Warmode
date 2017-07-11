using BestHTTP;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LargeFileDownloadSample : MonoBehaviour
{
	private const string URL = "http://ipv4.download.thinkbroadband.com/100MB.zip";

	private HTTPRequest request;

	private string status = string.Empty;

	private float progress;

	private int fragmentSize = 4096;

	private void Awake()
	{
		if (PlayerPrefs.HasKey("DownloadLength"))
		{
			this.progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
		}
	}

	private void OnDestroy()
	{
		if (this.request != null && this.request.State < HTTPRequestStates.Finished)
		{
			this.request.OnProgress = null;
			this.request.Callback = null;
			this.request.Abort();
		}
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.Label("Request status: " + this.status, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(string.Format("Progress: {0:P2} of {1:N0}Mb", this.progress, PlayerPrefs.GetInt("DownloadLength") / 1048576), new GUILayoutOption[0]);
			GUILayout.HorizontalSlider(this.progress, 0f, 1f, new GUILayoutOption[0]);
			GUILayout.Space(50f);
			if (this.request == null)
			{
				GUILayout.Label(string.Format("Desired Fragment Size: {0:N} KBytes", (float)this.fragmentSize / 1024f), new GUILayoutOption[0]);
				this.fragmentSize = (int)GUILayout.HorizontalSlider((float)this.fragmentSize, 4096f, 1.048576E+07f, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				string text = (!PlayerPrefs.HasKey("DownloadProgress")) ? "Start Download" : "Continue Download";
				if (GUILayout.Button(text, new GUILayoutOption[0]))
				{
					this.StreamLargeFileTest();
				}
			}
			else if (this.request.State == HTTPRequestStates.Processing && GUILayout.Button("Abort Download", new GUILayoutOption[0]))
			{
				this.request.Abort();
			}
		});
	}

	private void StreamLargeFileTest()
	{
		this.request = new HTTPRequest(new Uri("http://ipv4.download.thinkbroadband.com/100MB.zip"), delegate(HTTPRequest req, HTTPResponse resp)
		{
			switch (req.State)
			{
			case HTTPRequestStates.Processing:
				if (!PlayerPrefs.HasKey("DownloadLength"))
				{
					string firstHeaderValue = resp.GetFirstHeaderValue("content-length");
					if (!string.IsNullOrEmpty(firstHeaderValue))
					{
						PlayerPrefs.SetInt("DownloadLength", int.Parse(firstHeaderValue));
					}
				}
				this.ProcessFragments(resp.GetStreamedFragments());
				this.status = "Processing";
				break;
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					this.ProcessFragments(resp.GetStreamedFragments());
					if (resp.IsStreamingFinished)
					{
						this.status = "Streaming finished!";
						PlayerPrefs.DeleteKey("DownloadProgress");
						PlayerPrefs.Save();
						this.request = null;
					}
					else
					{
						this.status = "Processing";
					}
				}
				else
				{
					this.status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText);
					Debug.LogWarning(this.status);
					this.request = null;
				}
				break;
			case HTTPRequestStates.Error:
				this.status = "Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				Debug.LogError(this.status);
				this.request = null;
				break;
			case HTTPRequestStates.Aborted:
				this.status = "Request Aborted!";
				Debug.LogWarning(this.status);
				this.request = null;
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				this.status = "Connection Timed Out!";
				Debug.LogError(this.status);
				this.request = null;
				break;
			case HTTPRequestStates.TimedOut:
				this.status = "Processing the request Timed Out!";
				Debug.LogError(this.status);
				this.request = null;
				break;
			}
		});
		if (PlayerPrefs.HasKey("DownloadProgress"))
		{
			this.request.SetRangeHeader(PlayerPrefs.GetInt("DownloadProgress"));
		}
		else
		{
			PlayerPrefs.SetInt("DownloadProgress", 0);
		}
		this.request.DisableCache = true;
		this.request.UseStreaming = true;
		this.request.StreamFragmentSize = this.fragmentSize;
		this.request.Send();
	}

	private void ProcessFragments(List<byte[]> fragments)
	{
		if (fragments != null && fragments.Count > 0)
		{
			for (int i = 0; i < fragments.Count; i++)
			{
				int value = PlayerPrefs.GetInt("DownloadProgress") + fragments[i].Length;
				PlayerPrefs.SetInt("DownloadProgress", value);
			}
			PlayerPrefs.Save();
			this.progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
		}
	}
}
