using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class SolverManager : MonoBehaviour
	{
		public float timeStep;

		public bool fixTransforms = true;

		private float lastTime;

		private Animator animator;

		private Animation animation;

		private bool updateFrame;

		private bool componentInitiated;

		private bool animatePhysics
		{
			get
			{
				if (this.animator != null)
				{
					return this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
				}
				return this.animation != null && this.animation.animatePhysics;
			}
		}

		private bool isAnimated
		{
			get
			{
				return this.animator != null || this.animation != null;
			}
		}

		public void Disable()
		{
			this.Initiate();
			base.enabled = false;
		}

		protected virtual void InitiateSolver()
		{
		}

		protected virtual void UpdateSolver()
		{
		}

		protected virtual void FixTransforms()
		{
		}

		private void Start()
		{
			this.Initiate();
		}

		private void Update()
		{
			if (this.animatePhysics)
			{
				return;
			}
			if (this.fixTransforms)
			{
				this.FixTransforms();
			}
		}

		private void Initiate()
		{
			if (this.componentInitiated)
			{
				return;
			}
			this.FindAnimatorRecursive(base.transform, true);
			this.InitiateSolver();
			this.componentInitiated = true;
		}

		private void FindAnimatorRecursive(Transform t, bool findInChildren)
		{
			if (this.isAnimated)
			{
				return;
			}
			this.animator = t.GetComponent<Animator>();
			this.animation = t.GetComponent<Animation>();
			if (this.isAnimated)
			{
				return;
			}
			if (this.animator == null && findInChildren)
			{
				this.animator = t.GetComponentInChildren<Animator>();
			}
			if (this.animation == null && findInChildren)
			{
				this.animation = t.GetComponentInChildren<Animation>();
			}
			if (!this.isAnimated && t.parent != null)
			{
				this.FindAnimatorRecursive(t.parent, false);
			}
		}

		private void FixedUpdate()
		{
			this.updateFrame = true;
			if (this.animatePhysics && this.fixTransforms)
			{
				this.FixTransforms();
			}
		}

		private void LateUpdate()
		{
			if (!this.animatePhysics)
			{
				this.updateFrame = true;
			}
			if (!this.updateFrame)
			{
				return;
			}
			this.updateFrame = false;
			if (this.timeStep == 0f)
			{
				this.UpdateSolver();
			}
			else if (Time.time >= this.lastTime + this.timeStep)
			{
				this.UpdateSolver();
				this.lastTime = Time.time;
			}
		}
	}
}
