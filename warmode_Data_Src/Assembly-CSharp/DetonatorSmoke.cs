using System;
using UnityEngine;

[AddComponentMenu("Detonator/Smoke"), RequireComponent(typeof(Detonator))]
public class DetonatorSmoke : DetonatorComponent
{
	private const float _baseSize = 1f;

	private const float _baseDuration = 8f;

	private const float _baseDamping = 0.1300004f;

	private Color _baseColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	private float _scaledDuration;

	private GameObject _smokeA;

	private DetonatorBurstEmitter _smokeAEmitter;

	public Material smokeAMaterial;

	private GameObject _smokeB;

	private DetonatorBurstEmitter _smokeBEmitter;

	public Material smokeBMaterial;

	public bool drawSmokeA = true;

	public bool drawSmokeB = true;

	public override void Init()
	{
		this.FillMaterials(false);
		this.BuildSmokeA();
		this.BuildSmokeB();
	}

	public void FillMaterials(bool wipe)
	{
		if (!this.smokeAMaterial || wipe)
		{
			this.smokeAMaterial = base.MyDetonator().smokeAMaterial;
		}
		if (!this.smokeBMaterial || wipe)
		{
			this.smokeBMaterial = base.MyDetonator().smokeBMaterial;
		}
	}

	public void BuildSmokeA()
	{
		this._smokeA = new GameObject("SmokeA");
		this._smokeAEmitter = this._smokeA.AddComponent<DetonatorBurstEmitter>();
		this._smokeA.transform.parent = base.transform;
		this._smokeA.transform.localPosition = this.localPosition;
		this._smokeA.transform.localRotation = Quaternion.identity;
		this._smokeAEmitter.material = this.smokeAMaterial;
		this._smokeAEmitter.exponentialGrowth = false;
		this._smokeAEmitter.sizeGrow = 0.095f;
		this._smokeAEmitter.useWorldSpace = base.MyDetonator().useWorldSpace;
		this._smokeAEmitter.upwardsBias = base.MyDetonator().upwardsBias;
	}

	public void UpdateSmokeA()
	{
		this._smokeA.transform.localPosition = Vector3.Scale(this.localPosition, new Vector3(this.size, this.size, this.size));
		this._smokeA.transform.LookAt(Camera.main.transform);
		this._smokeA.transform.localPosition = -(Vector3.forward * -1.5f);
		this._smokeAEmitter.color = this.color;
		this._smokeAEmitter.duration = this.duration * 0.5f;
		this._smokeAEmitter.durationVariation = 0f;
		this._smokeAEmitter.timeScale = this.timeScale;
		this._smokeAEmitter.count = 4f;
		this._smokeAEmitter.particleSize = 25f;
		this._smokeAEmitter.sizeVariation = 3f;
		this._smokeAEmitter.velocity = this.velocity;
		this._smokeAEmitter.startRadius = 10f;
		this._smokeAEmitter.size = this.size;
		this._smokeAEmitter.useExplicitColorAnimation = true;
		this._smokeAEmitter.explodeDelayMin = this.explodeDelayMin;
		this._smokeAEmitter.explodeDelayMax = this.explodeDelayMax;
		Color color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color color2 = new Color(0.2f, 0.2f, 0.2f, 0.7f);
		Color color3 = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color color4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		this._smokeAEmitter.colorAnimation[0] = color;
		this._smokeAEmitter.colorAnimation[1] = color2;
		this._smokeAEmitter.colorAnimation[2] = color2;
		this._smokeAEmitter.colorAnimation[3] = color3;
		this._smokeAEmitter.colorAnimation[4] = color4;
	}

	public void BuildSmokeB()
	{
		this._smokeB = new GameObject("SmokeB");
		this._smokeBEmitter = this._smokeB.AddComponent<DetonatorBurstEmitter>();
		this._smokeB.transform.parent = base.transform;
		this._smokeB.transform.localPosition = this.localPosition;
		this._smokeB.transform.localRotation = Quaternion.identity;
		this._smokeBEmitter.material = this.smokeBMaterial;
		this._smokeBEmitter.exponentialGrowth = false;
		this._smokeBEmitter.sizeGrow = 0.095f;
		this._smokeBEmitter.useWorldSpace = base.MyDetonator().useWorldSpace;
		this._smokeBEmitter.upwardsBias = base.MyDetonator().upwardsBias;
	}

	public void UpdateSmokeB()
	{
		this._smokeB.transform.localPosition = Vector3.Scale(this.localPosition, new Vector3(this.size, this.size, this.size));
		this._smokeB.transform.LookAt(Camera.main.transform);
		this._smokeB.transform.localPosition = -(Vector3.forward * -1f);
		this._smokeBEmitter.color = this.color;
		this._smokeBEmitter.duration = this.duration * 0.5f;
		this._smokeBEmitter.durationVariation = 0f;
		this._smokeBEmitter.count = 2f;
		this._smokeBEmitter.particleSize = 25f;
		this._smokeBEmitter.sizeVariation = 3f;
		this._smokeBEmitter.velocity = this.velocity;
		this._smokeBEmitter.startRadius = 10f;
		this._smokeBEmitter.size = this.size;
		this._smokeBEmitter.useExplicitColorAnimation = true;
		this._smokeBEmitter.explodeDelayMin = this.explodeDelayMin;
		this._smokeBEmitter.explodeDelayMax = this.explodeDelayMax;
		Color color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color color2 = new Color(0.2f, 0.2f, 0.2f, 0.7f);
		Color color3 = new Color(0.2f, 0.2f, 0.2f, 0.4f);
		Color color4 = new Color(0.2f, 0.2f, 0.2f, 0f);
		this._smokeBEmitter.colorAnimation[0] = color;
		this._smokeBEmitter.colorAnimation[1] = color2;
		this._smokeBEmitter.colorAnimation[2] = color2;
		this._smokeBEmitter.colorAnimation[3] = color3;
		this._smokeBEmitter.colorAnimation[4] = color4;
	}

	public void Reset()
	{
		this.FillMaterials(true);
		this.on = true;
		this.size = 1f;
		this.duration = 8f;
		this.explodeDelayMin = 0f;
		this.explodeDelayMax = 0f;
		this.color = this._baseColor;
		this.velocity = new Vector3(3f, 3f, 3f);
	}

	public override void Explode()
	{
		if (this.detailThreshold > this.detail)
		{
			return;
		}
		if (this.on)
		{
			this.UpdateSmokeA();
			this.UpdateSmokeB();
			if (this.drawSmokeA)
			{
				this._smokeAEmitter.Explode();
			}
			if (this.drawSmokeB)
			{
				this._smokeBEmitter.Explode();
			}
		}
	}
}
