using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class SlowMo : MonoBehaviour
	{
		[SerializeField]
		private KeyCode[] keyCodes;

		[SerializeField]
		private bool mouse0;

		[SerializeField]
		private bool mouse1;

		[SerializeField]
		private float slowMoTimeScale = 0.3f;

		private void Update()
		{
			Time.timeScale = ((!this.IsSlowMotion()) ? 1f : this.slowMoTimeScale);
		}

		private bool IsSlowMotion()
		{
			if (this.mouse0 && Input.GetMouseButton(0))
			{
				return true;
			}
			if (this.mouse1 && Input.GetMouseButton(1))
			{
				return true;
			}
			for (int i = 0; i < this.keyCodes.Length; i++)
			{
				if (Input.GetKey(this.keyCodes[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
