using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	internal sealed class LocalClient : NetworkClient
	{
		private struct InternalMsg
		{
			internal byte[] buffer;

			internal int channelId;
		}

		private const int k_InitialFreeMessagePoolSize = 64;

		private List<LocalClient.InternalMsg> m_InternalMsgs = new List<LocalClient.InternalMsg>();

		private List<LocalClient.InternalMsg> m_InternalMsgs2 = new List<LocalClient.InternalMsg>();

		private Stack<LocalClient.InternalMsg> m_FreeMessages;

		private NetworkServer m_LocalServer;

		private bool m_Connected;

		private NetworkMessage s_InternalMessage = new NetworkMessage();

		public override void Disconnect()
		{
			ClientScene.HandleClientDisconnect(this.m_Connection);
			if (this.m_Connected)
			{
				this.PostInternalMessage(33);
				this.m_Connected = false;
			}
			this.m_AsyncConnect = NetworkClient.ConnectState.Disconnected;
		}

		internal void InternalConnectLocalServer()
		{
			if (this.m_FreeMessages == null)
			{
				this.m_FreeMessages = new Stack<LocalClient.InternalMsg>();
				for (int i = 0; i < 64; i++)
				{
					LocalClient.InternalMsg item = default(LocalClient.InternalMsg);
					this.m_FreeMessages.Push(item);
				}
			}
			this.m_LocalServer = NetworkServer.instance;
			this.m_Connection = new ULocalConnectionToServer(this.m_LocalServer);
			base.SetHandlers(this.m_Connection);
			this.m_Connection.connectionId = this.m_LocalServer.AddLocalClient(this);
			this.m_AsyncConnect = NetworkClient.ConnectState.Connected;
			NetworkClient.SetActive(true);
			base.RegisterSystemHandlers(true);
			this.PostInternalMessage(32);
			this.m_Connected = true;
		}

		internal override void Update()
		{
			this.ProcessInternalMessages();
		}

		internal void AddLocalPlayer(PlayerController localPlayer)
		{
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Local client AddLocalPlayer ",
					localPlayer.gameObject.name,
					" conn=",
					this.m_Connection.connectionId
				}));
			}
			this.m_Connection.isReady = true;
			this.m_Connection.SetPlayerController(localPlayer);
			NetworkIdentity unetView = localPlayer.unetView;
			if (unetView != null)
			{
				ClientScene.SetLocalObject(unetView.netId, localPlayer.gameObject);
				unetView.SetConnectionToServer(this.m_Connection);
			}
			ClientScene.InternalAddPlayer(unetView, localPlayer.playerControllerId);
		}

		private void PostInternalMessage(byte[] buffer, int channelId)
		{
			LocalClient.InternalMsg item;
			if (this.m_FreeMessages.Count == 0)
			{
				item = default(LocalClient.InternalMsg);
			}
			else
			{
				item = this.m_FreeMessages.Pop();
			}
			item.buffer = buffer;
			item.channelId = channelId;
			this.m_InternalMsgs.Add(item);
		}

		private void PostInternalMessage(short msgType)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(msgType);
			networkWriter.FinishMessage();
			this.PostInternalMessage(networkWriter.AsArray(), 0);
		}

		private void ProcessInternalMessages()
		{
			if (this.m_InternalMsgs.Count == 0)
			{
				return;
			}
			List<LocalClient.InternalMsg> internalMsgs = this.m_InternalMsgs;
			this.m_InternalMsgs = this.m_InternalMsgs2;
			foreach (LocalClient.InternalMsg current in internalMsgs)
			{
				if (this.s_InternalMessage.reader == null)
				{
					this.s_InternalMessage.reader = new NetworkReader(current.buffer);
				}
				else
				{
					this.s_InternalMessage.reader.Replace(current.buffer);
				}
				this.s_InternalMessage.reader.ReadInt16();
				this.s_InternalMessage.channelId = current.channelId;
				this.s_InternalMessage.conn = base.connection;
				this.s_InternalMessage.msgType = this.s_InternalMessage.reader.ReadInt16();
				this.m_Connection.InvokeHandler(this.s_InternalMessage);
				this.m_FreeMessages.Push(current);
				base.connection.lastMessageTime = Time.time;
			}
			this.m_InternalMsgs = internalMsgs;
			this.m_InternalMsgs.Clear();
			foreach (LocalClient.InternalMsg current2 in this.m_InternalMsgs2)
			{
				this.m_InternalMsgs.Add(current2);
			}
			this.m_InternalMsgs2.Clear();
		}

		internal void InvokeHandlerOnClient(short msgType, MessageBase msg, int channelId)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(msgType);
			msg.Serialize(networkWriter);
			networkWriter.FinishMessage();
			this.InvokeBytesOnClient(networkWriter.AsArray(), channelId);
		}

		internal void InvokeBytesOnClient(byte[] buffer, int channelId)
		{
			this.PostInternalMessage(buffer, channelId);
		}
	}
}
