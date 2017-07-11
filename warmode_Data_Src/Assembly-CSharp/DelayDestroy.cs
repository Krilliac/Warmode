using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
	public float destroytime = 3f;

	private void Start()
	{
		base.StartCoroutine("func");
	}

	[DebuggerHidden]
	private IEnumerator func()
	{
		DelayDestroy.<func>c__Iterator11 <func>c__Iterator = new DelayDestroy.<func>c__Iterator11();
		<func>c__Iterator.<>f__this = this;
		return <func>c__Iterator;
	}
}
