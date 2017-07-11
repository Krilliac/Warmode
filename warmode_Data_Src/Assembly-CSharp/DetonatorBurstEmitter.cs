using System;
using UnityEngine;

public class DetonatorBurstEmitter : DetonatorComponent
{
	private ParticleEmitter _particleEmitter;

	private ParticleRenderer _particleRenderer;

	private ParticleAnimator _particleAnimator;

	private float _baseDamping = 0.1300004f;

	private float _baseSize = 1f;

	private Color _baseColor = Color.white;

	public float damping = 1f;

	public float startRadius = 1f;

	public float maxScreenSize = 2f;

	public bool explodeOnAwake;

	public bool oneShot = true;

	public float sizeVariation;

	public float particleSize = 1f;

	public float count = 1f;

	public float sizeGrow = 20f;

	public bool exponentialGrowth = true;

	public float durationVariation;

	public bool useWorldSpace = true;

	public float upwardsBias;

	public float angularVelocity = 20f;

	public bool randomRotation = true;

	public ParticleRenderMode renderMode;

	public bool useExplicitColorAnimation;

	public Color[] colorAnimation = new Color[5];

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public Material material;

	private float _emitTime;

	private float speed = 3f;

	private float initFraction = 0.1f;

	private static float epsilon = 0.01f;

	private float _tmpParticleSize;

	private Vector3 _tmpPos;

	private Vector3 _tmpDir;

	private Vector3 _thisPos;

	private float _tmpDuration;

	private float _tmpCount;

	private float _scaledDuration;

	private float _scaledDurationVariation;

	private float _scaledStartRadius;

	private float _scaledColor;

	private float _randomizedRotation;

	private float _tmpAngularVelocity;

	public override void Init()
	{
		MonoBehaviour.print("UNUSED");
	}

	public void Awake()
	{
		this._particleEmitter = base.gameObject.AddComponent<EllipsoidParticleEmitter>();
		this._particleRenderer = base.gameObject.AddComponent<ParticleRenderer>();
		this._particleAnimator = base.gameObject.AddComponent<ParticleAnimator>();
		this._particleEmitter.hideFlags = HideFlags.HideAndDontSave;
		this._particleRenderer.hideFlags = HideFlags.HideAndDontSave;
		this._particleAnimator.hideFlags = HideFlags.HideAndDontSave;
		this._particleAnimator.damping = this._baseDamping;
		this._particleEmitter.emit = false;
		this._particleRenderer.maxParticleSize = this.maxScreenSize;
		this._particleRenderer.material = this.material;
		this._particleRenderer.material.color = Color.white;
		this._particleAnimator.sizeGrow = this.sizeGrow;
		if (this.explodeOnAwake)
		{
			this.Explode();
		}
	}

	private void Update()
	{
		if (this.exponentialGrowth)
		{
			float num = Time.time - this._emitTime;
			float num2 = this.SizeFunction(num - DetonatorBurstEmitter.epsilon);
			float num3 = this.SizeFunction(num);
			float num4 = (num3 / num2 - 1f) / DetonatorBurstEmitter.epsilon;
			this._particleAnimator.sizeGrow = num4;
		}
		else
		{
			this._particleAnimator.sizeGrow = this.sizeGrow;
		}
		if (this._delayedExplosionStarted)
		{
			this._explodeDelay -= Time.deltaTime;
			if (this._explodeDelay <= 0f)
			{
				this.Explode();
			}
		}
	}

	private float SizeFunction(float elapsedTime)
	{
		float num = 1f - 1f / (1f + elapsedTime * this.speed);
		return this.initFraction + (1f - this.initFraction) * num;
	}

	public void Reset()
	{
		this.size = this._baseSize;
		this.color = this._baseColor;
		this.damping = this._baseDamping;
	}

	public override void Explode()
	{
		if (this.on)
		{
			this._particleEmitter.useWorldSpace = this.useWorldSpace;
			this._scaledDuration = this.timeScale * this.duration;
			this._scaledDurationVariation = this.timeScale * this.durationVariation;
			this._scaledStartRadius = this.size * this.startRadius;
			this._particleRenderer.particleRenderMode = this.renderMode;
			if (!this._delayedExplosionStarted)
			{
				this._explodeDelay = this.explodeDelayMin + UnityEngine.Random.value * (this.explodeDelayMax - this.explodeDelayMin);
			}
			if (this._explodeDelay <= 0f)
			{
				Color[] array = this._particleAnimator.colorAnimation;
				if (this.useExplicitColorAnimation)
				{
					array[0] = this.colorAnimation[0];
					array[1] = this.colorAnimation[1];
					array[2] = this.colorAnimation[2];
					array[3] = this.colorAnimation[3];
					array[4] = this.colorAnimation[4];
				}
				else
				{
					array[0] = new Color(this.color.r, this.color.g, this.color.b, this.color.a * 0.7f);
					array[1] = new Color(this.color.r, this.color.g, this.color.b, this.color.a * 1f);
					array[2] = new Color(this.color.r, this.color.g, this.color.b, this.color.a * 0.5f);
					array[3] = new Color(this.color.r, this.color.g, this.color.b, this.color.a * 0.3f);
					array[4] = new Color(this.color.r, this.color.g, this.color.b, this.color.a * 0f);
				}
				this._particleAnimator.colorAnimation = array;
				this._particleRenderer.material = this.material;
				this._particleAnimator.force = this.force;
				this._tmpCount = this.count * this.detail;
				if (this._tmpCount < 1f)
				{
					this._tmpCount = 1f;
				}
				if (this._particleEmitter.useWorldSpace)
				{
					this._thisPos = base.gameObject.transform.position;
				}
				else
				{
					this._thisPos = new Vector3(0f, 0f, 0f);
				}
				int num = 1;
				while ((float)num <= this._tmpCount)
				{
					this._tmpPos = Vector3.Scale(UnityEngine.Random.insideUnitSphere, new Vector3(this._scaledStartRadius, this._scaledStartRadius, this._scaledStartRadius));
					this._tmpPos = this._thisPos + this._tmpPos;
					this._tmpDir = Vector3.Scale(UnityEngine.Random.insideUnitSphere, new Vector3(this.velocity.x, this.velocity.y, this.velocity.z));
					this._tmpDir.y = this._tmpDir.y + 2f * (Mathf.Abs(this._tmpDir.y) * this.upwardsBias);
					if (this.randomRotation)
					{
						this._randomizedRotation = UnityEngine.Random.Range(-1f, 1f);
						this._tmpAngularVelocity = UnityEngine.Random.Range(-1f, 1f) * this.angularVelocity;
					}
					else
					{
						this._randomizedRotation = 0f;
						this._tmpAngularVelocity = this.angularVelocity;
					}
					this._tmpDir = Vector3.Scale(this._tmpDir, new Vector3(this.size, this.size, this.size));
					this._tmpParticleSize = this.size * (this.particleSize + UnityEngine.Random.value * this.sizeVariation);
					this._tmpDuration = this._scaledDuration + UnityEngine.Random.value * this._scaledDurationVariation;
					this._particleEmitter.Emit(this._tmpPos, this._tmpDir, this._tmpParticleSize, this._tmpDuration, this.color, this._randomizedRotation, this._tmpAngularVelocity);
					num++;
				}
				this._emitTime = Time.time;
				this._delayedExplosionStarted = false;
				this._explodeDelay = 0f;
			}
			else
			{
				this._delayedExplosionStarted = true;
			}
		}
	}
}
