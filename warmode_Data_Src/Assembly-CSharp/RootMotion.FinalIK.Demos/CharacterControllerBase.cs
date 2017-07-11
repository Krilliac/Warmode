using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public abstract class CharacterControllerBase : MonoBehaviour
	{
		protected Vector3 GetInputDirection()
		{
			Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			vector.z += Mathf.Abs(vector.x) * 0.05f;
			vector.x -= Mathf.Abs(vector.z) * 0.05f;
			return vector.normalized;
		}
	}
}
