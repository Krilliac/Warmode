using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	[AddComponentMenu("")]
	public class ActRotatorExample : MonoBehaviour
	{
		[Range(1f, 100f)]
		public float speed = 5f;

		private void Update()
		{
			base.transform.Rotate(0f, this.speed * Time.deltaTime, 0f);
		}
	}
}
