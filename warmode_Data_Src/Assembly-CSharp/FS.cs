using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FS : MonoBehaviour
{
	public void ToggleFullscreen()
	{
		int width = Screen.width;
		int height = Screen.height;
		try
		{
			width = Screen.resolutions[Options.resolution].width;
			height = Screen.resolutions[Options.resolution].height;
		}
		catch
		{
			UnityEngine.Debug.Log("Set fullscreen error");
		}
		Screen.SetResolution(width, height, !Screen.fullScreen);
	}

	[DebuggerHidden]
	private IEnumerator WaitForScreenChange(bool fullscreen)
	{
		FS.<WaitForScreenChange>c__Iterator12 <WaitForScreenChange>c__Iterator = new FS.<WaitForScreenChange>c__Iterator12();
		<WaitForScreenChange>c__Iterator.fullscreen = fullscreen;
		<WaitForScreenChange>c__Iterator.<$>fullscreen = fullscreen;
		return <WaitForScreenChange>c__Iterator;
	}
}
