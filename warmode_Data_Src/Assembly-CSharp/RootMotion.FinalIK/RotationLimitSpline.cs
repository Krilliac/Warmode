using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Spline")]
	public class RotationLimitSpline : RotationLimit
	{
		[Range(0f, 180f)]
		public float twistLimit = 180f;

		[HideInInspector, SerializeField]
		public AnimationCurve spline;

		public void SetSpline(Keyframe[] keyframes)
		{
			this.spline.keys = keyframes;
		}

		protected override Quaternion LimitRotation(Quaternion rotation)
		{
			Quaternion rotation2 = this.LimitSwing(rotation);
			return RotationLimit.LimitTwist(rotation2, this.axis, base.secondaryAxis, this.twistLimit);
		}

		public Quaternion LimitSwing(Quaternion rotation)
		{
			if (this.axis == Vector3.zero)
			{
				return rotation;
			}
			if (rotation == Quaternion.identity)
			{
				return rotation;
			}
			Vector3 vector = rotation * this.axis;
			float num = RotationLimit.GetOrthogonalAngle(vector, base.secondaryAxis, this.axis);
			float num2 = Vector3.Dot(vector, base.crossAxis);
			if (num2 < 0f)
			{
				num = 180f + (180f - num);
			}
			float maxDegreesDelta = this.spline.Evaluate(num);
			Quaternion to = Quaternion.FromToRotation(this.axis, vector);
			Quaternion rotation2 = Quaternion.RotateTowards(Quaternion.identity, to, maxDegreesDelta);
			Quaternion lhs = Quaternion.FromToRotation(vector, rotation2 * this.axis);
			return lhs * rotation;
		}

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_spline.html");
		}
	}
}
