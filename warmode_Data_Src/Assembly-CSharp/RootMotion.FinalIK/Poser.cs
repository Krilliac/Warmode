using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public abstract class Poser : MonoBehaviour
	{
		public Transform poseRoot;

		[Range(0f, 1f)]
		public float weight = 1f;

		[Range(0f, 1f)]
		public float localRotationWeight = 1f;

		[Range(0f, 1f)]
		public float localPositionWeight;

		public abstract void AutoMapping();
	}
}
