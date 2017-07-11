using System;
using UnityEngine;

public class GrenadeFlash : MonoBehaviour
{
	private float flashIntensity = 2.5f;

	private Light lightComp;

	private float destroytime;

	public static AudioSource asDetonation;

	private AudioClip detonationSound = new AudioClip();

	private void Start()
	{
		this.detonationSound = SND.GetSoundByName("flashbang");
		GrenadeFlash.asDetonation = base.GetComponent<AudioSource>();
		GrenadeFlash.asDetonation.maxDistance = 50f;
		GrenadeFlash.asDetonation.rolloffMode = AudioRolloffMode.Linear;
		GrenadeFlash.asDetonation.volume = 0.5f * Options.gamevol;
		GrenadeFlash.asDetonation.spatialBlend = 1f;
		GrenadeFlash.asDetonation.PlayOneShot(this.detonationSound);
		this.lightComp = base.GetComponentInChildren<Light>();
		this.destroytime = Time.time + 3f;
	}

	private void Update()
	{
		if (this.flashIntensity > 0f)
		{
			this.flashIntensity -= Time.deltaTime * 5.5f;
			if (this.flashIntensity < 0f)
			{
				this.flashIntensity = 0f;
			}
			this.lightComp.intensity = this.flashIntensity;
		}
		if (this.destroytime != 0f && Time.time > this.destroytime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
