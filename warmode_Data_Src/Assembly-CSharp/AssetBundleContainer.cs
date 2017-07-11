using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleContainer
{
	private AssetBundle thisAssetBundle;

	private string bundleName;

	private List<GameObject> objectList = new List<GameObject>();

	public AssetBundle ThisAssetBundle
	{
		get
		{
			return this.thisAssetBundle;
		}
		set
		{
			this.thisAssetBundle = value;
		}
	}

	public List<GameObject> ObjectList
	{
		get
		{
			return this.objectList;
		}
	}

	public string BundleName
	{
		get
		{
			return this.bundleName;
		}
		set
		{
			this.bundleName = value;
		}
	}

	public bool IsListEmpty()
	{
		return this.objectList.Count == 0;
	}

	public void ClearEmptyObjects()
	{
		for (int i = this.objectList.Count - 1; i >= 0; i--)
		{
			if (this.objectList[i] == null)
			{
				this.objectList.RemoveAt(i);
			}
		}
	}

	public void Unload()
	{
		Debug.Log(string.Concat(new object[]
		{
			"Objects that holds a reference to ",
			this.bundleName,
			": ",
			this.objectList.Count
		}));
		Debug.Log("Unloading AssetBundle(true):" + this.bundleName);
		this.thisAssetBundle.Unload(true);
	}
}
