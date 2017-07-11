using System;
using UnityEngine;

[AddComponentMenu("Detonator/Glow"), RequireComponent(typeof(Detonator))]
public class DetonatorGlow : DetonatorComponent
{
	private float _baseSize = 1f;

	private float _baseDuration = 3f;

	private Vector3 _baseVelocity = new Vector3(0f, 0f, 0f);

	private Color _baseColor = Color.black;

	private float _scaledDuration;

	private GameObject _glow;

	private DetonatorBurstEmitter _glowEmitter;

	public Material glowMaterial;

	public override void Init()
	{
		this.FillMaterials(false);
		this.BuildGlow();
	}

	public void FillMaterials(bool wipe)
	{
		if (!this.glowMaterial || wipe)
		{
			this.glowMaterial = base.MyDetonator().glowMaterial;
		}
	}

	public void BuildGlow()
	{
		this._glow = new GameObject("Glow");
		this._glowEmitter = this._glow.AddComponent<DetonatorBurstEmitter>();
		this._glow.transform.parent = base.transform;
		this._glow.transform.localPosition = this.localPosition;
		this._glowEmitter.material = this.glowMaterial;
		this._glowEmitter.exponentialGrowth = false;
		this._glowEmitter.useExplicitColorAnimation = true;
		this._glowEmitter.useWorldSpace = base.MyDetonator().useWorldSpace;
	}

	public void UpdateGlow()
	{
		this._glow.transform.localPosition = Vector3.Scale(this.localPosition, new Vector3(this.size, this.size, this.size));
		this._glowEmitter.color = this.color;
		this._glowEmitter.duration = this.duration;
		this._glowEmitter.timeScale = this.timeScale;
		this._glowEmitter.count = 1f;
		this._glowEmitter.particleSize = 65f;
		this._glowEmitter.sizeVariation = 0f;
		this._glowEmitter.velocity = new Vector3(0f, 0f, 0f);
		this._glowEmitter.startRadius = 0f;
		this._glowEmitter.sizeGrow = 0f;
		this._glowEmitter.size = this.size;
		this._glowEmitter.explodeDelayMin = this.explodeDelayMin;
		this._glowEmitter.explodeDelayMax = this.explodeDelayMax;
		Color color = Color.Lerp(this.color, new Color(0.5f, 0.1f, 0.1f, 1f), 0.5f);
		color.a = 0.9f;
		Color color2 = Color.Lerp(this.color, new Color(0.6f, 0.3f, 0.3f, 1f), 0.5f);
		color2.a = 0.8f;
		Color color3 = Color.Lerp(this.color, new Color(0.7f, 0.3f, 0.3f, 1f), 0.5f);
		color3.a = 0.5f;
		Color color4 = Color.Lerp(this.color, new Color(0.4f, 0.3f, 0.4f, 1f), 0.5f);
		color4.a = 0.2f;
		Color color5 = new Color(0.1f, 0.1f, 0.4f, 0f);
		this._glowEmitter.colorAnimation[0] = color;
		this._glowEmitter.colorAnimation[1] = color2;
		this._glowEmitter.colorAnimation[2] = color3;
		this._glowEmitter.colorAnimation[3] = color4;
		this._glowEmitter.colorAnimation[4] = color5;
	}

	private void Update()
	{
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
	}

	public override void Explode()
	{
		if (this.detailThreshold > this.detail)
		{
			return;
		}
		if (this.on)
		{
			this.UpdateGlow();
			this._glowEmitter.Explode();
		}
	}
}
