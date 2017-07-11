using System;
using UnityEngine.Networking.NetworkSystem;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkAnimator"), DisallowMultipleComponent, RequireComponent(typeof(NetworkIdentity)), RequireComponent(typeof(Animator))]
	public class NetworkAnimator : NetworkBehaviour
	{
		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private uint m_ParameterSendBits;

		private static AnimationMessage s_AnimationMessage = new AnimationMessage();

		private static AnimationParametersMessage s_AnimationParametersMessage = new AnimationParametersMessage();

		private static AnimationTriggerMessage s_AnimationTriggerMessage = new AnimationTriggerMessage();

		private int m_AnimationHash;

		private int m_TransitionHash;

		private NetworkWriter m_ParameterWriter;

		private float m_SendTimer;

		public string param0;

		public string param1;

		public string param2;

		public string param3;

		public string param4;

		public string param5;

		public Animator animator
		{
			get
			{
				return this.m_Animator;
			}
			set
			{
				this.m_Animator = value;
				this.ResetParameterOptions();
			}
		}

		public void SetParameterAutoSend(int index, bool value)
		{
			if (value)
			{
				this.m_ParameterSendBits |= 1u << index;
			}
			else
			{
				this.m_ParameterSendBits &= ~(1u << index);
			}
		}

		public bool GetParameterAutoSend(int index)
		{
			return (this.m_ParameterSendBits & 1u << index) != 0u;
		}

		internal void ResetParameterOptions()
		{
			Debug.Log("ResetParameterOptions");
			this.m_ParameterSendBits = 0u;
		}

		private void InitializeAuthority()
		{
			this.m_ParameterWriter = new NetworkWriter();
		}

		public override void OnStartServer()
		{
			if (!base.localPlayerAuthority)
			{
				this.InitializeAuthority();
			}
		}

		public override void OnStartLocalPlayer()
		{
			if (base.localPlayerAuthority)
			{
				this.InitializeAuthority();
			}
		}

		private void FixedUpdate()
		{
			if (this.m_ParameterWriter == null)
			{
				return;
			}
			this.CheckSendRate();
			int stateHash;
			float normalizedTime;
			if (!this.CheckAnimStateChanged(out stateHash, out normalizedTime))
			{
				return;
			}
			AnimationMessage animationMessage = new AnimationMessage();
			animationMessage.netId = base.netId;
			animationMessage.stateHash = stateHash;
			animationMessage.normalizedTime = normalizedTime;
			this.m_ParameterWriter.SeekZero();
			this.WriteParameters(this.m_ParameterWriter, false);
			animationMessage.parameters = this.m_ParameterWriter.ToArray();
			if (base.hasAuthority || ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.Send(40, animationMessage);
				return;
			}
			if (base.isServer && !base.localPlayerAuthority)
			{
				NetworkServer.SendToReady(base.gameObject, 40, animationMessage);
			}
		}

		private bool CheckAnimStateChanged(out int stateHash, out float normalizedTime)
		{
			stateHash = 0;
			normalizedTime = 0f;
			if (this.m_Animator.IsInTransition(0))
			{
				AnimatorTransitionInfo animatorTransitionInfo = this.m_Animator.GetAnimatorTransitionInfo(0);
				if (animatorTransitionInfo.fullPathHash != this.m_TransitionHash)
				{
					this.m_TransitionHash = animatorTransitionInfo.fullPathHash;
					this.m_AnimationHash = 0;
					return true;
				}
				return false;
			}
			else
			{
				AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.fullPathHash != this.m_AnimationHash)
				{
					if (this.m_AnimationHash != 0)
					{
						stateHash = currentAnimatorStateInfo.fullPathHash;
						normalizedTime = currentAnimatorStateInfo.normalizedTime;
					}
					this.m_TransitionHash = 0;
					this.m_AnimationHash = currentAnimatorStateInfo.fullPathHash;
					return true;
				}
				return false;
			}
		}

		private void CheckSendRate()
		{
			if (this.GetNetworkSendInterval() != 0f && this.m_SendTimer < Time.time)
			{
				this.m_SendTimer = Time.time + this.GetNetworkSendInterval();
				AnimationParametersMessage animationParametersMessage = new AnimationParametersMessage();
				animationParametersMessage.netId = base.netId;
				this.m_ParameterWriter.SeekZero();
				this.WriteParameters(this.m_ParameterWriter, true);
				animationParametersMessage.parameters = this.m_ParameterWriter.ToArray();
				if (base.hasAuthority && ClientScene.readyConnection != null)
				{
					ClientScene.readyConnection.Send(41, animationParametersMessage);
					return;
				}
				if (base.isServer && !base.localPlayerAuthority)
				{
					NetworkServer.SendToReady(base.gameObject, 41, animationParametersMessage);
				}
			}
		}

		private void SetSendTrackingParam(string p, int i)
		{
			p = "Sent Param: " + p;
			if (i == 0)
			{
				this.param0 = p;
			}
			if (i == 1)
			{
				this.param1 = p;
			}
			if (i == 2)
			{
				this.param2 = p;
			}
			if (i == 3)
			{
				this.param3 = p;
			}
			if (i == 4)
			{
				this.param4 = p;
			}
			if (i == 5)
			{
				this.param5 = p;
			}
		}

		private void SetRecvTrackingParam(string p, int i)
		{
			p = "Recv Param: " + p;
			if (i == 0)
			{
				this.param0 = p;
			}
			if (i == 1)
			{
				this.param1 = p;
			}
			if (i == 2)
			{
				this.param2 = p;
			}
			if (i == 3)
			{
				this.param3 = p;
			}
			if (i == 4)
			{
				this.param4 = p;
			}
			if (i == 5)
			{
				this.param5 = p;
			}
		}

		internal void HandleAnimMsg(AnimationMessage msg, NetworkReader reader)
		{
			if (base.hasAuthority)
			{
				return;
			}
			if (msg.stateHash != 0)
			{
				this.m_Animator.Play(msg.stateHash, 0, msg.normalizedTime);
			}
			this.ReadParameters(reader, false);
		}

		internal void HandleAnimParamsMsg(AnimationParametersMessage msg, NetworkReader reader)
		{
			if (base.hasAuthority)
			{
				return;
			}
			this.ReadParameters(reader, true);
		}

		internal void HandleAnimTriggerMsg(int hash)
		{
			this.m_Animator.SetTrigger(hash);
		}

		private void WriteParameters(NetworkWriter writer, bool autoSend)
		{
			for (int i = 0; i < this.m_Animator.parameters.Length; i++)
			{
				if (!autoSend || this.GetParameterAutoSend(i))
				{
					AnimatorControllerParameter animatorControllerParameter = this.m_Animator.parameters[i];
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
					{
						writer.WritePackedUInt32((uint)this.m_Animator.GetInteger(animatorControllerParameter.nameHash));
						this.SetSendTrackingParam(animatorControllerParameter.name + ":" + this.m_Animator.GetInteger(animatorControllerParameter.nameHash), i);
					}
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
					{
						writer.Write(this.m_Animator.GetFloat(animatorControllerParameter.nameHash));
						this.SetSendTrackingParam(animatorControllerParameter.name + ":" + this.m_Animator.GetFloat(animatorControllerParameter.nameHash), i);
					}
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
					{
						writer.Write(this.m_Animator.GetBool(animatorControllerParameter.nameHash));
						this.SetSendTrackingParam(animatorControllerParameter.name + ":" + this.m_Animator.GetBool(animatorControllerParameter.nameHash), i);
					}
				}
			}
		}

		private void ReadParameters(NetworkReader reader, bool autoSend)
		{
			for (int i = 0; i < this.m_Animator.parameters.Length; i++)
			{
				if (!autoSend || this.GetParameterAutoSend(i))
				{
					AnimatorControllerParameter animatorControllerParameter = this.m_Animator.parameters[i];
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
					{
						int num = (int)reader.ReadPackedUInt32();
						this.m_Animator.SetInteger(animatorControllerParameter.nameHash, num);
						this.SetRecvTrackingParam(animatorControllerParameter.name + ":" + num, i);
					}
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
					{
						float num2 = reader.ReadSingle();
						this.m_Animator.SetFloat(animatorControllerParameter.nameHash, num2);
						this.SetRecvTrackingParam(animatorControllerParameter.name + ":" + num2, i);
					}
					if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
					{
						bool flag = reader.ReadBoolean();
						this.m_Animator.SetBool(animatorControllerParameter.nameHash, flag);
						this.SetRecvTrackingParam(animatorControllerParameter.name + ":" + flag, i);
					}
				}
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				if (this.m_Animator.IsInTransition(0))
				{
					AnimatorStateInfo nextAnimatorStateInfo = this.m_Animator.GetNextAnimatorStateInfo(0);
					writer.Write(nextAnimatorStateInfo.fullPathHash);
					writer.Write(nextAnimatorStateInfo.normalizedTime);
				}
				else
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(0);
					writer.Write(currentAnimatorStateInfo.fullPathHash);
					writer.Write(currentAnimatorStateInfo.normalizedTime);
				}
				this.WriteParameters(writer, false);
				return true;
			}
			return false;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				int stateNameHash = reader.ReadInt32();
				float normalizedTime = reader.ReadSingle();
				this.ReadParameters(reader, false);
				this.m_Animator.Play(stateNameHash, 0, normalizedTime);
			}
		}

		public void SetTrigger(string triggerName)
		{
			this.SetTrigger(Animator.StringToHash(triggerName));
		}

		public void SetTrigger(int hash)
		{
			AnimationTriggerMessage animationTriggerMessage = new AnimationTriggerMessage();
			animationTriggerMessage.netId = base.netId;
			animationTriggerMessage.hash = hash;
			if (base.hasAuthority && base.localPlayerAuthority)
			{
				if (NetworkClient.allClients.Count > 0)
				{
					NetworkConnection readyConnection = ClientScene.readyConnection;
					if (readyConnection != null)
					{
						readyConnection.Send(42, animationTriggerMessage);
					}
				}
				return;
			}
			if (base.isServer && !base.localPlayerAuthority)
			{
				NetworkServer.SendToReady(base.gameObject, 42, animationTriggerMessage);
			}
		}

		internal static void OnAnimationServerMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationMessage>(NetworkAnimator.s_AnimationMessage);
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"OnAnimationMessage for netId=",
					NetworkAnimator.s_AnimationMessage.netId,
					" conn=",
					netMsg.conn
				}));
			}
			GameObject gameObject = NetworkServer.FindLocalObject(NetworkAnimator.s_AnimationMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				NetworkReader reader = new NetworkReader(NetworkAnimator.s_AnimationMessage.parameters);
				component.HandleAnimMsg(NetworkAnimator.s_AnimationMessage, reader);
				NetworkServer.SendToReady(gameObject, 40, NetworkAnimator.s_AnimationMessage);
			}
		}

		internal static void OnAnimationParametersServerMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationParametersMessage>(NetworkAnimator.s_AnimationParametersMessage);
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"OnAnimationParametersMessage for netId=",
					NetworkAnimator.s_AnimationParametersMessage.netId,
					" conn=",
					netMsg.conn
				}));
			}
			GameObject gameObject = NetworkServer.FindLocalObject(NetworkAnimator.s_AnimationParametersMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				NetworkReader reader = new NetworkReader(NetworkAnimator.s_AnimationParametersMessage.parameters);
				component.HandleAnimParamsMsg(NetworkAnimator.s_AnimationParametersMessage, reader);
				NetworkServer.SendToReady(gameObject, 41, NetworkAnimator.s_AnimationParametersMessage);
			}
		}

		internal static void OnAnimationTriggerServerMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationTriggerMessage>(NetworkAnimator.s_AnimationTriggerMessage);
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"OnAnimationTriggerMessage for netId=",
					NetworkAnimator.s_AnimationTriggerMessage.netId,
					" conn=",
					netMsg.conn
				}));
			}
			GameObject gameObject = NetworkServer.FindLocalObject(NetworkAnimator.s_AnimationTriggerMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				component.HandleAnimTriggerMsg(NetworkAnimator.s_AnimationTriggerMessage.hash);
				NetworkServer.SendToReady(gameObject, 42, NetworkAnimator.s_AnimationTriggerMessage);
			}
		}

		internal static void OnAnimationClientMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationMessage>(NetworkAnimator.s_AnimationMessage);
			GameObject gameObject = ClientScene.FindLocalObject(NetworkAnimator.s_AnimationMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				NetworkReader reader = new NetworkReader(NetworkAnimator.s_AnimationMessage.parameters);
				component.HandleAnimMsg(NetworkAnimator.s_AnimationMessage, reader);
			}
		}

		internal static void OnAnimationParametersClientMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationParametersMessage>(NetworkAnimator.s_AnimationParametersMessage);
			GameObject gameObject = ClientScene.FindLocalObject(NetworkAnimator.s_AnimationParametersMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				NetworkReader reader = new NetworkReader(NetworkAnimator.s_AnimationParametersMessage.parameters);
				component.HandleAnimParamsMsg(NetworkAnimator.s_AnimationParametersMessage, reader);
			}
		}

		internal static void OnAnimationTriggerClientMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<AnimationTriggerMessage>(NetworkAnimator.s_AnimationTriggerMessage);
			GameObject gameObject = ClientScene.FindLocalObject(NetworkAnimator.s_AnimationTriggerMessage.netId);
			if (gameObject == null)
			{
				return;
			}
			NetworkAnimator component = gameObject.GetComponent<NetworkAnimator>();
			if (component != null)
			{
				component.HandleAnimTriggerMsg(NetworkAnimator.s_AnimationTriggerMessage.hash);
			}
		}
	}
}
