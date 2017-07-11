using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class MechSpiderLeg : MonoBehaviour
	{
		public MechSpider mechSpider;

		public MechSpiderLeg unSync;

		public Vector3 offset;

		public float minDelay = 0.2f;

		public float maxOffset = 1f;

		public float stepSpeed = 5f;

		public float footHeight = 0.15f;

		public float velocityPrediction = 0.2f;

		public float raycastFocus = 0.1f;

		public AnimationCurve yOffset;

		public ParticleSystem sand;

		private IK ik;

		private float stepProgress = 1f;

		private float lastStepTime;

		private Vector3 defaultPosition;

		private RaycastHit hit = default(RaycastHit);

		public bool isStepping
		{
			get
			{
				return this.stepProgress < 1f;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.ik.GetIKSolver().GetIKPosition();
			}
			set
			{
				this.ik.GetIKSolver().SetIKPosition(value);
			}
		}

		private void Start()
		{
			this.ik = base.GetComponent<IK>();
			this.stepProgress = 1f;
			this.hit = default(RaycastHit);
			IKSolver.Point[] points = this.ik.GetIKSolver().GetPoints();
			this.position = points[points.Length - 1].transform.position;
			this.hit.point = this.position;
			this.defaultPosition = this.mechSpider.transform.InverseTransformPoint(this.position + this.offset);
		}

		private Vector3 GetStepTarget(out bool stepFound, float focus, float distance)
		{
			stepFound = false;
			Vector3 a = this.mechSpider.transform.TransformPoint(this.defaultPosition);
			a += (this.hit.point - this.position) * this.velocityPrediction;
			Vector3 vector = this.mechSpider.transform.up;
			Vector3 rhs = this.mechSpider.body.position - this.position;
			Vector3 axis = Vector3.Cross(vector, rhs);
			vector = Quaternion.AngleAxis(focus, axis) * vector;
			if (Physics.Raycast(a + vector * this.mechSpider.raycastHeight, -vector, out this.hit, this.mechSpider.raycastHeight + distance, this.mechSpider.raycastLayers))
			{
				stepFound = true;
			}
			return this.hit.point + this.mechSpider.transform.up * this.footHeight;
		}

		private void Update()
		{
			if (this.isStepping)
			{
				return;
			}
			if (Time.time < this.lastStepTime + this.minDelay)
			{
				return;
			}
			if (this.unSync != null && this.unSync.isStepping)
			{
				return;
			}
			bool flag = false;
			Vector3 stepTarget = this.GetStepTarget(out flag, this.raycastFocus, this.mechSpider.raycastDistance);
			if (!flag)
			{
				stepTarget = this.GetStepTarget(out flag, -this.raycastFocus, this.mechSpider.raycastDistance * 3f);
			}
			if (!flag)
			{
				return;
			}
			if (Vector3.Distance(this.position, stepTarget) < this.maxOffset * UnityEngine.Random.Range(0.9f, 1.2f))
			{
				return;
			}
			base.StopAllCoroutines();
			base.StartCoroutine(this.Step(this.position, stepTarget));
		}

		[DebuggerHidden]
		private IEnumerator Step(Vector3 stepStartPosition, Vector3 targetPosition)
		{
			MechSpiderLeg.<Step>c__Iterator19 <Step>c__Iterator = new MechSpiderLeg.<Step>c__Iterator19();
			<Step>c__Iterator.stepStartPosition = stepStartPosition;
			<Step>c__Iterator.targetPosition = targetPosition;
			<Step>c__Iterator.<$>stepStartPosition = stepStartPosition;
			<Step>c__Iterator.<$>targetPosition = targetPosition;
			<Step>c__Iterator.<>f__this = this;
			return <Step>c__Iterator;
		}
	}
}
