using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkManager")]
	public class NetworkManager : MonoBehaviour
	{
		[SerializeField]
		private int m_NetworkPort = 7777;

		[SerializeField]
		private bool m_ServerBindToIP;

		[SerializeField]
		private string m_ServerBindAddress = string.Empty;

		[SerializeField]
		private string m_NetworkAddress = "localhost";

		[SerializeField]
		private bool m_DontDestroyOnLoad = true;

		[SerializeField]
		private bool m_RunInBackground = true;

		[SerializeField]
		private bool m_ScriptCRCCheck = true;

		[SerializeField]
		private bool m_SendPeerInfo;

		[SerializeField]
		private float m_MaxDelay = 0.01f;

		[SerializeField]
		private LogFilter.FilterLevel m_LogLevel = LogFilter.FilterLevel.Info;

		[SerializeField]
		private GameObject m_PlayerPrefab;

		[SerializeField]
		private bool m_AutoCreatePlayer = true;

		[SerializeField]
		private PlayerSpawnMethod m_PlayerSpawnMethod;

		[SerializeField]
		private string m_OfflineScene = string.Empty;

		[SerializeField]
		private string m_OnlineScene = string.Empty;

		[SerializeField]
		private List<GameObject> m_SpawnPrefabs = new List<GameObject>();

		[SerializeField]
		private bool m_CustomConfig;

		[SerializeField]
		private int m_MaxConnections = 4;

		[SerializeField]
		private ConnectionConfig m_ConnectionConfig;

		[SerializeField]
		private List<QosType> m_Channels = new List<QosType>();

		[SerializeField]
		private bool m_UseWebSockets;

		[SerializeField]
		private bool m_UseSimulator;

		[SerializeField]
		private int m_SimulatedLatency = 1;

		[SerializeField]
		private float m_PacketLossPercentage;

		[SerializeField]
		private string m_MatchHost = "mm.unet.unity3d.com";

		[SerializeField]
		private int m_MatchPort = 443;

		private EndPoint m_EndPoint;

		public static string networkSceneName = string.Empty;

		public bool isNetworkActive;

		public NetworkClient client;

		private static List<Transform> s_StartPositions = new List<Transform>();

		private static int s_StartPositionIndex;

		public MatchInfo matchInfo;

		public NetworkMatch matchMaker;

		public List<MatchDesc> matches;

		public string matchName = "default";

		public uint matchSize = 4u;

		public static NetworkManager singleton;

		private static AddPlayerMessage s_AddPlayerMessage = new AddPlayerMessage();

		private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();

		private static ErrorMessage s_ErrorMessage = new ErrorMessage();

		private static AsyncOperation s_LoadingSceneAsync;

		private static NetworkConnection s_ClientReadyConnection;

		private static string s_Address;

		public int networkPort
		{
			get
			{
				return this.m_NetworkPort;
			}
			set
			{
				this.m_NetworkPort = value;
			}
		}

		public bool serverBindToIP
		{
			get
			{
				return this.m_ServerBindToIP;
			}
			set
			{
				this.m_ServerBindToIP = value;
			}
		}

		public string serverBindAddress
		{
			get
			{
				return this.m_ServerBindAddress;
			}
			set
			{
				this.m_ServerBindAddress = value;
			}
		}

		public string networkAddress
		{
			get
			{
				return this.m_NetworkAddress;
			}
			set
			{
				this.m_NetworkAddress = value;
			}
		}

		public bool dontDestroyOnLoad
		{
			get
			{
				return this.m_DontDestroyOnLoad;
			}
			set
			{
				this.m_DontDestroyOnLoad = value;
			}
		}

		public bool runInBackground
		{
			get
			{
				return this.m_RunInBackground;
			}
			set
			{
				this.m_RunInBackground = value;
			}
		}

		public bool scriptCRCCheck
		{
			get
			{
				return this.m_ScriptCRCCheck;
			}
			set
			{
				this.m_ScriptCRCCheck = value;
			}
		}

		public bool sendPeerInfo
		{
			get
			{
				return this.m_SendPeerInfo;
			}
			set
			{
				this.m_SendPeerInfo = value;
			}
		}

		public float maxDelay
		{
			get
			{
				return this.m_MaxDelay;
			}
			set
			{
				this.m_MaxDelay = value;
			}
		}

		public LogFilter.FilterLevel logLevel
		{
			get
			{
				return this.m_LogLevel;
			}
			set
			{
				this.m_LogLevel = value;
				LogFilter.currentLogLevel = (int)value;
			}
		}

		public GameObject playerPrefab
		{
			get
			{
				return this.m_PlayerPrefab;
			}
			set
			{
				this.m_PlayerPrefab = value;
			}
		}

		public bool autoCreatePlayer
		{
			get
			{
				return this.m_AutoCreatePlayer;
			}
			set
			{
				this.m_AutoCreatePlayer = value;
			}
		}

		public PlayerSpawnMethod playerSpawnMethod
		{
			get
			{
				return this.m_PlayerSpawnMethod;
			}
			set
			{
				this.m_PlayerSpawnMethod = value;
			}
		}

		public string offlineScene
		{
			get
			{
				return this.m_OfflineScene;
			}
			set
			{
				this.m_OfflineScene = value;
			}
		}

		public string onlineScene
		{
			get
			{
				return this.m_OnlineScene;
			}
			set
			{
				this.m_OnlineScene = value;
			}
		}

		public List<GameObject> spawnPrefabs
		{
			get
			{
				return this.m_SpawnPrefabs;
			}
		}

		public List<Transform> startPositions
		{
			get
			{
				return NetworkManager.s_StartPositions;
			}
		}

		public bool customConfig
		{
			get
			{
				return this.m_CustomConfig;
			}
			set
			{
				this.m_CustomConfig = value;
			}
		}

		public ConnectionConfig connectionConfig
		{
			get
			{
				if (this.m_ConnectionConfig == null)
				{
					this.m_ConnectionConfig = new ConnectionConfig();
				}
				return this.m_ConnectionConfig;
			}
		}

		public int maxConnections
		{
			get
			{
				return this.m_MaxConnections;
			}
			set
			{
				this.m_MaxConnections = value;
			}
		}

		public List<QosType> channels
		{
			get
			{
				return this.m_Channels;
			}
		}

		public EndPoint secureTunnelEndpoint
		{
			get
			{
				return this.m_EndPoint;
			}
			set
			{
				this.m_EndPoint = value;
			}
		}

		public bool useWebSockets
		{
			get
			{
				return this.m_UseWebSockets;
			}
			set
			{
				this.m_UseWebSockets = value;
			}
		}

		public bool useSimulator
		{
			get
			{
				return this.m_UseSimulator;
			}
			set
			{
				this.m_UseSimulator = value;
			}
		}

		public int simulatedLatency
		{
			get
			{
				return this.m_SimulatedLatency;
			}
			set
			{
				this.m_SimulatedLatency = value;
			}
		}

		public float packetLossPercentage
		{
			get
			{
				return this.m_PacketLossPercentage;
			}
			set
			{
				this.m_PacketLossPercentage = value;
			}
		}

		public string matchHost
		{
			get
			{
				return this.m_MatchHost;
			}
			set
			{
				this.m_MatchHost = value;
			}
		}

		public int matchPort
		{
			get
			{
				return this.m_MatchPort;
			}
			set
			{
				this.m_MatchPort = value;
			}
		}

		public int numPlayers
		{
			get
			{
				int num = 0;
				foreach (NetworkConnection current in NetworkServer.connections)
				{
					if (current != null)
					{
						foreach (PlayerController current2 in current.playerControllers)
						{
							if (current2.IsValid)
							{
								num++;
							}
						}
					}
				}
				foreach (NetworkConnection current3 in NetworkServer.localConnections)
				{
					if (current3 != null)
					{
						foreach (PlayerController current4 in current3.playerControllers)
						{
							if (current4.IsValid)
							{
								num++;
							}
						}
					}
				}
				return num;
			}
		}

		private void Awake()
		{
			LogFilter.currentLogLevel = (int)this.m_LogLevel;
			if (this.m_DontDestroyOnLoad)
			{
				if (NetworkManager.singleton != null)
				{
					if (LogFilter.logWarn)
					{
						Debug.LogWarning("Multiple NetworkManagers detected in the scene. Only one NetworkManager can exist at a time. The duplicate NetworkManager will not be used.");
					}
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkManager created singleton (DontDestroyOnLoad)");
				}
				NetworkManager.singleton = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				if (LogFilter.logDev)
				{
					Debug.Log("NetworkManager created singleton (ForScene)");
				}
				NetworkManager.singleton = this;
			}
			if (this.m_NetworkAddress != string.Empty)
			{
				NetworkManager.s_Address = this.m_NetworkAddress;
			}
			else if (NetworkManager.s_Address != string.Empty)
			{
				this.m_NetworkAddress = NetworkManager.s_Address;
			}
		}

		private void OnValidate()
		{
			if (this.m_SimulatedLatency < 1)
			{
				this.m_SimulatedLatency = 1;
			}
			if (this.m_SimulatedLatency > 500)
			{
				this.m_SimulatedLatency = 500;
			}
			if (this.m_PacketLossPercentage < 0f)
			{
				this.m_PacketLossPercentage = 0f;
			}
			if (this.m_PacketLossPercentage > 99f)
			{
				this.m_PacketLossPercentage = 99f;
			}
			if (this.m_MaxConnections <= 0)
			{
				this.m_MaxConnections = 1;
			}
			if (this.m_MaxConnections > 32000)
			{
				this.m_MaxConnections = 32000;
			}
			if (this.m_PlayerPrefab != null && this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkManager - playerPrefab must have a NetworkIdentity.");
				}
				this.m_PlayerPrefab = null;
			}
		}

		internal void RegisterServerMessages()
		{
			NetworkServer.RegisterHandler(32, new NetworkMessageDelegate(this.OnServerConnectInternal));
			NetworkServer.RegisterHandler(33, new NetworkMessageDelegate(this.OnServerDisconnectInternal));
			NetworkServer.RegisterHandler(35, new NetworkMessageDelegate(this.OnServerReadyMessageInternal));
			NetworkServer.RegisterHandler(37, new NetworkMessageDelegate(this.OnServerAddPlayerMessageInternal));
			NetworkServer.RegisterHandler(38, new NetworkMessageDelegate(this.OnServerRemovePlayerMessageInternal));
			NetworkServer.RegisterHandler(34, new NetworkMessageDelegate(this.OnServerErrorInternal));
		}

		public bool StartServer(ConnectionConfig config, int maxConnections)
		{
			return this.StartServer(null, config, maxConnections);
		}

		public bool StartServer()
		{
			return this.StartServer(null);
		}

		public bool StartServer(MatchInfo info)
		{
			return this.StartServer(info, null, -1);
		}

		private bool StartServer(MatchInfo info, ConnectionConfig config, int maxConnections)
		{
			this.OnStartServer();
			if (this.m_RunInBackground)
			{
				Application.runInBackground = true;
			}
			NetworkCRC.scriptCRCCheck = this.scriptCRCCheck;
			if (this.m_CustomConfig && this.m_ConnectionConfig != null && config == null)
			{
				this.m_ConnectionConfig.Channels.Clear();
				foreach (QosType current in this.m_Channels)
				{
					this.m_ConnectionConfig.AddChannel(current);
				}
				NetworkServer.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
			}
			this.RegisterServerMessages();
			NetworkServer.sendPeerInfo = this.m_SendPeerInfo;
			NetworkServer.useWebSockets = this.m_UseWebSockets;
			if (config != null)
			{
				NetworkServer.Configure(config, maxConnections);
			}
			if (info != null)
			{
				if (!NetworkServer.Listen(info, this.m_NetworkPort))
				{
					if (LogFilter.logError)
					{
						Debug.LogError("StartServer listen failed.");
					}
					return false;
				}
			}
			else if (this.m_ServerBindToIP && !string.IsNullOrEmpty(this.m_ServerBindAddress))
			{
				if (!NetworkServer.Listen(this.m_ServerBindAddress, this.m_NetworkPort))
				{
					if (LogFilter.logError)
					{
						Debug.LogError("StartServer listen on " + this.m_ServerBindAddress + " failed.");
					}
					return false;
				}
			}
			else if (!NetworkServer.Listen(this.m_NetworkPort))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("StartServer listen failed.");
				}
				return false;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager StartServer port:" + this.m_NetworkPort);
			}
			this.isNetworkActive = true;
			if (this.m_OnlineScene != string.Empty && this.m_OnlineScene != Application.loadedLevelName && this.m_OnlineScene != this.m_OfflineScene)
			{
				this.ServerChangeScene(this.m_OnlineScene);
			}
			else
			{
				NetworkServer.SpawnObjects();
			}
			return true;
		}

		internal void RegisterClientMessages(NetworkClient client)
		{
			client.RegisterHandler(32, new NetworkMessageDelegate(this.OnClientConnectInternal));
			client.RegisterHandler(33, new NetworkMessageDelegate(this.OnClientDisconnectInternal));
			client.RegisterHandler(36, new NetworkMessageDelegate(this.OnClientNotReadyMessageInternal));
			client.RegisterHandler(34, new NetworkMessageDelegate(this.OnClientErrorInternal));
			client.RegisterHandler(39, new NetworkMessageDelegate(this.OnClientSceneInternal));
			if (this.m_PlayerPrefab != null)
			{
				ClientScene.RegisterPrefab(this.m_PlayerPrefab);
			}
			foreach (GameObject current in this.m_SpawnPrefabs)
			{
				if (current != null)
				{
					ClientScene.RegisterPrefab(current);
				}
			}
		}

		public void UseExternalClient(NetworkClient externalClient)
		{
			if (this.m_RunInBackground)
			{
				Application.runInBackground = true;
			}
			this.isNetworkActive = true;
			this.client = externalClient;
			this.RegisterClientMessages(this.client);
			this.OnStartClient(this.client);
			NetworkManager.s_Address = this.m_NetworkAddress;
		}

		public NetworkClient StartClient(MatchInfo info, ConnectionConfig config)
		{
			this.matchInfo = info;
			if (this.m_RunInBackground)
			{
				Application.runInBackground = true;
			}
			this.isNetworkActive = true;
			this.client = new NetworkClient();
			if (config != null)
			{
				this.client.Configure(config, 1);
			}
			else if (this.m_CustomConfig && this.m_ConnectionConfig != null)
			{
				this.m_ConnectionConfig.Channels.Clear();
				foreach (QosType current in this.m_Channels)
				{
					this.m_ConnectionConfig.AddChannel(current);
				}
				this.client.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
			}
			this.RegisterClientMessages(this.client);
			if (this.matchInfo != null)
			{
				if (LogFilter.logDebug)
				{
					Debug.Log("NetworkManager StartClient match: " + this.matchInfo);
				}
				this.client.Connect(this.matchInfo);
			}
			else if (this.m_EndPoint != null)
			{
				if (LogFilter.logDebug)
				{
					Debug.Log("NetworkManager StartClient using provided SecureTunnel");
				}
				this.client.Connect(this.m_EndPoint);
			}
			else
			{
				if (string.IsNullOrEmpty(this.m_NetworkAddress))
				{
					if (LogFilter.logError)
					{
						Debug.LogError("Must set the Network Address field in the manager");
					}
					return null;
				}
				if (LogFilter.logDebug)
				{
					Debug.Log(string.Concat(new object[]
					{
						"NetworkManager StartClient address:",
						this.m_NetworkAddress,
						" port:",
						this.m_NetworkPort
					}));
				}
				if (this.m_UseSimulator)
				{
					this.client.ConnectWithSimulator(this.m_NetworkAddress, this.m_NetworkPort, this.m_SimulatedLatency, this.m_PacketLossPercentage);
				}
				else
				{
					this.client.Connect(this.m_NetworkAddress, this.m_NetworkPort);
				}
			}
			this.OnStartClient(this.client);
			NetworkManager.s_Address = this.m_NetworkAddress;
			return this.client;
		}

		public NetworkClient StartClient(MatchInfo matchInfo)
		{
			return this.StartClient(matchInfo, null);
		}

		public NetworkClient StartClient()
		{
			return this.StartClient(null, null);
		}

		public virtual NetworkClient StartHost(ConnectionConfig config, int maxConnections)
		{
			this.OnStartHost();
			if (this.StartServer(config, maxConnections))
			{
				NetworkClient networkClient = this.ConnectLocalClient();
				this.OnServerConnect(networkClient.connection);
				this.OnStartClient(networkClient);
				return networkClient;
			}
			return null;
		}

		public virtual NetworkClient StartHost(MatchInfo info)
		{
			this.OnStartHost();
			this.matchInfo = info;
			if (this.StartServer(info))
			{
				NetworkClient networkClient = this.ConnectLocalClient();
				this.OnServerConnect(networkClient.connection);
				this.OnStartClient(networkClient);
				return networkClient;
			}
			return null;
		}

		public virtual NetworkClient StartHost()
		{
			this.OnStartHost();
			if (this.StartServer())
			{
				NetworkClient networkClient = this.ConnectLocalClient();
				this.OnServerConnect(networkClient.connection);
				this.OnStartClient(networkClient);
				return networkClient;
			}
			return null;
		}

		private NetworkClient ConnectLocalClient()
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager StartHost port:" + this.m_NetworkPort);
			}
			this.m_NetworkAddress = "localhost";
			this.client = ClientScene.ConnectLocalServer();
			this.RegisterClientMessages(this.client);
			return this.client;
		}

		public void StopHost()
		{
			this.OnStopHost();
			this.StopServer();
			this.StopClient();
		}

		public void StopServer()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.OnStopServer();
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager StopServer");
			}
			this.isNetworkActive = false;
			NetworkServer.Shutdown();
			this.StopMatchMaker();
			if (this.m_OfflineScene != string.Empty)
			{
				this.ServerChangeScene(this.m_OfflineScene);
			}
		}

		public void StopClient()
		{
			this.OnStopClient();
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager StopClient");
			}
			this.isNetworkActive = false;
			if (this.client != null)
			{
				this.client.Disconnect();
				this.client.Shutdown();
				this.client = null;
			}
			this.StopMatchMaker();
			ClientScene.DestroyAllClientObjects();
			if (this.m_OfflineScene != string.Empty)
			{
				this.ClientChangeScene(this.m_OfflineScene, false);
			}
		}

		public virtual void ServerChangeScene(string newSceneName)
		{
			if (string.IsNullOrEmpty(newSceneName))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("ServerChangeScene empty scene name");
				}
				return;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("ServerChangeScene " + newSceneName);
			}
			NetworkServer.SetAllClientsNotReady();
			NetworkManager.networkSceneName = newSceneName;
			NetworkManager.s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);
			StringMessage msg = new StringMessage(NetworkManager.networkSceneName);
			NetworkServer.SendToAll(39, msg);
			NetworkManager.s_StartPositionIndex = 0;
			NetworkManager.s_StartPositions.Clear();
		}

		internal void ClientChangeScene(string newSceneName, bool forceReload)
		{
			if (string.IsNullOrEmpty(newSceneName))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("ClientChangeScene empty scene name");
				}
				return;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("ClientChangeScene newSceneName:" + newSceneName + " networkSceneName:" + NetworkManager.networkSceneName);
			}
			if (newSceneName == NetworkManager.networkSceneName && !forceReload)
			{
				return;
			}
			NetworkManager.s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);
			NetworkManager.networkSceneName = newSceneName;
		}

		private void FinishLoadScene()
		{
			if (this.client != null)
			{
				if (NetworkManager.s_ClientReadyConnection != null)
				{
					this.OnClientConnect(NetworkManager.s_ClientReadyConnection);
					NetworkManager.s_ClientReadyConnection = null;
				}
			}
			else if (LogFilter.logDev)
			{
				Debug.Log("FinishLoadScene client is null");
			}
			if (NetworkServer.active)
			{
				NetworkServer.SpawnObjects();
				this.OnServerSceneChanged(NetworkManager.networkSceneName);
			}
			if (this.IsClientConnected() && this.client != null)
			{
				this.RegisterClientMessages(this.client);
				this.OnClientSceneChanged(this.client.connection);
			}
		}

		internal static void UpdateScene()
		{
			if (NetworkManager.singleton == null)
			{
				return;
			}
			if (NetworkManager.s_LoadingSceneAsync == null)
			{
				return;
			}
			if (!NetworkManager.s_LoadingSceneAsync.isDone)
			{
				return;
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("ClientChangeScene done readyCon:" + NetworkManager.s_ClientReadyConnection);
			}
			NetworkManager.singleton.FinishLoadScene();
			NetworkManager.s_LoadingSceneAsync.allowSceneActivation = true;
			NetworkManager.s_LoadingSceneAsync = null;
		}

		private void OnDestroy()
		{
			if (LogFilter.logDev)
			{
				Debug.Log("NetworkManager destroyed");
			}
		}

		public static void RegisterStartPosition(Transform start)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("RegisterStartPosition:" + start);
			}
			NetworkManager.s_StartPositions.Add(start);
		}

		public static void UnRegisterStartPosition(Transform start)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("UnRegisterStartPosition:" + start);
			}
			NetworkManager.s_StartPositions.Remove(start);
		}

		public bool IsClientConnected()
		{
			return this.client != null && this.client.isConnected;
		}

		public static void Shutdown()
		{
			if (NetworkManager.singleton == null)
			{
				return;
			}
			NetworkManager.s_StartPositions.Clear();
			NetworkManager.s_StartPositionIndex = 0;
			NetworkManager.s_ClientReadyConnection = null;
			NetworkManager.singleton.StopHost();
			NetworkManager.singleton = null;
		}

		internal void OnServerConnectInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerConnectInternal");
			}
			netMsg.conn.SetMaxDelay(this.m_MaxDelay);
			if (NetworkManager.networkSceneName != string.Empty && NetworkManager.networkSceneName != this.m_OfflineScene)
			{
				StringMessage msg = new StringMessage(NetworkManager.networkSceneName);
				netMsg.conn.Send(39, msg);
			}
			this.OnServerConnect(netMsg.conn);
		}

		internal void OnServerDisconnectInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerDisconnectInternal");
			}
			this.OnServerDisconnect(netMsg.conn);
		}

		internal void OnServerReadyMessageInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerReadyMessageInternal");
			}
			this.OnServerReady(netMsg.conn);
		}

		internal void OnServerAddPlayerMessageInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerAddPlayerMessageInternal");
			}
			netMsg.ReadMessage<AddPlayerMessage>(NetworkManager.s_AddPlayerMessage);
			if (NetworkManager.s_AddPlayerMessage.msgSize != 0)
			{
				NetworkReader extraMessageReader = new NetworkReader(NetworkManager.s_AddPlayerMessage.msgData);
				this.OnServerAddPlayer(netMsg.conn, NetworkManager.s_AddPlayerMessage.playerControllerId, extraMessageReader);
			}
			else
			{
				this.OnServerAddPlayer(netMsg.conn, NetworkManager.s_AddPlayerMessage.playerControllerId);
			}
		}

		internal void OnServerRemovePlayerMessageInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerRemovePlayerMessageInternal");
			}
			netMsg.ReadMessage<RemovePlayerMessage>(NetworkManager.s_RemovePlayerMessage);
			PlayerController player;
			netMsg.conn.GetPlayerController(NetworkManager.s_RemovePlayerMessage.playerControllerId, out player);
			this.OnServerRemovePlayer(netMsg.conn, player);
			netMsg.conn.RemovePlayerController(NetworkManager.s_RemovePlayerMessage.playerControllerId);
		}

		internal void OnServerErrorInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnServerErrorInternal");
			}
			netMsg.ReadMessage<ErrorMessage>(NetworkManager.s_ErrorMessage);
			this.OnServerError(netMsg.conn, NetworkManager.s_ErrorMessage.errorCode);
		}

		internal void OnClientConnectInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnClientConnectInternal");
			}
			netMsg.conn.SetMaxDelay(this.m_MaxDelay);
			if (string.IsNullOrEmpty(this.m_OnlineScene) || this.m_OnlineScene == this.m_OfflineScene)
			{
				this.OnClientConnect(netMsg.conn);
			}
			else
			{
				NetworkManager.s_ClientReadyConnection = netMsg.conn;
			}
		}

		internal void OnClientDisconnectInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnClientDisconnectInternal");
			}
			if (this.m_OfflineScene != string.Empty)
			{
				this.ClientChangeScene(this.m_OfflineScene, false);
			}
			this.OnClientDisconnect(netMsg.conn);
		}

		internal void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnClientNotReadyMessageInternal");
			}
			ClientScene.SetNotReady();
			this.OnClientNotReady(netMsg.conn);
		}

		internal void OnClientErrorInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnClientErrorInternal");
			}
			netMsg.ReadMessage<ErrorMessage>(NetworkManager.s_ErrorMessage);
			this.OnClientError(netMsg.conn, NetworkManager.s_ErrorMessage.errorCode);
		}

		internal void OnClientSceneInternal(NetworkMessage netMsg)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager:OnClientSceneInternal");
			}
			string newSceneName = netMsg.reader.ReadString();
			if (this.IsClientConnected() && !NetworkServer.active)
			{
				this.ClientChangeScene(newSceneName, true);
			}
		}

		public virtual void OnServerConnect(NetworkConnection conn)
		{
		}

		public virtual void OnServerDisconnect(NetworkConnection conn)
		{
			NetworkServer.DestroyPlayersForConnection(conn);
		}

		public virtual void OnServerReady(NetworkConnection conn)
		{
			if (conn.playerControllers.Count == 0 && LogFilter.logDebug)
			{
				Debug.Log("Ready with no player object");
			}
			NetworkServer.SetClientReady(conn);
		}

		public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId);
		}

		public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId);
		}

		private void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId)
		{
			if (this.m_PlayerPrefab == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
				}
				return;
			}
			if (this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
				}
				return;
			}
			if ((int)playerControllerId < conn.playerControllers.Count && conn.playerControllers[(int)playerControllerId].IsValid && conn.playerControllers[(int)playerControllerId].gameObject != null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("There is already a player at that playerControllerId for this connections.");
				}
				return;
			}
			Transform startPosition = this.GetStartPosition();
			GameObject player;
			if (startPosition != null)
			{
				player = (GameObject)UnityEngine.Object.Instantiate(this.m_PlayerPrefab, startPosition.position, startPosition.rotation);
			}
			else
			{
				player = (GameObject)UnityEngine.Object.Instantiate(this.m_PlayerPrefab, Vector3.zero, Quaternion.identity);
			}
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		}

		public Transform GetStartPosition()
		{
			if (NetworkManager.s_StartPositions.Count > 0)
			{
				for (int i = NetworkManager.s_StartPositions.Count - 1; i >= 0; i--)
				{
					if (NetworkManager.s_StartPositions[i] == null)
					{
						NetworkManager.s_StartPositions.RemoveAt(i);
					}
				}
			}
			if (this.m_PlayerSpawnMethod == PlayerSpawnMethod.Random && NetworkManager.s_StartPositions.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, NetworkManager.s_StartPositions.Count);
				return NetworkManager.s_StartPositions[index];
			}
			if (this.m_PlayerSpawnMethod == PlayerSpawnMethod.RoundRobin && NetworkManager.s_StartPositions.Count > 0)
			{
				if (NetworkManager.s_StartPositionIndex >= NetworkManager.s_StartPositions.Count)
				{
					NetworkManager.s_StartPositionIndex = 0;
				}
				Transform result = NetworkManager.s_StartPositions[NetworkManager.s_StartPositionIndex];
				NetworkManager.s_StartPositionIndex++;
				return result;
			}
			return null;
		}

		public virtual void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
		{
			if (player.gameObject != null)
			{
				NetworkServer.Destroy(player.gameObject);
			}
		}

		public virtual void OnServerError(NetworkConnection conn, int errorCode)
		{
		}

		public virtual void OnServerSceneChanged(string sceneName)
		{
		}

		public virtual void OnClientConnect(NetworkConnection conn)
		{
			if (string.IsNullOrEmpty(this.m_OnlineScene) || this.m_OnlineScene == this.m_OfflineScene)
			{
				ClientScene.Ready(conn);
				if (this.m_AutoCreatePlayer)
				{
					ClientScene.AddPlayer(0);
				}
			}
		}

		public virtual void OnClientDisconnect(NetworkConnection conn)
		{
			this.StopClient();
		}

		public virtual void OnClientError(NetworkConnection conn, int errorCode)
		{
		}

		public virtual void OnClientNotReady(NetworkConnection conn)
		{
		}

		public virtual void OnClientSceneChanged(NetworkConnection conn)
		{
			ClientScene.Ready(conn);
			if (!this.m_AutoCreatePlayer)
			{
				return;
			}
			bool flag = ClientScene.localPlayers.Count == 0;
			bool flag2 = false;
			foreach (PlayerController current in ClientScene.localPlayers)
			{
				if (current.gameObject != null)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
			}
			if (flag)
			{
				ClientScene.AddPlayer(0);
			}
		}

		public void StartMatchMaker()
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager StartMatchMaker");
			}
			this.SetMatchHost(this.m_MatchHost, this.m_MatchPort, true);
		}

		public void StopMatchMaker()
		{
			if (this.matchMaker != null)
			{
				UnityEngine.Object.Destroy(this.matchMaker);
				this.matchMaker = null;
			}
			this.matchInfo = null;
			this.matches = null;
		}

		public void SetMatchHost(string newHost, int port, bool https)
		{
			if (this.matchMaker == null)
			{
				this.matchMaker = base.gameObject.AddComponent<NetworkMatch>();
			}
			if (newHost == "localhost" || newHost == "127.0.0.1")
			{
				newHost = Environment.MachineName;
			}
			string text = "http://";
			if (https)
			{
				text = "https://";
			}
			if (LogFilter.logDebug)
			{
				Debug.Log("SetMatchHost:" + newHost);
			}
			this.m_MatchHost = newHost;
			this.m_MatchPort = port;
			this.matchMaker.baseUri = new Uri(string.Concat(new object[]
			{
				text,
				this.m_MatchHost,
				":",
				this.m_MatchPort
			}));
		}

		public virtual void OnStartHost()
		{
		}

		public virtual void OnStartServer()
		{
		}

		public virtual void OnStartClient(NetworkClient client)
		{
		}

		public virtual void OnStopServer()
		{
		}

		public virtual void OnStopClient()
		{
		}

		public virtual void OnStopHost()
		{
		}

		public virtual void OnMatchCreate(CreateMatchResponse matchInfo)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager OnMatchCreate " + matchInfo);
			}
			if (matchInfo.success)
			{
				Utility.SetAccessTokenForNetwork(matchInfo.networkId, new NetworkAccessToken(matchInfo.accessTokenString));
				this.StartHost(new MatchInfo(matchInfo));
			}
			else if (LogFilter.logError)
			{
				Debug.LogError("Create Failed:" + matchInfo);
			}
		}

		public virtual void OnMatchList(ListMatchResponse matchList)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager OnMatchList ");
			}
			this.matches = matchList.matches;
		}

		public void OnMatchJoined(JoinMatchResponse matchInfo)
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkManager OnMatchJoined ");
			}
			if (matchInfo.success)
			{
				Utility.SetAccessTokenForNetwork(matchInfo.networkId, new NetworkAccessToken(matchInfo.accessTokenString));
				this.StartClient(new MatchInfo(matchInfo));
			}
			else if (LogFilter.logError)
			{
				Debug.LogError("Join Failed:" + matchInfo);
			}
		}
	}
}
