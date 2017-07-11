using BestHTTP;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Statistics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SampleSelector : MonoBehaviour
{
	public const int statisticsHeight = 160;

	private List<SampleDescriptor> Samples = new List<SampleDescriptor>();

	public static SampleDescriptor SelectedSample;

	private Vector2 scrollPos;

	private void Awake()
	{
		this.Samples.Add(new SampleDescriptor(typeof(TextureDownloadSample), "Texture Download", "With HTTPManager.MaxConnectionPerServer you can control how many requests can be processed per server parallel.\n\nFeatures demoed in this example:\n-Parallel requests to the same server\n-Controlling the parallelization\n-Automatic Caching\n-Create a Texture2D from the downloaded data", CodeBlocks.TextureDownloadSample));
		this.Samples.Add(new SampleDescriptor(typeof(AssetBundleSample), "AssetBundle Download", "A small example that shows a possible way to download an AssetBundle and load a resource from it.\n\nFeatures demoed in this example:\n-Using HTTPRequest without a callback\n-Using HTTPRequest in a Coroutine\n-Loading an AssetBundle from the downloaded bytes\n-Automatic Caching", CodeBlocks.AssetBundleSample));
		this.Samples.Add(new SampleDescriptor(typeof(LargeFileDownloadSample), "Large File Download", "This example demonstrates how you can download a (large) file and continue the download after the connection is aborted.\n\nFeatures demoed in this example:\n-Setting up a streamed download\n-How to access the downloaded data while the download is in progress\n-Setting the HTTPRequest's StreamFragmentSize to controll the frequency and size of the fragments\n-How to use the SetRangeHeader to continue a previously disconnected download\n-How to disable the local, automatic caching", CodeBlocks.LargeFileDownloadSample));
		this.Samples.Add(new SampleDescriptor(typeof(WebSocketSample), "WebSocket - Echo", "A WebSocket demonstration that connects to a WebSocket echo service.\n\nFeatures demoed in this example:\n-Basic useage of the WebSocket class", CodeBlocks.WebSocketSample));
		this.Samples.Add(new SampleDescriptor(typeof(SocketIOChatSample), "Socket.IO - Chat", "This example uses the Socket.IO implementation to connect to the official Chat demo server(http://chat.socket.io/).\n\nFeatures demoed in this example:\n-Instantiating and setting up a SocketManager to connect to a Socket.IO server\n-Changing SocketOptions property\n-Subscribing to Socket.IO events\n-Sending custom events to the server", CodeBlocks.SocketIOChatSample));
		this.Samples.Add(new SampleDescriptor(typeof(SocketIOWePlaySample), "Socket.IO - WePlay", "This example uses the Socket.IO implementation to connect to the official WePlay demo server(http://weplay.io/).\n\nFeatures demoed in this example:\n-Instantiating and setting up a SocketManager to connect to a Socket.IO server\n-Subscribing to Socket.IO events\n-Receiving binary data\n-How to load a texture from the received binary data\n-How to disable payload decoding for fine tune for some speed\n-Sending custom events to the server", CodeBlocks.SocketIOWePlaySample));
		this.Samples.Add(new SampleDescriptor(typeof(CacheMaintenanceSample), "Cache Maintenance", "With this demo you can see how you can use the HTTPCacheService's BeginMaintainence function to delete too old cached entities and keep the cache size under a specified value.\n\nFeatures demoed in this example:\n-How to set up a HTTPCacheMaintananceParams\n-How to call the BeginMaintainence function", CodeBlocks.CacheMaintenanceSample));
		SampleSelector.SelectedSample = this.Samples[0];
	}

	private void Start()
	{
		GUIHelper.ClientArea = new Rect(0f, 165f, (float)Screen.width, (float)(Screen.height - 160 - 50));
	}

	private void OnGUI()
	{
		GeneralStatistics stats = HTTPManager.GetGeneralStatistics(StatisticsQueryFlags.All);
		GUIHelper.DrawArea(new Rect(0f, 0f, (float)(Screen.width / 3), 160f), false, delegate
		{
			GUIHelper.DrawCenteredText("Connections");
			GUILayout.Space(5f);
			GUIHelper.DrawRow("Active Connections:", stats.ActiveConnections.ToString());
			GUIHelper.DrawRow("Free Connections:", stats.FreeConnections.ToString());
			GUIHelper.DrawRow("Recycled Connections:", stats.RecycledConnections.ToString());
			GUIHelper.DrawRow("Requests in queue:", stats.RequestsInQueue.ToString());
		});
		GUIHelper.DrawArea(new Rect((float)(Screen.width / 3), 0f, (float)(Screen.width / 3), 160f), false, delegate
		{
			GUIHelper.DrawCenteredText("Cache");
			GUILayout.Space(5f);
			GUIHelper.DrawRow("Cached entities:", stats.CacheEntityCount.ToString());
			GUIHelper.DrawRow("Sum Size (bytes): ", stats.CacheSize.ToString("N0"));
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear Cache", new GUILayoutOption[0]))
			{
				HTTPCacheService.BeginClear();
			}
			GUILayout.EndVertical();
		});
		GUIHelper.DrawArea(new Rect((float)(Screen.width / 3 * 2), 0f, (float)(Screen.width / 3), 160f), false, delegate
		{
			GUIHelper.DrawCenteredText("Cookies");
			GUILayout.Space(5f);
			GUIHelper.DrawRow("Cookies:", stats.CookieCount.ToString());
			GUIHelper.DrawRow("Estimated size (bytes):", stats.CookieJarSize.ToString("N0"));
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear Cookies", new GUILayoutOption[0]))
			{
				CookieJar.Clear();
			}
			GUILayout.EndVertical();
		});
		if (SampleSelector.SelectedSample == null || (SampleSelector.SelectedSample != null && !SampleSelector.SelectedSample.IsRunning))
		{
			GUIHelper.DrawArea(new Rect(0f, 165f, (float)((SampleSelector.SelectedSample != null) ? (Screen.width / 3) : Screen.width), (float)(Screen.height - 160 - 5)), false, delegate
			{
				this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
				for (int i = 0; i < this.Samples.Count; i++)
				{
					this.DrawSample(this.Samples[i]);
				}
				GUILayout.EndScrollView();
			});
			if (SampleSelector.SelectedSample != null)
			{
				this.DrawSampleDetails(SampleSelector.SelectedSample);
			}
		}
		else if (SampleSelector.SelectedSample != null && SampleSelector.SelectedSample.IsRunning)
		{
			GUILayout.BeginArea(new Rect(0f, (float)(Screen.height - 50), (float)Screen.width, 50f), string.Empty);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Back", new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				SampleSelector.SelectedSample.DestroyUnityObject();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	private void DrawSample(SampleDescriptor sample)
	{
		if (GUILayout.Button(sample.DisplayName, new GUILayoutOption[0]))
		{
			sample.IsSelected = true;
			if (SampleSelector.SelectedSample != null)
			{
				SampleSelector.SelectedSample.IsSelected = false;
			}
			SampleSelector.SelectedSample = sample;
		}
	}

	private void DrawSampleDetails(SampleDescriptor sample)
	{
		Rect rect = new Rect((float)(Screen.width / 3), 165f, (float)(Screen.width / 3 * 2), (float)(Screen.height - 160 - 5));
		GUI.Box(rect, string.Empty);
		GUILayout.BeginArea(rect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUIHelper.DrawCenteredText(sample.DisplayName);
		GUILayout.Space(5f);
		GUILayout.Label(sample.Description, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start", new GUILayoutOption[0]))
		{
			sample.CreateUnityObject();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
