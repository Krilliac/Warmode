using System;
using UnityEngine;

public abstract class DetonatorComponent : MonoBehaviour
{
	public bool on = true;

	public bool detonatorControlled = true;

	[HideInInspector]
	public float startSize = 1f;

	public float size = 1f;

	public float explodeDelayMin;

	public float explodeDelayMax;

	[HideInInspector]
	public float startDuration = 2f;

	public float duration = 2f;

	[HideInInspector]
	public float timeScale = 1f;

	[HideInInspector]
	public float startDetail = 1f;

	public float detail = 1f;

	[HideInInspector]
	public Color startColor = Color.white;

	public Color color = Color.white;

	[HideInInspector]
	public Vector3 startLocalPosition = Vector3.zero;

	public Vector3 localPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 startForce = Vector3.zero;

	public Vector3 force = Vector3.zero;

	[HideInInspector]
	public Vector3 startVelocity = Vector3.zero;

	public Vector3 velocity = Vector3.zero;

	public float detailThreshold;

	public abstract void Explode();

	public abstract void Init();

	public void SetStartValues()
	{
		this.startSize = this.size;
		this.startForce = this.force;
		this.startVelocity = this.velocity;
		this.startDuration = this.duration;
		this.startDetail = this.detail;
		this.startColor = this.color;
		this.startLocalPosition = this.localPosition;
	}

	public Detonator MyDetonator()
	{
		return base.GetComponent("Detonator") as Detonator;
	}
}
