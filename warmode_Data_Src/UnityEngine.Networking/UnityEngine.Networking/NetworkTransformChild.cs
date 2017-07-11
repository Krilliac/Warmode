using System;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkTransformChild")]
	public class NetworkTransformChild : NetworkBehaviour
	{
		private const float k_LocalMovementThreshold = 1E-05f;

		private const float k_LocalRotationThreshold = 1E-05f;

		[SerializeField]
		private Transform m_Target;

		[SerializeField]
		private uint m_ChildIndex;

		private NetworkTransform m_Root;

		[SerializeField]
		private float m_SendInterval = 0.1f;

		[SerializeField]
		private NetworkTransform.AxisSyncMode m_SyncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;

		[SerializeField]
		private NetworkTransform.CompressionSyncMode m_RotationSyncCompression;

		[SerializeField]
		private float m_MovementThreshold = 0.001f;

		[SerializeField]
		private float m_InterpolateRotation = 0.5f;

		[SerializeField]
		private float m_InterpolateMovement = 0.5f;

		[SerializeField]
		private NetworkTransform.ClientMoveCallback3D m_ClientMoveCallback3D;

		private Vector3 m_TargetSyncPosition;

		private Quaternion m_TargetSyncRotation3D;

		private float m_LastClientSyncTime;

		private float m_LastClientSendTime;

		private Vector3 m_PrevPosition;

		private Quaternion m_PrevRotation;

		private NetworkWriter m_LocalTransformWriter;

		public Transform target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
				this.OnValidate();
			}
		}

		public uint childIndex
		{
			get
			{
				return this.m_ChildIndex;
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

		public float movementThreshold
		{
			get
			{
				return this.m_MovementThreshold;
			}
			set
			{
				this.m_MovementThreshold = value;
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

		public Quaternion targetSyncRotation3D
		{
			get
			{
				return this.m_TargetSyncRotation3D;
			}
		}

		private void OnValidate()
		{
			if (this.m_Target != null)
			{
				Transform parent = this.m_Target.parent;
				if (parent == null)
				{
					if (LogFilter.logError)
					{
						Debug.LogError("NetworkTransformChild target cannot be the root transform.");
					}
					this.m_Target = null;
					return;
				}
				while (parent.parent != null)
				{
					parent = parent.parent;
				}
				this.m_Root = parent.gameObject.GetComponent<NetworkTransform>();
				if (this.m_Root == null)
				{
					if (LogFilter.logError)
					{
						Debug.LogError("NetworkTransformChild root must have NetworkTransform");
					}
					this.m_Target = null;
					return;
				}
			}
			this.m_ChildIndex = 4294967295u;
			NetworkTransformChild[] components = this.m_Root.GetComponents<NetworkTransformChild>();
			uint num = 0u;
			while ((ulong)num < (ulong)((long)components.Length))
			{
				if (components[(int)((UIntPtr)num)] == this)
				{
					this.m_ChildIndex = num;
					break;
				}
				num += 1u;
			}
			if (this.m_ChildIndex == 4294967295u)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkTransformChild component must be a child in the same hierarchy");
				}
				this.m_Target = null;
			}
			if (this.m_SendInterval < 0f)
			{
				this.m_SendInterval = 0f;
			}
			if (this.m_SyncRotationAxis < NetworkTransform.AxisSyncMode.None || this.m_SyncRotationAxis > NetworkTransform.AxisSyncMode.AxisXYZ)
			{
				this.m_SyncRotationAxis = NetworkTransform.AxisSyncMode.None;
			}
			if (this.movementThreshold < 0f)
			{
				this.movementThreshold = 0f;
			}
			if (this.interpolateRotation < 0f)
			{
				this.interpolateRotation = 0.01f;
			}
			if (this.interpolateRotation > 1f)
			{
				this.interpolateRotation = 1f;
			}
			if (this.interpolateMovement < 0f)
			{
				this.interpolateMovement = 0.01f;
			}
			if (this.interpolateMovement > 1f)
			{
				this.interpolateMovement = 1f;
			}
		}

		private void Awake()
		{
			this.m_PrevPosition = this.m_Target.localPosition;
			this.m_PrevRotation = this.m_Target.localRotation;
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
			this.SerializeModeTransform(writer);
			return true;
		}

		private void SerializeModeTransform(NetworkWriter writer)
		{
			writer.Write(this.m_Target.localPosition);
			if (this.m_SyncRotationAxis != NetworkTransform.AxisSyncMode.None)
			{
				NetworkTransform.SerializeRotation3D(writer, this.m_Target.localRotation, this.syncRotationAxis, this.rotationSyncCompression);
			}
			this.m_PrevPosition = this.m_Target.localPosition;
			this.m_PrevRotation = this.m_Target.localRotation;
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
			this.UnserializeModeTransform(reader, initialState);
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
				Vector3 targetSyncPosition = reader.ReadVector3();
				Vector3 zero = Vector3.zero;
				Quaternion targetSyncRotation3D = Quaternion.identity;
				if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
				{
					targetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
				}
				if (!this.m_ClientMoveCallback3D(ref targetSyncPosition, ref zero, ref targetSyncRotation3D))
				{
					return;
				}
				this.m_TargetSyncPosition = targetSyncPosition;
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
			float num = (this.m_Target.localPosition - this.m_PrevPosition).sqrMagnitude;
			if (num < this.movementThreshold)
			{
				num = Quaternion.Angle(this.m_PrevRotation, this.m_Target.localRotation);
				if (num < this.movementThreshold)
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
			if (this.m_LastClientSyncTime != 0f)
			{
				this.m_Target.localPosition = Vector3.Lerp(this.m_Target.localPosition, this.m_TargetSyncPosition, this.m_InterpolateMovement);
				this.m_Target.localRotation = Quaternion.Slerp(this.m_Target.localRotation, this.m_TargetSyncRotation3D, this.m_InterpolateRotation);
			}
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
			float num = (this.m_Target.localPosition - this.m_PrevPosition).sqrMagnitude;
			if (num > 1E-05f)
			{
				return true;
			}
			num = Quaternion.Angle(this.m_Target.localRotation, this.m_PrevRotation);
			return num > 1E-05f;
		}

		[Client]
		private void SendTransform()
		{
			if (!this.HasMoved() || ClientScene.readyConnection == null)
			{
				return;
			}
			this.m_LocalTransformWriter.StartMessage(16);
			this.m_LocalTransformWriter.Write(base.netId);
			this.m_LocalTransformWriter.WritePackedUInt32(this.m_ChildIndex);
			this.SerializeModeTransform(this.m_LocalTransformWriter);
			this.m_PrevPosition = this.m_Target.localPosition;
			this.m_PrevRotation = this.m_Target.localRotation;
			this.m_LocalTransformWriter.FinishMessage();
			ClientScene.readyConnection.SendWriter(this.m_LocalTransformWriter, this.GetNetworkChannel());
		}

		internal static void HandleChildTransform(NetworkMessage netMsg)
		{
			NetworkInstanceId networkInstanceId = netMsg.reader.ReadNetworkId();
			uint num = netMsg.reader.ReadPackedUInt32();
			GameObject gameObject = NetworkServer.FindLocalObject(networkInstanceId);
			if (gameObject == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleChildTransform no gameObject");
				}
				return;
			}
			NetworkTransformChild[] components = gameObject.GetComponents<NetworkTransformChild>();
			if (components == null || components.Length == 0)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleChildTransform no children");
				}
				return;
			}
			if ((ulong)num >= (ulong)((long)components.Length))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleChildTransform childIndex invalid");
				}
				return;
			}
			NetworkTransformChild networkTransformChild = components[(int)((UIntPtr)num)];
			if (networkTransformChild == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleChildTransform null target");
				}
				return;
			}
			if (!networkTransformChild.localPlayerAuthority)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleChildTransform no localPlayerAuthority");
				}
				return;
			}
			if (!netMsg.conn.clientOwnedObjects.Contains(networkInstanceId))
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("NetworkTransformChild netId:" + networkInstanceId + " is not for a valid player");
				}
				return;
			}
			networkTransformChild.UnserializeModeTransform(netMsg.reader, false);
			networkTransformChild.m_LastClientSyncTime = Time.time;
			if (!networkTransformChild.isClient)
			{
				networkTransformChild.m_Target.localPosition = networkTransformChild.m_TargetSyncPosition;
				networkTransformChild.m_Target.localRotation = networkTransformChild.m_TargetSyncRotation3D;
			}
		}

		public override int GetNetworkChannel()
		{
			return 1;
		}

		public override float GetNetworkSendInterval()
		{
			return this.m_SendInterval;
		}
	}
}
