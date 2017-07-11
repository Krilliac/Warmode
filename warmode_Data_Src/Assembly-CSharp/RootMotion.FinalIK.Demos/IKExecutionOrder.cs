using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class IKExecutionOrder : MonoBehaviour
	{
		public IK[] IKComponents;

		private void Start()
		{
			for (int i = 0; i < this.IKComponents.Length; i++)
			{
				this.IKComponents[i].Disable();
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < this.IKComponents.Length; i++)
			{
				this.IKComponents[i].GetIKSolver().Update();
			}
		}
	}
}
