using System;
using UnityEngine;

public class Reverb : MonoBehaviour
{
	public AudioReverbFilter localreverb;

	private float lastupdate;

	private void Update()
	{
		if (Time.time < this.lastupdate + 0.5f)
		{
			return;
		}
		this.lastupdate = Time.time;
		Vector3 position = base.transform.position;
		Transform transform = base.transform;
		int num = 0;
		int layerMask = 1;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(position + Vector3.up * 2f, Vector3.up), out raycastHit, 60f, layerMask))
		{
			num++;
		}
		if (Physics.Raycast(new Ray(position + Vector3.up * 2f + transform.forward * 4f + transform.right * 4f, Vector3.up), out raycastHit, 60f, layerMask))
		{
			num++;
		}
		if (Physics.Raycast(new Ray(position + Vector3.up * 2f + transform.forward * 4f + transform.right * -4f, Vector3.up), out raycastHit, 60f, layerMask))
		{
			num++;
		}
		if (Physics.Raycast(new Ray(position + Vector3.up * 2f + transform.right * 4f, Vector3.up), out raycastHit, 60f, layerMask))
		{
			num++;
		}
		if (Physics.Raycast(new Ray(position + Vector3.up * 2f + transform.right * -4f, Vector3.up), out raycastHit, 60f, layerMask))
		{
			num++;
		}
		this.localreverb.reverbLevel = (float)(-10000 + num * 2080);
	}
}
