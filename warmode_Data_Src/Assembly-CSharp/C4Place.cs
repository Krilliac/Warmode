using System;
using UnityEngine;

public class C4Place : MonoBehaviour
{
	public static AudioSource asBeep;

	public static AudioSource asInstr;

	private static AudioClip detonationSound = new AudioClip();

	private static AudioClip detonationFarSound = new AudioClip();

	private static AudioClip beepSound = new AudioClip();

	private static AudioClip diffuseStartSound = new AudioClip();

	private static AudioClip diffuseEndSound = new AudioClip();

	public static AudioClip plantEndSound = new AudioClip();

	public static bool isActive = false;

	private static float tBeepIndic = 0f;

	private static float incBeepTime;

	public void Init()
	{
		C4Place.detonationSound = SND.GetSoundByName("c4_explosion");
		C4Place.detonationFarSound = SND.GetSoundByName("FarExplosionA");
		C4Place.beepSound = SND.GetSoundByName("c4_beep");
		C4Place.diffuseStartSound = SND.GetSoundByName("c4_diffuse");
		C4Place.diffuseEndSound = SND.GetSoundByName("c4_diffuse_end");
		C4Place.plantEndSound = SND.GetSoundByName("c4_plant_end");
		C4Place.asBeep = base.GetComponent<AudioSource>();
		C4Place.asBeep.maxDistance = 50f;
		C4Place.asBeep.rolloffMode = AudioRolloffMode.Linear;
		C4Place.asBeep.volume = 0.5f * Options.gamevol;
		C4Place.asBeep.spatialBlend = 1f;
		C4Place.asInstr = base.GetComponent<AudioSource>();
		C4Place.asInstr.maxDistance = 50f;
		C4Place.asInstr.rolloffMode = AudioRolloffMode.Linear;
		C4Place.asInstr.volume = 0.5f * Options.gamevol;
		C4Place.asInstr.spatialBlend = 1f;
	}

	public static void Activate(bool val)
	{
		C4Place.isActive = val;
		if (C4Place.isActive)
		{
			C4Place.tBeepIndic = 0.5f;
		}
	}

	public static void Explosion()
	{
		C4Place.isActive = false;
		if (C4Place.asBeep == null)
		{
			return;
		}
		float num;
		if (SpecCam.show)
		{
			if (SpecCam.FID >= 0 && SpecCam.mode == 1 && PlayerControll.Player[SpecCam.FID] != null)
			{
				num = Vector3.Distance(C4.bombGo.transform.position, PlayerControll.Player[SpecCam.FID].go.transform.position);
			}
			else
			{
				num = Vector3.Distance(C4.bombGo.transform.position, SpecCam.position);
			}
		}
		else
		{
			num = Vector3.Distance(C4.bombGo.transform.position, BasePlayer.go.transform.position);
		}
		if (num < 50f)
		{
			C4Place.asBeep.volume = 0.5f * Options.gamevol;
			C4Place.asBeep.maxDistance = 55f;
			C4Place.asBeep.PlayOneShot(C4Place.detonationSound);
		}
		else
		{
			C4Place.asBeep.volume = 1f * Options.gamevol;
			C4Place.asBeep.maxDistance = 999999f;
			C4Place.asBeep.PlayOneShot(C4Place.detonationFarSound);
		}
		GameObject original = ContentLoader_.LoadGameObject("Detonator");
		UnityEngine.Object.Instantiate(original, C4.bombGo.transform.position + Vector3.up * 0.1f, C4.bombGo.transform.rotation);
		if (vp_FPController.cs == null || vp_FPCamera.cs == null)
		{
			return;
		}
		float num2 = Vector3.Distance(vp_FPController.cs.SmoothPosition, C4.bombGo.transform.position);
		if (num2 > 30f)
		{
			return;
		}
		float num3 = 0.001f;
		if (num2 < 5f)
		{
			num3 = 0.005f;
		}
		else if (num2 < 10f)
		{
			num3 = 0.003f;
		}
		num2 = 30f - num2;
		vp_FPCamera.cs.AddForce2(new Vector3(2f, -10f, 2f) * num2 * num3);
		if (UnityEngine.Random.value > 0.5f)
		{
			num3 = -num3;
		}
		vp_FPCamera.cs.AddRollForce(num3 * 200f);
		C4.DestroyBomb();
	}

	public static void PlayDiffuseSound(int val)
	{
		if (C4Place.asInstr == null)
		{
			return;
		}
		C4Place.asInstr.volume = 0.5f * Options.gamevol;
		if (val == 0)
		{
			C4Place.asInstr.PlayOneShot(C4Place.diffuseStartSound);
		}
		else if (val == 1)
		{
			C4Place.asInstr.PlayOneShot(C4Place.diffuseEndSound);
		}
	}

	public static void PlayPlantSound(int val)
	{
		if (C4Place.asInstr == null)
		{
			return;
		}
		C4Place.asInstr.volume = 0.5f * Options.gamevol;
		if (val == 0)
		{
			C4Place.asInstr.PlayOneShot(C4.plantStartSound);
		}
		else if (val == 1)
		{
			C4Place.asInstr.PlayOneShot(C4Place.plantEndSound);
		}
	}

	private void Update()
	{
		if (C4Place.isActive)
		{
			if (C4Place.asBeep == null)
			{
				return;
			}
			float num = (float)ScoreTop.TimeLeft / ScoreTop.detonationTime;
			C4Place.incBeepTime = (num / (num + 0.2f) * 1f + 0.001f) * 6.8f;
			C4Place.tBeepIndic += (6f - C4Place.incBeepTime) * Time.deltaTime;
			if (C4Place.tBeepIndic > 1f)
			{
				C4Place.tBeepIndic -= 1f;
				C4Place.asBeep.volume = 0.5f * Options.gamevol;
				C4Place.asBeep.maxDistance = 50f;
				C4Place.asBeep.PlayOneShot(C4Place.beepSound);
			}
		}
	}
}
