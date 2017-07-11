using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_PrototypeMeleeAttack : vp_Component
{
	private vp_FPWeapon m_Weapon;

	private vp_FPController m_Controller;

	private vp_FPCamera m_Camera;

	public string WeaponStateCharge = "Charge";

	public string WeaponStateSwing = "Swing";

	public float SwingDelay = 0.5f;

	public float SwingDuration = 0.5f;

	public float SwingRate = 1f;

	protected float m_NextAllowedSwingTime;

	public int SwingSoftForceFrames = 50;

	public Vector3 SwingPositionSoftForce = new Vector3(-0.5f, -0.1f, 0.3f);

	public Vector3 SwingRotationSoftForce = new Vector3(50f, -25f, 0f);

	public float ImpactTime = 0.11f;

	public Vector3 ImpactPositionSpringRecoil = new Vector3(0.01f, 0.03f, -0.05f);

	public Vector3 ImpactPositionSpring2Recoil = Vector3.zero;

	public Vector3 ImpactRotationSpringRecoil = Vector3.zero;

	public Vector3 ImpactRotationSpring2Recoil = new Vector3(0f, 0f, 10f);

	public string DamageMethodName = "Damage";

	public float Damage = 5f;

	public float DamageRadius = 0.3f;

	public float DamageRange = 2f;

	public float DamageForce = 1000f;

	protected int m_AttackCurrent;

	public float SparkFactor = 0.1f;

	public GameObject m_DustPrefab;

	public GameObject m_SparkPrefab;

	public GameObject m_DebrisPrefab;

	public List<UnityEngine.Object> SoundSwing = new List<UnityEngine.Object>();

	public List<UnityEngine.Object> SoundImpact = new List<UnityEngine.Object>();

	public Vector2 SoundSwingPitch = new Vector2(0.5f, 1.5f);

	public Vector2 SoundImpactPitch = new Vector2(1f, 1.5f);

	private vp_Timer.Handle SwingDelayTimer = new vp_Timer.Handle();

	private vp_Timer.Handle ImpactTimer = new vp_Timer.Handle();

	private vp_Timer.Handle SwingDurationTimer = new vp_Timer.Handle();

	private vp_Timer.Handle ResetTimer = new vp_Timer.Handle();

	private vp_FPPlayerEventHandler m_Player;

	private vp_FPPlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && this.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)this.EventHandler;
			}
			return this.m_Player;
		}
	}

	protected override void Start()
	{
		base.Start();
		this.m_Controller = (vp_FPController)base.Root.GetComponent(typeof(vp_FPController));
		this.m_Camera = (vp_FPCamera)base.Root.GetComponentInChildren(typeof(vp_FPCamera));
		this.m_Weapon = (vp_FPWeapon)base.Transform.GetComponent(typeof(vp_FPWeapon));
	}

	protected override void Update()
	{
		base.Update();
		this.UpdateAttack();
	}

	private void UpdateAttack()
	{
		if (!this.Player.Attack.Active)
		{
			return;
		}
		if (this.Player.SetWeapon.Active)
		{
			return;
		}
		if (this.m_Weapon == null)
		{
			return;
		}
		if (!this.m_Weapon.Wielded)
		{
			return;
		}
		if (Time.time < this.m_NextAllowedSwingTime)
		{
			return;
		}
		this.m_NextAllowedSwingTime = Time.time + this.SwingRate;
		this.PickAttack();
		vp_Timer.In(this.SwingDelay, delegate
		{
			if (this.SoundSwing.Count > 0)
			{
				base.Audio.pitch = UnityEngine.Random.Range(this.SoundSwingPitch.x, this.SoundSwingPitch.y) * Time.timeScale;
				base.Audio.PlayOneShot((AudioClip)this.SoundSwing[UnityEngine.Random.Range(0, this.SoundSwing.Count)]);
			}
			this.m_Weapon.SetState(this.WeaponStateCharge, false, false, false);
			this.m_Weapon.SetState(this.WeaponStateSwing, true, false, false);
			this.m_Weapon.Refresh();
			this.m_Weapon.AddSoftForce(this.SwingPositionSoftForce, this.SwingRotationSoftForce, this.SwingSoftForceFrames);
			vp_Timer.In(this.ImpactTime, delegate
			{
				Ray ray = new Ray(new Vector3(this.m_Controller.Transform.position.x, this.m_Camera.Transform.position.y, this.m_Controller.Transform.position.z), this.m_Camera.Transform.forward);
				RaycastHit hit;
				Physics.SphereCast(ray, this.DamageRadius, out hit, this.DamageRange, vp_Layer.GetBulletBlockers());
				if (hit.collider != null)
				{
					this.SpawnImpactFX(hit);
					this.ApplyDamage(hit);
					this.ApplyRecoil();
				}
				else
				{
					vp_Timer.In(this.SwingDuration - this.ImpactTime, delegate
					{
						this.m_Weapon.StopSprings();
						this.Reset();
					}, this.SwingDurationTimer);
				}
			}, this.ImpactTimer);
		}, this.SwingDelayTimer);
	}

	private void PickAttack()
	{
		int num;
		do
		{
			num = UnityEngine.Random.Range(0, this.States.Count - 1);
		}
		while (this.States.Count > 1 && num == this.m_AttackCurrent && UnityEngine.Random.value < 0.5f);
		this.m_AttackCurrent = num;
		base.SetState(this.States[this.m_AttackCurrent].Name, true, false, false);
		this.m_Weapon.SetState(this.WeaponStateCharge, true, false, false);
		this.m_Weapon.Refresh();
	}

	private void SpawnImpactFX(RaycastHit hit)
	{
		Quaternion rotation = Quaternion.LookRotation(hit.normal);
		if (this.m_DustPrefab != null)
		{
			UnityEngine.Object.Instantiate(this.m_DustPrefab, hit.point, rotation);
		}
		if (this.m_SparkPrefab != null && UnityEngine.Random.value < this.SparkFactor)
		{
			UnityEngine.Object.Instantiate(this.m_SparkPrefab, hit.point, rotation);
		}
		if (this.m_DebrisPrefab != null)
		{
			UnityEngine.Object.Instantiate(this.m_DebrisPrefab, hit.point, rotation);
		}
		if (this.SoundImpact.Count > 0)
		{
			base.Audio.pitch = UnityEngine.Random.Range(this.SoundImpactPitch.x, this.SoundImpactPitch.y) * Time.timeScale;
			base.Audio.PlayOneShot((AudioClip)this.SoundImpact[UnityEngine.Random.Range(0, this.SoundImpact.Count)]);
		}
	}

	private void ApplyDamage(RaycastHit hit)
	{
		hit.collider.SendMessage(this.DamageMethodName, this.Damage, SendMessageOptions.DontRequireReceiver);
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody != null && !attachedRigidbody.isKinematic)
		{
			attachedRigidbody.AddForceAtPosition(this.m_Camera.Transform.forward * this.DamageForce / Time.timeScale, hit.point);
		}
	}

	private void ApplyRecoil()
	{
		this.m_Weapon.StopSprings();
		this.m_Weapon.AddForce(this.ImpactPositionSpringRecoil, this.ImpactRotationSpringRecoil);
		this.m_Weapon.AddForce2(this.ImpactPositionSpring2Recoil, this.ImpactRotationSpring2Recoil);
		this.Reset();
	}

	private void Reset()
	{
		vp_Timer.In(0.05f, delegate
		{
			if (this.m_Weapon != null)
			{
				this.m_Weapon.SetState(this.WeaponStateCharge, false, false, false);
				this.m_Weapon.SetState(this.WeaponStateSwing, false, false, false);
				this.m_Weapon.Refresh();
				base.ResetState();
			}
		}, this.ResetTimer);
	}
}
