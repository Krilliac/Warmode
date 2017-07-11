using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkDiscovery"), DisallowMultipleComponent]
	public class NetworkDiscovery : MonoBehaviour
	{
		private const int k_MaxBroadcastMsgSize = 1024;

		[SerializeField]
		private int m_BroadcastPort = 47777;

		[SerializeField]
		private int m_BroadcastKey = 2222;

		[SerializeField]
		private int m_BroadcastVersion = 1;

		[SerializeField]
		private int m_BroadcastSubVersion = 1;

		[SerializeField]
		private int m_BroadcastInterval = 1000;

		[SerializeField]
		private bool m_UseNetworkManager = true;

		[SerializeField]
		private string m_BroadcastData = "HELLO";

		[SerializeField]
		private bool m_ShowGUI = true;

		[SerializeField]
		private int m_OffsetX;

		[SerializeField]
		private int m_OffsetY;

		private int m_HostId = -1;

		private bool m_Running;

		private bool m_IsServer;

		private bool m_IsClient;

		private byte[] m_MsgOutBuffer;

		private byte[] m_MsgInBuffer;

		private HostTopology m_DefaultTopology;

		private Dictionary<string, NetworkBroadcastResult> m_BroadcastsReceived;

		public int broadcastPort
		{
			get
			{
				return this.m_BroadcastPort;
			}
			set
			{
				this.m_BroadcastPort = value;
			}
		}

		public int broadcastKey
		{
			get
			{
				return this.m_BroadcastKey;
			}
			set
			{
				this.m_BroadcastKey = value;
			}
		}

		public int broadcastVersion
		{
			get
			{
				return this.m_BroadcastVersion;
			}
			set
			{
				this.m_BroadcastVersion = value;
			}
		}

		public int broadcastSubVersion
		{
			get
			{
				return this.m_BroadcastSubVersion;
			}
			set
			{
				this.m_BroadcastSubVersion = value;
			}
		}

		public int broadcastInterval
		{
			get
			{
				return this.m_BroadcastInterval;
			}
			set
			{
				this.m_BroadcastInterval = value;
			}
		}

		public bool useNetworkManager
		{
			get
			{
				return this.m_UseNetworkManager;
			}
			set
			{
				this.m_UseNetworkManager = value;
			}
		}

		public string broadcastData
		{
			get
			{
				return this.m_BroadcastData;
			}
			set
			{
				this.m_BroadcastData = value;
			}
		}

		public bool showGUI
		{
			get
			{
				return this.m_ShowGUI;
			}
			set
			{
				this.m_ShowGUI = value;
			}
		}

		public int offsetX
		{
			get
			{
				return this.m_OffsetX;
			}
			set
			{
				this.m_OffsetX = value;
			}
		}

		public int offsetY
		{
			get
			{
				return this.m_OffsetY;
			}
			set
			{
				this.m_OffsetY = value;
			}
		}

		public int hostId
		{
			get
			{
				return this.m_HostId;
			}
			set
			{
				this.m_HostId = value;
			}
		}

		public bool running
		{
			get
			{
				return this.m_Running;
			}
			set
			{
				this.m_Running = value;
			}
		}

		public bool isServer
		{
			get
			{
				return this.m_IsServer;
			}
			set
			{
				this.m_IsServer = value;
			}
		}

		public bool isClient
		{
			get
			{
				return this.m_IsClient;
			}
			set
			{
				this.m_IsClient = value;
			}
		}

		public Dictionary<string, NetworkBroadcastResult> broadcastsReceived
		{
			get
			{
				return this.m_BroadcastsReceived;
			}
		}

		private static byte[] StringToBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		private static string BytesToString(byte[] bytes)
		{
			char[] array = new char[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return new string(array);
		}

		public bool Initialize()
		{
			if (this.m_BroadcastData.Length >= 1024)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkDiscovery Initialize - data too large. max is " + 1024);
				}
				return false;
			}
			if (!NetworkTransport.IsStarted)
			{
				NetworkTransport.Init();
			}
			if (this.m_UseNetworkManager && NetworkManager.singleton != null)
			{
				this.m_BroadcastData = string.Concat(new object[]
				{
					"NetworkManager:",
					NetworkManager.singleton.networkAddress,
					":",
					NetworkManager.singleton.networkPort
				});
				if (LogFilter.logInfo)
				{
					Debug.Log("NetwrokDiscovery set broadbast data to:" + this.m_BroadcastData);
				}
			}
			this.m_MsgOutBuffer = NetworkDiscovery.StringToBytes(this.m_BroadcastData);
			this.m_MsgInBuffer = new byte[1024];
			this.m_BroadcastsReceived = new Dictionary<string, NetworkBroadcastResult>();
			ConnectionConfig connectionConfig = new ConnectionConfig();
			connectionConfig.AddChannel(QosType.Unreliable);
			this.m_DefaultTopology = new HostTopology(connectionConfig, 1);
			if (this.m_IsServer)
			{
				this.StartAsServer();
			}
			if (this.m_IsClient)
			{
				this.StartAsClient();
			}
			return true;
		}

		public bool StartAsClient()
		{
			if (this.m_HostId != -1 || this.m_Running)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("NetworkDiscovery StartAsClient already started");
				}
				return false;
			}
			this.m_HostId = NetworkTransport.AddHost(this.m_DefaultTopology, this.m_BroadcastPort);
			if (this.m_HostId == -1)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkDiscovery StartAsClient - addHost failed");
				}
				return false;
			}
			byte b;
			NetworkTransport.SetBroadcastCredentials(this.m_HostId, this.m_BroadcastKey, this.m_BroadcastVersion, this.m_BroadcastSubVersion, out b);
			this.m_Running = true;
			this.m_IsClient = true;
			if (LogFilter.logDebug)
			{
				Debug.Log("StartAsClient Discovery listening");
			}
			return true;
		}

		public bool StartAsServer()
		{
			if (this.m_HostId != -1 || this.m_Running)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("NetworkDiscovery StartAsServer already started");
				}
				return false;
			}
			this.m_HostId = NetworkTransport.AddHost(this.m_DefaultTopology, 0);
			if (this.m_HostId == -1)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkDiscovery StartAsServer - addHost failed");
				}
				return false;
			}
			byte b;
			if (!NetworkTransport.StartBroadcastDiscovery(this.m_HostId, this.m_BroadcastPort, this.m_BroadcastKey, this.m_BroadcastVersion, this.m_BroadcastSubVersion, this.m_MsgOutBuffer, this.m_MsgOutBuffer.Length, this.m_BroadcastInterval, out b))
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + b);
				}
				return false;
			}
			this.m_Running = true;
			this.m_IsServer = true;
			if (LogFilter.logDebug)
			{
				Debug.Log("StartAsServer Discovery broadcasting");
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return true;
		}

		public void StopBroadcast()
		{
			if (this.m_HostId == -1)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkDiscovery StopBroadcast not initialized");
				}
				return;
			}
			if (!this.m_Running)
			{
				Debug.LogWarning("NetworkDiscovery StopBroadcast not started");
				return;
			}
			if (this.m_IsServer)
			{
				NetworkTransport.StopBroadcastDiscovery();
			}
			NetworkTransport.RemoveHost(this.m_HostId);
			this.m_HostId = -1;
			this.m_Running = false;
			this.m_IsServer = false;
			this.m_IsClient = false;
			this.m_MsgInBuffer = null;
			this.m_BroadcastsReceived = null;
			if (LogFilter.logDebug)
			{
				Debug.Log("Stopped Discovery broadcasting");
			}
		}

		private void Update()
		{
			if (this.m_HostId == -1)
			{
				return;
			}
			if (this.m_IsServer)
			{
				return;
			}
			NetworkEventType networkEventType;
			do
			{
				int num;
				int num2;
				int num3;
				byte b;
				networkEventType = NetworkTransport.ReceiveFromHost(this.m_HostId, out num, out num2, this.m_MsgInBuffer, 1024, out num3, out b);
				if (networkEventType == NetworkEventType.BroadcastEvent)
				{
					NetworkTransport.GetBroadcastConnectionMessage(this.m_HostId, this.m_MsgInBuffer, 1024, out num3, out b);
					string text;
					int num4;
					NetworkTransport.GetBroadcastConnectionInfo(this.m_HostId, out text, out num4, out b);
					NetworkBroadcastResult value = default(NetworkBroadcastResult);
					value.serverAddress = text;
					value.broadcastData = new byte[num3];
					Buffer.BlockCopy(this.m_MsgInBuffer, 0, value.broadcastData, 0, num3);
					this.m_BroadcastsReceived[text] = value;
					this.OnReceivedBroadcast(text, NetworkDiscovery.BytesToString(this.m_MsgInBuffer));
				}
			}
			while (networkEventType != NetworkEventType.Nothing);
		}

		public virtual void OnReceivedBroadcast(string fromAddress, string data)
		{
		}

		private void OnGUI()
		{
			if (!this.m_ShowGUI)
			{
				return;
			}
			int num = 10 + this.m_OffsetX;
			int num2 = 40 + this.m_OffsetY;
			if (this.m_MsgInBuffer == null)
			{
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Initialize Broadcast"))
				{
					this.Initialize();
				}
				return;
			}
			string str = string.Empty;
			if (this.m_IsServer)
			{
				str = " (server)";
			}
			if (this.m_IsClient)
			{
				str = " (client)";
			}
			GUI.Label(new Rect((float)num, (float)num2, 200f, 20f), "initialized" + str);
			num2 += 24;
			if (this.m_Running)
			{
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Stop"))
				{
					this.StopBroadcast();
				}
				num2 += 24;
				if (this.m_BroadcastsReceived != null)
				{
					foreach (string current in this.m_BroadcastsReceived.Keys)
					{
						NetworkBroadcastResult networkBroadcastResult = this.m_BroadcastsReceived[current];
						if (GUI.Button(new Rect((float)num, (float)(num2 + 20), 200f, 20f), "Game at " + current) && this.m_UseNetworkManager)
						{
							string text = NetworkDiscovery.BytesToString(networkBroadcastResult.broadcastData);
							string[] array = text.Split(new char[]
							{
								':'
							});
							if (array.Length == 3 && array[0] == "NetworkManager" && NetworkManager.singleton != null && NetworkManager.singleton.client == null)
							{
								NetworkManager.singleton.networkAddress = array[1];
								NetworkManager.singleton.networkPort = Convert.ToInt32(array[2]);
								NetworkManager.singleton.StartClient();
							}
						}
						num2 += 24;
					}
				}
			}
			else
			{
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Start Broadcasting"))
				{
					this.StartAsServer();
				}
				num2 += 24;
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Listen for Broadcast"))
				{
					this.StartAsClient();
				}
				num2 += 24;
			}
		}
	}
}
