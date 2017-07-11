using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class TouchWalls : MonoBehaviour
	{
		[Serializable]
		public class EffectorLink
		{
			public bool enabled = true;

			public FullBodyBipedEffector effectorType;

			public InteractionObject interactionObject;

			public Transform spherecastFrom;

			public float spherecastRadius = 0.1f;

			public float minDistance = 0.3f;

			public LayerMask touchLayers;

			public float lerpSpeed = 10f;

			public float minSwitchTime = 0.2f;

			public float releaseDistance = 0.4f;

			public bool sliding;

			private Vector3 raycastDirectionLocal;

			private float raycastDistance;

			private bool inTouch;

			private RaycastHit hit = default(RaycastHit);

			private Vector3 targetPosition;

			private Quaternion targetRotation;

			private bool initiated;

			private float nextSwitchTime;

			private float speedF;

			public void Initiate(InteractionSystem interactionSystem)
			{
				this.raycastDirectionLocal = this.spherecastFrom.InverseTransformDirection(this.interactionObject.transform.position - this.spherecastFrom.position);
				this.raycastDistance = Vector3.Distance(this.spherecastFrom.position, this.interactionObject.transform.position);
				interactionSystem.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Combine(interactionSystem.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.OnInteractionStart));
				interactionSystem.OnInteractionResume = (InteractionSystem.InteractionDelegate)Delegate.Combine(interactionSystem.OnInteractionResume, new InteractionSystem.InteractionDelegate(this.OnInteractionResume));
				interactionSystem.OnInteractionStop = (InteractionSystem.InteractionDelegate)Delegate.Combine(interactionSystem.OnInteractionStop, new InteractionSystem.InteractionDelegate(this.OnInteractionStop));
				this.hit.normal = Vector3.forward;
				this.targetPosition = this.interactionObject.transform.position;
				this.targetRotation = this.interactionObject.transform.rotation;
				this.initiated = true;
			}

			private bool FindWalls(Vector3 direction)
			{
				if (!this.enabled)
				{
					return false;
				}
				bool result = Physics.SphereCast(this.spherecastFrom.position, this.spherecastRadius, direction, out this.hit, this.raycastDistance, this.touchLayers);
				if (this.hit.distance < this.minDistance)
				{
					result = false;
				}
				return result;
			}

			public void Update(InteractionSystem interactionSystem)
			{
				if (!this.initiated)
				{
					return;
				}
				Vector3 vector = this.spherecastFrom.TransformDirection(this.raycastDirectionLocal);
				this.hit.point = this.spherecastFrom.position + vector;
				bool flag = this.FindWalls(vector);
				if (!this.inTouch)
				{
					if (flag && Time.time > this.nextSwitchTime)
					{
						this.interactionObject.transform.parent = null;
						interactionSystem.StartInteraction(this.effectorType, this.interactionObject, true);
						this.nextSwitchTime = Time.time + this.minSwitchTime;
						this.targetPosition = this.hit.point;
						this.targetRotation = Quaternion.LookRotation(-this.hit.normal);
						this.interactionObject.transform.position = this.targetPosition;
						this.interactionObject.transform.rotation = this.targetRotation;
					}
				}
				else
				{
					if (!flag)
					{
						this.StopTouch(interactionSystem);
					}
					else if (!interactionSystem.IsPaused(this.effectorType) || this.sliding)
					{
						this.targetPosition = this.hit.point;
						this.targetRotation = Quaternion.LookRotation(-this.hit.normal);
					}
					if (Vector3.Distance(this.interactionObject.transform.position, this.hit.point) > this.releaseDistance)
					{
						if (flag)
						{
							this.targetPosition = this.hit.point;
							this.targetRotation = Quaternion.LookRotation(-this.hit.normal);
						}
						else
						{
							this.StopTouch(interactionSystem);
						}
					}
				}
				float b = (this.inTouch && (!interactionSystem.IsPaused(this.effectorType) || !(this.interactionObject.transform.position == this.targetPosition))) ? 1f : 0f;
				this.speedF = Mathf.Lerp(this.speedF, b, Time.deltaTime * 3f);
				this.interactionObject.transform.position = Vector3.Lerp(this.interactionObject.transform.position, this.targetPosition, Time.deltaTime * this.lerpSpeed * this.speedF);
				this.interactionObject.transform.rotation = Quaternion.Slerp(this.interactionObject.transform.rotation, this.targetRotation, Time.deltaTime * this.lerpSpeed * this.speedF);
			}

			private void StopTouch(InteractionSystem interactionSystem)
			{
				this.interactionObject.transform.parent = interactionSystem.transform;
				this.nextSwitchTime = Time.time + this.minSwitchTime;
				if (interactionSystem.IsPaused(this.effectorType))
				{
					interactionSystem.ResumeInteraction(this.effectorType);
				}
				else
				{
					this.speedF = 0f;
					this.targetPosition = this.hit.point;
					this.targetRotation = Quaternion.LookRotation(-this.hit.normal);
				}
			}

			private void OnInteractionStart(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
			{
				if (effectorType != this.effectorType || interactionObject != this.interactionObject)
				{
					return;
				}
				this.inTouch = true;
			}

			private void OnInteractionResume(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
			{
				if (effectorType != this.effectorType || interactionObject != this.interactionObject)
				{
					return;
				}
				this.inTouch = false;
			}

			private void OnInteractionStop(FullBodyBipedEffector effectorType, InteractionObject interactionObject)
			{
				if (effectorType != this.effectorType || interactionObject != this.interactionObject)
				{
					return;
				}
				this.inTouch = false;
			}

			public void Destroy(InteractionSystem interactionSystem)
			{
				if (!this.initiated)
				{
					return;
				}
				interactionSystem.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Remove(interactionSystem.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.OnInteractionStart));
				interactionSystem.OnInteractionResume = (InteractionSystem.InteractionDelegate)Delegate.Remove(interactionSystem.OnInteractionResume, new InteractionSystem.InteractionDelegate(this.OnInteractionResume));
				interactionSystem.OnInteractionStop = (InteractionSystem.InteractionDelegate)Delegate.Remove(interactionSystem.OnInteractionStop, new InteractionSystem.InteractionDelegate(this.OnInteractionStop));
			}
		}

		public InteractionSystem interactionSystem;

		public TouchWalls.EffectorLink[] effectorLinks;

		private void Start()
		{
			TouchWalls.EffectorLink[] array = this.effectorLinks;
			for (int i = 0; i < array.Length; i++)
			{
				TouchWalls.EffectorLink effectorLink = array[i];
				effectorLink.Initiate(this.interactionSystem);
			}
		}

		private void FixedUpdate()
		{
			for (int i = 0; i < this.effectorLinks.Length; i++)
			{
				this.effectorLinks[i].Update(this.interactionSystem);
			}
		}

		private void OnDestroy()
		{
			if (this.interactionSystem != null)
			{
				for (int i = 0; i < this.effectorLinks.Length; i++)
				{
					this.effectorLinks[i].Destroy(this.interactionSystem);
				}
			}
		}
	}
}
