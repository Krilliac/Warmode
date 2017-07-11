using System;
using UnityEngine;

[RequireComponent(typeof(vp_FPWeapon))]
public class vp_FPWeaponShooter : vp_Shooter
{
	protected vp_FPWeapon m_FPSWeapon;

	protected vp_FPCamera m_FPSCamera;

	public float ProjectileTapFiringRate = 0.1f;

	protected float m_LastFireTime;

	public Vector3 MotionPositionRecoil = new Vector3(0f, 0f, -0.035f);

	public Vector3 MotionRotationRecoil = new Vector3(-10f, 0f, 0f);

	public float MotionRotationRecoilDeadZone = 0.5f;

	public float MotionPositionReset = 0.5f;

	public float MotionRotationReset = 0.5f;

	public float MotionPositionPause = 1f;

	public float MotionRotationPause = 1f;

	public float MotionDryFireRecoil = -0.1f;

	public float MotionRecoilDelay;

	public string AnimationFire;

	public string SoundDryFire;

	public AudioClip sndSoundDryFire;

	protected AudioSource as_dryfire;

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

	protected override void Awake()
	{
		base.Awake();
		this.m_FPSCamera = base.transform.root.GetComponentInChildren<vp_FPCamera>();
		this.m_OperatorTransform = this.m_FPSCamera.transform;
		this.m_NextAllowedFireTime = Time.time;
		this.ProjectileSpawnDelay = Mathf.Min(this.ProjectileSpawnDelay, this.ProjectileFiringRate - 0.1f);
		this.as_dryfire = base.Parent.GetComponent<AudioSource>();
		if (this.sndSoundDryFire == null)
		{
			this.sndSoundDryFire = SND.GetSoundByName(this.SoundDryFire);
		}
	}

	protected override void Start()
	{
		base.Start();
		this.m_FPSWeapon = base.transform.GetComponent<vp_FPWeapon>();
	}

	protected override void Update()
	{
		base.Update();
		if (this.Player.Attack.Active)
		{
			this.TryFire();
		}
	}

	public override void TryFire()
	{
		if (Time.time < this.m_NextAllowedFireTime)
		{
			return;
		}
		if (this.Player.SetWeapon.Active)
		{
			return;
		}
		if (!this.m_FPSWeapon.Wielded)
		{
			return;
		}
		if (Time.time < vp_FPInput.DrawTime)
		{
			return;
		}
		if (this.m_FPSWeapon.WeaponSlot == 1 && !vp_FPInput.CanPistolFire)
		{
			return;
		}
		if (!BasePlayer.CurrWeaponShot())
		{
			this.DryFire();
			if (!this.Player.Reload.Active)
			{
				this.Player.Reload.TryStart();
				this.Player.Zoom.TryStop();
				Crosshair.SetActive(true);
			}
			return;
		}
		this.Fire();
		vp_FPInput.LastFireTime = Time.realtimeSinceStartup;
		this.Player.Run.TryStop();
		vp_FPInput.UpdateSpeed(false);
		if (this.m_FPSWeapon.WeaponSlot == 1)
		{
			vp_FPInput.CanPistolFire = false;
		}
		float d = -0.05f;
		float d2 = UnityEngine.Random.Range(-0.025f, 0.025f);
		float d3 = (!BasePlayer.EvenClip()) ? -0.05f : 0.05f;
		if (this.m_FPSWeapon.WeaponID == 4)
		{
			d = -0.2f;
			d3 = 0.2f;
		}
		if (this.m_FPSWeapon.WeaponID == 9 || this.m_FPSWeapon.WeaponID == 16)
		{
			d = -0.5f;
			d2 = UnityEngine.Random.Range(-0.1f, 0.1f);
			d3 = ((!BasePlayer.EvenClip()) ? -0.25f : 0.25f);
		}
		vp_FPCamera.cs.AddRotationForce(Vector3.forward * d3);
		vp_FPCamera.cs.AddRotationForce(Vector2.up * d2);
		vp_FPCamera.cs.AddRotationForce(Vector2.right * d);
		if (this.m_FPSWeapon.WeaponID == 21 || this.m_FPSWeapon.WeaponID == 22 || this.m_FPSWeapon.WeaponID == 23)
		{
			vp_FPInput.LastZoomTime = Time.time + this.ProjectileTapFiringRate;
			this.Player.Zoom.TryStop();
			Crosshair.SetActive(true);
		}
	}

	protected override void Fire()
	{
		if (this.AnimationFire == string.Empty)
		{
			this.AnimationFire = "Shot";
		}
		Crosshair.SetOffsetInc(8);
		this.m_LastFireTime = Time.time;
		if (this.AnimationFire != string.Empty)
		{
			if (!this.m_FPSCamera.inZoom() || this.m_FPSWeapon.WeaponSlot != 1)
			{
				this.m_FPSWeapon.WeaponCoreModel.GetComponent<Animation>().Stop(this.AnimationFire);
				this.m_FPSWeapon.WeaponCoreModel.GetComponent<Animation>().Play(this.AnimationFire);
			}
		}
		if (this.MotionRecoilDelay == 0f)
		{
			this.ApplyRecoil();
		}
		else
		{
			vp_Timer.In(this.MotionRecoilDelay, new vp_Timer.Callback(this.ApplyRecoil), null);
		}
		base.Fire();
		if (!SpecCam.show)
		{
			Client.cs.send_attack();
		}
		this.m_FPSWeapon.SetMuzzleLight();
	}

	protected virtual void ApplyRecoil()
	{
		this.m_FPSWeapon.ResetSprings(this.MotionPositionReset, this.MotionRotationReset, this.MotionPositionPause, this.MotionRotationPause);
		if (this.Player.Zoom.Active)
		{
			this.MotionRotationRecoil = Vector3.zero;
		}
		if (this.MotionRotationRecoil.z == 0f)
		{
			this.m_FPSWeapon.AddForce2(this.MotionPositionRecoil, this.MotionRotationRecoil);
		}
		else
		{
			this.m_FPSWeapon.AddForce2(this.MotionPositionRecoil, Vector3.Scale(this.MotionRotationRecoil, Vector3.one + Vector3.back) + ((UnityEngine.Random.value >= 0.5f) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(this.MotionRotationRecoil.z * this.MotionRotationRecoilDeadZone, this.MotionRotationRecoil.z));
		}
	}

	public virtual void DryFire()
	{
		this.m_LastFireTime = Time.time;
		if (this.as_dryfire != null)
		{
			this.as_dryfire.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			this.as_dryfire.PlayOneShot(this.sndSoundDryFire);
			this.DisableFiring(1E+07f);
		}
	}

	protected virtual void OnStop_Attack()
	{
		if (this.ProjectileFiringRate == 0f)
		{
			this.EnableFiring();
			return;
		}
		this.DisableFiring(this.ProjectileTapFiringRate - (Time.time - this.m_LastFireTime));
	}
}
