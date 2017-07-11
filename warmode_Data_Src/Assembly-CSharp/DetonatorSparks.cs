using System;
using UnityEngine;

[AddComponentMenu("Detonator/Sparks"), RequireComponent(typeof(Detonator))]
public class DetonatorSparks : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 4f;

	private Vector3 _baseVelocity = new Vector3(155f, 155f, 155f);

	private Color _baseColor = Color.white;

	private Vector3 _baseForce = Physics.gravity;

	private float _scaledDuration;

	private GameObject _sparks;

	private DetonatorBurstEmitter _sparksEmitter;

	public Material sparksMaterial;

	public override void Init()
	{
		this.FillMaterials(false);
		this.BuildSparks();
	}

	public void FillMaterials(bool wipe)
	{
		if (!this.sparksMaterial || wipe)
		{
			this.sparksMaterial = base.MyDetonator().sparksMaterial;
		}
	}

	public void BuildSparks()
	{
		this._sparks = new GameObject("Sparks");
		this._sparksEmitter = this._sparks.AddComponent<DetonatorBurstEmitter>();
		this._sparks.transform.parent = base.transform;
		this._sparks.transform.localPosition = this.localPosition;
		this._sparks.transform.localRotation = Quaternion.identity;
		this._sparksEmitter.material = this.sparksMaterial;
		this._sparksEmitter.force = Physics.gravity;
		this._sparksEmitter.useExplicitColorAnimation = false;
		this._sparksEmitter.useWorldSpace = base.MyDetonator().useWorldSpace;
		this._sparksEmitter.upwardsBias = base.MyDetonator().upwardsBias;
	}

	public void UpdateSparks()
	{
		this._scaledDuration = this.duration * this.timeScale;
		this._sparksEmitter.color = this.color;
		this._sparksEmitter.duration = this._scaledDuration / 4f;
		this._sparksEmitter.durationVariation = this._scaledDuration;
		this._sparksEmitter.count = (float)((int)(this.detail * 50f));
		this._sparksEmitter.particleSize = 0.5f;
		this._sparksEmitter.sizeVariation = 0.25f;
		if (this._sparksEmitter.upwardsBias > 0f)
		{
			this._sparksEmitter.velocity = new Vector3(this.velocity.x / Mathf.Log(this._sparksEmitter.upwardsBias), this.velocity.y * Mathf.Log(this._sparksEmitter.upwardsBias), this.velocity.z / Mathf.Log(this._sparksEmitter.upwardsBias));
		}
		else
		{
			this._sparksEmitter.velocity = this.velocity;
		}
		this._sparksEmitter.startRadius = 0f;
		this._sparksEmitter.size = this.size;
		this._sparksEmitter.explodeDelayMin = this.explodeDelayMin;
		this._sparksEmitter.explodeDelayMax = this.explodeDelayMax;
	}

	public void Reset()
	{
		this.FillMaterials(true);
		this.on = true;
		this.size = this._baseSize;
		this.duration = this._baseDuration;
		this.explodeDelayMin = 0f;
		this.explodeDelayMax = 0f;
		this.color = this._baseColor;
		this.velocity = this._baseVelocity;
		this.force = this._baseForce;
	}

	public override void Explode()
	{
		if (this.on)
		{
			this.UpdateSparks();
			this._sparksEmitter.Explode();
		}
	}
}
