using System;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
	private float triggertime;

	public Collider col_;

	private void OnTriggerEnter(Collider col)
	{
		this.triggertime = Time.time;
		if (this.col_ != null && this.col_.enabled)
		{
			this.col_.enabled = false;
		}
	}

	private void OnTriggerStay(Collider col)
	{
		this.triggertime = Time.time;
		if (this.col_ != null && this.col_.enabled)
		{
			this.col_.enabled = false;
		}
	}

	private void Update()
	{
		if (Time.time > this.triggertime + 0.2f && this.col_ != null && !this.col_.enabled)
		{
			this.col_.enabled = true;
		}
	}
}
