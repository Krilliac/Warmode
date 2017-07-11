using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(InteractionSystem))]
	public class InteractionSystemTestGUI : MonoBehaviour
	{
		[SerializeField, Tooltip("The object to interact to")]
		private InteractionObject interactionObject;

		[SerializeField, Tooltip("The effectors to interact with")]
		private FullBodyBipedEffector[] effectors;

		private InteractionSystem interactionSystem;

		private void Awake()
		{
			this.interactionSystem = base.GetComponent<InteractionSystem>();
		}

		private void OnGUI()
		{
			if (this.interactionSystem == null)
			{
				return;
			}
			if (GUILayout.Button("Start Interaction With " + this.interactionObject.name, new GUILayoutOption[0]))
			{
				if (this.effectors.Length == 0)
				{
					Debug.Log("Please select the effectors to interact with.");
				}
				FullBodyBipedEffector[] array = this.effectors;
				for (int i = 0; i < array.Length; i++)
				{
					FullBodyBipedEffector effectorType = array[i];
					this.interactionSystem.StartInteraction(effectorType, this.interactionObject, true);
				}
			}
			if (this.effectors.Length == 0)
			{
				return;
			}
			if (this.interactionSystem.IsPaused(this.effectors[0]) && GUILayout.Button("Resume Interaction With " + this.interactionObject.name, new GUILayoutOption[0]))
			{
				this.interactionSystem.ResumeAll();
			}
		}
	}
}
