using System;
using UnityEngine;

public class GrenadeSmoke : MonoBehaviour
{
	private float smoketime;

	private float destroytime;

	public static AudioSource asDetonation;

	private AudioClip detonationSound = new AudioClip();

	private void Start()
	{
		this.detonationSound = SND.GetSoundByName("smokegrenade");
		GrenadeSmoke.asDetonation = base.GetComponent<AudioSource>();
		GrenadeSmoke.asDetonation.maxDistance = 50f;
		GrenadeSmoke.asDetonation.rolloffMode = AudioRolloffMode.Linear;
		GrenadeSmoke.asDetonation.volume = 0.5f * Options.gamevol;
		GrenadeSmoke.asDetonation.spatialBlend = 1f;
		GrenadeSmoke.asDetonation.PlayOneShot(this.detonationSound);
		this.smoketime = Time.time + 35f;
		this.destroytime = Time.time + 50f;
	}

	private void Update()
	{
		if (this.smoketime != 0f && Time.time > this.smoketime)
		{
			base.GetComponent<ParticleSystem>().startLifetime = 0f;
		}
	}
}
