using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(SphereCollider))]
public abstract class vp_Pickup : MonoBehaviour
{
	protected Transform m_Transform;

	protected Rigidbody m_Rigidbody;

	protected AudioSource m_Audio;

	public string InventoryName = "Unnamed";

	public List<string> RecipientTags = new List<string>();

	private Collider m_LastCollider;

	private vp_FPPlayerEventHandler m_Recipient;

	public string GiveMessage = "Picked up an item";

	public string FailMessage = "You currently can't pick up this item!";

	protected Vector3 m_SpawnPosition = Vector3.zero;

	protected Vector3 m_SpawnScale = Vector3.zero;

	public bool Billboard;

	public Vector3 Spin = Vector3.zero;

	public float BobAmp;

	public float BobRate;

	public float BobOffset = -1f;

	public float RespawnDuration = 10f;

	public float RespawnScaleUpDuration;

	public AudioClip PickupSound;

	public AudioClip PickupFailSound;

	public AudioClip RespawnSound;

	public bool PickupSoundSlomo = true;

	public bool FailSoundSlomo = true;

	public bool RespawnSoundSlomo = true;

	protected bool m_Depleted;

	protected bool m_AlreadyFailed;

	protected vp_Timer.Handle m_RespawnTimer = new vp_Timer.Handle();

	private Transform m_CameraMainTransform;

	protected virtual void Start()
	{
		this.m_Transform = base.transform;
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Audio = base.GetComponent<AudioSource>();
		if (Camera.main != null)
		{
			this.m_CameraMainTransform = Camera.main.transform;
		}
		base.GetComponent<Collider>().isTrigger = true;
		this.m_Audio.clip = this.PickupSound;
		this.m_Audio.playOnAwake = false;
		this.m_Audio.minDistance = 3f;
		this.m_Audio.maxDistance = 150f;
		this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
		this.m_Audio.dopplerLevel = 0f;
		this.m_SpawnPosition = this.m_Transform.position;
		this.m_SpawnScale = this.m_Transform.localScale;
		this.RespawnScaleUpDuration = ((!(this.m_Rigidbody == null)) ? 0f : Mathf.Abs(this.RespawnScaleUpDuration));
		if (this.BobOffset == -1f)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
		if (this.RecipientTags.Count == 0)
		{
			this.RecipientTags.Add("Player");
		}
	}

	protected virtual void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted && !this.m_Audio.isPlaying)
		{
			this.Remove();
		}
	}

	protected virtual void UpdateMotion()
	{
		if (this.m_Rigidbody != null)
		{
			return;
		}
		if (this.Billboard)
		{
			if (this.m_CameraMainTransform != null)
			{
				this.m_Transform.localEulerAngles = this.m_CameraMainTransform.eulerAngles;
			}
		}
		else
		{
			this.m_Transform.localEulerAngles += this.Spin * Time.deltaTime;
		}
		if (this.BobRate != 0f && this.BobAmp != 0f)
		{
			this.m_Transform.position = this.m_SpawnPosition + Vector3.up * (Mathf.Cos((Time.time + this.BobOffset) * (this.BobRate * 10f)) * this.BobAmp);
		}
		if (this.m_Transform.localScale != this.m_SpawnScale)
		{
			this.m_Transform.localScale = Vector3.Lerp(this.m_Transform.localScale, this.m_SpawnScale, Time.deltaTime / this.RespawnScaleUpDuration);
		}
	}

	protected virtual void OnTriggerEnter(Collider col)
	{
		if (this.m_Depleted)
		{
			return;
		}
		foreach (string current in this.RecipientTags)
		{
			if (col.gameObject.tag == current)
			{
				goto IL_5E;
			}
		}
		return;
		IL_5E:
		if (col != this.m_LastCollider)
		{
			this.m_Recipient = col.gameObject.GetComponent<vp_FPPlayerEventHandler>();
		}
		if (this.m_Recipient == null)
		{
			return;
		}
		if (this.TryGive(this.m_Recipient))
		{
			this.m_Audio.pitch = ((!this.PickupSoundSlomo) ? 1f : Time.timeScale);
			this.m_Audio.Play();
			base.GetComponent<Renderer>().enabled = false;
			this.m_Depleted = true;
			this.m_Recipient.HUDText.Send(this.GiveMessage);
		}
		else if (!this.m_AlreadyFailed)
		{
			this.m_Audio.pitch = ((!this.FailSoundSlomo) ? 1f : Time.timeScale);
			this.m_Audio.PlayOneShot(this.PickupFailSound);
			this.m_AlreadyFailed = true;
			this.m_Recipient.HUDText.Send(this.FailMessage);
		}
	}

	protected virtual void OnTriggerExit(Collider col)
	{
		this.m_AlreadyFailed = false;
	}

	protected virtual bool TryGive(vp_FPPlayerEventHandler player)
	{
		return true;
	}

	protected virtual void Remove()
	{
		if (this.RespawnDuration == 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (!this.m_RespawnTimer.Active)
		{
			vp_Utility.Activate(base.gameObject, false);
			vp_Timer.In(this.RespawnDuration, new vp_Timer.Callback(this.Respawn), this.m_RespawnTimer);
		}
	}

	protected virtual void Respawn()
	{
		if (Camera.main != null)
		{
			this.m_CameraMainTransform = Camera.main.transform;
		}
		this.m_RespawnTimer.Cancel();
		this.m_Transform.position = this.m_SpawnPosition;
		if (this.m_Rigidbody == null && this.RespawnScaleUpDuration > 0f)
		{
			this.m_Transform.localScale = Vector3.zero;
		}
		base.GetComponent<Renderer>().enabled = true;
		vp_Utility.Activate(base.gameObject, true);
		this.m_Audio.pitch = ((!this.RespawnSoundSlomo) ? 1f : Time.timeScale);
		this.m_Audio.PlayOneShot(this.RespawnSound);
		this.m_Depleted = false;
		if (this.BobOffset == -1f)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
	}
}
