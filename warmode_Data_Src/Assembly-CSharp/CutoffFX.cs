using System;
using UnityEngine;

public class CutoffFX : MonoBehaviour
{
	private static float currentCutoffVal = 0f;

	private static float maxCutoffVal = 0f;

	private static float minFreq = 150f;

	private static float maxFreq = 22000f;

	private static float incFreq = 1f;

	private static int phaseStep = 0;

	private static AudioSource asDetonation = null;

	private static AudioClip fxSound = new AudioClip();

	private void Start()
	{
		CutoffFX.asDetonation = base.GetComponent<AudioSource>();
		CutoffFX.fxSound = SND.GetSoundByName("flashbang_fx");
		CutoffFX.phaseStep = 4;
	}

	public static void PlayDetonationSound()
	{
		CutoffFX.asDetonation.maxDistance = 50f;
		CutoffFX.asDetonation.rolloffMode = AudioRolloffMode.Linear;
		CutoffFX.asDetonation.volume = 0.5f * Options.gamevol * CutoffFX.currentCutoffVal;
		CutoffFX.asDetonation.spatialBlend = 1f;
		CutoffFX.asDetonation.PlayOneShot(CutoffFX.fxSound);
	}

	private static float AddFX()
	{
		float result = 22000f;
		AudioSource[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		AudioSource[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AudioSource audioSource = array2[i];
			if (!(audioSource.gameObject.name == "CutoffFX"))
			{
				AudioLowPassFilter audioLowPassFilter = audioSource.gameObject.GetComponent<AudioLowPassFilter>();
				if (audioLowPassFilter == null)
				{
					audioLowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
				}
				else
				{
					result = audioLowPassFilter.cutoffFrequency;
				}
				audioLowPassFilter.cutoffFrequency = 22000f;
			}
		}
		return result;
	}

	public static void RemoveFX()
	{
		CutoffFX.phaseStep = 4;
		AudioLowPassFilter[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioLowPassFilter)) as AudioLowPassFilter[];
		AudioLowPassFilter[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AudioLowPassFilter obj = array2[i];
			UnityEngine.Object.Destroy(obj);
		}
	}

	private void ChangeFXVal(float val)
	{
		AudioLowPassFilter[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioLowPassFilter)) as AudioLowPassFilter[];
		AudioLowPassFilter[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AudioLowPassFilter audioLowPassFilter = array2[i];
			float t = (val / (val + 0.2f) * 1f + 0.001f) * 1.19f;
			audioLowPassFilter.gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = Mathf.Lerp(CutoffFX.maxFreq, CutoffFX.minFreq, t);
		}
	}

	public static void SetFX(float maxFXVal, float incFreqVal)
	{
		CutoffFX.maxCutoffVal = maxFXVal;
		CutoffFX.phaseStep = 0;
		float num = CutoffFX.AddFX();
		CutoffFX.incFreq = 4f - incFreqVal;
		CutoffFX.PlayDetonationSound();
	}

	private void Update()
	{
		if (CutoffFX.phaseStep == 0)
		{
			CutoffFX.currentCutoffVal = 0f;
			this.ChangeFXVal(CutoffFX.currentCutoffVal);
			CutoffFX.phaseStep = 1;
		}
		if (CutoffFX.phaseStep == 1)
		{
			CutoffFX.currentCutoffVal += Time.deltaTime * 7f;
			if (CutoffFX.currentCutoffVal > 1f)
			{
				CutoffFX.currentCutoffVal = 1f;
				CutoffFX.phaseStep = 2;
			}
			if (CutoffFX.asDetonation != null)
			{
				CutoffFX.asDetonation.volume = 0.5f * Options.gamevol * CutoffFX.currentCutoffVal;
			}
			this.ChangeFXVal(CutoffFX.currentCutoffVal);
		}
		if (CutoffFX.phaseStep == 2)
		{
			CutoffFX.currentCutoffVal -= Time.deltaTime * CutoffFX.incFreq * 0.25f;
			if (CutoffFX.currentCutoffVal < 0f)
			{
				CutoffFX.currentCutoffVal = 0f;
				CutoffFX.phaseStep = 3;
			}
			if (CutoffFX.asDetonation != null)
			{
				CutoffFX.asDetonation.volume = 0.5f * Options.gamevol * CutoffFX.currentCutoffVal;
			}
			this.ChangeFXVal(CutoffFX.currentCutoffVal);
		}
		if (CutoffFX.phaseStep == 3)
		{
			if (CutoffFX.asDetonation != null)
			{
				CutoffFX.asDetonation.Stop();
			}
			CutoffFX.RemoveFX();
		}
	}
}
