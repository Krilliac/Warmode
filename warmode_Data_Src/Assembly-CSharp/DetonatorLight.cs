using System;
using UnityEngine;

[AddComponentMenu("Detonator/Light"), RequireComponent(typeof(Detonator))]
public class DetonatorLight : DetonatorComponent
{
	private float _baseIntensity = 1f;

	private Color _baseColor = Color.white;

	private float _scaledDuration;

	private float _explodeTime = -1000f;

	private GameObject _light;

	private Light _lightComponent;

	public float intensity;

	private float _reduceAmount;

	public override void Init()
	{
		this._light = new GameObject("Light");
		this._light.transform.parent = base.transform;
		this._light.transform.localPosition = this.localPosition;
		this._lightComponent = this._light.AddComponent<Light>();
		this._lightComponent.type = LightType.Point;
		this._lightComponent.enabled = false;
	}

	private void Update()
	{
		if (this._explodeTime + this._scaledDuration > Time.time && this._lightComponent.intensity > 0f)
		{
			this._reduceAmount = this.intensity * (Time.deltaTime / this._scaledDuration);
			this._lightComponent.intensity -= this._reduceAmount;
		}
		else if (this._lightComponent)
		{
			this._lightComponent.enabled = false;
		}
	}

	public override void Explode()
	{
		if (this.detailThreshold > this.detail)
		{
			return;
		}
		this._lightComponent.color = this.color;
		this._lightComponent.range = this.size;
		this._scaledDuration = this.duration * this.timeScale;
		this._lightComponent.enabled = true;
		this._lightComponent.intensity = this.intensity;
		this._explodeTime = Time.time;
	}

	public void Reset()
	{
		this.color = this._baseColor;
		this.intensity = this._baseIntensity;
	}
}
