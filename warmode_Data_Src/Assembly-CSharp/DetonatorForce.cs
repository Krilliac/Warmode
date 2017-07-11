using System;
using UnityEngine;

[AddComponentMenu("Detonator/Force"), RequireComponent(typeof(Detonator))]
public class DetonatorForce : DetonatorComponent
{
	private float _baseRadius = 50f;

	private float _basePower = 4000f;

	private float _scaledRange;

	private float _scaledIntensity;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float radius;

	public float power;

	public GameObject fireObject;

	public float fireObjectLife;

	private Collider[] _colliders;

	private GameObject _tempFireObject;

	private Vector3 _explosionPosition;

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
		if (!this.on)
		{
			return;
		}
		if (this.detailThreshold > this.detail)
		{
			return;
		}
		if (!this._delayedExplosionStarted)
		{
			this._explodeDelay = this.explodeDelayMin + UnityEngine.Random.value * (this.explodeDelayMax - this.explodeDelayMin);
		}
		if (this._explodeDelay <= 0f)
		{
			this._explosionPosition = base.transform.position;
			this._colliders = Physics.OverlapSphere(this._explosionPosition, this.radius);
			Collider[] colliders = this._colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				Collider collider = colliders[i];
				if (collider)
				{
					if (collider.GetComponent<Rigidbody>())
					{
						collider.GetComponent<Rigidbody>().AddExplosionForce(this.power * this.size, this._explosionPosition, this.radius * this.size, 4f * base.MyDetonator().upwardsBias * this.size);
						base.SendMessage("OnDetonatorForceHit", null, SendMessageOptions.DontRequireReceiver);
						if (this.fireObject)
						{
							if (collider.transform.Find(this.fireObject.name + "(Clone)"))
							{
								return;
							}
							this._tempFireObject = (UnityEngine.Object.Instantiate(this.fireObject, base.transform.position, base.transform.rotation) as GameObject);
							this._tempFireObject.transform.parent = collider.transform;
							this._tempFireObject.transform.localPosition = new Vector3(0f, 0f, 0f);
							if (this._tempFireObject.GetComponent<ParticleEmitter>())
							{
								this._tempFireObject.GetComponent<ParticleEmitter>().emit = true;
								UnityEngine.Object.Destroy(this._tempFireObject, this.fireObjectLife);
							}
						}
					}
				}
			}
			this._delayedExplosionStarted = false;
			this._explodeDelay = 0f;
		}
		else
		{
			this._delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
		this.radius = this._baseRadius;
		this.power = this._basePower;
	}
}
