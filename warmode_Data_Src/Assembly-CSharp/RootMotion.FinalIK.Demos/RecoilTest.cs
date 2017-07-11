using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class RecoilTest : MonoBehaviour
	{
		public Recoil recoil;

		public float magnitude = 1f;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				this.recoil.Fire(this.magnitude);
				MonoBehaviour.print("recoil");
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("Press R for procedural recoil.", new GUILayoutOption[0]);
		}
	}
}
