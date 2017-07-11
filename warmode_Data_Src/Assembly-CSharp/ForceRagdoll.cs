using System;
using UnityEngine;

public class ForceRagdoll : MonoBehaviour
{
	public Animator a;

	public bool rd;

	public GameObject go;

	public bool autostart;

	private void Start()
	{
		if (this.autostart)
		{
			this.StartRagDoll();
		}
	}

	private void OnTriggerEnter()
	{
		if (!this.autostart)
		{
			this.StartRagDoll();
		}
	}

	private void OnTriggerStay()
	{
		if (!this.autostart)
		{
			this.StartRagDoll();
		}
	}

	private void StartRagDoll()
	{
		if (this.rd)
		{
			return;
		}
		this.rd = true;
		this.a.enabled = false;
		Rigidbody[] componentsInChildren = this.go.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Rigidbody rigidbody = componentsInChildren[i];
			if (!(rigidbody.gameObject.name == "Bip001 Spine1"))
			{
				if (!(rigidbody.gameObject.name == "Spine_3"))
				{
					rigidbody.isKinematic = false;
				}
			}
		}
	}
}
