using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class vp_Explosion : MonoBehaviour
{
	public float Radius = 15f;

	public float Force = 1000f;

	public float UpForce = 10f;

	public float Damage = 10f;

	public float CameraShake = 1f;

	public string DamageMessageName = "Damage";

	private AudioSource m_Audio;

	public AudioClip Sound;

	public float SoundMinPitch = 0.8f;

	public float SoundMaxPitch = 1.2f;

	public List<GameObject> FXPrefabs = new List<GameObject>();

	private void Awake()
	{
		Transform transform = base.transform;
		this.m_Audio = base.GetComponent<AudioSource>();
		foreach (GameObject current in this.FXPrefabs)
		{
			Component[] components = current.GetComponents<vp_Explosion>();
			if (components.Length == 0)
			{
				UnityEngine.Object.Instantiate(current, transform.position, transform.rotation);
			}
			else
			{
				Debug.LogError("Error: vp_Explosion->FXPrefab must not be a vp_Explosion (risk of infinite loop).");
			}
		}
		Collider[] array = Physics.OverlapSphere(transform.position, this.Radius, -746586133);
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Collider collider = array2[i];
			if (collider != base.GetComponent<Collider>())
			{
				float num = 1f - Vector3.Distance(transform.position, collider.transform.position) / this.Radius;
				if (collider.GetComponent<Rigidbody>())
				{
					Ray ray = new Ray(collider.transform.position, -Vector3.up);
					RaycastHit raycastHit;
					if (!Physics.Raycast(ray, out raycastHit, 1f))
					{
						this.UpForce = 0f;
					}
					collider.GetComponent<Rigidbody>().AddExplosionForce(this.Force / Time.timeScale, transform.position, this.Radius, this.UpForce);
				}
				else if (collider.gameObject.layer == 30)
				{
					vp_FPPlayerEventHandler component = collider.GetComponent<vp_FPPlayerEventHandler>();
					if (component)
					{
						component.ForceImpact.Send((collider.transform.position - transform.position).normalized * this.Force * 0.001f * num);
						component.BombShake.Send(num * this.CameraShake);
					}
				}
				if (collider.gameObject.layer != 29)
				{
					collider.gameObject.BroadcastMessage(this.DamageMessageName, num * this.Damage, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		this.m_Audio.clip = this.Sound;
		this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
		if (!this.m_Audio.playOnAwake)
		{
			this.m_Audio.Play();
		}
	}

	private void Update()
	{
		if (!this.m_Audio.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
