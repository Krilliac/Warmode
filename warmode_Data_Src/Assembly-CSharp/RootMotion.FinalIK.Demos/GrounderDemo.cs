using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class GrounderDemo : MonoBehaviour
	{
		public GameObject[] characters;

		private void OnGUI()
		{
			if (GUILayout.Button("Biped", new GUILayoutOption[0]))
			{
				this.Activate(0);
			}
			if (GUILayout.Button("Quadruped", new GUILayoutOption[0]))
			{
				this.Activate(1);
			}
			if (GUILayout.Button("Mech", new GUILayoutOption[0]))
			{
				this.Activate(2);
			}
			if (GUILayout.Button("Bot", new GUILayoutOption[0]))
			{
				this.Activate(3);
			}
		}

		public void Activate(int index)
		{
			for (int i = 0; i < this.characters.Length; i++)
			{
				this.characters[i].SetActive(i == index);
			}
		}
	}
}
