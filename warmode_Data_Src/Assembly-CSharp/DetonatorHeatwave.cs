using System;
using UnityEngine;

[AddComponentMenu("Detonator/Heatwave (Pro Only)"), RequireComponent(typeof(Detonator))]
public class DetonatorHeatwave : DetonatorComponent
{
	private GameObject _heatwave;

	private float s;

	private float _startSize;

	private float _maxSize;

	private float _baseDuration = 0.25f;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float zOffset = 0.5f;

	public float distortion = 64f;

	private float _elapsedTime;

	private float _normalizedTime;

	public Material heatwaveMaterial;

	private Material _material;

	public override void Init()
	{
	}

	private void Update()
	{
		if (this._delayedExplosionStarted)
		{
			this._explodeDelay -= Time.deltaTime;
			if (this._explodeDelay <= 0f)
			{
				this.Explode();
			}
		}
		if (this._heatwave)
		{
			this._heatwave.transform.rotation = Quaternion.FromToRotation(Vector3.up, Camera.main.transform.position - this._heatwave.transform.position);
			this._heatwave.transform.localPosition = this.localPosition + Vector3.forward * this.zOffset;
			this._elapsedTime += Time.deltaTime;
			this._normalizedTime = this._elapsedTime / this.duration;
			this.s = Mathf.Lerp(this._startSize, this._maxSize, this._normalizedTime);
			this._heatwave.GetComponent<Renderer>().material.SetFloat("_BumpAmt", (1f - this._normalizedTime) * this.distortion);
			this._heatwave.gameObject.transform.localScale = new Vector3(this.s, this.s, this.s);
			if (this._elapsedTime > this.duration)
			{
				UnityEngine.Object.Destroy(this._heatwave.gameObject);
			}
		}
	}

	public override void Explode()
	{
		if (SystemInfo.supportsImageEffects && Options.xGrenade > 0)
		{
			if (this.detailThreshold > this.detail || !this.on)
			{
				return;
			}
			if (!this._delayedExplosionStarted)
			{
				this._explodeDelay = this.explodeDelayMin + UnityEngine.Random.value * (this.explodeDelayMax - this.explodeDelayMin);
			}
			if (this._explodeDelay <= 0f)
			{
				this._startSize = 0f;
				this._maxSize = this.size * 10f;
				this._material = new Material(Shader.Find("HeatDistort"));
				this._heatwave = GameObject.CreatePrimitive(PrimitiveType.Plane);
				this._heatwave.layer = 29;
				UnityEngine.Object.Destroy(this._heatwave.GetComponent(typeof(MeshCollider)));
				if (!this.heatwaveMaterial)
				{
					this.heatwaveMaterial = base.MyDetonator().heatwaveMaterial;
				}
				this._material.CopyPropertiesFromMaterial(this.heatwaveMaterial);
				this._heatwave.GetComponent<Renderer>().material = this._material;
				this._heatwave.transform.parent = base.transform;
				this._delayedExplosionStarted = false;
				this._explodeDelay = 0f;
			}
			else
			{
				this._delayedExplosionStarted = true;
			}
		}
	}

	public void Reset()
	{
		this.duration = this._baseDuration;
	}
}
