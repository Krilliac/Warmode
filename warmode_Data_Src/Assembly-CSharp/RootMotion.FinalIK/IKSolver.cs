using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public abstract class IKSolver
	{
		[Serializable]
		public class Point
		{
			public Transform transform;

			[Range(0f, 1f)]
			public float weight = 1f;

			public Vector3 solverPosition;

			public Quaternion solverRotation = Quaternion.identity;

			public Vector3 defaultLocalPosition;

			public Quaternion defaultLocalRotation;

			public void StoreDefaultLocalState()
			{
				this.defaultLocalPosition = this.transform.localPosition;
				this.defaultLocalRotation = this.transform.localRotation;
			}

			public void FixTransform()
			{
				if (this.transform.localPosition != this.defaultLocalPosition)
				{
					this.transform.localPosition = this.defaultLocalPosition;
				}
				if (this.transform.localRotation != this.defaultLocalRotation)
				{
					this.transform.localRotation = this.defaultLocalRotation;
				}
			}
		}

		[Serializable]
		public class Bone : IKSolver.Point
		{
			public float length;

			public Vector3 axis = -Vector3.right;

			private RotationLimit _rotationLimit;

			private bool isLimited = true;

			public RotationLimit rotationLimit
			{
				get
				{
					if (!this.isLimited)
					{
						return null;
					}
					if (this._rotationLimit == null)
					{
						this._rotationLimit = this.transform.GetComponent<RotationLimit>();
					}
					this.isLimited = (this._rotationLimit != null);
					return this._rotationLimit;
				}
				set
				{
					this._rotationLimit = value;
					this.isLimited = (value != null);
				}
			}

			public Bone()
			{
			}

			public Bone(Transform transform)
			{
				this.transform = transform;
			}

			public Bone(Transform transform, float weight)
			{
				this.transform = transform;
				this.weight = weight;
			}

			public void Swing(Vector3 swingTarget, float weight = 1f)
			{
				if (weight <= 0f)
				{
					return;
				}
				Quaternion quaternion = Quaternion.FromToRotation(this.transform.rotation * this.axis, swingTarget - this.transform.position);
				if (weight >= 1f)
				{
					this.transform.rotation = quaternion * this.transform.rotation;
					return;
				}
				this.transform.rotation = Quaternion.Lerp(Quaternion.identity, quaternion, weight) * this.transform.rotation;
			}

			public Quaternion GetSolverSwing(Vector3 swingTarget, float weight = 1f)
			{
				if (weight <= 0f)
				{
					return Quaternion.identity;
				}
				Quaternion quaternion = Quaternion.FromToRotation(this.solverRotation * this.axis, swingTarget - this.solverPosition);
				if (weight >= 1f)
				{
					return quaternion;
				}
				return Quaternion.Lerp(Quaternion.identity, quaternion, weight);
			}

			public void SetToSolverPosition()
			{
				this.transform.position = this.solverPosition;
			}
		}

		[Serializable]
		public class Node : IKSolver.Point
		{
			public float length;

			public float effectorPositionWeight;

			public float effectorRotationWeight;

			public Vector3 offset;

			public Node()
			{
			}

			public Node(Transform transform)
			{
				this.transform = transform;
			}

			public Node(Transform transform, float weight)
			{
				this.transform = transform;
				this.weight = weight;
			}
		}

		public delegate void UpdateDelegate();

		public delegate void IterationDelegate(int i);

		[HideInInspector]
		public Vector3 IKPosition;

		[Range(0f, 1f)]
		public float IKPositionWeight = 1f;

		public IKSolver.UpdateDelegate OnPreInitiate;

		public IKSolver.UpdateDelegate OnPostInitiate;

		public IKSolver.UpdateDelegate OnPreUpdate;

		public IKSolver.UpdateDelegate OnPostUpdate;

		protected bool firstInitiation = true;

		[SerializeField]
		protected Transform root;

		public bool initiated
		{
			get;
			private set;
		}

		public abstract bool IsValid(bool log);

		public void Initiate(Transform root)
		{
			if (this.OnPreInitiate != null)
			{
				this.OnPreInitiate();
			}
			if (root == null)
			{
				Debug.LogError("Initiating IKSolver with null root Transform.");
			}
			this.root = root;
			this.initiated = false;
			if (!this.IsValid(Application.isPlaying))
			{
				return;
			}
			this.OnInitiate();
			this.StoreDefaultLocalState();
			this.initiated = true;
			this.firstInitiation = false;
			if (this.OnPostInitiate != null)
			{
				this.OnPostInitiate();
			}
		}

		public void Update()
		{
			if (this.OnPreUpdate != null)
			{
				this.OnPreUpdate();
			}
			if (this.firstInitiation)
			{
				this.LogWarning("Trying to update IK solver before initiating it. If you intended to disable the IK component to manage it's updating, use ik.Disable() instead of ik.enabled = false, the latter does not guarantee solver initiation.");
			}
			if (!this.initiated)
			{
				return;
			}
			this.OnUpdate();
			if (this.OnPostUpdate != null)
			{
				this.OnPostUpdate();
			}
		}

		public Vector3 GetIKPosition()
		{
			return this.IKPosition;
		}

		public void SetIKPosition(Vector3 position)
		{
			this.IKPosition = position;
		}

		public float GetIKPositionWeight()
		{
			return this.IKPositionWeight;
		}

		public void SetIKPositionWeight(float weight)
		{
			this.IKPositionWeight = Mathf.Clamp(weight, 0f, 1f);
		}

		public Transform GetRoot()
		{
			return this.root;
		}

		public abstract IKSolver.Point[] GetPoints();

		public abstract IKSolver.Point GetPoint(Transform transform);

		public abstract void FixTransforms();

		public abstract void StoreDefaultLocalState();

		protected abstract void OnInitiate();

		protected abstract void OnUpdate();

		protected void LogWarning(string message)
		{
			Warning.Log(message, this.root, true);
		}

		public static Transform ContainsDuplicateBone(IKSolver.Bone[] bones)
		{
			for (int i = 0; i < bones.Length; i++)
			{
				for (int j = 0; j < bones.Length; j++)
				{
					if (i != j && bones[i].transform == bones[j].transform)
					{
						return bones[i].transform;
					}
				}
			}
			return null;
		}

		public static bool HierarchyIsValid(IKSolver.Bone[] bones)
		{
			for (int i = 1; i < bones.Length; i++)
			{
				if (!Hierarchy.IsAncestor(bones[i].transform, bones[i - 1].transform))
				{
					return false;
				}
			}
			return true;
		}
	}
}
