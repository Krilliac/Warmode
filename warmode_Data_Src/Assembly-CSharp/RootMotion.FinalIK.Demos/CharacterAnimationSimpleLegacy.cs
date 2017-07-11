using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CharacterAnimationSimpleLegacy : CharacterAnimationBase
	{
		[SerializeField]
		private CharacterThirdPerson characterController;

		[SerializeField]
		private Animation animation;

		[SerializeField]
		private float pivotOffset;

		[SerializeField]
		private string idleName;

		[SerializeField]
		private string moveName;

		[SerializeField]
		private float idleAnimationSpeed = 0.3f;

		[SerializeField]
		private float moveAnimationSpeed = 0.75f;

		[SerializeField]
		private AnimationCurve moveSpeed;

		private void Start()
		{
			this.animation[this.idleName].speed = this.idleAnimationSpeed;
			this.animation[this.moveName].speed = this.moveAnimationSpeed;
		}

		public override Vector3 GetPivotPoint()
		{
			return base.transform.position + base.transform.forward * this.pivotOffset;
		}

		private void Update()
		{
			if (Time.deltaTime == 0f)
			{
				return;
			}
			if (this.characterController.animState.moveDirection.z > 0.4f)
			{
				this.animation.CrossFade(this.moveName, 0.1f);
			}
			else
			{
				this.animation.CrossFade(this.idleName);
			}
			this.characterController.Move(this.characterController.transform.forward * Time.deltaTime * this.moveSpeed.Evaluate(this.characterController.animState.moveDirection.z));
		}
	}
}
