using System;
using UnityEngine;

public class vp_AnimEvent : MonoBehaviour
{
	public void Shake(int param)
	{
		if (param == 0)
		{
			vp_FPCamera.cs.AddRotationForce(Vector2.right * -0.05f);
		}
		else if (param == 2)
		{
			vp_FPCamera.cs.AddRotationForce(Vector2.right * 0.025f);
		}
	}
}
