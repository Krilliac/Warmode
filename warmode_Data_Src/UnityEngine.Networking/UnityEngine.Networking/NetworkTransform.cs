using System;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkTransform"), DisallowMultipleComponent]
	public class NetworkTransform : NetworkBehaviour
	{
		public enum TransformSyncMode
		{
			SyncNone,
			SyncTransform,
			SyncRigidbody2D,
			SyncRigidbody3D,
			SyncCharacterController
		}

		public enum AxisSyncMode
		{
			None,
			AxisX,
			AxisY,
			AxisZ,
			AxisXY,
			AxisXZ,
			AxisYZ,
			AxisXYZ
		}

		public enum CompressionSyncMode
		{
			None,
			Low,
			High
		}

		public delegate bool ClientMoveCallback3D(ref Vector3 position, ref Vector3 velocity, ref Quaternion rotation);

		public delegate bool ClientMoveCallback2D(ref Vector2 position, ref Vector2 velocity, ref float rotation);

		private const float k_LocalMovementThreshold = 1E-05f;

		private const float k_LocalRotationThreshold = 1E-05f;

		private const float k_LocalVelocityThreshold = 1E-05f;

		private const float k_MoveAheadRatio = 0.1f;

		[SerializeField]
		private NetworkTransform.TransformSyncMode m_TransformSyncMode;

		[SerializeField]
		private float m_SendInterval = 0.1f;

		[SerializeField]
		private NetworkTransform.AxisSyncMode m_SyncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;

		[SerializeField]
		private NetworkTransform.CompressionSyncMode m_RotationSyncCompression;

		[SerializeField]
		private bool m_SyncSpin;

		[SerializeField]
		private float m_MovementTheshold = 0.001f;

		[SerializeField]
		private float m_SnapThreshold = 5f;

		[SerializeField]
		private float m_InterpolateRotation = 1f;

		[SerializeField]
		private float m_InterpolateMovement = 1f;

		[SerializeField]
		private NetworkTransform.ClientMoveCallback3D m_ClientMoveCallback3D;

		[SerializeField]
		private NetworkTransform.ClientMoveCallback2D m_ClientMoveCallback2D;

		private Rigidbody m_RigidBody3D;

		private Rigidbody2D m_RigidBody2D;

		private CharacterController m_CharacterController;

		private bool m_Grounded = true;

		private Vector3 m_TargetSyncPosition;

		private Vector3 m_TargetSyncVelocity;

		private Vector3 m_FixedPosDiff;

		private Quaternion m_TargetSyncRotation3D;

		private Vector3 m_TargetSyncAngularVelocity3D;

		private float m_TargetSyncRotation2D;

		private float m_TargetSyncAngularVelocity2D;

		private float m_LastClientSyncTime;

		private float m_LastClientSendTime;

		private Vector3 m_PrevPosition;

		private Quaternion m_PrevRotation;

		private float m_PrevRotation2D;

		private float m_PrevVelocity;

		private NetworkWriter m_LocalTransformWriter;

		public NetworkTransform.TransformSyncMode transformSyncMode
		{
			get
			{
				return this.m_TransformSyncMode;
			}
			set
			{
				this.m_TransformSyncMode = value;
			}
		}

		public float sendInterval
		{
			get
			{
				return this.m_SendInterval;
			}
			set
			{
				this.m_SendInterval = value;
			}
		}

		public NetworkTransform.AxisSyncMode syncRotationAxis
		{
			get
			{
				return this.m_SyncRotationAxis;
			}
			set
			{
				this.m_SyncRotationAxis = value;
			}
		}

		public NetworkTransform.CompressionSyncMode rotationSyncCompression
		{
			get
			{
				return this.m_RotationSyncCompression;
			}
			set
			{
				this.m_RotationSyncCompression = value;
			}
		}

		public bool syncSpin
		{
			get
			{
				return this.m_SyncSpin;
			}
			set
			{
				this.m_SyncSpin = value;
			}
		}

		public float movementTheshold
		{
			get
			{
				return this.m_MovementTheshold;
			}
			set
			{
				this.m_MovementTheshold = value;
			}
		}

		public float snapThreshold
		{
			get
			{
				return this.m_SnapThreshold;
			}
			set
			{
				this.m_SnapThreshold = value;
			}
		}

		public float interpolateRotation
		{
			get
			{
				return this.m_InterpolateRotation;
			}
			set
			{
				this.m_InterpolateRotation = value;
			}
		}

		public float interpolateMovement
		{
			get
			{
				return this.m_InterpolateMovement;
			}
			set
			{
				this.m_InterpolateMovement = value;
			}
		}

		public NetworkTransform.ClientMoveCallback3D clientMoveCallback3D
		{
			get
			{
				return this.m_ClientMoveCallback3D;
			}
			set
			{
				this.m_ClientMoveCallback3D = value;
			}
		}

		public NetworkTransform.ClientMoveCallback2D clientMoveCallback2D
		{
			get
			{
				return this.m_ClientMoveCallback2D;
			}
			set
			{
				this.m_ClientMoveCallback2D = value;
			}
		}

		public CharacterController characterContoller
		{
			get
			{
				return this.m_CharacterController;
			}
		}

		public Rigidbody rigidbody3D
		{
			get
			{
				return this.m_RigidBody3D;
			}
		}

		public Rigidbody2D rigidbody2D
		{
			get
			{
				return this.m_RigidBody2D;
			}
		}

		public float lastSyncTime
		{
			get
			{
				return this.m_LastClientSyncTime;
			}
		}

		public Vector3 targetSyncPosition
		{
			get
			{
				return this.m_TargetSyncPosition;
			}
		}

		public Vector3 targetSyncVelocity
		{
			get
			{
				return this.m_TargetSyncVelocity;
			}
		}

		public Quaternion targetSyncRotation3D
		{
			get
			{
				return this.m_TargetSyncRotation3D;
			}
		}

		public float targetSyncRotation2D
		{
			get
			{
				return this.m_TargetSyncRotation2D;
			}
		}

		public bool grounded
		{
			get
			{
				return this.m_Grounded;
			}
			set
			{
				this.m_Grounded = value;
			}
		}

		private void OnValidate()
		{
			if (this.m_TransformSyncMode < NetworkTransform.TransformSyncMode.SyncNone || this.m_TransformSyncMode > NetworkTransform.TransformSyncMode.SyncCharacterController)
			{
				this.m_TransformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
			}
			if (this.m_SendInterval < 0f)
			{
				this.m_SendInterval = 0f;
			}
			if (this.m_SyncRotationAxis < NetworkTransform.AxisSyncMode.None || this.m_SyncRotationAxis > NetworkTransform.AxisSyncMode.AxisXYZ)
			{
				this.m_SyncRotationAxis = NetworkTransform.AxisSyncMode.None;
			}
			if (this.m_MovementTheshold < 0f)
			{
				this.m_MovementTheshold = 0f;
			}
			if (this.m_SnapThreshold < 0f)
			{
				this.m_SnapThreshold = 0.01f;
			}
			if (this.m_InterpolateRotation < 0f)
			{
				this.m_InterpolateRotation = 0.01f;
			}
			if (this.m_InterpolateMovement < 0f)
			{
				this.m_InterpolateMovement = 0.01f;
			}
		}

		private void Awake()
		{
			this.m_RigidBody3D = base.GetComponent<Rigidbody>();
			this.m_RigidBody2D = base.GetComponent<Rigidbody2D>();
			this.m_CharacterController = base.GetComponent<CharacterController>();
			this.m_PrevPosition = base.transform.position;
			this.m_PrevRotation = base.transform.rotation;
			this.m_PrevVelocity = 0f;
			if (base.localPlayerAuthority)
			{
				this.m_LocalTransformWriter = new NetworkWriter();
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			if (!initialState)
			{
				if (base.syncVarDirtyBits == 0u)
				{
					writer.WritePackedUInt32(0u);
					return false;
				}
				writer.WritePackedUInt32(1u);
			}
			switch (this.transformSyncMode)
			{
			case NetworkTransform.TransformSyncMode.SyncNone:
				return false;
			case NetworkTransform.TransformSyncMode.SyncTransform:
				this.SerializeModeTransform(writer);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody2D:
				this.SerializeMode2D(writer);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody3D:
				this.SerializeMode3D(writer);
				break;
			case NetworkTransform.TransformSyncMode.SyncCharacterController:
				this.SerializeModeCharacterController(writer);
				break;
			}
			return true;
		}

		private void SerializeModeTransform(NetworkWriter writer)
		{
			writer.Write(base.transform.position);
			if (this.m_SyncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				NetworkTransform.SerializeRotation3D(writer, base.transform.rotation, this.syncRotationAxis, this.rotationSyncCompression);
			}
			this.m_PrevPosition = base.transform.position;
			this.m_PrevRotation = base.transform.rotation;
			this.m_PrevVelocity = 0f;
		}

		private void SerializeMode3D(NetworkWriter writer)
		{
			if (base.isServer && this.m_LastClientSyncTime != 0f)
			{
				writer.Write(this.m_TargetSyncPosition);
				NetworkTransform.SerializeVelocity3D(writer, this.m_TargetSyncVelocity, NetworkTransform.CompressionSyncMode.None);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.SerializeRotation3D(writer, this.m_TargetSyncRotation3D, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			else
			{
				writer.Write(this.m_RigidBody3D.position);
				NetworkTransform.SerializeVelocity3D(writer, this.m_RigidBody3D.velocity, NetworkTransform.CompressionSyncMode.None);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.SerializeRotation3D(writer, this.m_RigidBody3D.rotation, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			if (this.m_SyncSpin)
			{
				NetworkTransform.SerializeSpin3D(writer, this.m_RigidBody3D.angularVelocity, this.syncRotationAxis, this.rotationSyncCompression);
			}
			this.m_PrevPosition = this.m_RigidBody3D.position;
			this.m_PrevRotation = base.transform.rotation;
			this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
		}

		private void SerializeModeCharacterController(NetworkWriter writer)
		{
			if (base.isServer && this.m_LastClientSyncTime != 0f)
			{
				writer.Write(this.m_TargetSyncPosition);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.SerializeRotation3D(writer, this.m_TargetSyncRotation3D, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			else
			{
				writer.Write(base.transform.position);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.SerializeRotation3D(writer, base.transform.rotation, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			this.m_PrevPosition = base.transform.position;
			this.m_PrevRotation = base.transform.rotation;
			this.m_PrevVelocity = 0f;
		}

		private void SerializeMode2D(NetworkWriter writer)
		{
			if (base.isServer && this.m_LastClientSyncTime != 0f)
			{
				writer.Write(this.m_TargetSyncPosition);
				NetworkTransform.SerializeVelocity2D(writer, this.m_TargetSyncVelocity, NetworkTransform.CompressionSyncMode.None);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					float num = this.m_TargetSyncRotation2D % 360f;
					if (num < 0f)
					{
						num += 360f;
					}
					NetworkTransform.SerializeRotation2D(writer, num, this.rotationSyncCompression);
				}
			}
			else
			{
				writer.Write(this.m_RigidBody2D.position);
				NetworkTransform.SerializeVelocity2D(writer, this.m_RigidBody2D.velocity, NetworkTransform.CompressionSyncMode.None);
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					float num2 = this.m_RigidBody2D.rotation % 360f;
					if (num2 < 0f)
					{
						num2 += 360f;
					}
					NetworkTransform.SerializeRotation2D(writer, num2, this.rotationSyncCompression);
				}
			}
			if (this.m_SyncSpin)
			{
				NetworkTransform.SerializeSpin2D(writer, this.m_RigidBody2D.angularVelocity, this.rotationSyncCompression);
			}
			this.m_PrevPosition = this.m_RigidBody2D.position;
			this.m_PrevRotation = base.transform.rotation;
			this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (base.isServer && NetworkServer.localClientActive)
			{
				return;
			}
			if (!initialState && reader.ReadPackedUInt32() == 0u)
			{
				return;
			}
			switch (this.transformSyncMode)
			{
			case NetworkTransform.TransformSyncMode.SyncNone:
				return;
			case NetworkTransform.TransformSyncMode.SyncTransform:
				this.UnserializeModeTransform(reader, initialState);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody2D:
				this.UnserializeMode2D(reader, initialState);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody3D:
				this.UnserializeMode3D(reader, initialState);
				break;
			case NetworkTransform.TransformSyncMode.SyncCharacterController:
				this.UnserializeModeCharacterController(reader, initialState);
				break;
			}
			this.m_LastClientSyncTime = Time.time;
		}

		private void UnserializeModeTransform(NetworkReader reader, bool initialState)
		{
			if (base.hasAuthority)
			{
				reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				return;
			}
			if (base.isServer && this.m_ClientMoveCallback3D != null)
			{
				Vector3 position = reader.ReadVector3();
				Vector3 zero = Vector3.zero;
				Quaternion rotation = Quaternion.identity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					rotation = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				if (!this.m_ClientMoveCallback3D(ref position, ref zero, ref rotation))
				{
					return;
				}
				base.transform.position = position;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					base.transform.rotation = rotation;
				}
			}
			else
			{
				base.transform.position = reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					base.transform.rotation = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
		}

		private void UnserializeMode3D(NetworkReader reader, bool initialState)
		{
			if (base.hasAuthority)
			{
				reader.ReadVector3();
				reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				if (this.syncSpin)
				{
					NetworkTransform.UnserializeSpin3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				return;
			}
			if (base.isServer && this.m_ClientMoveCallback3D != null)
			{
				Vector3 targetSyncPosition = reader.ReadVector3();
				Vector3 targetSyncVelocity = reader.ReadVector3();
				Quaternion targetSyncRotation3D = Quaternion.identity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					targetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				if (!this.m_ClientMoveCallback3D(ref targetSyncPosition, ref targetSyncVelocity, ref targetSyncRotation3D))
				{
					return;
				}
				this.m_TargetSyncPosition = targetSyncPosition;
				this.m_TargetSyncVelocity = targetSyncVelocity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation3D = targetSyncRotation3D;
				}
			}
			else
			{
				this.m_TargetSyncPosition = reader.ReadVector3();
				this.m_TargetSyncVelocity = reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			if (this.syncSpin)
			{
				this.m_TargetSyncAngularVelocity3D = NetworkTransform.UnserializeSpin3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
			}
			if (this.m_RigidBody3D == null)
			{
				return;
			}
			if (base.isServer && !base.isClient)
			{
				this.m_RigidBody3D.MovePosition(this.m_TargetSyncPosition);
				this.m_RigidBody3D.MoveRotation(this.m_TargetSyncRotation3D);
				this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
				return;
			}
			if (this.GetNetworkSendInterval() == 0f)
			{
				this.m_RigidBody3D.MovePosition(this.m_TargetSyncPosition);
				this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_RigidBody3D.MoveRotation(this.m_TargetSyncRotation3D);
				}
				if (this.syncSpin)
				{
					this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
				}
				return;
			}
			float magnitude = (this.m_RigidBody3D.position - this.m_TargetSyncPosition).magnitude;
			if (magnitude > this.snapThreshold)
			{
				this.m_RigidBody3D.position = this.m_TargetSyncPosition;
				this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
			}
			if (this.interpolateRotation == 0f && this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
				if (this.syncSpin)
				{
					this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
				}
			}
			if (this.m_InterpolateMovement == 0f)
			{
				this.m_RigidBody3D.position = this.m_TargetSyncPosition;
			}
			if (initialState && this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
			}
		}

		private void UnserializeMode2D(NetworkReader reader, bool initialState)
		{
			if (base.hasAuthority)
			{
				reader.ReadVector2();
				reader.ReadVector2();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.UnserializeRotation2D(reader, this.rotationSyncCompression);
				}
				if (this.syncSpin)
				{
					NetworkTransform.UnserializeSpin2D(reader, this.rotationSyncCompression);
				}
				return;
			}
			if (this.m_RigidBody2D == null)
			{
				return;
			}
			if (base.isServer && this.m_ClientMoveCallback2D != null)
			{
				Vector2 v = reader.ReadVector2();
				Vector2 v2 = reader.ReadVector2();
				float targetSyncRotation2D = 0f;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					targetSyncRotation2D = NetworkTransform.UnserializeRotation2D(reader, this.rotationSyncCompression);
				}
				if (!this.m_ClientMoveCallback2D(ref v, ref v2, ref targetSyncRotation2D))
				{
					return;
				}
				this.m_TargetSyncPosition = v;
				this.m_TargetSyncVelocity = v2;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation2D = targetSyncRotation2D;
				}
			}
			else
			{
				this.m_TargetSyncPosition = reader.ReadVector2();
				this.m_TargetSyncVelocity = reader.ReadVector2();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation2D = NetworkTransform.UnserializeRotation2D(reader, this.rotationSyncCompression);
				}
			}
			if (this.syncSpin)
			{
				this.m_TargetSyncAngularVelocity2D = NetworkTransform.UnserializeSpin2D(reader, this.rotationSyncCompression);
			}
			if (base.isServer && !base.isClient)
			{
				base.transform.position = this.m_TargetSyncPosition;
				this.m_RigidBody2D.MoveRotation(this.m_TargetSyncRotation2D);
				this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
				return;
			}
			if (this.GetNetworkSendInterval() == 0f)
			{
				base.transform.position = this.m_TargetSyncPosition;
				this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_RigidBody2D.MoveRotation(this.m_TargetSyncRotation2D);
				}
				if (this.syncSpin)
				{
					this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
				}
				return;
			}
			float magnitude = (this.m_RigidBody2D.position - this.m_TargetSyncPosition).magnitude;
			if (magnitude > this.snapThreshold)
			{
				this.m_RigidBody2D.position = this.m_TargetSyncPosition;
				this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
			}
			if (this.interpolateRotation == 0f && this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
				if (this.syncSpin)
				{
					this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
				}
			}
			if (this.m_InterpolateMovement == 0f)
			{
				this.m_RigidBody2D.position = this.m_TargetSyncPosition;
			}
			if (initialState)
			{
				this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
			}
		}

		private void UnserializeModeCharacterController(NetworkReader reader, bool initialState)
		{
			if (base.hasAuthority)
			{
				reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				return;
			}
			if (base.isServer && this.m_ClientMoveCallback3D != null)
			{
				Vector3 targetSyncPosition = reader.ReadVector3();
				Quaternion targetSyncRotation3D = Quaternion.identity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					targetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				if (this.m_CharacterController == null)
				{
					return;
				}
				Vector3 velocity = this.m_CharacterController.velocity;
				if (!this.m_ClientMoveCallback3D(ref targetSyncPosition, ref velocity, ref targetSyncRotation3D))
				{
					return;
				}
				this.m_TargetSyncPosition = targetSyncPosition;
				this.m_TargetSyncVelocity = velocity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation3D = targetSyncRotation3D;
				}
			}
			else
			{
				this.m_TargetSyncPosition = reader.ReadVector3();
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					this.m_TargetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
			}
			if (this.m_CharacterController == null)
			{
				return;
			}
			Vector3 a = this.m_TargetSyncPosition - base.transform.position;
			Vector3 a2 = a / this.GetNetworkSendInterval();
			this.m_FixedPosDiff = a2 * Time.fixedDeltaTime;
			if (base.isServer && !base.isClient)
			{
				base.transform.position = this.m_TargetSyncPosition;
				base.transform.rotation = this.m_TargetSyncRotation3D;
				return;
			}
			if (this.GetNetworkSendInterval() == 0f)
			{
				base.transform.position = this.m_TargetSyncPosition;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					base.transform.rotation = this.m_TargetSyncRotation3D;
				}
				return;
			}
			float magnitude = (base.transform.position - this.m_TargetSyncPosition).magnitude;
			if (magnitude > this.snapThreshold)
			{
				base.transform.position = this.m_TargetSyncPosition;
			}
			if (this.interpolateRotation == 0f && this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				base.transform.rotation = this.m_TargetSyncRotation3D;
			}
			if (this.m_InterpolateMovement == 0f)
			{
				base.transform.position = this.m_TargetSyncPosition;
			}
			if (initialState && this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				base.transform.rotation = this.m_TargetSyncRotation3D;
			}
		}

		private void FixedUpdate()
		{
			if (base.isServer)
			{
				this.FixedUpdateServer();
			}
			if (base.isClient)
			{
				this.FixedUpdateClient();
			}
		}

		private void FixedUpdateServer()
		{
			if (base.syncVarDirtyBits != 0u)
			{
				return;
			}
			if (!NetworkServer.active)
			{
				return;
			}
			if (!base.isServer)
			{
				return;
			}
			if (this.GetNetworkSendInterval() == 0f)
			{
				return;
			}
			float num = (base.transform.position - this.m_PrevPosition).magnitude;
			if (num < this.movementTheshold)
			{
				num = Quaternion.Angle(this.m_PrevRotation, base.transform.rotation);
				if (num < this.movementTheshold)
				{
					return;
				}
			}
			base.SetDirtyBit(1u);
		}

		private void FixedUpdateClient()
		{
			if (this.m_LastClientSyncTime == 0f)
			{
				return;
			}
			if (!NetworkServer.active && !NetworkClient.active)
			{
				return;
			}
			if (!base.isServer && !base.isClient)
			{
				return;
			}
			if (this.GetNetworkSendInterval() == 0f)
			{
				return;
			}
			if (base.hasAuthority)
			{
				return;
			}
			switch (this.transformSyncMode)
			{
			case NetworkTransform.TransformSyncMode.SyncNone:
				return;
			case NetworkTransform.TransformSyncMode.SyncTransform:
				return;
			case NetworkTransform.TransformSyncMode.SyncRigidbody2D:
				this.InterpolateTransformMode2D();
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody3D:
				this.InterpolateTransformMode3D();
				break;
			case NetworkTransform.TransformSyncMode.SyncCharacterController:
				this.InterpolateTransformModeCharacterController();
				break;
			}
		}

		private void InterpolateTransformMode3D()
		{
			if (this.m_InterpolateMovement != 0f)
			{
				Vector3 velocity = (this.m_TargetSyncPosition - this.m_RigidBody3D.position) * this.m_InterpolateMovement / this.GetNetworkSendInterval();
				this.m_RigidBody3D.velocity = velocity;
			}
			if (this.interpolateRotation != 0f)
			{
				this.m_RigidBody3D.MoveRotation(Quaternion.Slerp(this.m_RigidBody3D.rotation, this.m_TargetSyncRotation3D, Time.fixedDeltaTime * this.interpolateRotation));
			}
			this.m_TargetSyncPosition += this.m_TargetSyncVelocity * Time.fixedDeltaTime * 0.1f;
		}

		private void InterpolateTransformModeCharacterController()
		{
			if (this.m_FixedPosDiff == Vector3.zero && this.m_TargetSyncRotation3D == base.transform.rotation)
			{
				return;
			}
			if (this.m_InterpolateMovement != 0f)
			{
				this.m_CharacterController.Move(this.m_FixedPosDiff * this.m_InterpolateMovement);
			}
			if (this.interpolateRotation != 0f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.m_TargetSyncRotation3D, Time.fixedDeltaTime * this.interpolateRotation * 10f);
			}
			if (Time.time - this.m_LastClientSyncTime > this.GetNetworkSendInterval())
			{
				this.m_FixedPosDiff = Vector3.zero;
				Vector3 motion = this.m_TargetSyncPosition - base.transform.position;
				this.m_CharacterController.Move(motion);
			}
		}

		private void InterpolateTransformMode2D()
		{
			if (this.m_InterpolateMovement != 0f)
			{
				Vector2 velocity = this.m_RigidBody2D.velocity;
				Vector2 velocity2 = (this.m_TargetSyncPosition - this.m_RigidBody2D.position) * this.m_InterpolateMovement / this.GetNetworkSendInterval();
				if (!this.m_Grounded && velocity2.y < 0f)
				{
					velocity2.y = velocity.y;
				}
				this.m_RigidBody2D.velocity = velocity2;
			}
			if (this.interpolateRotation != 0f)
			{
				float num = this.m_RigidBody2D.rotation % 360f;
				if (num < 0f)
				{
					num += 360f;
				}
				Quaternion quaternion = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 0f, this.m_TargetSyncRotation2D), Time.fixedDeltaTime * this.interpolateRotation / this.GetNetworkSendInterval());
				this.m_RigidBody2D.MoveRotation(quaternion.eulerAngles.z);
				this.m_TargetSyncRotation2D += this.m_TargetSyncAngularVelocity2D * Time.fixedDeltaTime * 0.1f;
			}
			this.m_TargetSyncPosition += this.m_TargetSyncVelocity * Time.fixedDeltaTime * 0.1f;
		}

		private void Update()
		{
			if (!base.hasAuthority)
			{
				return;
			}
			if (!base.localPlayerAuthority)
			{
				return;
			}
			if (NetworkServer.active)
			{
				return;
			}
			if (Time.time - this.m_LastClientSendTime > this.GetNetworkSendInterval())
			{
				this.SendTransform();
				this.m_LastClientSendTime = Time.time;
			}
		}

		private bool HasMoved()
		{
			float num;
			if (this.m_RigidBody3D != null)
			{
				num = (this.m_RigidBody3D.position - this.m_PrevPosition).magnitude;
			}
			else if (this.m_RigidBody2D != null)
			{
				num = (this.m_RigidBody2D.position - this.m_PrevPosition).magnitude;
			}
			else
			{
				num = (base.transform.position - this.m_PrevPosition).magnitude;
			}
			if (num > 1E-05f)
			{
				return true;
			}
			if (this.m_RigidBody3D != null)
			{
				num = Quaternion.Angle(this.m_RigidBody3D.rotation, this.m_PrevRotation);
			}
			else if (this.m_RigidBody2D != null)
			{
				num = Math.Abs(this.m_RigidBody2D.rotation - this.m_PrevRotation2D);
			}
			else
			{
				num = Quaternion.Angle(base.transform.rotation, this.m_PrevRotation);
			}
			if (num > 1E-05f)
			{
				return true;
			}
			if (this.m_RigidBody3D != null)
			{
				num = Mathf.Abs(this.m_RigidBody3D.velocity.sqrMagnitude - this.m_PrevVelocity);
			}
			else if (this.m_RigidBody2D != null)
			{
				num = Mathf.Abs(this.m_RigidBody2D.velocity.sqrMagnitude - this.m_PrevVelocity);
			}
			return num > 1E-05f;
		}

		[Client]
		private void SendTransform()
		{
			if (!this.HasMoved() || ClientScene.readyConnection == null)
			{
				return;
			}
			this.m_LocalTransformWriter.StartMessage(6);
			this.m_LocalTransformWriter.Write(base.netId);
			switch (this.transformSyncMode)
			{
			case NetworkTransform.TransformSyncMode.SyncNone:
				return;
			case NetworkTransform.TransformSyncMode.SyncTransform:
				this.SerializeModeTransform(this.m_LocalTransformWriter);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody2D:
				this.SerializeMode2D(this.m_LocalTransformWriter);
				break;
			case NetworkTransform.TransformSyncMode.SyncRigidbody3D:
				this.SerializeMode3D(this.m_LocalTransformWriter);
				break;
			case NetworkTransform.TransformSyncMode.SyncCharacterController:
				this.SerializeModeCharacterController(this.m_LocalTransformWriter);
				break;
			}
			if (this.m_RigidBody3D != null)
			{
				this.m_PrevPosition = this.m_RigidBody3D.position;
				this.m_PrevRotation = this.m_RigidBody3D.rotation;
				this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
			}
			else if (this.m_RigidBody2D != null)
			{
				this.m_PrevPosition = this.m_RigidBody2D.position;
				this.m_PrevRotation2D = this.m_RigidBody2D.rotation;
				this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
			}
			else
			{
				this.m_PrevPosition = base.transform.position;
				this.m_PrevRotation = base.transform.rotation;
			}
			this.m_LocalTransformWriter.FinishMessage();
			ClientScene.readyConnection.SendWriter(this.m_LocalTransformWriter, this.GetNetworkChannel());
		}

		public static void HandleTransform(NetworkMessage netMsg)
		{
			NetworkInstanceId networkInstanceId = netMsg.reader.ReadNetworkId();
			GameObject gameObject = NetworkServer.FindLocalObject(networkInstanceId);
			if (gameObject == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleTransform no gameObject");
				}
				return;
			}
			NetworkTransform component = gameObject.GetComponent<NetworkTransform>();
			if (component == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleTransform null target");
				}
				return;
			}
			if (!component.localPlayerAuthority)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleTransform no localPlayerAuthority");
				}
				return;
			}
			if (netMsg.conn.clientOwnedObjects == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleTransform object not owned by connection");
				}
				return;
			}
			if (netMsg.conn.clientOwnedObjects.Contains(networkInstanceId))
			{
				switch (component.transformSyncMode)
				{
				case NetworkTransform.TransformSyncMode.SyncNone:
					return;
				case NetworkTransform.TransformSyncMode.SyncTransform:
					component.UnserializeModeTransform(netMsg.reader, false);
					break;
				case NetworkTransform.TransformSyncMode.SyncRigidbody2D:
					component.UnserializeMode2D(netMsg.reader, false);
					break;
				case NetworkTransform.TransformSyncMode.SyncRigidbody3D:
					component.UnserializeMode3D(netMsg.reader, false);
					break;
				case NetworkTransform.TransformSyncMode.SyncCharacterController:
					component.UnserializeModeCharacterController(netMsg.reader, false);
					break;
				}
				component.m_LastClientSyncTime = Time.time;
				return;
			}
			if (LogFilter.logWarn)
			{
				Debug.LogWarning("HandleTransform netId:" + networkInstanceId + " is not for a valid player");
			}
		}

		private static void WriteAngle(NetworkWriter writer, float angle, NetworkTransform.CompressionSyncMode compression)
		{
			switch (compression)
			{
			case NetworkTransform.CompressionSyncMode.None:
				writer.Write(angle);
				break;
			case NetworkTransform.CompressionSyncMode.Low:
				writer.Write((short)angle);
				break;
			case NetworkTransform.CompressionSyncMode.High:
				writer.Write((short)angle);
				break;
			}
		}

		private static float ReadAngle(NetworkReader reader, NetworkTransform.CompressionSyncMode compression)
		{
			switch (compression)
			{
			case NetworkTransform.CompressionSyncMode.None:
				return reader.ReadSingle();
			case NetworkTransform.CompressionSyncMode.Low:
				return (float)reader.ReadInt16();
			case NetworkTransform.CompressionSyncMode.High:
				return (float)reader.ReadInt16();
			default:
				return 0f;
			}
		}

		public static void SerializeVelocity3D(NetworkWriter writer, Vector3 velocity, NetworkTransform.CompressionSyncMode compression)
		{
			writer.Write(velocity);
		}

		public static void SerializeVelocity2D(NetworkWriter writer, Vector2 velocity, NetworkTransform.CompressionSyncMode compression)
		{
			writer.Write(velocity);
		}

		public static void SerializeRotation3D(NetworkWriter writer, Quaternion rot, NetworkTransform.AxisSyncMode mode, NetworkTransform.CompressionSyncMode compression)
		{
			switch (mode)
			{
			case NetworkTransform.AxisSyncMode.AxisX:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.x, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisY:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.y, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisZ:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXY:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.x, compression);
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.y, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXZ:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.x, compression);
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisYZ:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.y, compression);
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXYZ:
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.x, compression);
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.y, compression);
				NetworkTransform.WriteAngle(writer, rot.eulerAngles.z, compression);
				break;
			}
		}

		public static void SerializeRotation2D(NetworkWriter writer, float rot, NetworkTransform.CompressionSyncMode compression)
		{
			NetworkTransform.WriteAngle(writer, rot, compression);
		}

		public static void SerializeSpin3D(NetworkWriter writer, Vector3 angularVelocity, NetworkTransform.AxisSyncMode mode, NetworkTransform.CompressionSyncMode compression)
		{
			switch (mode)
			{
			case NetworkTransform.AxisSyncMode.AxisX:
				NetworkTransform.WriteAngle(writer, angularVelocity.x, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisY:
				NetworkTransform.WriteAngle(writer, angularVelocity.y, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisZ:
				NetworkTransform.WriteAngle(writer, angularVelocity.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXY:
				NetworkTransform.WriteAngle(writer, angularVelocity.x, compression);
				NetworkTransform.WriteAngle(writer, angularVelocity.y, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXZ:
				NetworkTransform.WriteAngle(writer, angularVelocity.x, compression);
				NetworkTransform.WriteAngle(writer, angularVelocity.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisYZ:
				NetworkTransform.WriteAngle(writer, angularVelocity.y, compression);
				NetworkTransform.WriteAngle(writer, angularVelocity.z, compression);
				break;
			case NetworkTransform.AxisSyncMode.AxisXYZ:
				NetworkTransform.WriteAngle(writer, angularVelocity.x, compression);
				NetworkTransform.WriteAngle(writer, angularVelocity.y, compression);
				NetworkTransform.WriteAngle(writer, angularVelocity.z, compression);
				break;
			}
		}

		public static void SerializeSpin2D(NetworkWriter writer, float angularVelocity, NetworkTransform.CompressionSyncMode compression)
		{
			NetworkTransform.WriteAngle(writer, angularVelocity, compression);
		}

		public static Vector3 UnserializeVelocity3D(NetworkReader reader, NetworkTransform.CompressionSyncMode compression)
		{
			return reader.ReadVector3();
		}

		public static Vector3 UnserializeVelocity2D(NetworkReader reader, NetworkTransform.CompressionSyncMode compression)
		{
			return reader.ReadVector2();
		}

		public static Quaternion UnserializeRotation3D(NetworkReader reader, NetworkTransform.AxisSyncMode mode, NetworkTransform.CompressionSyncMode compression)
		{
			Quaternion identity = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			switch (mode)
			{
			case NetworkTransform.AxisSyncMode.AxisX:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), 0f, 0f);
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisY:
				zero.Set(0f, NetworkTransform.ReadAngle(reader, compression), 0f);
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisZ:
				zero.Set(0f, 0f, NetworkTransform.ReadAngle(reader, compression));
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisXY:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression), 0f);
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisXZ:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), 0f, NetworkTransform.ReadAngle(reader, compression));
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisYZ:
				zero.Set(0f, NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression));
				identity.eulerAngles = zero;
				break;
			case NetworkTransform.AxisSyncMode.AxisXYZ:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression));
				identity.eulerAngles = zero;
				break;
			}
			return identity;
		}

		public static float UnserializeRotation2D(NetworkReader reader, NetworkTransform.CompressionSyncMode compression)
		{
			return NetworkTransform.ReadAngle(reader, compression);
		}

		public static Vector3 UnserializeSpin3D(NetworkReader reader, NetworkTransform.AxisSyncMode mode, NetworkTransform.CompressionSyncMode compression)
		{
			Vector3 zero = Vector3.zero;
			switch (mode)
			{
			case NetworkTransform.AxisSyncMode.AxisX:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), 0f, 0f);
				break;
			case NetworkTransform.AxisSyncMode.AxisY:
				zero.Set(0f, NetworkTransform.ReadAngle(reader, compression), 0f);
				break;
			case NetworkTransform.AxisSyncMode.AxisZ:
				zero.Set(0f, 0f, NetworkTransform.ReadAngle(reader, compression));
				break;
			case NetworkTransform.AxisSyncMode.AxisXY:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression), 0f);
				break;
			case NetworkTransform.AxisSyncMode.AxisXZ:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), 0f, NetworkTransform.ReadAngle(reader, compression));
				break;
			case NetworkTransform.AxisSyncMode.AxisYZ:
				zero.Set(0f, NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression));
				break;
			case NetworkTransform.AxisSyncMode.AxisXYZ:
				zero.Set(NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression), NetworkTransform.ReadAngle(reader, compression));
				break;
			}
			return zero;
		}

		public static float UnserializeSpin2D(NetworkReader reader, NetworkTransform.CompressionSyncMode compression)
		{
			return NetworkTransform.ReadAngle(reader, compression);
		}

		public override int GetNetworkChannel()
		{
			return 1;
		}

		public override float GetNetworkSendInterval()
		{
			return this.m_SendInterval;
		}

		public override void OnStartAuthority()
		{
			this.m_LastClientSyncTime = 0f;
		}
	}
}
