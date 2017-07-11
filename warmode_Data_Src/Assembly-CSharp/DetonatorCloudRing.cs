using System;
using UnityEngine;

[RequireComponent(typeof(Detonator))]
public class DetonatorCloudRing : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 5f;

	private Vector3 _baseVelocity = new Vector3(155f, 5f, 155f);

	private Color _baseColor = Color.white;

	private Vector3 _baseForce = new Vector3(0.162f, 2.56f, 0f);

	private GameObject _cloudRing;

	private DetonatorBurstEmitter _cloudRingEmitter;

	public Material cloudRingMaterial;

	public override void Init()
	{
		this.FillMaterials(false);
		this.BuildCloudRing();
	}

	public void FillMaterials(bool wipe)
	{
		if (!this.cloudRingMaterial || wipe)
		{
			this.cloudRingMaterial = base.MyDetonator().smokeBMaterial;
		}
	}

	public void BuildCloudRing()
	{
		this._cloudRing = new GameObject("CloudRing");
		this._cloudRingEmitter = this._cloudRing.AddComponent<DetonatorBurstEmitter>();
		this._cloudRing.transform.parent = base.transform;
		this._cloudRing.transform.localPosition = this.localPosition;
		this._cloudRingEmitter.material = this.cloudRingMaterial;
		this._cloudRingEmitter.useExplicitColorAnimation = true;
	}

	public void UpdateCloudRing()
	{
		this._cloudRing.transform.localPosition = Vector3.Scale(this.localPosition, new Vector3(this.size, this.size, this.size));
		this._cloudRingEmitter.color = this.color;
		this._cloudRingEmitter.duration = this.duration;
		this._cloudRingEmitter.durationVariation = this.duration / 4f;
		this._cloudRingEmitter.count = (float)((int)(this.detail * 50f));
		this._cloudRingEmitter.particleSize = 10f;
		this._cloudRingEmitter.sizeVariation = 2f;
		this._cloudRingEmitter.velocity = this.velocity;
		this._cloudRingEmitter.startRadius = 3f;
		this._cloudRingEmitter.size = this.size;
		this._cloudRingEmitter.force = this.force;
		this._cloudRingEmitter.explodeDelayMin = this.explodeDelayMin;
		this._cloudRingEmitter.explodeDelayMax = this.explodeDelayMax;
		Color color = Color.Lerp(this.color, new Color(0.2f, 0.2f, 0.2f, 0.6f), 0.5f);
		Color color2 = new Color(0.2f, 0.2f, 0.2f, 0.5f);
		Color color3 = new Color(0.2f, 0.2f, 0.2f, 0.3f);
		Color color4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		this._cloudRingEmitter.colorAnimation[0] = color;
		this._cloudRingEmitter.colorAnimation[1] = color2;
		this._cloudRingEmitter.colorAnimation[2] = color2;
		this._cloudRingEmitter.colorAnimation[3] = color3;
		this._cloudRingEmitter.colorAnimation[4] = color4;
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
			this.UpdateCloudRing();
			this._cloudRingEmitter.Explode();
		}
	}
}
