using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class HandPoser : Poser
	{
		private Transform _poseRoot;

		private Transform[] children;

		private Transform[] poseChildren;

		private void Start()
		{
			this.children = base.GetComponentsInChildren<Transform>();
		}

		public override void AutoMapping()
		{
			if (this.poseRoot == null)
			{
				this.poseChildren = new Transform[0];
			}
			else
			{
				this.poseChildren = this.poseRoot.GetComponentsInChildren<Transform>();
			}
			this._poseRoot = this.poseRoot;
		}

		private void LateUpdate()
		{
			if (this.weight <= 0f)
			{
				return;
			}
			if (this.localPositionWeight <= 0f && this.localRotationWeight <= 0f)
			{
				return;
			}
			if (this._poseRoot != this.poseRoot)
			{
				this.AutoMapping();
			}
			if (this.poseRoot == null)
			{
				return;
			}
			if (this.children.Length != this.poseChildren.Length)
			{
				Warning.Log(string.Concat(new string[]
				{
					"Number of children does not match with the pose ",
					this.children.Length.ToString(),
					" ",
					this.poseChildren.Length.ToString(),
					" "
				}), base.transform, false);
				return;
			}
			float t = this.localRotationWeight * this.weight;
			float t2 = this.localPositionWeight * this.weight;
			for (int i = 0; i < this.children.Length; i++)
			{
				if (this.children[i] != base.transform)
				{
					this.children[i].localRotation = Quaternion.Lerp(this.children[i].localRotation, this.poseChildren[i].localRotation, t);
					this.children[i].localPosition = Vector3.Lerp(this.children[i].localPosition, this.poseChildren[i].localPosition, t2);
				}
			}
		}
	}
}
