using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class FBBIKHeadEffector : MonoBehaviour
	{
		[Serializable]
		public class BendBone
		{
			[Tooltip("Assign spine and/or neck bones.")]
			public Transform transform;

			[Range(0f, 1f), Tooltip("The weight of rotating this bone.")]
			public float weight = 0.5f;
		}

		[Tooltip("Reference to the FBBIK component.")]
		public FullBodyBipedIK ik;

		[Header("Position"), Range(0f, 1f), Tooltip("Master weight for positioning the head.")]
		public float positionWeight = 1f;

		[Range(0f, 1f), Tooltip("The weight of moving the body along with the head")]
		public float bodyWeight = 0.8f;

		[Range(0f, 1f), Tooltip("The weight of moving the thighs along with the head")]
		public float thighWeight = 0.8f;

		[Header("Rotation"), Range(0f, 1f), Tooltip("The weight of rotating the head bone after solving")]
		public float rotationWeight;

		[Range(0f, 1f), Tooltip("The master weight of bending/twisting the spine to the rotation of the head effector. This is similar to CCD, but uses the rotation of the head effector not the position.")]
		public float bendWeight = 1f;

		[Tooltip("The bones to use for bending.")]
		public FBBIKHeadEffector.BendBone[] bendBones;

		[Header("CCD"), Range(0f, 1f), Tooltip("Optional. The master weight of the CCD (Cyclic Coordinate Descent) IK effect that bends the spine towards the head effector before FBBIK solves.")]
		public float CCDWeight = 1f;

		[Range(0f, 1f), Tooltip("The weight of rolling the bones in towards the target")]
		public float roll;

		[Range(0f, 1000f), Tooltip("Smoothing the CCD effect.")]
		public float damper = 500f;

		[Tooltip("Bones to use for the CCD pass. Assign spine and/or neck bones.")]
		public Transform[] CCDBones;

		[Header("Stretching"), Range(0f, 1f), Tooltip("Stretching the spine/neck to help reach the target. This is useful for making sure the head stays locked relative to the VR headset. NB! Stretching is done after FBBIK has solved so if you have the hand effectors pinned and spine bones included in the 'Stretch Bones', the hands might become offset from their target positions.")]
		public float stretchWeight = 1f;

		[Tooltip("Stretch magnitude limit.")]
		public float maxStretch = 0.1f;

		[Tooltip("If > 0, dampers the stretching effect.")]
		public float stretchDamper;

		[Tooltip("If true, will fix head position to this Transform no matter what. Good for making sure the head will not budge away from the VR headset")]
		public bool fixHead;

		[Tooltip("Bones to use for stretching. The more bones you add, the less noticable the effect.")]
		public Transform[] stretchBones;

		private Vector3 offset;

		private Vector3 headToBody;

		private Vector3 shoulderCenterToHead;

		private Vector3 headToLeftThigh;

		private Vector3 headToRightThigh;

		private Vector3 leftShoulderPos;

		private Vector3 rightShoulderPos;

		private float shoulderDist;

		private float leftShoulderDist;

		private float rightShoulderDist;

		private Quaternion chestRotation;

		private Quaternion headRotationRelativeToRoot;

		private void Start()
		{
			IKSolverFullBodyBiped expr_0B = this.ik.solver;
			expr_0B.OnPreRead = (IKSolver.UpdateDelegate)Delegate.Combine(expr_0B.OnPreRead, new IKSolver.UpdateDelegate(this.OnPreRead));
			IKSolverFullBodyBiped expr_37 = this.ik.solver;
			expr_37.OnPreIteration = (IKSolver.IterationDelegate)Delegate.Combine(expr_37.OnPreIteration, new IKSolver.IterationDelegate(this.Iterate));
			IKSolverFullBodyBiped expr_63 = this.ik.solver;
			expr_63.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(expr_63.OnPostUpdate, new IKSolver.UpdateDelegate(this.OnPostUpdate));
			this.headRotationRelativeToRoot = Quaternion.Inverse(this.ik.references.root.rotation) * this.ik.references.head.rotation;
		}

		private void OnPreRead()
		{
			if (this.ik.solver.iterations == 0)
			{
				return;
			}
			this.SpineBend();
			this.CCDPass();
			this.offset = base.transform.position - this.ik.references.head.position;
			this.shoulderDist = Vector3.Distance(this.ik.references.leftUpperArm.position, this.ik.references.rightUpperArm.position);
			this.leftShoulderDist = Vector3.Distance(this.ik.references.head.position, this.ik.references.leftUpperArm.position);
			this.rightShoulderDist = Vector3.Distance(this.ik.references.head.position, this.ik.references.rightUpperArm.position);
			this.headToBody = this.ik.solver.rootNode.position - this.ik.references.head.position;
			this.headToLeftThigh = this.ik.references.leftThigh.position - this.ik.references.head.position;
			this.headToRightThigh = this.ik.references.rightThigh.position - this.ik.references.head.position;
			this.leftShoulderPos = this.ik.references.leftUpperArm.position + this.offset * this.bodyWeight;
			this.rightShoulderPos = this.ik.references.rightUpperArm.position + this.offset * this.bodyWeight;
			this.chestRotation = Quaternion.LookRotation(this.ik.references.head.position - this.ik.references.leftUpperArm.position, this.ik.references.rightUpperArm.position - this.ik.references.leftUpperArm.position);
		}

		private void SpineBend()
		{
			if (this.bendWeight <= 0f)
			{
				return;
			}
			if (this.bendBones.Length == 0)
			{
				return;
			}
			Quaternion b = base.transform.rotation * Quaternion.Inverse(this.ik.references.root.rotation * this.headRotationRelativeToRoot);
			float num = 1f / (float)this.bendBones.Length;
			for (int i = 0; i < this.bendBones.Length; i++)
			{
				if (this.bendBones[i].transform != null)
				{
					this.bendBones[i].transform.rotation = Quaternion.Lerp(Quaternion.identity, b, num * this.bendBones[i].weight * this.bendWeight) * this.bendBones[i].transform.rotation;
				}
			}
		}

		private void CCDPass()
		{
			if (this.CCDWeight <= 0f)
			{
				return;
			}
			for (int i = this.CCDBones.Length - 1; i > -1; i--)
			{
				Quaternion quaternion = Quaternion.FromToRotation(this.ik.references.head.position - this.CCDBones[i].position, base.transform.position - this.CCDBones[i].position) * this.CCDBones[i].rotation;
				float num = Mathf.Lerp((float)((this.CCDBones.Length - i) / this.CCDBones.Length), 1f, this.roll);
				float num2 = Quaternion.Angle(Quaternion.identity, quaternion);
				num2 = Mathf.Lerp(0f, num2, (this.damper - num2) / this.damper);
				this.CCDBones[i].rotation = Quaternion.RotateTowards(this.CCDBones[i].rotation, quaternion, num2 * this.CCDWeight * num);
			}
		}

		private void Iterate(int iteration)
		{
			if (this.ik.solver.iterations == 0)
			{
				return;
			}
			this.leftShoulderPos = base.transform.position + (this.leftShoulderPos - base.transform.position).normalized * this.leftShoulderDist;
			this.rightShoulderPos = base.transform.position + (this.rightShoulderPos - base.transform.position).normalized * this.rightShoulderDist;
			this.Solve(ref this.leftShoulderPos, ref this.rightShoulderPos, this.shoulderDist);
			this.LerpSolverPosition(this.ik.solver.leftShoulderEffector, this.leftShoulderPos, this.positionWeight);
			this.LerpSolverPosition(this.ik.solver.rightShoulderEffector, this.rightShoulderPos, this.positionWeight);
			Quaternion to = Quaternion.LookRotation(base.transform.position - this.leftShoulderPos, this.rightShoulderPos - this.leftShoulderPos);
			Quaternion quaternion = QuaTools.FromToRotation(this.chestRotation, to);
			Vector3 b = quaternion * this.headToBody;
			this.LerpSolverPosition(this.ik.solver.bodyEffector, base.transform.position + b, this.positionWeight);
			Quaternion rotation = Quaternion.Lerp(Quaternion.identity, quaternion, this.thighWeight);
			Vector3 b2 = rotation * this.headToLeftThigh;
			Vector3 b3 = rotation * this.headToRightThigh;
			this.LerpSolverPosition(this.ik.solver.leftThighEffector, base.transform.position + b2, this.positionWeight);
			this.LerpSolverPosition(this.ik.solver.rightThighEffector, base.transform.position + b3, this.positionWeight);
		}

		private void OnPostUpdate()
		{
			this.Stretching();
			this.ik.references.head.rotation = Quaternion.Lerp(this.ik.references.head.rotation, base.transform.rotation, this.rotationWeight);
		}

		private void Stretching()
		{
			if (this.stretchWeight <= 0f)
			{
				return;
			}
			Vector3 a = Vector3.ClampMagnitude(base.transform.position - this.ik.references.head.position, this.maxStretch);
			a *= this.stretchWeight;
			this.stretchDamper = Mathf.Max(this.stretchDamper, 0f);
			if (this.stretchDamper > 0f)
			{
				a /= (1f + a.magnitude) * (1f + this.stretchDamper);
			}
			for (int i = 0; i < this.stretchBones.Length; i++)
			{
				if (this.stretchBones[i] != null)
				{
					this.stretchBones[i].position += a / (float)this.stretchBones.Length;
				}
			}
			if (this.fixHead)
			{
				this.ik.references.head.position = base.transform.position;
			}
		}

		private void LerpSolverPosition(IKEffector effector, Vector3 position, float weight)
		{
			effector.GetNode().solverPosition = Vector3.Lerp(effector.GetNode().solverPosition, position, weight);
		}

		private void Solve(ref Vector3 pos1, ref Vector3 pos2, float nominalDistance)
		{
			Vector3 a = pos2 - pos1;
			float magnitude = a.magnitude;
			if (magnitude == nominalDistance)
			{
				return;
			}
			if (magnitude == 0f)
			{
				return;
			}
			float num = 1f;
			num *= 1f - nominalDistance / magnitude;
			Vector3 b = a * num * 0.5f;
			pos1 += b;
			pos2 -= b;
		}

		private void OnDestroy()
		{
			if (this.ik != null)
			{
				IKSolverFullBodyBiped expr_1C = this.ik.solver;
				expr_1C.OnPreRead = (IKSolver.UpdateDelegate)Delegate.Remove(expr_1C.OnPreRead, new IKSolver.UpdateDelegate(this.OnPreRead));
				IKSolverFullBodyBiped expr_48 = this.ik.solver;
				expr_48.OnPreIteration = (IKSolver.IterationDelegate)Delegate.Remove(expr_48.OnPreIteration, new IKSolver.IterationDelegate(this.Iterate));
				IKSolverFullBodyBiped expr_74 = this.ik.solver;
				expr_74.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(expr_74.OnPostUpdate, new IKSolver.UpdateDelegate(this.OnPostUpdate));
			}
		}
	}
}
