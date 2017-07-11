using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private static AssetBundleManager instance;

	private Dictionary<string, AssetBundleContainer> assetBundles = new Dictionary<string, AssetBundleContainer>();

	public static AssetBundleManager Instance
	{
		get
		{
			if (AssetBundleManager.instance == null)
			{
				Debug.Log("Creating an AssetBundle manager instance");
				GameObject gameObject = new GameObject();
				AssetBundleManager.instance = gameObject.AddComponent<AssetBundleManager>();
				gameObject.name = "AssetBundleManager";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return AssetBundleManager.instance;
		}
	}

	private void Start()
	{
		if (AssetBundleManager.instance == null)
		{
			AssetBundleManager.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		base.InvokeRepeating("CheckForUnusedBundles", 5f, 5f);
	}

	private void CheckForUnusedBundles()
	{
		if (this.assetBundles.Count > 0)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, AssetBundleContainer> current in this.assetBundles)
			{
				current.Value.ClearEmptyObjects();
				if (current.Value.IsListEmpty())
				{
					current.Value.Unload();
					list.Add(current.Key);
				}
			}
			foreach (string current2 in list)
			{
				this.assetBundles.Remove(current2);
			}
		}
	}

	public void AddBundle(string bundleName, AssetBundle assetBundle, GameObject instantiatedObject)
	{
		if (!this.assetBundles.ContainsKey(bundleName))
		{
			AssetBundleContainer assetBundleContainer = new AssetBundleContainer();
			assetBundleContainer.ThisAssetBundle = assetBundle;
			assetBundleContainer.ObjectList.Add(instantiatedObject);
			assetBundleContainer.BundleName = bundleName;
			this.assetBundles.Add(bundleName, assetBundleContainer);
		}
		else
		{
			AssetBundleContainer assetBundleContainer2 = null;
			this.assetBundles.TryGetValue(bundleName, out assetBundleContainer2);
			if (assetBundleContainer2 != null)
			{
				assetBundleContainer2.ObjectList.Add(instantiatedObject);
			}
			else
			{
				Debug.LogError(string.Concat(new string[]
				{
					"AssetBundleManager.cs: Couldn't get the container for assetbundle: ",
					bundleName,
					". Removal Management for object:",
					instantiatedObject.name,
					" will not work"
				}));
			}
		}
	}

	public AssetBundleContainer GetAssetBundle(string bundleName)
	{
		AssetBundleContainer result = null;
		this.assetBundles.TryGetValue(bundleName, out result);
		return result;
	}

	public void DestroyAssetBundle(string bundleName)
	{
		AssetBundleContainer assetBundleContainer = null;
		this.assetBundles.TryGetValue(bundleName, out assetBundleContainer);
		if (assetBundleContainer != null)
		{
			foreach (GameObject current in assetBundleContainer.ObjectList)
			{
				if (current != null)
				{
					UnityEngine.Object.Destroy(current);
				}
			}
			assetBundleContainer.ObjectList.Clear();
			assetBundleContainer.Unload();
			this.assetBundles.Remove(bundleName);
		}
	}

	public void DestroyAllBundles()
	{
		foreach (KeyValuePair<string, AssetBundleContainer> current in this.assetBundles)
		{
			foreach (GameObject current2 in current.Value.ObjectList)
			{
				if (current2 != null)
				{
					UnityEngine.Object.Destroy(current2);
				}
			}
			current.Value.ObjectList.Clear();
			current.Value.Unload();
		}
		this.assetBundles.Clear();
	}
}
