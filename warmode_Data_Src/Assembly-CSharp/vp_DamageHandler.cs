using System;
using UnityEngine;

public class vp_DamageHandler : MonoBehaviour
{
	public float MaxHealth = 1f;

	public GameObject DeathEffect;

	public float MinDeathDelay;

	public float MaxDeathDelay;

	public float m_CurrentHealth;

	public bool Respawns = true;

	public float MinRespawnTime = 3f;

	public float MaxRespawnTime = 3f;

	public float RespawnCheckRadius = 1f;

	protected AudioSource m_Audio;

	public AudioClip DeathSound;

	public AudioClip RespawnSound;

	protected Vector3 m_StartPosition;

	protected Quaternion m_StartRotation;

	protected virtual void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_CurrentHealth = this.MaxHealth;
		this.m_StartPosition = base.transform.position;
		this.m_StartRotation = base.transform.rotation;
	}

	public virtual void Damage(float damage)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (this.m_CurrentHealth <= 0f)
		{
			return;
		}
		this.m_CurrentHealth = Mathf.Min(this.m_CurrentHealth - damage, this.MaxHealth);
		if (this.m_CurrentHealth <= 0f)
		{
			if (this.m_Audio != null)
			{
				this.m_Audio.pitch = Time.timeScale;
				this.m_Audio.PlayOneShot(this.DeathSound);
			}
			vp_Timer.In(UnityEngine.Random.Range(this.MinDeathDelay, this.MaxDeathDelay), new vp_Timer.Callback(this.Die), null);
			return;
		}
	}

	public virtual void Die()
	{
		if (!base.enabled || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		this.RemoveBulletHoles();
		vp_Utility.Activate(base.gameObject, false);
		if (this.DeathEffect != null)
		{
			UnityEngine.Object.Instantiate(this.DeathEffect, base.transform.position, base.transform.rotation);
		}
		if (this.Respawns)
		{
			vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.Respawn), null);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected virtual void Respawn()
	{
		if (this == null)
		{
			return;
		}
		if (Physics.CheckSphere(this.m_StartPosition, this.RespawnCheckRadius, 1342177280))
		{
			vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.Respawn), null);
			return;
		}
		this.Reset();
		this.Reactivate();
	}

	protected virtual void Reset()
	{
		this.m_CurrentHealth = this.MaxHealth;
		base.transform.position = this.m_StartPosition;
		base.transform.rotation = this.m_StartRotation;
		if (base.GetComponent<Rigidbody>() != null)
		{
			base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			base.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	protected virtual void Reactivate()
	{
		vp_Utility.Activate(base.gameObject, true);
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.RespawnSound);
		}
	}

	protected virtual void RemoveBulletHoles()
	{
		foreach (Transform transform in base.transform)
		{
			Component[] components = transform.GetComponents<vp_HitscanBullet>();
			if (components.Length != 0)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}
}
