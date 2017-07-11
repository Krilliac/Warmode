using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class Recoil : OffsetModifier
	{
		[Serializable]
		public class RecoilOffset
		{
			[Serializable]
			public class EffectorLink
			{
				[Tooltip("Type of the FBBIK effector to use")]
				public FullBodyBipedEffector effector;

				[Tooltip("Weight of using this effector")]
				public float weight;
			}

			[Tooltip("Offset vector for the associated effector when doing recoil.")]
			public Vector3 offset;

			[Tooltip("Linking this recoil offset to FBBIK effectors.")]
			public Recoil.RecoilOffset.EffectorLink[] effectorLinks;

			public void Apply(IKSolverFullBodyBiped solver, Quaternion rotation, float masterWeight)
			{
				Recoil.RecoilOffset.EffectorLink[] array = this.effectorLinks;
				for (int i = 0; i < array.Length; i++)
				{
					Recoil.RecoilOffset.EffectorLink effectorLink = array[i];
					solver.GetEffector(effectorLink.effector).positionOffset += rotation * (this.offset * masterWeight * effectorLink.weight);
				}
			}
		}

		[Serializable]
		public enum Handedness
		{
			Right,
			Left
		}

		[Tooltip("Reference to the AimIK component. Optional, only used to getting the aiming direction.")]
		public AimIK aimIK;

		[Tooltip("Which hand is holding the weapon?")]
		public Recoil.Handedness handedness;

		[Tooltip("Check for 2-handed weapons.")]
		public bool twoHanded = true;

		[Tooltip("Weight curve for the recoil offsets. Recoil procedure is as long as this curve.")]
		public AnimationCurve recoilWeight;

		[Tooltip("How much is the magnitude randomized each time Recoil is called?")]
		public float magnitudeRandom = 0.1f;

		[Tooltip("How much is the rotation randomized each time Recoil is called?")]
		public Vector3 rotationRandom;

		[Space(10f), Tooltip("Rotating the primary hand bone for the recoil (in local space).")]
		public Vector3 handRotationOffset;

		[Tooltip("FBBIK effector position offsets for the recoil (in aiming direction space).")]
		public Recoil.RecoilOffset[] offsets;

		private float magnitudeMlp = 1f;

		private float endTime = -1f;

		private Quaternion handRotation;

		private Quaternion secondaryHandRelativeRotation;

		private Quaternion randomRotation;

		private IKEffector primaryHandEffector
		{
			get
			{
				if (this.handedness == Recoil.Handedness.Right)
				{
					return this.ik.solver.rightHandEffector;
				}
				return this.ik.solver.leftHandEffector;
			}
		}

		private IKEffector secondaryHandEffector
		{
			get
			{
				if (this.handedness == Recoil.Handedness.Right)
				{
					return this.ik.solver.leftHandEffector;
				}
				return this.ik.solver.rightHandEffector;
			}
		}

		private Transform primaryHand
		{
			get
			{
				return this.primaryHandEffector.bone;
			}
		}

		private Transform secondaryHand
		{
			get
			{
				return this.secondaryHandEffector.bone;
			}
		}

		public void Fire(float magnitude)
		{
			float num = magnitude * UnityEngine.Random.value * this.magnitudeRandom;
			this.magnitudeMlp = magnitude + num;
			this.randomRotation = Quaternion.Euler(this.rotationRandom * UnityEngine.Random.value);
			this.endTime = Time.time + 1f;
		}

		protected override void Start()
		{
			base.Start();
			IKSolverFullBodyBiped expr_11 = this.ik.solver;
			expr_11.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(expr_11.OnPostUpdate, new IKSolver.UpdateDelegate(this.AfterFBBIK));
		}

		protected override void OnModifyOffset()
		{
			if (Time.time >= this.endTime)
			{
				return;
			}
			float num = this.recoilWeight.Evaluate(1f - (this.endTime - Time.time)) * this.magnitudeMlp;
			Quaternion quaternion = (!(this.aimIK != null)) ? this.ik.references.root.rotation : Quaternion.LookRotation(this.aimIK.solver.IKPosition - this.aimIK.solver.transform.position, this.ik.references.root.up);
			quaternion = this.randomRotation * quaternion;
			Recoil.RecoilOffset[] array = this.offsets;
			for (int i = 0; i < array.Length; i++)
			{
				Recoil.RecoilOffset recoilOffset = array[i];
				recoilOffset.Apply(this.ik.solver, quaternion, num);
			}
			Quaternion lhs = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(this.randomRotation * this.primaryHand.rotation * this.handRotationOffset), num);
			this.handRotation = lhs * this.primaryHand.rotation;
			if (this.twoHanded)
			{
				Vector3 point = this.primaryHand.InverseTransformPoint(this.secondaryHand.position);
				this.secondaryHandRelativeRotation = Quaternion.Inverse(this.primaryHand.rotation) * this.secondaryHand.rotation;
				Vector3 a = this.primaryHand.position + this.primaryHandEffector.positionOffset;
				Vector3 a2 = a + this.handRotation * point;
				this.secondaryHandEffector.positionOffset += a2 - (this.secondaryHand.position + this.secondaryHandEffector.positionOffset);
			}
		}

		private void AfterFBBIK()
		{
			if (Time.time >= this.endTime)
			{
				return;
			}
			this.primaryHand.rotation = this.handRotation;
			if (this.twoHanded)
			{
				this.secondaryHand.rotation = this.primaryHand.rotation * this.secondaryHandRelativeRotation;
			}
		}
	}
}
