using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class vp_HitscanBullet : MonoBehaviour
{
	public static RaycastHit lasthit;

	public static Ray lastray;

	public bool IgnoreLocalPlayer = true;

	public float Range = 100f;

	public float Force = 100f;

	public float Damage = 1f;

	public float m_SparkFactor = 0.5f;

	public GameObject m_ImpactPrefab;

	public GameObject m_DustPrefab;

	public GameObject m_SparkPrefab;

	public GameObject m_DebrisPrefab;

	public GameObject m_TracePrefab;

	protected AudioSource m_Audio;

	public List<AudioClip> m_ImpactSounds = new List<AudioClip>();

	public Vector2 SoundImpactPitch = new Vector2(1f, 1.5f);

	public int[] NoDecalOnTheseLayers;

	public bool started;

	private void Start()
	{
		base.StartCoroutine(this.CCAST());
	}

	[DebuggerHidden]
	private IEnumerator CCAST()
	{
		vp_HitscanBullet.<CCAST>c__IteratorE <CCAST>c__IteratorE = new vp_HitscanBullet.<CCAST>c__IteratorE();
		<CCAST>c__IteratorE.<>f__this = this;
		return <CCAST>c__IteratorE;
	}

	private void CAST()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		Transform transform = base.transform;
		Transform transform2 = Camera.main.transform;
		this.m_Audio = base.GetComponent<AudioSource>();
		Ray ray = new Ray(transform.position, base.transform.forward);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, this.Range, vp_Layer.GetBulletBlockers()))
		{
			Vector3 localScale = transform.localScale;
			transform.parent = raycastHit.transform;
			transform.localPosition = raycastHit.transform.InverseTransformPoint(raycastHit.point);
			transform.rotation = Quaternion.LookRotation(raycastHit.normal);
			if (raycastHit.transform.lossyScale == Vector3.one)
			{
				transform.Rotate(Vector3.forward, (float)UnityEngine.Random.Range(0, 360), Space.Self);
			}
			else
			{
				transform.parent = null;
				transform.localScale = localScale;
				transform.parent = raycastHit.transform;
			}
			Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
			if (attachedRigidbody != null && !attachedRigidbody.isKinematic)
			{
				attachedRigidbody.AddForceAtPosition(ray.direction * this.Force / Time.timeScale, raycastHit.point);
			}
			if (this.m_ImpactPrefab != null)
			{
				UnityEngine.Object.Instantiate(this.m_ImpactPrefab, transform.position, transform.rotation);
			}
			if (this.m_DustPrefab != null)
			{
				UnityEngine.Object.Instantiate(this.m_DustPrefab, transform.position, transform.rotation);
			}
			if (this.m_SparkPrefab != null && UnityEngine.Random.value < this.m_SparkFactor)
			{
				UnityEngine.Object.Instantiate(this.m_SparkPrefab, transform.position, transform.rotation);
			}
			if (this.m_DebrisPrefab != null)
			{
				UnityEngine.Object.Instantiate(this.m_DebrisPrefab, transform.position, transform.rotation);
			}
			if (this.m_TracePrefab != null && !vp_FPInput.cs.Player.Zoom.Active)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.m_TracePrefab, ray.origin + transform2.right * 1f - transform2.up * 1f, transform.rotation) as GameObject;
				LineRenderer component = gameObject.GetComponent<LineRenderer>();
				gameObject.GetComponent<vp_Trace>().e = transform.position;
			}
			if (this.m_ImpactSounds.Count > 0)
			{
				this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundImpactPitch.x, this.SoundImpactPitch.y) * Time.timeScale;
				this.m_Audio.PlayOneShot(this.m_ImpactSounds[UnityEngine.Random.Range(0, this.m_ImpactSounds.Count)]);
				this.m_Audio.volume = Options.gamevol;
			}
			vp_HitscanBullet.lasthit = raycastHit;
			vp_HitscanBullet.lastray = ray;
			vp_DamageHandlerPlayer component2 = raycastHit.collider.GetComponent<vp_DamageHandlerPlayer>();
			if (component2)
			{
				component2.Damage();
			}
			if (this.NoDecalOnTheseLayers.Length > 0)
			{
				int[] noDecalOnTheseLayers = this.NoDecalOnTheseLayers;
				for (int i = 0; i < noDecalOnTheseLayers.Length; i++)
				{
					int num = noDecalOnTheseLayers[i];
					if (raycastHit.transform.gameObject.layer == num)
					{
						this.TryDestroy();
						return;
					}
				}
			}
			if (base.gameObject.GetComponent<Renderer>() != null)
			{
				vp_DecalManager.Add(base.gameObject);
			}
			else
			{
				vp_Timer.In(1f, new vp_Timer.Callback(this.TryDestroy), null);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void TryDestroy()
	{
		if (this == null)
		{
			return;
		}
		if (!this.m_Audio.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			vp_Timer.In(1f, new vp_Timer.Callback(this.TryDestroy), null);
		}
	}
}
