using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class InteractionDemo : MonoBehaviour
	{
		public InteractionSystem interactionSystem;

		public bool interrupt;

		public InteractionObject ball;

		public InteractionObject benchMain;

		public InteractionObject benchHands;

		public InteractionObject button;

		public InteractionObject cigarette;

		public InteractionObject door;

		private bool isSitting;

		private void OnGUI()
		{
			this.interrupt = GUILayout.Toggle(this.interrupt, "Interrupt", new GUILayoutOption[0]);
			if (this.isSitting)
			{
				if (!this.interactionSystem.inInteraction && GUILayout.Button("Stand Up", new GUILayoutOption[0]))
				{
					this.interactionSystem.ResumeAll();
					this.isSitting = false;
				}
				return;
			}
			if (GUILayout.Button("Pick Up Ball", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, this.ball, this.interrupt);
			}
			if (GUILayout.Button("Button Left Hand", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, this.button, this.interrupt);
			}
			if (GUILayout.Button("Button Right Hand", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, this.button, this.interrupt);
			}
			if (GUILayout.Button("Put Out Cigarette", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightFoot, this.cigarette, this.interrupt);
			}
			if (GUILayout.Button("Open Door", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, this.door, this.interrupt);
			}
			if (!this.interactionSystem.inInteraction && GUILayout.Button("Sit Down", new GUILayoutOption[0]))
			{
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.Body, this.benchMain, this.interrupt);
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftThigh, this.benchMain, this.interrupt);
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightThigh, this.benchMain, this.interrupt);
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftFoot, this.benchMain, this.interrupt);
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, this.benchHands, this.interrupt);
				this.interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, this.benchHands, this.interrupt);
				this.isSitting = true;
			}
		}
	}
}
