using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Full Body Biped")]
	public class GrounderFBBIK : Grounder
	{
		[Serializable]
		public class SpineEffector
		{
			[Tooltip("The type of the effector.")]
			public FullBodyBipedEffector effectorType;

			[Tooltip("The weight of horizontal bend offset towards the slope.")]
			public float horizontalWeight = 1f;

			[Tooltip("The vertical bend offset weight.")]
			public float verticalWeight;
		}

		[Tooltip("Reference to the FBBIK componet.")]
		public FullBodyBipedIK ik;

		[Tooltip("The amount of spine bending towards upward slopes.")]
		public float spineBend = 2f;

		[Tooltip("The interpolation speed of spine bending.")]
		public float spineSpeed = 3f;

		[Tooltip("The spine bending effectors.")]
		public GrounderFBBIK.SpineEffector[] spine = new GrounderFBBIK.SpineEffector[0];

		private Transform[] feet = new Transform[2];

		private Vector3 spineOffset;

		private bool firstSolve;

		private bool IsReadyToInitiate()
		{
			return !(this.ik == null) && this.ik.solver.initiated;
		}

		private void Update()
		{
			this.firstSolve = true;
			this.weight = Mathf.Clamp(this.weight, 0f, 1f);
			if (this.weight <= 0f)
			{
				return;
			}
			if (this.initiated)
			{
				return;
			}
			if (!this.IsReadyToInitiate())
			{
				return;
			}
			this.Initiate();
		}

		private void Initiate()
		{
			this.ik.solver.leftLegMapping.maintainRotationWeight = 1f;
			this.ik.solver.rightLegMapping.maintainRotationWeight = 1f;
			this.feet = new Transform[2];
			this.feet[0] = this.ik.solver.leftFootEffector.bone;
			this.feet[1] = this.ik.solver.rightFootEffector.bone;
			IKSolverFullBodyBiped expr_85 = this.ik.solver;
			expr_85.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(expr_85.OnPreUpdate, new IKSolver.UpdateDelegate(this.OnSolverUpdate));
			this.solver.Initiate(this.ik.references.root, this.feet);
			this.initiated = true;
		}

		private void OnSolverUpdate()
		{
			if (!this.firstSolve)
			{
				return;
			}
			this.firstSolve = false;
			if (!base.enabled)
			{
				return;
			}
			if (this.weight <= 0f)
			{
				return;
			}
			if (this.OnPreGrounder != null)
			{
				this.OnPreGrounder();
			}
			this.solver.Update();
			this.ik.references.pelvis.position += this.solver.pelvis.IKOffset * this.weight;
			this.SetLegIK(this.ik.solver.leftFootEffector, this.solver.legs[0]);
			this.SetLegIK(this.ik.solver.rightFootEffector, this.solver.legs[1]);
			if (this.spineBend != 0f)
			{
				this.spineSpeed = Mathf.Clamp(this.spineSpeed, 0f, this.spineSpeed);
				Vector3 a = base.GetSpineOffsetTarget() * this.weight;
				this.spineOffset = Vector3.Lerp(this.spineOffset, a * this.spineBend, Time.deltaTime * this.spineSpeed);
				Vector3 a2 = this.ik.references.root.up * this.spineOffset.magnitude;
				for (int i = 0; i < this.spine.Length; i++)
				{
					this.ik.solver.GetEffector(this.spine[i].effectorType).positionOffset += this.spineOffset * this.spine[i].horizontalWeight + a2 * this.spine[i].verticalWeight;
				}
			}
			if (this.OnPostGrounder != null)
			{
				this.OnPostGrounder();
			}
		}

		private void SetLegIK(IKEffector effector, Grounding.Leg leg)
		{
			effector.positionOffset += (leg.IKPosition - effector.bone.position) * this.weight;
			effector.bone.rotation = Quaternion.Slerp(Quaternion.identity, leg.rotationOffset, this.weight) * effector.bone.rotation;
		}

		private void OnDestroy()
		{
			if (this.initiated && this.ik != null)
			{
				IKSolverFullBodyBiped expr_27 = this.ik.solver;
				expr_27.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(expr_27.OnPreUpdate, new IKSolver.UpdateDelegate(this.OnSolverUpdate));
			}
		}

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_f_b_b_i_k.html");
		}
	}
}
