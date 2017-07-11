using BestHTTP;
using System;
using UnityEngine;

public sealed class TextureDownloadSample : MonoBehaviour
{
	private const string BaseURL = "http://besthttp.azurewebsites.net/Content/";

	private string[] Images = new string[]
	{
		"One.png",
		"Two.png",
		"Three.png",
		"Four.png",
		"Five.png",
		"Six.png",
		"Seven.png",
		"Eight.png",
		"Nine.png"
	};

	private Texture2D[] Textures = new Texture2D[9];

	private bool allDownloadedFromLocalCache;

	private int finishedCount;

	private Vector2 scrollPos;

	private void Awake()
	{
		HTTPManager.MaxConnectionPerServer = 1;
		for (int i = 0; i < this.Images.Length; i++)
		{
			this.Textures[i] = new Texture2D(100, 150);
		}
	}

	private void OnDestroy()
	{
		HTTPManager.MaxConnectionPerServer = 4;
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
			GUILayout.SelectionGrid(0, this.Textures, 3, new GUILayoutOption[0]);
			if (this.finishedCount == this.Images.Length && this.allDownloadedFromLocalCache)
			{
				GUIHelper.DrawCenteredText("All images loaded from the local cache!");
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Max Connection/Server: ", new GUILayoutOption[]
			{
				GUILayout.Width(150f)
			});
			GUILayout.Label(HTTPManager.MaxConnectionPerServer.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			});
			HTTPManager.MaxConnectionPerServer = (byte)GUILayout.HorizontalSlider((float)HTTPManager.MaxConnectionPerServer, 1f, 10f, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Start Download", new GUILayoutOption[0]))
			{
				this.DownloadImages();
			}
			GUILayout.EndScrollView();
		});
	}

	private void DownloadImages()
	{
		this.allDownloadedFromLocalCache = true;
		this.finishedCount = 0;
		for (int i = 0; i < this.Images.Length; i++)
		{
			this.Textures[i] = new Texture2D(100, 150);
			new HTTPRequest(new Uri("http://besthttp.azurewebsites.net/Content/" + this.Images[i]), new OnRequestFinishedDelegate(this.ImageDownloaded))
			{
				Tag = this.Textures[i]
			}.Send();
		}
	}

	private void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
	{
		this.finishedCount++;
		switch (req.State)
		{
		case HTTPRequestStates.Finished:
			if (resp.IsSuccess)
			{
				Texture2D texture2D = req.Tag as Texture2D;
				texture2D.LoadImage(resp.Data);
				this.allDownloadedFromLocalCache = (this.allDownloadedFromLocalCache && resp.IsFromCache);
			}
			else
			{
				Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText));
			}
			break;
		case HTTPRequestStates.Error:
			Debug.LogError("Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace)));
			break;
		case HTTPRequestStates.Aborted:
			Debug.LogWarning("Request Aborted!");
			break;
		case HTTPRequestStates.ConnectionTimedOut:
			Debug.LogError("Connection Timed Out!");
			break;
		case HTTPRequestStates.TimedOut:
			Debug.LogError("Processing the request Timed Out!");
			break;
		}
	}
}
