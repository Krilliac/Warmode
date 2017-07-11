using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class LoadAssetFromBundle : MonoBehaviour
{
	public string baseURL = "ONLINE_URL_HERE";

	private UnityEngine.Object loadedAsset;

	private bool isDone;

	private bool downloadStarted;

	private string assetName;

	private string bundleName;

	private int version;

	private AssetBundle thisAssetBundle;

	private AssetBundleManager assetManager;

	public string AssetName
	{
		get
		{
			return this.assetName;
		}
	}

	public string AssetBundleName
	{
		get
		{
			return this.bundleName;
		}
	}

	public bool HasDownloadStarted
	{
		get
		{
			return this.downloadStarted;
		}
	}

	public bool IsDownloadDone
	{
		get
		{
			return this.isDone;
		}
	}

	public UnityEngine.Object GetDownloadedAsset
	{
		get
		{
			return this.loadedAsset;
		}
	}

	public void QueueBundleDownload(string asset, string bundleName, int version)
	{
		this.downloadStarted = false;
		this.assetName = asset;
		this.bundleName = bundleName;
		this.version = version;
	}

	public void DownloadAsset()
	{
		this.assetManager = AssetBundleManager.Instance;
		AssetBundleContainer assetBundle = this.assetManager.GetAssetBundle(this.bundleName);
		if (assetBundle == null)
		{
			base.StartCoroutine(this.DownloadAssetBundle(this.assetName, this.bundleName, this.version));
		}
		else
		{
			this.loadedAsset = assetBundle.ThisAssetBundle.LoadAsset(this.assetName, typeof(GameObject));
			this.isDone = true;
		}
	}

	[DebuggerHidden]
	private IEnumerator DownloadAssetBundle(string asset, string bundleName, int version)
	{
		LoadAssetFromBundle.<DownloadAssetBundle>c__Iterator18 <DownloadAssetBundle>c__Iterator = new LoadAssetFromBundle.<DownloadAssetBundle>c__Iterator18();
		<DownloadAssetBundle>c__Iterator.bundleName = bundleName;
		<DownloadAssetBundle>c__Iterator.version = version;
		<DownloadAssetBundle>c__Iterator.asset = asset;
		<DownloadAssetBundle>c__Iterator.<$>bundleName = bundleName;
		<DownloadAssetBundle>c__Iterator.<$>version = version;
		<DownloadAssetBundle>c__Iterator.<$>asset = asset;
		<DownloadAssetBundle>c__Iterator.<>f__this = this;
		return <DownloadAssetBundle>c__Iterator;
	}

	public GameObject InstantiateAsset()
	{
		if (this.isDone)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.loadedAsset) as GameObject;
			this.assetManager.AddBundle(this.bundleName, this.thisAssetBundle, gameObject);
			return gameObject;
		}
		UnityEngine.Debug.LogError("Asset is not downloaded!");
		return null;
	}
}
