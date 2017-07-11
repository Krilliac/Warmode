using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadBundlesScene : MonoBehaviour
{
	private List<LoadAssetFromBundle> assetsToLoad = new List<LoadAssetFromBundle>();

	private bool isDownloaded = true;

	private string baseURL;

	private string filePrefix = "file://";

	private void Start()
	{
		this.baseURL = Application.dataPath + "/../AssetBundles/";
		if (File.Exists(this.baseURL + "logobundle_01.unity3d"))
		{
			LoadAssetFromBundle loadAssetFromBundle = base.gameObject.AddComponent<LoadAssetFromBundle>();
			loadAssetFromBundle.QueueBundleDownload("pre_cryWolfLogo", "logobundle_01.unity3d", 1);
			loadAssetFromBundle.baseURL = this.filePrefix + this.baseURL;
			LoadAssetFromBundle loadAssetFromBundle2 = base.gameObject.AddComponent<LoadAssetFromBundle>();
			loadAssetFromBundle2.QueueBundleDownload("pre_cryWolfLogo_url", "logobundle_01.unity3d", 1);
			loadAssetFromBundle2.baseURL = this.filePrefix + this.baseURL;
			this.assetsToLoad.Add(loadAssetFromBundle);
			this.assetsToLoad.Add(loadAssetFromBundle2);
		}
		else
		{
			Debug.LogError("Bundles are not built! Open the Bundle Creator in Assets->BundleCreator>Asset Bundle Creator to build your bundles.");
		}
	}

	private void Update()
	{
		if (this.assetsToLoad.Count > 0)
		{
			for (int i = this.assetsToLoad.Count - 1; i >= 0; i--)
			{
				LoadAssetFromBundle loadAssetFromBundle = this.assetsToLoad[i];
				if (loadAssetFromBundle.IsDownloadDone)
				{
					loadAssetFromBundle.InstantiateAsset();
					this.assetsToLoad.RemoveAt(i);
					UnityEngine.Object.Destroy(loadAssetFromBundle);
					this.isDownloaded = true;
				}
			}
			if (this.isDownloaded)
			{
				foreach (LoadAssetFromBundle current in this.assetsToLoad)
				{
					if (!current.HasDownloadStarted)
					{
						current.DownloadAsset();
						this.isDownloaded = false;
						break;
					}
				}
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
