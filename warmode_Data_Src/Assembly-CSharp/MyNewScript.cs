using System;
using System.Collections.Generic;
using UnityEngine;

public class MyNewScript : MonoBehaviour
{
	private List<LoadAssetFromBundle> assetsToLoad = new List<LoadAssetFromBundle>();

	private bool isDownloaded = true;

	private string baseURL;

	private string filePrefix = "file://";

	private void Start()
	{
		this.baseURL = this.filePrefix + Application.dataPath + PlayerPrefs.GetString("cws_exportFolder");
		LoadAssetFromBundle loadAssetFromBundle = base.gameObject.AddComponent<LoadAssetFromBundle>();
		loadAssetFromBundle.QueueBundleDownload("MY_ASSET_NAME", "MY_BUNDLE_NAME.unity3d", 1);
		loadAssetFromBundle.baseURL = this.baseURL;
		this.assetsToLoad.Add(loadAssetFromBundle);
	}

	private void Update()
	{
	}
}
