using System;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class InteractionEffector
	{
		private Poser poser;

		private IKEffector effector;

		private float timer;

		private float length;

		private float weight;

		private float fadeInSpeed;

		private float defaultPull;

		private float defaultReach;

		private float defaultPush;

		private float defaultPushParent;

		private float resetTimer;

		private bool pickedUp;

		private bool defaults;

		private bool pickUpOnPostFBBIK;

		private Vector3 pickUpPosition;

		private Vector3 pausePositionRelative;

		private Quaternion pickUpRotation;

		private Quaternion pauseRotationRelative;

		private InteractionTarget interactionTarget;

		private Transform target;

		private List<bool> triggered = new List<bool>();

		private InteractionSystem interactionSystem;

		public FullBodyBipedEffector effectorType
		{
			get;
			private set;
		}

		public bool isPaused
		{
			get;
			private set;
		}

		public InteractionObject interactionObject
		{
			get;
			private set;
		}

		public bool inInteraction
		{
			get
			{
				return this.interactionObject != null;
			}
		}

		public float progress
		{
			get
			{
				if (!this.inInteraction)
				{
					return 0f;
				}
				if (this.length == 0f)
				{
					return 0f;
				}
				return this.timer / this.length;
			}
		}

		public InteractionEffector(FullBodyBipedEffector effectorType)
		{
			this.effectorType = effectorType;
		}

		public void Initiate(InteractionSystem interactionSystem, IKSolverFullBodyBiped solver)
		{
			this.interactionSystem = interactionSystem;
			if (this.effector == null)
			{
				this.effector = solver.GetEffector(this.effectorType);
				this.poser = this.effector.bone.GetComponent<Poser>();
			}
			this.defaultPull = solver.GetChain(this.effectorType).pull;
			this.defaultReach = solver.GetChain(this.effectorType).reach;
			this.defaultPush = solver.GetChain(this.effectorType).push;
			this.defaultPushParent = solver.GetChain(this.effectorType).pushParent;
		}

		public bool ResetToDefaults(IKSolverFullBodyBiped solver, float speed)
		{
			if (this.inInteraction)
			{
				return false;
			}
			if (this.isPaused)
			{
				return false;
			}
			if (this.defaults)
			{
				return false;
			}
			this.resetTimer = Mathf.Clamp(this.resetTimer -= Time.deltaTime * speed, 0f, 1f);
			if (this.effector.isEndEffector)
			{
				solver.GetChain(this.effectorType).pull = Mathf.Lerp(this.defaultPull, solver.GetChain(this.effectorType).pull, this.resetTimer);
				solver.GetChain(this.effectorType).reach = Mathf.Lerp(this.defaultReach, solver.GetChain(this.effectorType).reach, this.resetTimer);
				solver.GetChain(this.effectorType).push = Mathf.Lerp(this.defaultPush, solver.GetChain(this.effectorType).push, this.resetTimer);
				solver.GetChain(this.effectorType).pushParent = Mathf.Lerp(this.defaultPushParent, solver.GetChain(this.effectorType).pushParent, this.resetTimer);
			}
			this.effector.positionWeight = Mathf.Lerp(0f, this.effector.positionWeight, this.resetTimer);
			this.effector.rotationWeight = Mathf.Lerp(0f, this.effector.rotationWeight, this.resetTimer);
			if (this.resetTimer <= 0f)
			{
				this.defaults = true;
			}
			return true;
		}

		public bool Pause()
		{
			if (!this.inInteraction)
			{
				return false;
			}
			this.isPaused = true;
			this.pausePositionRelative = this.target.InverseTransformPoint(this.effector.position);
			this.pauseRotationRelative = Quaternion.Inverse(this.target.rotation) * this.effector.rotation;
			if (this.interactionSystem.OnInteractionPause != null)
			{
				this.interactionSystem.OnInteractionPause(this.effectorType, this.interactionObject);
			}
			return true;
		}

		public bool Resume()
		{
			if (!this.inInteraction)
			{
				return false;
			}
			this.isPaused = false;
			if (this.interactionSystem.OnInteractionResume != null)
			{
				this.interactionSystem.OnInteractionResume(this.effectorType, this.interactionObject);
			}
			return true;
		}

		public bool Start(InteractionObject interactionObject, string tag, float fadeInTime, bool interrupt)
		{
			if (!this.inInteraction)
			{
				this.effector.position = this.effector.bone.position;
				this.effector.rotation = this.effector.bone.rotation;
			}
			else if (!interrupt)
			{
				return false;
			}
			this.target = interactionObject.GetTarget(this.effectorType, tag);
			if (this.target == null)
			{
				return false;
			}
			this.interactionTarget = this.target.GetComponent<InteractionTarget>();
			this.interactionObject = interactionObject;
			if (this.interactionSystem.OnInteractionStart != null)
			{
				this.interactionSystem.OnInteractionStart(this.effectorType, interactionObject);
			}
			interactionObject.OnStartInteraction(this.interactionSystem);
			this.triggered.Clear();
			for (int i = 0; i < interactionObject.events.Length; i++)
			{
				this.triggered.Add(false);
			}
			if (this.poser != null)
			{
				if (this.poser.poseRoot == null)
				{
					this.poser.weight = 0f;
				}
				if (this.interactionTarget != null)
				{
					this.poser.poseRoot = this.target.transform;
				}
				else
				{
					this.poser.poseRoot = null;
				}
				this.poser.AutoMapping();
			}
			this.timer = 0f;
			this.weight = 0f;
			this.fadeInSpeed = ((fadeInTime <= 0f) ? 1000f : (1f / fadeInTime));
			this.length = interactionObject.length;
			this.isPaused = false;
			this.pickedUp = false;
			this.pickUpPosition = Vector3.zero;
			this.pickUpRotation = Quaternion.identity;
			if (this.interactionTarget != null)
			{
				this.interactionTarget.RotateTo(this.effector.bone.position);
			}
			return true;
		}

		public void Update(Transform root, IKSolverFullBodyBiped solver, float speed)
		{
			if (!this.inInteraction)
			{
				return;
			}
			if (this.interactionTarget != null && !this.interactionTarget.rotateOnce)
			{
				this.interactionTarget.RotateTo(this.effector.bone.position);
			}
			if (this.isPaused)
			{
				this.effector.position = this.target.TransformPoint(this.pausePositionRelative);
				this.effector.rotation = this.target.rotation * this.pauseRotationRelative;
				this.interactionObject.Apply(solver, this.effectorType, this.interactionTarget, this.timer, this.weight);
				return;
			}
			this.timer += Time.deltaTime * speed * ((!(this.interactionTarget != null)) ? 1f : this.interactionTarget.interactionSpeedMlp);
			this.weight = Mathf.Clamp(this.weight + Time.deltaTime * this.fadeInSpeed, 0f, 1f);
			bool flag = false;
			bool flag2 = false;
			this.TriggerUntriggeredEvents(true, out flag, out flag2);
			Vector3 b = (!this.pickedUp) ? this.target.position : this.pickUpPosition;
			Quaternion b2 = (!this.pickedUp) ? this.target.rotation : this.pickUpRotation;
			this.effector.position = Vector3.Lerp(this.effector.bone.position, b, this.weight);
			this.effector.rotation = Quaternion.Lerp(this.effector.bone.rotation, b2, this.weight);
			this.interactionObject.Apply(solver, this.effectorType, this.interactionTarget, this.timer, this.weight);
			if (flag)
			{
				this.PickUp(root);
			}
			if (flag2)
			{
				this.Pause();
			}
			float value = this.interactionObject.GetValue(InteractionObject.WeightCurve.Type.PoserWeight, this.interactionTarget, this.timer);
			if (this.poser != null)
			{
				this.poser.weight = Mathf.Lerp(this.poser.weight, value, this.weight);
			}
			else if (value > 0f)
			{
				Warning.Log(string.Concat(new string[]
				{
					"InteractionObject ",
					this.interactionObject.name,
					" has a curve/multipler for Poser Weight, but the bone of effector ",
					this.effectorType.ToString(),
					" has no HandPoser/GenericPoser attached."
				}), this.effector.bone, false);
			}
			if (this.timer >= this.length)
			{
				this.Stop();
			}
		}

		private void TriggerUntriggeredEvents(bool checkTime, out bool pickUp, out bool pause)
		{
			pickUp = false;
			pause = false;
			for (int i = 0; i < this.triggered.Count; i++)
			{
				if (!this.triggered[i] && (!checkTime || this.interactionObject.events[i].time < this.timer))
				{
					this.interactionObject.events[i].Activate(this.effector.bone);
					if (this.interactionObject.events[i].pickUp)
					{
						if (this.timer >= this.interactionObject.events[i].time)
						{
							this.timer = this.interactionObject.events[i].time;
						}
						pickUp = true;
					}
					if (this.interactionObject.events[i].pause)
					{
						if (this.timer >= this.interactionObject.events[i].time)
						{
							this.timer = this.interactionObject.events[i].time;
						}
						pause = true;
					}
					if (this.interactionSystem.OnInteractionEvent != null)
					{
						this.interactionSystem.OnInteractionEvent(this.effectorType, this.interactionObject, this.interactionObject.events[i]);
					}
					this.triggered[i] = true;
				}
			}
		}

		private void PickUp(Transform root)
		{
			this.pickUpPosition = this.effector.position;
			this.pickUpRotation = this.effector.rotation;
			this.pickUpOnPostFBBIK = true;
			this.pickedUp = true;
			if (this.interactionObject.targetsRoot.GetComponent<Rigidbody>() != null)
			{
				if (!this.interactionObject.targetsRoot.GetComponent<Rigidbody>().isKinematic)
				{
					this.interactionObject.targetsRoot.GetComponent<Rigidbody>().isKinematic = true;
				}
				if (root.GetComponent<Collider>() != null)
				{
					Collider[] componentsInChildren = this.interactionObject.targetsRoot.GetComponentsInChildren<Collider>();
					Collider[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						Collider collider = array[i];
						if (!collider.isTrigger)
						{
							Physics.IgnoreCollision(root.GetComponent<Collider>(), collider);
						}
					}
				}
			}
			if (this.interactionSystem.OnInteractionPickUp != null)
			{
				this.interactionSystem.OnInteractionPickUp(this.effectorType, this.interactionObject);
			}
		}

		public bool Stop()
		{
			if (!this.inInteraction)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			this.TriggerUntriggeredEvents(false, out flag, out flag2);
			if (this.interactionSystem.OnInteractionStop != null)
			{
				this.interactionSystem.OnInteractionStop(this.effectorType, this.interactionObject);
			}
			if (this.interactionTarget != null)
			{
				this.interactionTarget.ResetRotation();
			}
			this.interactionObject = null;
			this.weight = 0f;
			this.timer = 0f;
			this.isPaused = false;
			this.target = null;
			this.defaults = false;
			this.resetTimer = 1f;
			if (this.poser != null && !this.pickedUp)
			{
				this.poser.weight = 0f;
			}
			this.pickedUp = false;
			return true;
		}

		public void OnPostFBBIK(IKSolverFullBodyBiped fullBody)
		{
			if (!this.inInteraction)
			{
				return;
			}
			float num = this.interactionObject.GetValue(InteractionObject.WeightCurve.Type.RotateBoneWeight, this.interactionTarget, this.timer) * this.weight;
			if (num > 0f)
			{
				Quaternion b = (!this.pickedUp) ? this.effector.rotation : this.pickUpRotation;
				Quaternion rhs = Quaternion.Slerp(this.effector.bone.rotation, b, num * num);
				this.effector.bone.localRotation = Quaternion.Inverse(this.effector.bone.parent.rotation) * rhs;
			}
			if (this.pickUpOnPostFBBIK)
			{
				Vector3 position = this.effector.bone.position;
				this.effector.bone.position = this.pickUpPosition;
				this.interactionObject.targetsRoot.parent = this.effector.bone;
				this.effector.bone.position = position;
				this.pickUpOnPostFBBIK = false;
			}
		}
	}
}
