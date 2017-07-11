using System;
using UnityEngine;

public class vp_Shooter : vp_Component
{
	protected CharacterController m_CharacterController;

	public static float offset;

	protected Transform m_OperatorTransform;

	public GameObject ProjectilePrefab;

	public float ProjectileScale = 1f;

	public float ProjectileFiringRate = 0.3f;

	public float ProjectileSpawnDelay;

	public int ProjectileCount = 1;

	protected float ProjectileSpread;

	protected float m_NextAllowedFireTime;

	public Vector3 MuzzleFlashPosition = Vector3.zero;

	public Vector3 MuzzleFlashScale = Vector3.one;

	public float MuzzleFlashFadeSpeed = 0.075f;

	public GameObject MuzzleFlashPrefab;

	public float MuzzleFlashDelay;

	protected GameObject m_MuzzleFlash;

	public GameObject ShellPrefab;

	public float ShellScale = 1f;

	public Vector3 ShellEjectDirection = new Vector3(1f, 1f, 1f);

	public Vector3 ShellEjectPosition = new Vector3(1f, 0f, 1f);

	public float ShellEjectVelocity = 0.2f;

	public float ShellEjectDelay;

	public float ShellEjectSpin;

	protected AudioSource as_fire;

	public string SoundFire;

	public AudioClip sndSoundFire;

	public float SoundFireDelay;

	public Vector2 SoundFirePitch = new Vector2(1f, 1f);

	public GameObject MuzzleFlash
	{
		get
		{
			return this.m_MuzzleFlash;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.m_OperatorTransform = base.Transform;
		this.m_CharacterController = this.m_OperatorTransform.root.GetComponentInChildren<CharacterController>();
		this.m_NextAllowedFireTime = Time.time;
		this.ProjectileSpawnDelay = Mathf.Min(this.ProjectileSpawnDelay, this.ProjectileFiringRate - 0.1f);
		if (this.sndSoundFire == null)
		{
			this.sndSoundFire = SND.GetSoundByName(this.SoundFire);
		}
		if (this.sndSoundFire == null)
		{
			MonoBehaviour.print("[WEAPON] can't load audioclip: " + this.SoundFire);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (this.MuzzleFlashPrefab != null)
		{
			this.m_MuzzleFlash = (GameObject)UnityEngine.Object.Instantiate(this.MuzzleFlashPrefab, this.m_OperatorTransform.position, this.m_OperatorTransform.rotation);
			this.m_MuzzleFlash.name = base.transform.name + "MuzzleFlash";
			this.m_MuzzleFlash.transform.parent = this.m_OperatorTransform;
		}
		this.as_fire = base.Parent.GetComponent<AudioSource>();
		this.as_fire.playOnAwake = false;
		this.as_fire.dopplerLevel = 0f;
		base.RefreshDefaultState();
		this.Refresh();
	}

	public virtual void TryFire()
	{
		if (Time.time < this.m_NextAllowedFireTime)
		{
			return;
		}
		this.Fire();
	}

	protected virtual void Fire()
	{
		this.m_NextAllowedFireTime = Time.time + this.ProjectileFiringRate;
		if (this.SoundFireDelay == 0f)
		{
			this.PlayFireSound();
		}
		else
		{
			vp_Timer.In(this.SoundFireDelay, new vp_Timer.Callback(this.PlayFireSound), null);
		}
		if (this.ProjectileSpawnDelay == 0f)
		{
			this.SpawnProjectiles();
		}
		else
		{
			vp_Timer.In(this.ProjectileSpawnDelay, new vp_Timer.Callback(this.SpawnProjectiles), null);
		}
		if (this.ShellEjectDelay == 0f)
		{
			this.EjectShell();
		}
		else
		{
			vp_Timer.In(this.ShellEjectDelay, new vp_Timer.Callback(this.EjectShell), null);
		}
		if (this.MuzzleFlashDelay == 0f)
		{
			this.ShowMuzzleFlash();
		}
		else
		{
			vp_Timer.In(this.MuzzleFlashDelay, new vp_Timer.Callback(this.ShowMuzzleFlash), null);
		}
	}

	protected virtual void PlayFireSound()
	{
		if (this.as_fire == null)
		{
			return;
		}
		this.as_fire.pitch = UnityEngine.Random.Range(this.SoundFirePitch.x, this.SoundFirePitch.y);
		this.as_fire.clip = this.sndSoundFire;
		this.as_fire.Play();
	}

	protected virtual void SpawnProjectiles()
	{
		if (SpecCam.show)
		{
			return;
		}
		if (this.ProjectileCount > 1)
		{
			for (int i = 0; i < this.ProjectileCount; i++)
			{
				if (this.ProjectilePrefab != null)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.ProjectilePrefab, this.m_OperatorTransform.position, this.m_OperatorTransform.rotation);
					gameObject.transform.localScale = new Vector3(this.ProjectileScale, this.ProjectileScale, this.ProjectileScale);
					gameObject.transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
					gameObject.transform.Rotate(0f, UnityEngine.Random.Range(-3f, 3f), 0f);
				}
			}
		}
		else if (this.ProjectilePrefab != null)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(this.ProjectilePrefab, this.m_OperatorTransform.position, this.m_OperatorTransform.rotation);
			gameObject2.transform.localScale = new Vector3(this.ProjectileScale, this.ProjectileScale, this.ProjectileScale);
			float num = 0.15f + vp_FPInput.speed / 10f + ((!vp_FPController.m_LastGrounded) ? 2f : 0f);
			if (Time.realtimeSinceStartup - vp_FPInput.LastFireTime < this.ProjectileFiringRate * 1.1f)
			{
				vp_Shooter.offset += 0.25f;
			}
			else
			{
				vp_Shooter.offset = 0f;
			}
			num += vp_Shooter.offset;
			if (BasePlayer.currweapon != null && BasePlayer.currweapon.data.wid == 6)
			{
				gameObject2.transform.Rotate(-num / 2f, UnityEngine.Random.Range(-num, num), 0f);
			}
			else if (BasePlayer.currweapon != null && BasePlayer.currweapon.data.wid == 11)
			{
				gameObject2.transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
				gameObject2.transform.Rotate(-num / 3f, UnityEngine.Random.Range(-num / 2f, num / 2f), 0f);
			}
			else
			{
				gameObject2.transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
				gameObject2.transform.Rotate(0f, UnityEngine.Random.Range(-num, num), 0f);
			}
		}
	}

	protected virtual void ShowMuzzleFlash()
	{
		if (this.m_MuzzleFlash == null)
		{
			return;
		}
		this.m_MuzzleFlash.SendMessage("Shoot", SendMessageOptions.DontRequireReceiver);
	}

	protected virtual void EjectShell()
	{
		if (this.ShellPrefab == null)
		{
			return;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.ShellPrefab, this.m_OperatorTransform.position + this.m_OperatorTransform.TransformDirection(this.ShellEjectPosition), this.m_OperatorTransform.rotation);
		gameObject.transform.localScale = new Vector3(this.ShellScale, this.ShellScale, this.ShellScale);
		vp_Layer.Set(gameObject.gameObject, 29, false);
		if (gameObject.GetComponent<Rigidbody>())
		{
			Vector3 force = base.transform.TransformDirection(this.ShellEjectDirection) * this.ShellEjectVelocity;
			gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		}
		if (this.m_CharacterController)
		{
			Vector3 velocity = this.m_CharacterController.velocity;
			gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
		}
		if (this.ShellEjectSpin > 0f)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				gameObject.GetComponent<Rigidbody>().AddRelativeTorque(-UnityEngine.Random.rotation.eulerAngles * this.ShellEjectSpin);
			}
			else
			{
				gameObject.GetComponent<Rigidbody>().AddRelativeTorque(UnityEngine.Random.rotation.eulerAngles * this.ShellEjectSpin);
			}
		}
	}

	public virtual void DisableFiring(float seconds = 1E+07f)
	{
		this.m_NextAllowedFireTime = Time.time + seconds;
	}

	public virtual void EnableFiring()
	{
		this.m_NextAllowedFireTime = Time.time;
	}

	public override void Refresh()
	{
		if (this.m_MuzzleFlash != null)
		{
			this.m_MuzzleFlash.transform.localPosition = this.MuzzleFlashPosition;
			this.m_MuzzleFlash.transform.localScale = this.MuzzleFlashScale;
			this.m_MuzzleFlash.SendMessage("SetFadeSpeed", this.MuzzleFlashFadeSpeed, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (this.m_MuzzleFlash != null)
		{
			vp_Utility.Activate(this.m_MuzzleFlash, true);
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (this.m_MuzzleFlash != null)
		{
			vp_Utility.Activate(this.m_MuzzleFlash, false);
		}
	}
}
