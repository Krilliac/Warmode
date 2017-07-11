using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class OffsetEffector : OffsetModifier
	{
		[Serializable]
		public class EffectorLink
		{
			public FullBodyBipedEffector effectorType;

			public float weightMultiplier = 1f;

			[HideInInspector]
			public Vector3 localPosition;
		}

		public OffsetEffector.EffectorLink[] effectorLinks;

		protected override void Start()
		{
			base.Start();
			OffsetEffector.EffectorLink[] array = this.effectorLinks;
			for (int i = 0; i < array.Length; i++)
			{
				OffsetEffector.EffectorLink effectorLink = array[i];
				effectorLink.localPosition = base.transform.InverseTransformPoint(this.ik.solver.GetEffector(effectorLink.effectorType).bone.position);
				if (effectorLink.effectorType == FullBodyBipedEffector.Body)
				{
					this.ik.solver.bodyEffector.effectChildNodes = false;
				}
			}
		}

		protected override void OnModifyOffset()
		{
			OffsetEffector.EffectorLink[] array = this.effectorLinks;
			for (int i = 0; i < array.Length; i++)
			{
				OffsetEffector.EffectorLink effectorLink = array[i];
				Vector3 a = base.transform.TransformPoint(effectorLink.localPosition);
				this.ik.solver.GetEffector(effectorLink.effectorType).positionOffset += (a - (this.ik.solver.GetEffector(effectorLink.effectorType).bone.position + this.ik.solver.GetEffector(effectorLink.effectorType).positionOffset)) * this.weight * effectorLink.weightMultiplier;
			}
		}
	}
}
