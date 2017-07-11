using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public abstract class PickUp2Handed : MonoBehaviour
	{
		[SerializeField]
		private int GUIspace;

		public InteractionSystem interactionSystem;

		public InteractionObject obj;

		public Transform pivot;

		public Transform holdPoint;

		public float pickUpTime = 0.3f;

		private float holdWeight;

		private float holdWeightVel;

		private Vector3 pickUpPosition;

		private Quaternion pickUpRotation;

		private bool holding
		{
			get
			{
				return this.interactionSystem.IsPaused(FullBodyBipedEffector.LeftHand);
			}
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space((float)this.GUIspace);
			if (!this.holding)
			{
				if (GUILayout.Button("Pick Up " + this.obj.name, new GUILayoutOption[0]))
				{
					this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, this.obj, false);
					this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, this.obj, false);
				}
			}
			else if (GUILayout.Button("Drop " + this.obj.name, new GUILayoutOption[0]))
			{
				this.interactionSystem.ResumeAll();
			}
			GUILayout.EndHorizontal();
		}

		protected abstract void RotatePivot();

		private void Start()
		{
			InteractionSystem expr_06 = this.interactionSystem;
			expr_06.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Combine(expr_06.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.OnStart));
			InteractionSystem expr_2D = this.interactionSystem;
			expr_2D.OnInteractionPause = (InteractionSystem.InteractionDelegate)Delegate.Combine(expr_2D.OnInteractionPause, new InteractionSystem.InteractionDelegate(this.OnPause));
			InteractionSystem expr_54 = this.interactionSystem;
			expr_54.OnInteractionResume = (InteractionSystem.InteractionDelegate)Delegate.Combine(expr_54.OnInteractionResume, new InteractionSystem.InteractionDelegate(this.OnDrop));
		}

		private void OnPause(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
		{
			if (effectorType != FullBodyBipedEffector.LeftHand)
			{
				return;
			}
			if (interactionObject != this.obj)
			{
				return;
			}
			this.obj.transform.parent = this.interactionSystem.transform;
			if (this.obj.GetComponent<Rigidbody>() != null)
			{
				this.obj.GetComponent<Rigidbody>().isKinematic = true;
			}
			this.pickUpPosition = this.obj.transform.position;
			this.pickUpRotation = this.obj.transform.rotation;
			this.holdWeight = 0f;
			this.holdWeightVel = 0f;
		}

		private void OnStart(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
		{
			if (effectorType != FullBodyBipedEffector.LeftHand)
			{
				return;
			}
			if (interactionObject != this.obj)
			{
				return;
			}
			this.RotatePivot();
			this.holdPoint.rotation = this.obj.transform.rotation;
		}

		private void OnDrop(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
		{
			if (effectorType != FullBodyBipedEffector.LeftHand)
			{
				return;
			}
			if (interactionObject != this.obj)
			{
				return;
			}
			this.obj.transform.parent = null;
			if (this.obj.GetComponent<Rigidbody>() != null)
			{
				this.obj.GetComponent<Rigidbody>().isKinematic = false;
			}
		}

		private void LateUpdate()
		{
			if (this.holding)
			{
				this.holdWeight = Mathf.SmoothDamp(this.holdWeight, 1f, ref this.holdWeightVel, this.pickUpTime);
				this.obj.transform.position = Vector3.Lerp(this.pickUpPosition, this.holdPoint.position, this.holdWeight);
				this.obj.transform.rotation = Quaternion.Lerp(this.pickUpRotation, this.holdPoint.rotation, this.holdWeight);
			}
		}

		private void OnDestroy()
		{
			if (this.interactionSystem == null)
			{
				return;
			}
			InteractionSystem expr_18 = this.interactionSystem;
			expr_18.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Remove(expr_18.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.OnStart));
			InteractionSystem expr_3F = this.interactionSystem;
			expr_3F.OnInteractionPause = (InteractionSystem.InteractionDelegate)Delegate.Remove(expr_3F.OnInteractionPause, new InteractionSystem.InteractionDelegate(this.OnPause));
			InteractionSystem expr_66 = this.interactionSystem;
			expr_66.OnInteractionResume = (InteractionSystem.InteractionDelegate)Delegate.Remove(expr_66.OnInteractionResume, new InteractionSystem.InteractionDelegate(this.OnDrop));
		}
	}
}
