using System;
using UnityEngine;

[RequireComponent(typeof(vp_FPWeapon))]
public class vp_FPWeaponReloader : MonoBehaviour
{
	protected vp_FPWeapon m_Weapon;

	protected vp_FPPlayerEventHandler m_Player;

	protected AudioSource m_Audio;

	public string SoundReload;

	public string SoundReloadEmpty;

	private AudioClip sndSoundReload;

	private AudioClip sndSoundReloadEmpty;

	public string AnimationReload;

	public string AnimationReloadEmpty;

	public float ReloadDuration = 1f;

	private float StartReload;

	private string currAnimation = string.Empty;

	protected virtual float OnValue_CurrentWeaponReloadDuration
	{
		get
		{
			return this.ReloadDuration;
		}
	}

	protected virtual void Awake()
	{
		this.m_Audio = base.gameObject.transform.parent.GetComponent<AudioSource>();
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		if (this.sndSoundReload == null)
		{
			this.sndSoundReload = SND.GetSoundByName(this.SoundReload);
		}
		if (this.sndSoundReloadEmpty == null)
		{
			this.sndSoundReloadEmpty = SND.GetSoundByName(this.SoundReloadEmpty);
		}
	}

	protected virtual void Start()
	{
		this.m_Weapon = base.transform.GetComponent<vp_FPWeapon>();
		if (this.AnimationReload == string.Empty)
		{
			this.AnimationReload = "Reload";
		}
		if (this.AnimationReloadEmpty == string.Empty)
		{
			this.AnimationReloadEmpty = "ReloadEmpty";
		}
	}

	public void WeaponLoaded()
	{
	}

	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	protected virtual bool CanStart_Reload()
	{
		if (!this.m_Player.CurrentWeaponWielded.Get())
		{
			return false;
		}
		if (BasePlayer.currweapon == null)
		{
			return false;
		}
		int ammoType = BasePlayer.currweapon.data.ammoType;
		return BasePlayer.ammo[ammoType] > 0 && BasePlayer.currweapon.clip != BasePlayer.currweapon.data.maxClip;
	}

	protected virtual void OnStart_Reload()
	{
		this.m_Player.Reload.AutoDuration = this.ReloadDuration;
		if (this.AnimationReload == "null")
		{
			this.StartReload = Time.time;
			vp_FPInput.ZoomTap = false;
			return;
		}
		if (this.m_Weapon.WeaponID == 9 || this.m_Weapon.WeaponID == 16)
		{
			int ammoType = BasePlayer.currweapon.data.ammoType;
			if (BasePlayer.currweapon.data.maxClip - BasePlayer.currweapon.clip == 1 || BasePlayer.ammo[ammoType] == 1)
			{
				if (this.currAnimation == "ReloadStart" || this.currAnimation == "ReloadLoop")
				{
					this.currAnimation = "ReloadEnd";
					this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.currAnimation).length - 0.1f;
					this.m_Player.Reload.AutoDuration = this.ReloadDuration;
					this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop("ReloadEnd");
					this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play("ReloadEnd");
				}
				else
				{
					this.currAnimation = "ReloadOne";
					this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.currAnimation).length - 0.1f;
					this.m_Player.Reload.AutoDuration = this.ReloadDuration;
					this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop("ReloadOne");
					this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play("ReloadOne");
				}
			}
			else if (this.currAnimation == "Idle" || this.currAnimation == "ReloadEnd" || this.currAnimation == "ReloadOne")
			{
				this.currAnimation = "ReloadStart";
				this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.currAnimation).length - 0.1f;
				this.m_Player.Reload.AutoDuration = this.ReloadDuration;
				this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop("ReloadStart");
				this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play("ReloadStart");
			}
			else
			{
				this.currAnimation = "ReloadLoop";
				this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.currAnimation).length - 0.1f;
				this.m_Player.Reload.AutoDuration = this.ReloadDuration;
				this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop("ReloadLoop");
				this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play("ReloadLoop");
			}
			this.m_Audio.pitch = 1f;
			this.m_Audio.PlayOneShot(SND.GetSoundByName("bullet_insert"));
			this.StartReload = Time.time;
			vp_FPInput.ZoomTap = false;
			return;
		}
		if (this.m_Weapon.WeaponID == 24 || this.m_Weapon.WeaponID == 25)
		{
			this.AnimationReloadEmpty = "null";
		}
		if (BasePlayer.currweapon.clip == 0 && this.AnimationReloadEmpty != "null")
		{
			this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.AnimationReloadEmpty).length - 0.1f;
			this.m_Player.Reload.AutoDuration = this.ReloadDuration;
			this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop(this.AnimationReloadEmpty);
			this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play(this.AnimationReloadEmpty);
		}
		else
		{
			this.ReloadDuration = this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().GetClip(this.AnimationReload).length - 0.1f;
			this.m_Player.Reload.AutoDuration = this.ReloadDuration;
			this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Stop(this.AnimationReload);
			this.m_Weapon.WeaponCoreModel.GetComponent<Animation>().Play(this.AnimationReload);
		}
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = 1f;
			if (BasePlayer.currweapon.clip == 0)
			{
				this.m_Audio.PlayOneShot(this.sndSoundReloadEmpty);
			}
			else
			{
				this.m_Audio.PlayOneShot(this.sndSoundReload);
			}
		}
		this.StartReload = Time.time;
		vp_FPInput.ZoomTap = false;
	}

	protected virtual void OnStop_Reload()
	{
		string text = this.m_Player.CurrentWeaponName.Get();
		if (Time.time + 0.1f > this.StartReload + this.ReloadDuration)
		{
			BasePlayer.CurrWeaponReload();
			if (BasePlayer.currweapon != null && BasePlayer.currweapon.clip != BasePlayer.currweapon.data.maxClip)
			{
				this.m_Player.Reload.TryStart();
			}
		}
		if (BasePlayer.currweapon != null)
		{
			Client.cs.send_weapon_clip((byte)BasePlayer.currweapon.clip);
		}
	}
}
