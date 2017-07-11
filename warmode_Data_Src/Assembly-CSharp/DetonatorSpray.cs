using System;
using UnityEngine;

[AddComponentMenu("Detonator/Object Spray"), RequireComponent(typeof(Detonator))]
public class DetonatorSpray : DetonatorComponent
{
	public GameObject sprayObject;

	public int count = 10;

	public float startingRadius;

	public float minScale = 1f;

	public float maxScale = 1f;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	private Vector3 _explosionPosition;

	private float _tmpScale;

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
	}

	public override void Explode()
	{
		if (!this._delayedExplosionStarted)
		{
			this._explodeDelay = this.explodeDelayMin + UnityEngine.Random.value * (this.explodeDelayMax - this.explodeDelayMin);
		}
		if (this._explodeDelay <= 0f)
		{
			int num = (int)(this.detail * (float)this.count);
			for (int i = 0; i < num; i++)
			{
				Vector3 b = UnityEngine.Random.onUnitSphere * (this.startingRadius * this.size);
				Vector3 b2 = new Vector3(this.velocity.x * this.size, this.velocity.y * this.size, this.velocity.z * this.size);
				GameObject gameObject = UnityEngine.Object.Instantiate(this.sprayObject, base.transform.position + b, base.transform.rotation) as GameObject;
				gameObject.transform.parent = base.transform;
				this._tmpScale = this.minScale + UnityEngine.Random.value * (this.maxScale - this.minScale);
				this._tmpScale *= this.size;
				gameObject.transform.localScale = new Vector3(this._tmpScale, this._tmpScale, this._tmpScale);
				gameObject.GetComponent<Rigidbody>().velocity = Vector3.Scale(b.normalized, b2);
				UnityEngine.Object.Destroy(gameObject, this.duration * this.timeScale);
				this._delayedExplosionStarted = false;
				this._explodeDelay = 0f;
			}
		}
		else
		{
			this._delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
		this.velocity = new Vector3(15f, 15f, 15f);
	}
}
