using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BloodRotate : MonoBehaviour
{
	private float starttime;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		BloodRotate.<Start>c__Iterator10 <Start>c__Iterator = new BloodRotate.<Start>c__Iterator10();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	private void KillSelf()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		float num = (Time.time - this.starttime) * 2f;
		if (num > 1f)
		{
			num = 1f;
		}
		num = 1f - num;
		base.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, num);
	}
}
