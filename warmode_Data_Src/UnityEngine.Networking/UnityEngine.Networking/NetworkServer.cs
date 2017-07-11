using System;
using System.Collections.Generic;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking
{
	public sealed class NetworkServer
	{
		private const int k_MaxEventsPerFrame = 500;

		private const int k_RemoveListInterval = 100;

		private static Type s_NetworkConnectionClass = typeof(NetworkConnection);

		private static bool s_Active;

		private static volatile NetworkServer s_Instance;

		private static object s_Sync = new UnityEngine.Object();

		private static bool s_LocalClientActive;

		private static HashSet<int> s_ExternalConnections = new HashSet<int>();

		private int m_ServerId = -1;

		private int m_ServerPort = -1;

		private int m_RelaySlotId = -1;

		private NetworkMessageHandlers m_MessageHandlers = new NetworkMessageHandlers();

		private ConnectionArray m_Connections = new ConnectionArray();

		private static NetworkScene s_NetworkScene = new NetworkScene();

		private HostTopology m_HostTopology;

		private byte[] m_MsgBuffer;

		private bool m_SendPeerInfo = true;

		private bool m_UseWebSockets;

		private float m_MaxDelay = 0.1f;

		private List<LocalClient> m_LocalClients = new List<LocalClient>();

		private HashSet<NetworkInstanceId> m_RemoveList;

		private int m_RemoveListCount;

		internal static ushort maxPacketSize;

		private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();

		public static Dictionary<short, NetworkMessageDelegate> handlers
		{
			get
			{
				return NetworkServer.instance.m_MessageHandlers.GetHandlers();
			}
		}

		public static List<NetworkConnection> connections
		{
			get
			{
				return NetworkServer.instance.m_Connections.connections;
			}
		}

		public static List<NetworkConnection> localConnections
		{
			get
			{
				return NetworkServer.instance.m_Connections.localConnections;
			}
		}

		public static Dictionary<NetworkInstanceId, NetworkIdentity> objects
		{
			get
			{
				return NetworkServer.s_NetworkScene.localObjects;
			}
		}

		public static bool useWebSockets
		{
			get
			{
				return NetworkServer.instance.m_UseWebSockets;
			}
			set
			{
				NetworkServer.instance.m_UseWebSockets = value;
			}
		}

		public static bool sendPeerInfo
		{
			get
			{
				return NetworkServer.instance.m_SendPeerInfo;
			}
			set
			{
				NetworkServer.instance.m_SendPeerInfo = value;
			}
		}

		public static Type networkConnectionClass
		{
			get
			{
				return NetworkServer.s_NetworkConnectionClass;
			}
		}

		internal static NetworkServer instance
		{
			get
			{
				if (NetworkServer.s_Instance == null)
				{
					object obj = NetworkServer.s_Sync;
					lock (obj)
					{
						if (NetworkServer.s_Instance == null)
						{
							NetworkServer.s_Instance = new NetworkServer();
						}
					}
				}
				return NetworkServer.s_Instance;
			}
		}

		public static bool active
		{
			get
			{
				return NetworkServer.s_Active;
			}
		}

		public static bool localClientActive
		{
			get
			{
				return NetworkServer.s_LocalClientActive;
			}
		}

		public static int numChannels
		{
			get
			{
				return NetworkServer.instance.m_HostTopology.DefaultConfig.ChannelCount;
			}
		}

		public static float maxDelay
		{
			get
			{
				return NetworkServer.instance.m_MaxDelay;
			}
			set
			{
				NetworkServer.instance.InternalSetMaxDelay(value);
			}
		}

		public static HostTopology hostTopology
		{
			get
			{
				return NetworkServer.instance.m_HostTopology;
			}
		}

		private NetworkServer()
		{
			NetworkTransport.Init();
			if (LogFilter.logDev)
			{
				Debug.Log("NetworkServer Created version " + Version.Current);
			}
			this.m_MsgBuffer = new byte[49152];
			this.m_RemoveList = new HashSet<NetworkInstanceId>();
		}

		public static void SetNetworkConnectionClass<T>() where T : NetworkConnection
		{
			NetworkServer.s_NetworkConnectionClass = typeof(T);
		}

		public static bool Configure(ConnectionConfig config, int maxConnections)
		{
			HostTopology topology = new HostTopology(config, maxConnections);
			return NetworkServer.Configure(topology);
		}

		public static bool Configure(HostTopology topology)
		{
			NetworkServer.instance.m_HostTopology = topology;
			return true;
		}

		public static void Reset()
		{
			NetworkTransport.Shutdown();
			NetworkTransport.Init();
			NetworkServer.s_NetworkConnectionClass = typeof(NetworkConnection);
			NetworkServer.s_Instance = null;
			NetworkServer.s_Active = false;
			NetworkServer.s_LocalClientActive = false;
			NetworkServer.s_ExternalConnections = new HashSet<int>();
		}

		public static void Shutdown()
		{
			if (NetworkServer.s_Instance != null)
			{
				NetworkServer.s_Instance.InternalDisconnectAll();
				if (NetworkServer.s_Instance.m_ServerId != -1)
				{
					NetworkTransport.RemoveHost(NetworkServer.s_Instance.m_ServerId);
					NetworkServer.s_Instance.m_ServerId = -1;
				}
				NetworkServer.s_Instance = null;
			}
			NetworkServer.s_ExternalConnections = new HashSet<int>();
			NetworkServer.s_Active = false;
			NetworkServer.s_LocalClientActive = false;
		}

		public static bool Listen(MatchInfo matchInfo, int listenPort)
		{
			if (!matchInfo.usingRelay)
			{
				return NetworkServer.instance.InternalListen(null, listenPort);
			}
			NetworkServer.instance.InternalListenRelay(matchInfo.address, matchInfo.port, matchInfo.networkId, Utility.GetSourceID(), matchInfo.nodeId, listenPort);
			return true;
		}

		public static void ListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId)
		{
			NetworkServer.instance.InternalListenRelay(relayIp, relayPort, netGuid, sourceId, nodeId, 0);
		}

		internal void InternalListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId, int listenPort)
		{
			if (this.m_HostTopology == null)
			{
				ConnectionConfig connectionConfig = new ConnectionConfig();
				connectionConfig.AddChannel(QosType.Reliable);
				connectionConfig.AddChannel(QosType.Unreliable);
				this.m_HostTopology = new HostTopology(connectionConfig, 8);
			}
			this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, listenPort);
			if (LogFilter.logDebug)
			{
				Debug.Log("Server Host Slot Id: " + this.m_ServerId);
			}
			NetworkServer.Update();
			byte b;
			NetworkTransport.ConnectAsNetworkHost(this.m_ServerId, relayIp, relayPort, netGuid, sourceId, nodeId, out b);
			this.m_RelaySlotId = 0;
			if (LogFilter.logDebug)
			{
				Debug.Log("Relay Slot Id: " + this.m_RelaySlotId);
			}
			if (b != 0)
			{
				Debug.Log("ListenRelay Error: " + b);
			}
			NetworkServer.s_Active = true;
			this.m_MessageHandlers.RegisterHandlerSafe(35, new NetworkMessageDelegate(NetworkServer.OnClientReadyMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(5, new NetworkMessageDelegate(NetworkServer.OnCommandMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(6, new NetworkMessageDelegate(NetworkTransform.HandleTransform));
			this.m_MessageHandlers.RegisterHandlerSafe(16, new NetworkMessageDelegate(NetworkTransformChild.HandleChildTransform));
			this.m_MessageHandlers.RegisterHandlerSafe(40, new NetworkMessageDelegate(NetworkAnimator.OnAnimationServerMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(41, new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersServerMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(42, new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerServerMessage));
		}

		public static bool Listen(int serverPort)
		{
			return NetworkServer.instance.InternalListen(null, serverPort);
		}

		public static bool Listen(string ipAddress, int serverPort)
		{
			return NetworkServer.instance.InternalListen(ipAddress, serverPort);
		}

		internal bool InternalListen(string ipAddress, int serverPort)
		{
			if (this.m_HostTopology == null)
			{
				ConnectionConfig connectionConfig = new ConnectionConfig();
				connectionConfig.AddChannel(QosType.Reliable);
				connectionConfig.AddChannel(QosType.Unreliable);
				this.m_HostTopology = new HostTopology(connectionConfig, 8);
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("Server Listen. port: " + serverPort);
			}
			if (string.IsNullOrEmpty(ipAddress))
			{
				if (this.m_UseWebSockets)
				{
					this.m_ServerId = NetworkTransport.AddWebsocketHost(this.m_HostTopology, serverPort);
				}
				else
				{
					this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, serverPort);
				}
			}
			else if (this.m_UseWebSockets)
			{
				this.m_ServerId = NetworkTransport.AddWebsocketHost(this.m_HostTopology, serverPort, ipAddress);
			}
			else
			{
				this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, serverPort, ipAddress);
			}
			if (this.m_ServerId == -1)
			{
				return false;
			}
			this.m_ServerPort = serverPort;
			NetworkServer.s_Active = true;
			NetworkServer.maxPacketSize = NetworkServer.hostTopology.DefaultConfig.PacketSize;
			this.m_MessageHandlers.RegisterHandlerSafe(35, new NetworkMessageDelegate(NetworkServer.OnClientReadyMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(5, new NetworkMessageDelegate(NetworkServer.OnCommandMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(6, new NetworkMessageDelegate(NetworkTransform.HandleTransform));
			this.m_MessageHandlers.RegisterHandlerSafe(16, new NetworkMessageDelegate(NetworkTransformChild.HandleChildTransform));
			this.m_MessageHandlers.RegisterHandlerSafe(38, new NetworkMessageDelegate(NetworkServer.OnRemovePlayerMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(40, new NetworkMessageDelegate(NetworkAnimator.OnAnimationServerMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(41, new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersServerMessage));
			this.m_MessageHandlers.RegisterHandlerSafe(42, new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerServerMessage));
			return true;
		}

		internal void InternalSetMaxDelay(float seconds)
		{
			for (int i = this.m_Connections.LocalIndex; i < this.m_Connections.Count; i++)
			{
				NetworkConnection networkConnection = this.m_Connections.Get(i);
				if (networkConnection != null)
				{
					networkConnection.SetMaxDelay(seconds);
				}
			}
			this.m_MaxDelay = seconds;
		}

		internal int AddLocalClient(LocalClient localClient)
		{
			this.m_LocalClients.Add(localClient);
			ULocalConnectionToClient uLocalConnectionToClient = new ULocalConnectionToClient(localClient);
			uLocalConnectionToClient.SetHandlers(this.m_MessageHandlers);
			uLocalConnectionToClient.InvokeHandlerNoData(32);
			return this.m_Connections.AddLocal(uLocalConnectionToClient);
		}

		internal void SetLocalObjectOnServer(NetworkInstanceId netId, GameObject obj)
		{
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"SetLocalObjectOnServer ",
					netId,
					" ",
					obj
				}));
			}
			NetworkServer.s_NetworkScene.SetLocalObject(netId, obj, false, true);
		}

		internal void ActivateLocalClientScene()
		{
			if (NetworkServer.s_LocalClientActive)
			{
				return;
			}
			NetworkServer.s_LocalClientActive = true;
			foreach (NetworkIdentity current in NetworkServer.objects.Values)
			{
				if (!current.isClient)
				{
					if (LogFilter.logDev)
					{
						Debug.Log(string.Concat(new object[]
						{
							"ActivateClientScene ",
							current.netId,
							" ",
							current.gameObject
						}));
					}
					ClientScene.SetLocalObject(current.netId, current.gameObject);
					current.OnStartClient();
				}
			}
		}

		public static bool SendToAll(short msgType, MessageBase msg)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendToAll msgType:" + msgType);
			}
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			bool flag = true;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					flag &= networkConnection.Send(msgType, msg);
				}
			}
			return flag;
		}

		private static bool SendToObservers(GameObject contextObj, short msgType, MessageBase msg)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendToObservers id:" + msgType);
			}
			bool flag = true;
			NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
			if (component == null || component.observers == null)
			{
				return false;
			}
			int count = component.observers.Count;
			for (int i = 0; i < count; i++)
			{
				NetworkConnection networkConnection = component.observers[i];
				flag &= networkConnection.Send(msgType, msg);
			}
			return flag;
		}

		public static bool SendToReady(GameObject contextObj, short msgType, MessageBase msg)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendToReady id:" + msgType);
			}
			if (contextObj == null)
			{
				for (int i = NetworkServer.s_Instance.m_Connections.LocalIndex; i < NetworkServer.s_Instance.m_Connections.Count; i++)
				{
					NetworkConnection networkConnection = NetworkServer.s_Instance.m_Connections.Get(i);
					if (networkConnection != null && networkConnection.isReady)
					{
						networkConnection.Send(msgType, msg);
					}
				}
				return true;
			}
			bool flag = true;
			NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
			if (component == null || component.observers == null)
			{
				return false;
			}
			int count = component.observers.Count;
			for (int j = 0; j < count; j++)
			{
				NetworkConnection networkConnection2 = component.observers[j];
				if (networkConnection2.isReady)
				{
					flag &= networkConnection2.Send(msgType, msg);
				}
			}
			return flag;
		}

		public static void SendWriterToReady(GameObject contextObj, NetworkWriter writer, int channelId)
		{
			if (writer.AsArraySegment().Count > 32767)
			{
				throw new UnityException("NetworkWriter used buffer is too big!");
			}
			NetworkServer.SendBytesToReady(contextObj, writer.AsArraySegment().Array, writer.AsArraySegment().Count, channelId);
		}

		public static void SendBytesToReady(GameObject contextObj, byte[] buffer, int numBytes, int channelId)
		{
			if (contextObj == null)
			{
				bool flag = true;
				for (int i = NetworkServer.s_Instance.m_Connections.LocalIndex; i < NetworkServer.s_Instance.m_Connections.Count; i++)
				{
					NetworkConnection networkConnection = NetworkServer.s_Instance.m_Connections.Get(i);
					if (networkConnection != null && networkConnection.isReady && !networkConnection.SendBytes(buffer, numBytes, channelId))
					{
						flag = false;
					}
				}
				if (!flag && LogFilter.logWarn)
				{
					Debug.LogWarning("SendBytesToReady failed");
				}
				return;
			}
			NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
			try
			{
				bool flag2 = true;
				int count = component.observers.Count;
				for (int j = 0; j < count; j++)
				{
					NetworkConnection networkConnection2 = component.observers[j];
					if (networkConnection2.isReady)
					{
						if (!networkConnection2.SendBytes(buffer, numBytes, channelId))
						{
							flag2 = false;
						}
					}
				}
				if (!flag2 && LogFilter.logWarn)
				{
					Debug.LogWarning("SendBytesToReady failed for " + contextObj);
				}
			}
			catch (NullReferenceException)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("SendBytesToReady object " + contextObj + " has not been spawned");
				}
			}
		}

		public static void SendBytesToPlayer(GameObject player, byte[] buffer, int numBytes, int channelId)
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			NetworkConnection networkConnection;
			if (connections.ContainsPlayer(player, out networkConnection))
			{
				networkConnection.SendBytes(buffer, numBytes, channelId);
			}
		}

		public static bool SendUnreliableToAll(short msgType, MessageBase msg)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendUnreliableToAll msgType:" + msgType);
			}
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			bool flag = true;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					flag &= networkConnection.SendUnreliable(msgType, msg);
				}
			}
			return flag;
		}

		public static bool SendUnreliableToReady(GameObject contextObj, short msgType, MessageBase msg)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendUnreliableToReady id:" + msgType);
			}
			if (contextObj == null)
			{
				for (int i = NetworkServer.s_Instance.m_Connections.LocalIndex; i < NetworkServer.s_Instance.m_Connections.Count; i++)
				{
					NetworkConnection networkConnection = NetworkServer.s_Instance.m_Connections.Get(i);
					if (networkConnection != null && networkConnection.isReady)
					{
						networkConnection.SendUnreliable(msgType, msg);
					}
				}
				return true;
			}
			bool flag = true;
			NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
			int count = component.observers.Count;
			for (int j = 0; j < count; j++)
			{
				NetworkConnection networkConnection2 = component.observers[j];
				if (networkConnection2.isReady)
				{
					flag &= networkConnection2.SendUnreliable(msgType, msg);
				}
			}
			return flag;
		}

		public static bool SendByChannelToAll(short msgType, MessageBase msg, int channelId)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendByChannelToAll id:" + msgType);
			}
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			bool flag = true;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					flag &= networkConnection.SendByChannel(msgType, msg, channelId);
				}
			}
			return flag;
		}

		public static bool SendByChannelToReady(GameObject contextObj, short msgType, MessageBase msg, int channelId)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("Server.SendByChannelToReady msgType:" + msgType);
			}
			if (contextObj == null)
			{
				for (int i = NetworkServer.s_Instance.m_Connections.LocalIndex; i < NetworkServer.s_Instance.m_Connections.Count; i++)
				{
					NetworkConnection networkConnection = NetworkServer.s_Instance.m_Connections.Get(i);
					if (networkConnection != null && networkConnection.isReady)
					{
						networkConnection.SendByChannel(msgType, msg, channelId);
					}
				}
				return true;
			}
			bool flag = true;
			NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
			int count = component.observers.Count;
			for (int j = 0; j < count; j++)
			{
				NetworkConnection networkConnection2 = component.observers[j];
				if (networkConnection2.isReady)
				{
					flag &= networkConnection2.SendByChannel(msgType, msg, channelId);
				}
			}
			return flag;
		}

		public static void DisconnectAll()
		{
			NetworkServer.instance.InternalDisconnectAll();
		}

		internal void InternalDisconnectAll()
		{
			for (int i = this.m_Connections.LocalIndex; i < this.m_Connections.Count; i++)
			{
				NetworkConnection networkConnection = this.m_Connections.Get(i);
				if (networkConnection != null)
				{
					networkConnection.Disconnect();
					networkConnection.Dispose();
				}
			}
			NetworkServer.s_Active = false;
			NetworkServer.s_LocalClientActive = false;
		}

		internal static void Update()
		{
			if (NetworkServer.s_Instance != null)
			{
				NetworkServer.s_Instance.InternalUpdate();
			}
		}

		internal void UpdateServerObjects()
		{
			foreach (NetworkIdentity current in NetworkServer.objects.Values)
			{
				try
				{
					current.UNetUpdate();
				}
				catch (NullReferenceException)
				{
				}
			}
			if (this.m_RemoveListCount++ % 100 == 0)
			{
				this.CheckForNullObjects();
			}
		}

		private void CheckForNullObjects()
		{
			foreach (NetworkInstanceId current in NetworkServer.objects.Keys)
			{
				NetworkIdentity networkIdentity = NetworkServer.objects[current];
				if (networkIdentity == null || networkIdentity.gameObject == null)
				{
					this.m_RemoveList.Add(current);
				}
			}
			if (this.m_RemoveList.Count > 0)
			{
				foreach (NetworkInstanceId current2 in this.m_RemoveList)
				{
					NetworkServer.objects.Remove(current2);
				}
				this.m_RemoveList.Clear();
			}
		}

		internal void InternalUpdate()
		{
			if (this.m_ServerId == -1 || !NetworkTransport.IsStarted)
			{
				return;
			}
			int num = 0;
			byte b;
			if (this.m_RelaySlotId != -1)
			{
				NetworkEventType networkEventType = NetworkTransport.ReceiveRelayEventFromHost(this.m_ServerId, out b);
				if (networkEventType != NetworkEventType.Nothing && LogFilter.logDebug)
				{
					Debug.Log("NetGroup event:" + networkEventType);
				}
				if (networkEventType == NetworkEventType.ConnectEvent && LogFilter.logDebug)
				{
					Debug.Log("NetGroup server connected");
				}
				if (networkEventType == NetworkEventType.DisconnectEvent && LogFilter.logDebug)
				{
					Debug.Log("NetGroup server disconnected");
				}
			}
			NetworkConnection networkConnection;
			while (true)
			{
				int num2;
				int channelId;
				int numBytes;
				NetworkEventType networkEventType = NetworkTransport.ReceiveFromHost(this.m_ServerId, out num2, out channelId, this.m_MsgBuffer, (int)((ushort)this.m_MsgBuffer.Length), out numBytes, out b);
				if (networkEventType != NetworkEventType.Nothing && LogFilter.logDev)
				{
					Debug.Log(string.Concat(new object[]
					{
						"Server event: host=",
						this.m_ServerId,
						" event=",
						networkEventType,
						" error=",
						b
					}));
				}
				switch (networkEventType)
				{
				case NetworkEventType.DataEvent:
					networkConnection = this.m_Connections.Get(num2);
					if (b != 0)
					{
						goto Block_15;
					}
					if (networkConnection != null)
					{
						networkConnection.TransportRecieve(this.m_MsgBuffer, numBytes, channelId);
					}
					else if (LogFilter.logError)
					{
						Debug.LogError("Unknown connection data event?!?");
					}
					break;
				case NetworkEventType.ConnectEvent:
				{
					if (LogFilter.logDebug)
					{
						Debug.Log("Server accepted client:" + num2);
					}
					if (b != 0)
					{
						goto Block_13;
					}
					string networkAddress;
					int num3;
					NetworkID networkID;
					NodeID nodeID;
					byte b2;
					NetworkTransport.GetConnectionInfo(this.m_ServerId, num2, out networkAddress, out num3, out networkID, out nodeID, out b2);
					NetworkConnection networkConnection2 = (NetworkConnection)Activator.CreateInstance(NetworkServer.s_NetworkConnectionClass);
					networkConnection2.SetHandlers(this.m_MessageHandlers);
					networkConnection2.Initialize(networkAddress, this.m_ServerId, num2, this.m_HostTopology);
					networkConnection2.SetMaxDelay(this.m_MaxDelay);
					this.m_Connections.Add(num2, networkConnection2);
					networkConnection2.InvokeHandlerNoData(32);
					if (this.m_SendPeerInfo)
					{
						this.SendNetworkInfo(networkConnection2);
					}
					NetworkServer.SendCrc(networkConnection2);
					break;
				}
				case NetworkEventType.DisconnectEvent:
				{
					NetworkConnection @unsafe = this.m_Connections.GetUnsafe(num2);
					if (b != 0)
					{
						if (b != 6)
						{
							this.GenerateDisconnectError(@unsafe, (int)b);
						}
					}
					this.m_Connections.Remove(num2);
					if (@unsafe != null)
					{
						@unsafe.InvokeHandlerNoData(33);
						for (int i = 0; i < @unsafe.playerControllers.Count; i++)
						{
							if (@unsafe.playerControllers[i].gameObject != null && LogFilter.logWarn)
							{
								Debug.LogWarning("Player not destroyed when connection disconnected.");
							}
						}
						if (LogFilter.logDebug)
						{
							Debug.Log("Server lost client:" + num2);
						}
						@unsafe.RemoveObservers();
						@unsafe.Dispose();
					}
					else if (LogFilter.logDebug)
					{
						Debug.Log("Connection is null in disconnect event");
					}
					if (this.m_SendPeerInfo)
					{
						this.SendNetworkInfo(@unsafe);
					}
					break;
				}
				case NetworkEventType.Nothing:
					break;
				default:
					if (LogFilter.logError)
					{
						Debug.LogError("Unknown network message type received: " + networkEventType);
					}
					break;
				}
				if (++num >= 500)
				{
					goto Block_28;
				}
				if (networkEventType == NetworkEventType.Nothing)
				{
					goto IL_39D;
				}
			}
			Block_13:
			this.GenerateConnectError((int)b);
			return;
			Block_15:
			this.GenerateDataError(networkConnection, (int)b);
			return;
			Block_28:
			if (LogFilter.logDebug)
			{
				Debug.Log("kMaxEventsPerFrame hit (" + 500 + ")");
			}
			IL_39D:
			this.UpdateServerObjects();
			for (int j = this.m_Connections.LocalIndex; j < this.m_Connections.Count; j++)
			{
				NetworkConnection networkConnection3 = this.m_Connections.Get(j);
				if (networkConnection3 != null)
				{
					networkConnection3.FlushChannels();
				}
			}
		}

		private void GenerateConnectError(int error)
		{
			if (LogFilter.logError)
			{
				Debug.LogError("UNet Server Connect Error: " + error);
			}
			this.GenerateError(null, error);
		}

		private void GenerateDataError(NetworkConnection conn, int error)
		{
			if (LogFilter.logError)
			{
				Debug.LogError("UNet Server Data Error: " + (NetworkError)error);
			}
			this.GenerateError(conn, error);
		}

		private void GenerateDisconnectError(NetworkConnection conn, int error)
		{
			if (LogFilter.logError)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"UNet Server Disconnect Error: ",
					(NetworkError)error,
					" conn:[",
					conn,
					"]:",
					conn.connectionId
				}));
			}
			this.GenerateError(conn, error);
		}

		private void GenerateError(NetworkConnection conn, int error)
		{
			NetworkMessageDelegate handler = this.m_MessageHandlers.GetHandler(34);
			if (handler != null)
			{
				ErrorMessage errorMessage = new ErrorMessage();
				errorMessage.errorCode = error;
				NetworkWriter writer = new NetworkWriter();
				errorMessage.Serialize(writer);
				NetworkReader reader = new NetworkReader(writer);
				conn.InvokeHandler(34, reader, 0);
			}
		}

		public static void RegisterHandler(short msgType, NetworkMessageDelegate handler)
		{
			NetworkServer.instance.m_MessageHandlers.RegisterHandler(msgType, handler);
		}

		public static void UnregisterHandler(short msgType)
		{
			NetworkServer.instance.m_MessageHandlers.UnregisterHandler(msgType);
		}

		public static void ClearHandlers()
		{
			NetworkServer.instance.m_MessageHandlers.ClearMessageHandlers();
		}

		public static void ClearSpawners()
		{
			NetworkScene.ClearSpawners();
		}

		public static void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
		{
			numMsgs = 0;
			numBufferedMsgs = 0;
			numBytes = 0;
			lastBufferedPerSecond = 0;
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					int num;
					int num2;
					int num3;
					int num4;
					networkConnection.GetStatsOut(out num, out num2, out num3, out num4);
					numMsgs += num;
					numBufferedMsgs += num2;
					numBytes += num3;
					lastBufferedPerSecond += num4;
				}
			}
		}

		public static void GetStatsIn(out int numMsgs, out int numBytes)
		{
			numMsgs = 0;
			numBytes = 0;
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					int num;
					int num2;
					networkConnection.GetStatsIn(out num, out num2);
					numMsgs += num;
					numBytes += num2;
				}
			}
		}

		public static void SendToClientOfPlayer(GameObject player, short msgType, MessageBase msg)
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			NetworkConnection networkConnection;
			if (connections.ContainsPlayer(player, out networkConnection))
			{
				networkConnection.Send(msgType, msg);
			}
			else if (LogFilter.logError)
			{
				Debug.LogError("Failed to send message to player object '" + player.name + ", not found in connection list");
			}
		}

		public static void SendToClient(int connectionId, short msgType, MessageBase msg)
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			NetworkConnection networkConnection = connections.Get(connectionId);
			if (networkConnection != null)
			{
				networkConnection.Send(msgType, msg);
			}
			else if (LogFilter.logError)
			{
				Debug.LogError("Failed to send message to connection ID '" + connectionId + ", not found in connection list");
			}
		}

		public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
		{
			NetworkIdentity networkIdentity;
			if (NetworkServer.GetNetworkIdentity(player, out networkIdentity))
			{
				networkIdentity.SetDynamicAssetId(assetId);
			}
			return NetworkServer.instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);
		}

		public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId)
		{
			return NetworkServer.instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);
		}

		public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
		{
			NetworkIdentity networkIdentity;
			if (NetworkServer.GetNetworkIdentity(player, out networkIdentity))
			{
				networkIdentity.SetDynamicAssetId(assetId);
			}
			return NetworkServer.instance.InternalAddPlayerForConnection(conn, player, playerControllerId);
		}

		public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId)
		{
			return NetworkServer.instance.InternalAddPlayerForConnection(conn, player, playerControllerId);
		}

		internal bool InternalAddPlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
		{
			NetworkIdentity networkIdentity;
			if (!NetworkServer.GetNetworkIdentity(playerGameObject, out networkIdentity))
			{
				if (LogFilter.logError)
				{
					Debug.Log("AddPlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
				}
				return false;
			}
			if (!NetworkServer.CheckPlayerControllerIdForConnection(conn, playerControllerId))
			{
				return false;
			}
			PlayerController playerController = null;
			GameObject x = null;
			if (conn.GetPlayerController(playerControllerId, out playerController))
			{
				x = playerController.gameObject;
			}
			if (x != null)
			{
				if (LogFilter.logError)
				{
					Debug.Log("AddPlayer: player object already exists for playerControllerId of " + playerControllerId);
				}
				return false;
			}
			PlayerController playerController2 = new PlayerController(playerGameObject, playerControllerId);
			conn.SetPlayerController(playerController2);
			networkIdentity.SetConnectionToClient(conn, playerController2.playerControllerId);
			NetworkServer.SetClientReady(conn);
			if (this.SetupLocalPlayerForConnection(conn, networkIdentity, playerController2))
			{
				return true;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Adding new playerGameObject object netId: ",
					playerGameObject.GetComponent<NetworkIdentity>().netId,
					" asset ID ",
					playerGameObject.GetComponent<NetworkIdentity>().assetId
				}));
			}
			NetworkServer.FinishPlayerForConnection(conn, networkIdentity, playerGameObject);
			if (networkIdentity.localPlayerAuthority)
			{
				networkIdentity.SetClientOwner(conn);
			}
			return true;
		}

		private static bool CheckPlayerControllerIdForConnection(NetworkConnection conn, short playerControllerId)
		{
			if (playerControllerId < 0)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AddPlayer: playerControllerId of " + playerControllerId + " is negative");
				}
				return false;
			}
			if (playerControllerId > 32)
			{
				if (LogFilter.logError)
				{
					Debug.Log(string.Concat(new object[]
					{
						"AddPlayer: playerControllerId of ",
						playerControllerId,
						" is too high. max is ",
						32
					}));
				}
				return false;
			}
			if (playerControllerId > 16 && LogFilter.logWarn)
			{
				Debug.LogWarning("AddPlayer: playerControllerId of " + playerControllerId + " is unusually high");
			}
			return true;
		}

		private bool SetupLocalPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, PlayerController newPlayerController)
		{
			if (LogFilter.logDev)
			{
				Debug.Log("NetworkServer SetupLocalPlayerForConnection netID:" + uv.netId);
			}
			ULocalConnectionToClient uLocalConnectionToClient = conn as ULocalConnectionToClient;
			if (uLocalConnectionToClient != null)
			{
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkServer AddPlayer handling ULocalConnectionToClient");
				}
				if (uv.netId.IsEmpty())
				{
					uv.OnStartServer();
				}
				uv.RebuildObservers(true);
				this.SendSpawnMessage(uv, null);
				uLocalConnectionToClient.localClient.AddLocalPlayer(newPlayerController);
				uv.ForceAuthority(true);
				uv.SetLocalPlayer(newPlayerController.playerControllerId);
				return true;
			}
			return false;
		}

		private static void FinishPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, GameObject playerGameObject)
		{
			if (uv.netId.IsEmpty())
			{
				NetworkServer.Spawn(playerGameObject);
			}
			conn.Send(4, new OwnerMessage
			{
				netId = uv.netId,
				playerControllerId = uv.playerControllerId
			});
		}

		internal bool InternalReplacePlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
		{
			NetworkIdentity networkIdentity;
			if (!NetworkServer.GetNetworkIdentity(playerGameObject, out networkIdentity))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("ReplacePlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
				}
				return false;
			}
			if (!NetworkServer.CheckPlayerControllerIdForConnection(conn, playerControllerId))
			{
				return false;
			}
			if (LogFilter.logDev)
			{
				Debug.Log("NetworkServer ReplacePlayer");
			}
			PlayerController playerController;
			if (conn.GetPlayerController(playerControllerId, out playerController))
			{
				playerController.unetView.SetNotLocalPlayer();
			}
			PlayerController playerController2 = new PlayerController(playerGameObject, playerControllerId);
			conn.SetPlayerController(playerController2);
			networkIdentity.SetConnectionToClient(conn, playerController2.playerControllerId);
			if (LogFilter.logDev)
			{
				Debug.Log("NetworkServer ReplacePlayer setup local");
			}
			if (this.SetupLocalPlayerForConnection(conn, networkIdentity, playerController2))
			{
				return true;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Replacing playerGameObject object netId: ",
					playerGameObject.GetComponent<NetworkIdentity>().netId,
					" asset ID ",
					playerGameObject.GetComponent<NetworkIdentity>().assetId
				}));
			}
			NetworkServer.FinishPlayerForConnection(conn, networkIdentity, playerGameObject);
			if (networkIdentity.localPlayerAuthority)
			{
				networkIdentity.SetClientOwner(conn);
			}
			return true;
		}

		private static bool GetNetworkIdentity(GameObject go, out NetworkIdentity view)
		{
			view = go.GetComponent<NetworkIdentity>();
			if (view == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("UNET failure. GameObject doesn't have NetworkIdentity.");
				}
				return false;
			}
			return true;
		}

		public static void SetClientReady(NetworkConnection conn)
		{
			NetworkServer.instance.SetClientReadyInternal(conn);
		}

		internal void SetClientReadyInternal(NetworkConnection conn)
		{
			if (conn.isReady)
			{
				if (LogFilter.logDebug)
				{
					Debug.Log("SetClientReady conn " + conn.connectionId + " already ready");
				}
				return;
			}
			if (conn.playerControllers.Count == 0 && LogFilter.logDebug)
			{
				Debug.LogWarning("Ready with no player object");
			}
			conn.isReady = true;
			ULocalConnectionToClient uLocalConnectionToClient = conn as ULocalConnectionToClient;
			if (uLocalConnectionToClient != null)
			{
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkServer Ready handling ULocalConnectionToClient");
				}
				foreach (NetworkIdentity current in NetworkServer.objects.Values)
				{
					if (current != null && current.gameObject != null)
					{
						bool flag = current.OnCheckObserver(conn);
						if (flag)
						{
							current.AddObserver(conn);
						}
						if (!current.isClient)
						{
							if (LogFilter.logDev)
							{
								Debug.Log("LocalClient.SetSpawnObject calling OnStartClient");
							}
							current.OnStartClient();
						}
					}
				}
				return;
			}
			ObjectSpawnFinishedMessage objectSpawnFinishedMessage = new ObjectSpawnFinishedMessage();
			objectSpawnFinishedMessage.state = 0u;
			conn.Send(12, objectSpawnFinishedMessage);
			foreach (NetworkIdentity current2 in NetworkServer.objects.Values)
			{
				if (current2 == null)
				{
					if (LogFilter.logWarn)
					{
						Debug.LogWarning("Invalid object found in server local object list (null NetworkIdentity).");
					}
				}
				else
				{
					if (LogFilter.logDebug)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Sending spawn message for current server objects name='",
							current2.gameObject.name,
							"' netId=",
							current2.netId
						}));
					}
					bool flag2 = current2.OnCheckObserver(conn);
					if (flag2)
					{
						current2.AddObserver(conn);
					}
				}
			}
			objectSpawnFinishedMessage.state = 1u;
			conn.Send(12, objectSpawnFinishedMessage);
		}

		internal static void ShowForConnection(NetworkIdentity uv, NetworkConnection conn)
		{
			if (conn.isReady)
			{
				NetworkServer.instance.SendSpawnMessage(uv, conn);
			}
		}

		internal static void HideForConnection(NetworkIdentity uv, NetworkConnection conn)
		{
			conn.Send(13, new ObjectDestroyMessage
			{
				netId = uv.netId
			});
		}

		public static void SetAllClientsNotReady()
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					NetworkServer.SetClientNotReady(networkConnection);
				}
			}
		}

		public static void SetClientNotReady(NetworkConnection conn)
		{
			NetworkServer.instance.InternalSetClientNotReady(conn);
		}

		internal void InternalSetClientNotReady(NetworkConnection conn)
		{
			if (conn.isReady)
			{
				if (LogFilter.logDebug)
				{
					Debug.Log("PlayerNotReady " + conn);
				}
				conn.isReady = false;
				conn.RemoveObservers();
				NotReadyMessage msg = new NotReadyMessage();
				conn.Send(36, msg);
			}
		}

		private static void OnClientReadyMessage(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("Default handler for ready message from " + netMsg.conn);
			}
			NetworkServer.SetClientReady(netMsg.conn);
		}

		private static void OnRemovePlayerMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<RemovePlayerMessage>(NetworkServer.s_RemovePlayerMessage);
			PlayerController playerController = null;
			netMsg.conn.GetPlayerController(NetworkServer.s_RemovePlayerMessage.playerControllerId, out playerController);
			if (playerController != null)
			{
				netMsg.conn.RemovePlayerController(NetworkServer.s_RemovePlayerMessage.playerControllerId);
				NetworkServer.Destroy(playerController.gameObject);
			}
			else if (LogFilter.logError)
			{
				Debug.LogError("Received remove player message but could not find the player ID: " + NetworkServer.s_RemovePlayerMessage.playerControllerId);
			}
		}

		private static void OnCommandMessage(NetworkMessage netMsg)
		{
			int cmdHash = (int)netMsg.reader.ReadPackedUInt32();
			NetworkInstanceId networkInstanceId = netMsg.reader.ReadNetworkId();
			GameObject gameObject = NetworkServer.FindLocalObject(networkInstanceId);
			if (gameObject == null)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("Instance not found when handling Command message [netId=" + networkInstanceId + "]");
				}
				return;
			}
			NetworkIdentity component = gameObject.GetComponent<NetworkIdentity>();
			if (component == null)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("NetworkIdentity deleted when handling Command message [netId=" + networkInstanceId + "]");
				}
				return;
			}
			bool flag = false;
			foreach (PlayerController current in netMsg.conn.playerControllers)
			{
				if (current.gameObject != null && current.gameObject.GetComponent<NetworkIdentity>().netId == component.netId)
				{
					flag = true;
					break;
				}
			}
			if (!flag && component.clientAuthorityOwner != netMsg.conn)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("Command for object without authority [netId=" + networkInstanceId + "]");
				}
				return;
			}
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"OnCommandMessage for netId=",
					networkInstanceId,
					" conn=",
					netMsg.conn
				}));
			}
			component.HandleCommand(cmdHash, netMsg.reader);
		}

		internal void SpawnObject(GameObject obj)
		{
			if (!NetworkServer.active)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("SpawnObject for " + obj + ", NetworkServer is not active. Cannot spawn objects without an active server.");
				}
				return;
			}
			NetworkIdentity networkIdentity;
			if (!NetworkServer.GetNetworkIdentity(obj, out networkIdentity))
			{
				if (LogFilter.logError)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"SpawnObject ",
						obj,
						" has no NetworkIdentity. Please add a NetworkIdentity to ",
						obj
					}));
				}
				return;
			}
			networkIdentity.OnStartServer();
			if (LogFilter.logDebug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"SpawnObject instance ID ",
					networkIdentity.netId,
					" asset ID ",
					networkIdentity.assetId
				}));
			}
			networkIdentity.RebuildObservers(true);
		}

		internal void SendSpawnMessage(NetworkIdentity uv, NetworkConnection conn)
		{
			if (uv.serverOnly)
			{
				return;
			}
			if (uv.sceneId.IsEmpty())
			{
				ObjectSpawnMessage objectSpawnMessage = new ObjectSpawnMessage();
				objectSpawnMessage.netId = uv.netId;
				objectSpawnMessage.assetId = uv.assetId;
				objectSpawnMessage.position = uv.transform.position;
				NetworkWriter networkWriter = new NetworkWriter();
				uv.UNetSerializeAllVars(networkWriter);
				if (networkWriter.Position > 0)
				{
					objectSpawnMessage.payload = networkWriter.ToArray();
				}
				if (conn != null)
				{
					conn.Send(3, objectSpawnMessage);
				}
				else
				{
					NetworkServer.SendToReady(uv.gameObject, 3, objectSpawnMessage);
				}
			}
			else
			{
				ObjectSpawnSceneMessage objectSpawnSceneMessage = new ObjectSpawnSceneMessage();
				objectSpawnSceneMessage.netId = uv.netId;
				objectSpawnSceneMessage.sceneId = uv.sceneId;
				objectSpawnSceneMessage.position = uv.transform.position;
				NetworkWriter networkWriter2 = new NetworkWriter();
				uv.UNetSerializeAllVars(networkWriter2);
				if (networkWriter2.Position > 0)
				{
					objectSpawnSceneMessage.payload = networkWriter2.ToArray();
				}
				if (conn != null)
				{
					conn.Send(10, objectSpawnSceneMessage);
				}
				else
				{
					NetworkServer.SendToReady(uv.gameObject, 3, objectSpawnSceneMessage);
				}
			}
		}

		public static void DestroyPlayersForConnection(NetworkConnection conn)
		{
			if (conn.playerControllers.Count == 0)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("Empty player list given to NetworkServer.Destroy(), nothing to do.");
				}
				return;
			}
			if (conn.clientOwnedObjects != null)
			{
				HashSet<NetworkInstanceId> hashSet = new HashSet<NetworkInstanceId>(conn.clientOwnedObjects);
				foreach (NetworkInstanceId current in hashSet)
				{
					GameObject gameObject = NetworkServer.FindLocalObject(current);
					if (gameObject != null)
					{
						NetworkServer.DestroyObject(gameObject);
					}
				}
			}
			foreach (PlayerController current2 in conn.playerControllers)
			{
				if (current2.IsValid)
				{
					if (!(current2.unetView == null))
					{
						NetworkServer.DestroyObject(current2.unetView, true);
					}
					current2.gameObject = null;
				}
			}
			conn.playerControllers.Clear();
		}

		private static void UnSpawnObject(GameObject obj)
		{
			if (obj == null)
			{
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkServer UnspawnObject is null");
				}
				return;
			}
			NetworkIdentity uv;
			if (!NetworkServer.GetNetworkIdentity(obj, out uv))
			{
				return;
			}
			NetworkServer.UnSpawnObject(uv);
		}

		private static void UnSpawnObject(NetworkIdentity uv)
		{
			NetworkServer.DestroyObject(uv, false);
		}

		private static void DestroyObject(GameObject obj)
		{
			if (obj == null)
			{
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkServer DestroyObject is null");
				}
				return;
			}
			NetworkIdentity uv;
			if (!NetworkServer.GetNetworkIdentity(obj, out uv))
			{
				return;
			}
			NetworkServer.DestroyObject(uv, true);
		}

		private static void DestroyObject(NetworkIdentity uv, bool destroyServerObject)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("DestroyObject instance:" + uv.netId);
			}
			if (NetworkServer.objects.ContainsKey(uv.netId))
			{
				NetworkServer.objects.Remove(uv.netId);
			}
			if (uv.clientAuthorityOwner != null)
			{
				uv.clientAuthorityOwner.RemoveOwnedObject(uv);
			}
			ObjectDestroyMessage objectDestroyMessage = new ObjectDestroyMessage();
			objectDestroyMessage.netId = uv.netId;
			NetworkServer.SendToObservers(uv.gameObject, 1, objectDestroyMessage);
			uv.ClearObservers();
			if (NetworkClient.active && NetworkServer.s_LocalClientActive)
			{
				uv.OnNetworkDestroy();
				ClientScene.SetLocalObject(objectDestroyMessage.netId, null);
			}
			if (destroyServerObject)
			{
				UnityEngine.Object.Destroy(uv.gameObject);
			}
			uv.SetNoServer();
		}

		public static void ClearLocalObjects()
		{
			NetworkServer.objects.Clear();
		}

		public static void Spawn(GameObject obj)
		{
			NetworkServer.instance.SpawnObject(obj);
		}

		public static bool SpawnWithClientAuthority(GameObject obj, GameObject player)
		{
			NetworkIdentity component = player.GetComponent<NetworkIdentity>();
			if (component == null)
			{
				Debug.LogError("SpawnWithClientAuthority player object has no NetworkIdentity");
				return false;
			}
			if (component.connectionToClient == null)
			{
				Debug.LogError("SpawnWithClientAuthority player object is not a player.");
				return false;
			}
			return NetworkServer.SpawnWithClientAuthority(obj, component.connectionToClient);
		}

		public static bool SpawnWithClientAuthority(GameObject obj, NetworkConnection conn)
		{
			NetworkServer.Spawn(obj);
			NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
			return !(component == null) && component.isServer && component.AssignClientAuthority(conn);
		}

		public static bool SpawnWithClientAuthority(GameObject obj, NetworkHash128 assetId, NetworkConnection conn)
		{
			NetworkServer.Spawn(obj, assetId);
			NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
			return !(component == null) && component.isServer && component.AssignClientAuthority(conn);
		}

		public static void Spawn(GameObject obj, NetworkHash128 assetId)
		{
			NetworkIdentity networkIdentity;
			if (NetworkServer.GetNetworkIdentity(obj, out networkIdentity))
			{
				networkIdentity.SetDynamicAssetId(assetId);
			}
			NetworkServer.instance.SpawnObject(obj);
		}

		public static void Destroy(GameObject obj)
		{
			NetworkServer.DestroyObject(obj);
		}

		public static void UnSpawn(GameObject obj)
		{
			NetworkServer.UnSpawnObject(obj);
		}

		internal bool InvokeBytes(ULocalConnectionToServer conn, byte[] buffer, int numBytes, int channelId)
		{
			NetworkReader networkReader = new NetworkReader(buffer);
			networkReader.ReadInt16();
			short msgType = networkReader.ReadInt16();
			NetworkMessageDelegate handler = this.m_MessageHandlers.GetHandler(msgType);
			if (handler != null)
			{
				NetworkConnection networkConnection = this.m_Connections.Get(conn.connectionId);
				if (networkConnection != null)
				{
					ULocalConnectionToClient uLocalConnectionToClient = (ULocalConnectionToClient)networkConnection;
					uLocalConnectionToClient.InvokeHandler(msgType, networkReader, channelId);
					return true;
				}
			}
			return false;
		}

		internal bool InvokeHandlerOnServer(ULocalConnectionToServer conn, short msgType, MessageBase msg, int channelId)
		{
			NetworkMessageDelegate handler = this.m_MessageHandlers.GetHandler(msgType);
			if (handler == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("Local invoke: Failed to find message handler for message ID " + msgType);
				}
				return false;
			}
			NetworkConnection networkConnection = this.m_Connections.Get(conn.connectionId);
			if (networkConnection != null)
			{
				ULocalConnectionToClient uLocalConnectionToClient = (ULocalConnectionToClient)networkConnection;
				NetworkWriter writer = new NetworkWriter();
				msg.Serialize(writer);
				NetworkReader reader = new NetworkReader(writer);
				uLocalConnectionToClient.InvokeHandler(msgType, reader, channelId);
				return true;
			}
			if (LogFilter.logError)
			{
				Debug.LogError("Local invoke: Failed to find local connection to invoke handler on [connectionId=" + conn.connectionId + "]");
			}
			return false;
		}

		public static GameObject FindLocalObject(NetworkInstanceId netId)
		{
			return NetworkServer.s_NetworkScene.FindLocalObject(netId);
		}

		public static Dictionary<short, NetworkConnection.PacketStat> GetConnectionStats()
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			Dictionary<short, NetworkConnection.PacketStat> dictionary = new Dictionary<short, NetworkConnection.PacketStat>();
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					foreach (short current in networkConnection.packetStats.Keys)
					{
						if (dictionary.ContainsKey(current))
						{
							NetworkConnection.PacketStat packetStat = dictionary[current];
							packetStat.count += networkConnection.packetStats[current].count;
							packetStat.bytes += networkConnection.packetStats[current].bytes;
							dictionary[current] = packetStat;
						}
						else
						{
							dictionary[current] = networkConnection.packetStats[current];
						}
					}
				}
			}
			return dictionary;
		}

		public static void ResetConnectionStats()
		{
			ConnectionArray connections = NetworkServer.instance.m_Connections;
			for (int i = connections.LocalIndex; i < connections.Count; i++)
			{
				NetworkConnection networkConnection = connections.Get(i);
				if (networkConnection != null)
				{
					networkConnection.ResetStats();
				}
			}
		}

		public static bool AddExternalConnection(NetworkConnection conn)
		{
			return NetworkServer.instance.AddExternalConnectionInternal(conn);
		}

		private bool AddExternalConnectionInternal(NetworkConnection conn)
		{
			if (this.m_Connections.Get(conn.connectionId) != null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AddExternalConnection failed, already connection for id:" + conn.connectionId);
				}
				return false;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("AddExternalConnection external connection " + conn.connectionId);
			}
			this.m_Connections.Add(conn.connectionId, conn);
			conn.SetHandlers(this.m_MessageHandlers);
			NetworkServer.s_ExternalConnections.Add(conn.connectionId);
			return true;
		}

		public static void RemoveExternalConnection(int connectionId)
		{
			NetworkServer.instance.RemoveExternalConnectionInternal(connectionId);
		}

		private bool RemoveExternalConnectionInternal(int connectionId)
		{
			if (!NetworkServer.s_ExternalConnections.Contains(connectionId))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("RemoveExternalConnection failed, no connection for id:" + connectionId);
				}
				return false;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("RemoveExternalConnection external connection " + connectionId);
			}
			this.m_Connections.Remove(connectionId);
			return true;
		}

		public static bool SpawnObjects()
		{
			if (NetworkServer.active)
			{
				NetworkIdentity[] array = Resources.FindObjectsOfTypeAll<NetworkIdentity>();
				NetworkIdentity[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					NetworkIdentity networkIdentity = array2[i];
					if (networkIdentity.gameObject.hideFlags != HideFlags.NotEditable && networkIdentity.gameObject.hideFlags != HideFlags.HideAndDontSave)
					{
						if (!networkIdentity.sceneId.IsEmpty())
						{
							if (LogFilter.logDebug)
							{
								Debug.Log(string.Concat(new object[]
								{
									"SpawnObjects sceneId:",
									networkIdentity.sceneId,
									" name:",
									networkIdentity.gameObject.name
								}));
							}
							networkIdentity.gameObject.SetActive(true);
						}
					}
				}
				NetworkIdentity[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					NetworkIdentity networkIdentity2 = array3[j];
					if (networkIdentity2.gameObject.hideFlags != HideFlags.NotEditable && networkIdentity2.gameObject.hideFlags != HideFlags.HideAndDontSave)
					{
						if (!networkIdentity2.sceneId.IsEmpty())
						{
							if (!networkIdentity2.isServer)
							{
								if (!(networkIdentity2.gameObject == null))
								{
									NetworkServer.Spawn(networkIdentity2.gameObject);
									networkIdentity2.ForceAuthority(true);
								}
							}
						}
					}
				}
			}
			return true;
		}

		private static void SendCrc(NetworkConnection targetConnection)
		{
			if (NetworkCRC.singleton == null)
			{
				return;
			}
			if (!NetworkCRC.scriptCRCCheck)
			{
				return;
			}
			CRCMessage cRCMessage = new CRCMessage();
			List<CRCMessageEntry> list = new List<CRCMessageEntry>();
			foreach (string current in NetworkCRC.singleton.scripts.Keys)
			{
				list.Add(new CRCMessageEntry
				{
					name = current,
					channel = (byte)NetworkCRC.singleton.scripts[current]
				});
			}
			cRCMessage.scripts = list.ToArray();
			targetConnection.Send(14, cRCMessage);
		}

		public void SendNetworkInfo(NetworkConnection targetConnection)
		{
			PeerListMessage peerListMessage = new PeerListMessage();
			List<PeerInfoMessage> list = new List<PeerInfoMessage>();
			for (int i = 0; i < this.m_Connections.Count; i++)
			{
				NetworkConnection networkConnection = this.m_Connections.Get(i);
				if (networkConnection != null)
				{
					PeerInfoMessage peerInfoMessage = new PeerInfoMessage();
					string address;
					int port;
					NetworkID networkID;
					NodeID nodeID;
					byte b;
					NetworkTransport.GetConnectionInfo(this.m_ServerId, networkConnection.connectionId, out address, out port, out networkID, out nodeID, out b);
					peerInfoMessage.connectionId = networkConnection.connectionId;
					peerInfoMessage.address = address;
					peerInfoMessage.port = port;
					peerInfoMessage.isHost = false;
					peerInfoMessage.isYou = (networkConnection == targetConnection);
					list.Add(peerInfoMessage);
				}
			}
			if (NetworkServer.localClientActive)
			{
				list.Add(new PeerInfoMessage
				{
					address = "HOST",
					port = this.m_ServerPort,
					connectionId = 0,
					isHost = true,
					isYou = false
				});
			}
			peerListMessage.peers = list.ToArray();
			for (int j = 0; j < this.m_Connections.Count; j++)
			{
				NetworkConnection networkConnection2 = this.m_Connections.Get(j);
				if (networkConnection2 != null)
				{
					networkConnection2.Send(11, peerListMessage);
				}
			}
		}
	}
}
