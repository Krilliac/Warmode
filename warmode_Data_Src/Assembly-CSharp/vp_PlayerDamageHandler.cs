using System;
using UnityEngine;

[RequireComponent(typeof(vp_FPPlayerEventHandler))]
public class vp_PlayerDamageHandler : vp_DamageHandler
{
	private vp_FPPlayerEventHandler m_Player;

	protected float m_RespawnOffset;

	protected virtual float OnValue_Health
	{
		get
		{
			return this.m_CurrentHealth;
		}
		set
		{
			this.m_CurrentHealth = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.m_Player = base.transform.GetComponent<vp_FPPlayerEventHandler>();
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

	public override void Damage(float damage)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		base.Damage(damage);
		this.m_Player.HUDDamageFlash.Send(damage);
	}

	public override void Die()
	{
		if (!base.enabled || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (this.DeathEffect != null)
		{
			UnityEngine.Object.Instantiate(this.DeathEffect, base.transform.position, base.transform.rotation);
		}
		this.m_Player.SetWeapon.Argument = 0;
		this.m_Player.SetWeapon.Start(0f);
		this.m_Player.Dead.Start(0f);
		this.m_Player.AllowGameplayInput.Set(false);
		if (this.Respawns)
		{
			vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.Respawn), null);
		}
	}

	protected override void Respawn()
	{
		if (this == null)
		{
			return;
		}
		if (Physics.CheckSphere(this.m_StartPosition + Vector3.up * this.m_RespawnOffset, this.RespawnCheckRadius, 1342177280))
		{
			this.m_RespawnOffset += 1f;
			this.Respawn();
			return;
		}
		this.m_RespawnOffset = 0f;
		this.Reactivate();
		this.Reset();
	}

	protected override void Reset()
	{
		this.m_CurrentHealth = this.MaxHealth;
		this.m_Player.Position.Set(this.m_StartPosition);
		this.m_Player.Rotation.Set(this.m_StartRotation.eulerAngles);
		this.m_Player.Stop.Send();
	}

	protected override void Reactivate()
	{
		this.m_Player.Dead.Stop(0f);
		this.m_Player.AllowGameplayInput.Set(true);
		this.m_Player.HUDDamageFlash.Send(0f);
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.RespawnSound);
		}
	}

	private void Update()
	{
		if (this.m_Player.Dead.Active && Time.timeScale < 1f)
		{
			vp_TimeUtility.FadeTimeScale(1f, 0.05f);
		}
	}
}
