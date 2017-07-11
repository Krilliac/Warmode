using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public abstract class OffsetModifier : MonoBehaviour
	{
		[Serializable]
		public class OffsetLimits
		{
			[Tooltip("The effector type (this is just an enum)")]
			public FullBodyBipedEffector effector;

			[Tooltip("Spring force, if zero then this is a hard limit, if not, offset can exceed the limit.")]
			public float spring;

			[Tooltip("Which axes to limit the offset on?")]
			public bool x;

			[Tooltip("Which axes to limit the offset on?")]
			public bool y;

			[Tooltip("Which axes to limit the offset on?")]
			public bool z;

			[Tooltip("The limits")]
			public float minX;

			[Tooltip("The limits")]
			public float maxX;

			[Tooltip("The limits")]
			public float minY;

			[Tooltip("The limits")]
			public float maxY;

			[Tooltip("The limits")]
			public float minZ;

			[Tooltip("The limits")]
			public float maxZ;

			public void Apply(IKEffector e, Quaternion rootRotation)
			{
				Vector3 point = Quaternion.Inverse(rootRotation) * e.positionOffset;
				if (this.spring <= 0f)
				{
					if (this.x)
					{
						point.x = Mathf.Clamp(point.x, this.minX, this.maxX);
					}
					if (this.y)
					{
						point.y = Mathf.Clamp(point.y, this.minY, this.maxY);
					}
					if (this.z)
					{
						point.z = Mathf.Clamp(point.z, this.minZ, this.maxZ);
					}
				}
				else
				{
					if (this.x)
					{
						point.x = this.SpringAxis(point.x, this.minX, this.maxX);
					}
					if (this.y)
					{
						point.y = this.SpringAxis(point.y, this.minY, this.maxY);
					}
					if (this.z)
					{
						point.z = this.SpringAxis(point.z, this.minZ, this.maxZ);
					}
				}
				e.positionOffset = rootRotation * point;
			}

			private float SpringAxis(float value, float min, float max)
			{
				if (value > min && value < max)
				{
					return value;
				}
				if (value < min)
				{
					return this.Spring(value, min, true);
				}
				return this.Spring(value, max, false);
			}

			private float Spring(float value, float limit, bool negative)
			{
				float num = value - limit;
				float num2 = num * this.spring;
				if (negative)
				{
					return value + Mathf.Clamp(-num2, 0f, -num);
				}
				return value - Mathf.Clamp(num2, 0f, num);
			}
		}

		[Tooltip("The master weight")]
		public float weight = 1f;

		[SerializeField, Tooltip("Reference to the FBBIK component")]
		protected FullBodyBipedIK ik;

		private float lastTime;

		protected float deltaTime
		{
			get
			{
				return Time.time - this.lastTime;
			}
		}

		protected abstract void OnModifyOffset();

		protected virtual void Start()
		{
			base.StartCoroutine(this.Initiate());
		}

		[DebuggerHidden]
		private IEnumerator Initiate()
		{
			OffsetModifier.<Initiate>c__Iterator1A <Initiate>c__Iterator1A = new OffsetModifier.<Initiate>c__Iterator1A();
			<Initiate>c__Iterator1A.<>f__this = this;
			return <Initiate>c__Iterator1A;
		}

		private void ModifyOffset()
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.weight <= 0f)
			{
				return;
			}
			if (this.deltaTime <= 0f)
			{
				return;
			}
			if (this.ik == null)
			{
				return;
			}
			this.weight = Mathf.Clamp(this.weight, 0f, 1f);
			this.OnModifyOffset();
			this.lastTime = Time.time;
		}

		protected void ApplyLimits(OffsetModifier.OffsetLimits[] limits)
		{
			for (int i = 0; i < limits.Length; i++)
			{
				OffsetModifier.OffsetLimits offsetLimits = limits[i];
				offsetLimits.Apply(this.ik.solver.GetEffector(offsetLimits.effector), base.transform.rotation);
			}
		}

		private void OnDestroy()
		{
			if (this.ik != null)
			{
				IKSolverFullBodyBiped expr_1C = this.ik.solver;
				expr_1C.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(expr_1C.OnPreUpdate, new IKSolver.UpdateDelegate(this.ModifyOffset));
			}
		}
	}
}
