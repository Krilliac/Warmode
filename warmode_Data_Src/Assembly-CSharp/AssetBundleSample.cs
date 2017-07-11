using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public sealed class AssetBundleSample : MonoBehaviour
{
	private const string URL = "http://besthttp.azurewebsites.net/Content/AssetBundle.html";

	private string status = "Waiting for user interaction";

	private AssetBundle cachedBundle;

	private Texture2D texture;

	private bool downloading;

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.Label("Status: " + this.status, new GUILayoutOption[0]);
			if (this.texture != null)
			{
				GUILayout.Box(this.texture, new GUILayoutOption[]
				{
					GUILayout.MaxHeight(256f)
				});
			}
			if (!this.downloading && GUILayout.Button("Start Download", new GUILayoutOption[0]))
			{
				this.UnloadBundle();
				base.StartCoroutine(this.DownloadAssetBundle());
			}
		});
	}

	private void OnDestroy()
	{
		this.UnloadBundle();
	}

	[DebuggerHidden]
	private IEnumerator DownloadAssetBundle()
	{
		AssetBundleSample.<DownloadAssetBundle>c__Iterator3 <DownloadAssetBundle>c__Iterator = new AssetBundleSample.<DownloadAssetBundle>c__Iterator3();
		<DownloadAssetBundle>c__Iterator.<>f__this = this;
		return <DownloadAssetBundle>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ProcessAssetBundle(AssetBundle bundle)
	{
		AssetBundleSample.<ProcessAssetBundle>c__Iterator4 <ProcessAssetBundle>c__Iterator = new AssetBundleSample.<ProcessAssetBundle>c__Iterator4();
		<ProcessAssetBundle>c__Iterator.bundle = bundle;
		<ProcessAssetBundle>c__Iterator.<$>bundle = bundle;
		<ProcessAssetBundle>c__Iterator.<>f__this = this;
		return <ProcessAssetBundle>c__Iterator;
	}

	private void UnloadBundle()
	{
		if (this.cachedBundle != null)
		{
			this.cachedBundle.Unload(true);
			this.cachedBundle = null;
		}
	}
}
