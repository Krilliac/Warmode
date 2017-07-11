using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Rigidbody))]
public class vp_Shell : MonoBehaviour
{
	public delegate void RestAngleFunc();

	private Transform m_Transform;

	private Rigidbody m_Rigidbody;

	private AudioSource m_Audio;

	public float LifeTime = 10f;

	protected float m_RemoveTime;

	public float m_Persistence = 1f;

	protected vp_Shell.RestAngleFunc m_RestAngleFunc;

	protected float m_RestTime;

	public List<AudioClip> m_BounceSounds = new List<AudioClip>();

	private void Start()
	{
		this.m_Transform = base.transform;
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_RestAngleFunc = null;
		this.m_RemoveTime = Time.time + this.LifeTime;
		this.m_RestTime = Time.time + this.LifeTime * 0.25f;
		this.m_Rigidbody.maxAngularVelocity = 100f;
		base.GetComponent<AudioSource>().playOnAwake = false;
		base.GetComponent<AudioSource>().dopplerLevel = 0f;
	}

	private void Update()
	{
		if (this.m_RestAngleFunc == null)
		{
			if (Time.time > this.m_RestTime)
			{
				this.DecideRestAngle();
			}
		}
		else
		{
			this.m_RestAngleFunc();
		}
		if (Time.time > this.m_RemoveTime)
		{
			this.m_Transform.localScale = Vector3.Lerp(this.m_Transform.localScale, Vector3.zero, Time.deltaTime * 60f * 0.2f);
			if (Time.time > this.m_RemoveTime + 0.5f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 2f)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				this.m_Rigidbody.AddRelativeTorque(-UnityEngine.Random.rotation.eulerAngles * 0.15f);
			}
			else
			{
				this.m_Rigidbody.AddRelativeTorque(UnityEngine.Random.rotation.eulerAngles * 0.15f);
			}
			if (this.m_Audio != null && this.m_BounceSounds.Count > 0)
			{
				this.m_Audio.pitch = Time.timeScale;
				this.m_Audio.PlayOneShot(this.m_BounceSounds[UnityEngine.Random.Range(0, this.m_BounceSounds.Count)]);
				this.m_Audio.volume = Options.gamevol;
			}
		}
		else if (UnityEngine.Random.value > this.m_Persistence)
		{
			base.GetComponent<Collider>().enabled = false;
			this.m_RemoveTime = Time.time + 0.5f;
		}
	}

	protected void DecideRestAngle()
	{
		float num = Mathf.Abs(this.m_Transform.eulerAngles.x - 270f);
		if (num < 55f)
		{
			Ray ray = new Ray(this.m_Transform.position, Vector3.down);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 1f) && raycastHit.normal == Vector3.up)
			{
				this.m_RestAngleFunc = new vp_Shell.RestAngleFunc(this.UpRight);
				this.m_Rigidbody.constraints = (RigidbodyConstraints)80;
			}
			return;
		}
		this.m_RestAngleFunc = new vp_Shell.RestAngleFunc(this.TippedOver);
	}

	protected void UpRight()
	{
		this.m_Transform.rotation = Quaternion.Lerp(this.m_Transform.rotation, Quaternion.Euler(-90f, this.m_Transform.rotation.y, this.m_Transform.rotation.z), Time.time * (Time.deltaTime * 60f * 0.05f));
	}

	protected void TippedOver()
	{
		this.m_Transform.localRotation = Quaternion.Lerp(this.m_Transform.localRotation, Quaternion.Euler(0f, this.m_Transform.localEulerAngles.y, this.m_Transform.localEulerAngles.z), Time.time * (Time.deltaTime * 60f * 0.005f));
	}
}
