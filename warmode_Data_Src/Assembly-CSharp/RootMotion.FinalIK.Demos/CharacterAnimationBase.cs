using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public abstract class CharacterAnimationBase : MonoBehaviour
	{
		public virtual bool animationGrounded
		{
			get
			{
				return true;
			}
		}

		public virtual Vector3 GetPivotPoint()
		{
			return base.transform.position;
		}

		public float GetAngleFromForward(Vector3 worldDirection)
		{
			Vector3 vector = base.transform.InverseTransformDirection(worldDirection);
			return Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		}
	}
}
