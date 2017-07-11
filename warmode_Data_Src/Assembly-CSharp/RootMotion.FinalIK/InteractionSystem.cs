using System;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction System")]
	public class InteractionSystem : MonoBehaviour
	{
		public delegate void InteractionDelegate(FullBodyBipedEffector effectorType, InteractionObject interactionObject);

		public delegate void InteractionEventDelegate(FullBodyBipedEffector effectorType, InteractionObject interactionObject, InteractionObject.InteractionEvent interactionEvent);

		[Tooltip("If not empty, only the targets with the specified tag will be used by this Interaction System.")]
		public string targetTag = string.Empty;

		[Tooltip("The fade in time of the interaction.")]
		public float fadeInTime = 0.3f;

		[Tooltip("The master speed for all interactions.")]
		public float speed = 1f;

		[Tooltip("If > 0, lerps all the FBBIK channels used by the Interaction System back to their default or initial values when not in interaction.")]
		public float resetToDefaultsSpeed = 1f;

		[Header("Triggering"), Tooltip("The collider that registers OnTriggerEnter and OnTriggerExit events with InteractionTriggers.")]
		public Collider collider;

		[Tooltip("Will be used by Interaction Triggers that need the camera's position. Assign the first person view character camera.")]
		public Transform camera;

		[Tooltip("The layers that will be raycasted from the camera (along camera.forward). All InteractionTrigger look at target colliders should be included.")]
		public LayerMask camRaycastLayers;

		[Tooltip("Max distance of raycasting from the camera.")]
		public float camRaycastDistance = 1f;

		private List<InteractionTrigger> inContact = new List<InteractionTrigger>();

		private List<int> bestRangeIndexes = new List<int>();

		public InteractionSystem.InteractionDelegate OnInteractionStart;

		public InteractionSystem.InteractionDelegate OnInteractionPause;

		public InteractionSystem.InteractionDelegate OnInteractionPickUp;

		public InteractionSystem.InteractionDelegate OnInteractionResume;

		public InteractionSystem.InteractionDelegate OnInteractionStop;

		public InteractionSystem.InteractionEventDelegate OnInteractionEvent;

		public RaycastHit raycastHit;

		[SerializeField, Space(10f), Tooltip("Reference to the FBBIK component.")]
		private FullBodyBipedIK fullBody;

		[Tooltip("Handles looking at the interactions.")]
		public InteractionLookAt lookAt = new InteractionLookAt();

		private InteractionEffector[] interactionEffectors = new InteractionEffector[]
		{
			new InteractionEffector(FullBodyBipedEffector.Body),
			new InteractionEffector(FullBodyBipedEffector.LeftFoot),
			new InteractionEffector(FullBodyBipedEffector.LeftHand),
			new InteractionEffector(FullBodyBipedEffector.LeftShoulder),
			new InteractionEffector(FullBodyBipedEffector.LeftThigh),
			new InteractionEffector(FullBodyBipedEffector.RightFoot),
			new InteractionEffector(FullBodyBipedEffector.RightHand),
			new InteractionEffector(FullBodyBipedEffector.RightShoulder),
			new InteractionEffector(FullBodyBipedEffector.RightThigh)
		};

		private bool initiated;

		private Collider lastCollider;

		private Collider c;

		public bool inInteraction
		{
			get
			{
				if (!this.IsValid(true))
				{
					return false;
				}
				for (int i = 0; i < this.interactionEffectors.Length; i++)
				{
					if (this.interactionEffectors[i].inInteraction && !this.interactionEffectors[i].isPaused)
					{
						return true;
					}
				}
				return false;
			}
		}

		public FullBodyBipedIK ik
		{
			get
			{
				return this.fullBody;
			}
		}

		public List<InteractionTrigger> triggersInRange
		{
			get;
			private set;
		}

		public bool IsInInteraction(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].inInteraction && !this.interactionEffectors[i].isPaused;
				}
			}
			return false;
		}

		public bool IsPaused(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].inInteraction && this.interactionEffectors[i].isPaused;
				}
			}
			return false;
		}

		public bool IsPaused()
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].inInteraction && this.interactionEffectors[i].isPaused)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInSync()
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].isPaused)
				{
					for (int j = 0; j < this.interactionEffectors.Length; j++)
					{
						if (j != i && this.interactionEffectors[j].inInteraction && !this.interactionEffectors[j].isPaused)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool StartInteraction(FullBodyBipedEffector effectorType, InteractionObject interactionObject, bool interrupt)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			if (interactionObject == null)
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].Start(interactionObject, this.targetTag, this.fadeInTime, interrupt);
				}
			}
			return false;
		}

		public bool PauseInteraction(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].Pause();
				}
			}
			return false;
		}

		public bool ResumeInteraction(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].Resume();
				}
			}
			return false;
		}

		public bool StopInteraction(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].Stop();
				}
			}
			return false;
		}

		public void PauseAll()
		{
			if (!this.IsValid(true))
			{
				return;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				this.interactionEffectors[i].Pause();
			}
		}

		public void ResumeAll()
		{
			if (!this.IsValid(true))
			{
				return;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				this.interactionEffectors[i].Resume();
			}
		}

		public void StopAll()
		{
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				this.interactionEffectors[i].Stop();
			}
		}

		public InteractionObject GetInteractionObject(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return null;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].interactionObject;
				}
			}
			return null;
		}

		public float GetProgress(FullBodyBipedEffector effectorType)
		{
			if (!this.IsValid(true))
			{
				return 0f;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].effectorType == effectorType)
				{
					return this.interactionEffectors[i].progress;
				}
			}
			return 0f;
		}

		public float GetMinActiveProgress()
		{
			if (!this.IsValid(true))
			{
				return 0f;
			}
			float num = 1f;
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				if (this.interactionEffectors[i].inInteraction)
				{
					float progress = this.interactionEffectors[i].progress;
					if (progress > 0f && progress < num)
					{
						num = progress;
					}
				}
			}
			return num;
		}

		public bool TriggerInteraction(int index, bool interrupt)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			if (!this.TriggerIndexIsValid(index))
			{
				return false;
			}
			bool result = true;
			InteractionTrigger.Range range = this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
			for (int i = 0; i < range.interactions.Length; i++)
			{
				for (int j = 0; j < range.interactions[i].effectors.Length; j++)
				{
					if (!this.StartInteraction(range.interactions[i].effectors[j], range.interactions[i].interactionObject, interrupt))
					{
						result = false;
					}
				}
			}
			return result;
		}

		public bool TriggerEffectorsReady(int index)
		{
			if (!this.IsValid(true))
			{
				return false;
			}
			if (!this.TriggerIndexIsValid(index))
			{
				return false;
			}
			for (int i = 0; i < this.triggersInRange[index].ranges.Length; i++)
			{
				InteractionTrigger.Range range = this.triggersInRange[index].ranges[i];
				for (int j = 0; j < range.interactions.Length; j++)
				{
					for (int k = 0; k < range.interactions[j].effectors.Length; k++)
					{
						if (this.IsInInteraction(range.interactions[j].effectors[k]))
						{
							return false;
						}
					}
				}
				for (int l = 0; l < range.interactions.Length; l++)
				{
					for (int m = 0; m < range.interactions[l].effectors.Length; m++)
					{
						if (this.IsPaused(range.interactions[l].effectors[m]))
						{
							for (int n = 0; n < range.interactions[l].effectors.Length; n++)
							{
								if (n != m && !this.IsPaused(range.interactions[l].effectors[n]))
								{
									return false;
								}
							}
						}
					}
				}
			}
			return true;
		}

		public InteractionTrigger.Range GetTriggerRange(int index)
		{
			if (!this.IsValid(true))
			{
				return null;
			}
			if (index < 0 || index >= this.bestRangeIndexes.Count)
			{
				Warning.Log("Index out of range.", base.transform, false);
				return null;
			}
			return this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
		}

		public int GetClosestTriggerIndex()
		{
			if (!this.IsValid(true))
			{
				return -1;
			}
			if (this.triggersInRange.Count == 0)
			{
				return -1;
			}
			if (this.triggersInRange.Count == 1)
			{
				return 0;
			}
			int result = -1;
			float num = float.PositiveInfinity;
			for (int i = 0; i < this.triggersInRange.Count; i++)
			{
				if (this.triggersInRange[i] != null)
				{
					float num2 = Vector3.SqrMagnitude(this.triggersInRange[i].transform.position - base.transform.position);
					if (num2 < num)
					{
						result = i;
						num = num2;
					}
				}
			}
			return result;
		}

		protected virtual void Start()
		{
			if (this.fullBody == null)
			{
				this.fullBody = base.GetComponent<FullBodyBipedIK>();
			}
			if (this.fullBody == null)
			{
				Warning.Log("InteractionSystem can not find a FullBodyBipedIK component", base.transform, false);
				return;
			}
			IKSolverFullBodyBiped expr_4B = this.fullBody.solver;
			expr_4B.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(expr_4B.OnPreUpdate, new IKSolver.UpdateDelegate(this.OnPreFBBIK));
			IKSolverFullBodyBiped expr_77 = this.fullBody.solver;
			expr_77.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(expr_77.OnPostUpdate, new IKSolver.UpdateDelegate(this.OnPostFBBIK));
			this.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Combine(this.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.LookAtInteraction));
			InteractionEffector[] array = this.interactionEffectors;
			for (int i = 0; i < array.Length; i++)
			{
				InteractionEffector interactionEffector = array[i];
				interactionEffector.Initiate(this, this.fullBody.solver);
			}
			this.triggersInRange = new List<InteractionTrigger>();
			this.c = base.GetComponent<Collider>();
			this.UpdateTriggerEventBroadcasting();
			this.initiated = true;
		}

		private void LookAtInteraction(FullBodyBipedEffector effector, InteractionObject interactionObject)
		{
			this.lookAt.Look(interactionObject.lookAtTarget, Time.time + interactionObject.length * 0.5f);
		}

		public void OnTriggerEnter(Collider c)
		{
			if (this.fullBody == null)
			{
				return;
			}
			InteractionTrigger component = c.GetComponent<InteractionTrigger>();
			if (this.inContact.Contains(component))
			{
				return;
			}
			this.inContact.Add(component);
		}

		public void OnTriggerExit(Collider c)
		{
			if (this.fullBody == null)
			{
				return;
			}
			InteractionTrigger component = c.GetComponent<InteractionTrigger>();
			this.inContact.Remove(component);
		}

		private bool ContactIsInRange(int index, out int bestRangeIndex)
		{
			bestRangeIndex = -1;
			if (!this.IsValid(true))
			{
				return false;
			}
			if (index < 0 || index >= this.inContact.Count)
			{
				Warning.Log("Index out of range.", base.transform, false);
				return false;
			}
			if (this.inContact[index] == null)
			{
				Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", base.transform, false);
				return false;
			}
			bestRangeIndex = this.inContact[index].GetBestRangeIndex(base.transform, this.camera, this.raycastHit);
			return bestRangeIndex != -1;
		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				return;
			}
			if (this.fullBody == null)
			{
				this.fullBody = base.GetComponent<FullBodyBipedIK>();
			}
			if (this.collider == null)
			{
				this.collider = base.GetComponent<Collider>();
			}
		}

		private void Update()
		{
			if (this.fullBody == null)
			{
				return;
			}
			this.UpdateTriggerEventBroadcasting();
			this.Raycasting();
			this.triggersInRange.Clear();
			this.bestRangeIndexes.Clear();
			for (int i = 0; i < this.inContact.Count; i++)
			{
				int item = -1;
				if (this.inContact[i] != null && this.ContactIsInRange(i, out item))
				{
					this.triggersInRange.Add(this.inContact[i]);
					this.bestRangeIndexes.Add(item);
				}
			}
			this.lookAt.Update();
		}

		private void Raycasting()
		{
			if (this.camRaycastLayers == -1)
			{
				return;
			}
			if (this.camera == null)
			{
				return;
			}
			Physics.Raycast(this.camera.position, this.camera.forward, out this.raycastHit, this.camRaycastDistance, this.camRaycastLayers);
		}

		private void UpdateTriggerEventBroadcasting()
		{
			if (this.collider == null)
			{
				this.collider = this.c;
			}
			if (this.collider != null && this.collider != this.c)
			{
				if (this.collider.GetComponent<TriggerEventBroadcaster>() == null)
				{
					TriggerEventBroadcaster triggerEventBroadcaster = this.collider.gameObject.AddComponent<TriggerEventBroadcaster>();
					triggerEventBroadcaster.target = base.gameObject;
				}
				if (this.lastCollider != null && this.lastCollider != this.c && this.lastCollider != this.collider)
				{
					TriggerEventBroadcaster component = this.lastCollider.GetComponent<TriggerEventBroadcaster>();
					if (component != null)
					{
						UnityEngine.Object.Destroy(component);
					}
				}
			}
			this.lastCollider = this.collider;
		}

		private void LateUpdate()
		{
			if (this.fullBody == null)
			{
				return;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				this.interactionEffectors[i].Update(base.transform, this.fullBody.solver, this.speed);
			}
			for (int j = 0; j < this.interactionEffectors.Length; j++)
			{
				this.interactionEffectors[j].ResetToDefaults(this.fullBody.solver, this.resetToDefaultsSpeed);
			}
		}

		private void OnPreFBBIK()
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.fullBody == null)
			{
				return;
			}
			this.lookAt.SolveSpine();
		}

		private void OnPostFBBIK()
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.fullBody == null)
			{
				return;
			}
			for (int i = 0; i < this.interactionEffectors.Length; i++)
			{
				this.interactionEffectors[i].OnPostFBBIK(this.fullBody.solver);
			}
			this.lookAt.SolveHead();
		}

		private void OnDestroy()
		{
			if (this.fullBody == null)
			{
				return;
			}
			IKSolverFullBodyBiped expr_1D = this.fullBody.solver;
			expr_1D.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(expr_1D.OnPreUpdate, new IKSolver.UpdateDelegate(this.OnPreFBBIK));
			IKSolverFullBodyBiped expr_49 = this.fullBody.solver;
			expr_49.OnPostUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(expr_49.OnPostUpdate, new IKSolver.UpdateDelegate(this.OnPostFBBIK));
			this.OnInteractionStart = (InteractionSystem.InteractionDelegate)Delegate.Remove(this.OnInteractionStart, new InteractionSystem.InteractionDelegate(this.LookAtInteraction));
		}

		private bool IsValid(bool log)
		{
			if (this.fullBody == null)
			{
				if (log)
				{
					Warning.Log("FBBIK is null. Will not update the InteractionSystem", base.transform, false);
				}
				return false;
			}
			if (!this.initiated)
			{
				if (log)
				{
					Warning.Log("The InteractionSystem has not been initiated yet.", base.transform, false);
				}
				return false;
			}
			return true;
		}

		private bool TriggerIndexIsValid(int index)
		{
			if (index < 0 || index >= this.triggersInRange.Count)
			{
				Warning.Log("Index out of range.", base.transform, false);
				return false;
			}
			if (this.triggersInRange[index] == null)
			{
				Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", base.transform, false);
				return false;
			}
			return true;
		}

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page10.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_interaction_system.html");
		}
	}
}
