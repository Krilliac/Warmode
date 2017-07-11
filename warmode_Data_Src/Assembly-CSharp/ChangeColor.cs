using System;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	public float GammaCorrection = 1f;

	private void Start()
	{
		RenderSettings.ambientIntensity = this.GammaCorrection;
	}
}
