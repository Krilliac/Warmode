using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class Turret : MonoBehaviour
	{
		[Serializable]
		public class Part
		{
			public Transform transform;

			private RotationLimit rotationLimit;

			public void AimAt(Transform target)
			{
				this.transform.LookAt(target.position, this.transform.up);
				if (this.rotationLimit == null)
				{
					this.rotationLimit = this.transform.GetComponent<RotationLimit>();
					this.rotationLimit.Disable();
				}
				this.rotationLimit.Apply();
			}
		}

		public Transform target;

		public Turret.Part[] parts;

		private void Update()
		{
			Turret.Part[] array = this.parts;
			for (int i = 0; i < array.Length; i++)
			{
				Turret.Part part = array[i];
				part.AimAt(this.target);
			}
		}
	}
}
