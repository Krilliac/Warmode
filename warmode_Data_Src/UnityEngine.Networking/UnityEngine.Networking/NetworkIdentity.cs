using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Networking.NetworkSystem;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkIdentity"), DisallowMultipleComponent, ExecuteInEditMode]
	public sealed class NetworkIdentity : MonoBehaviour
	{
		[SerializeField]
		private NetworkSceneId m_SceneId;

		[SerializeField]
		private NetworkHash128 m_AssetId;

		[SerializeField]
		private bool m_ServerOnly;

		[SerializeField]
		private bool m_LocalPlayerAuthority;

		private bool m_IsClient;

		private bool m_IsServer;

		private bool m_HasAuthority;

		private NetworkInstanceId m_NetId;

		private bool m_IsLocalPlayer;

		private NetworkConnection m_ConnectionToServer;

		private NetworkConnection m_ConnectionToClient;

		private short m_PlayerId = -1;

		private NetworkBehaviour[] m_NetworkBehaviours;

		private HashSet<int> m_ObserverConnections;

		private List<NetworkConnection> m_Observers;

		private NetworkConnection m_ClientAuthorityOwner;

		private static uint s_NextNetworkId = 1u;

		private static NetworkWriter s_UpdateWriter = new NetworkWriter();

		public bool isClient
		{
			get
			{
				return this.m_IsClient;
			}
		}

		public bool isServer
		{
			get
			{
				return this.m_IsServer && NetworkServer.active && this.m_IsServer;
			}
		}

		public bool hasAuthority
		{
			get
			{
				return this.m_HasAuthority;
			}
		}

		public NetworkInstanceId netId
		{
			get
			{
				return this.m_NetId;
			}
		}

		public NetworkSceneId sceneId
		{
			get
			{
				return this.m_SceneId;
			}
		}

		public bool serverOnly
		{
			get
			{
				return this.m_ServerOnly;
			}
			set
			{
				this.m_ServerOnly = value;
			}
		}

		public bool localPlayerAuthority
		{
			get
			{
				return this.m_LocalPlayerAuthority;
			}
			set
			{
				this.m_LocalPlayerAuthority = value;
			}
		}

		public NetworkConnection clientAuthorityOwner
		{
			get
			{
				return this.m_ClientAuthorityOwner;
			}
		}

		public NetworkHash128 assetId
		{
			get
			{
				return this.m_AssetId;
			}
		}

		public bool isLocalPlayer
		{
			get
			{
				return this.m_IsLocalPlayer;
			}
		}

		public short playerControllerId
		{
			get
			{
				return this.m_PlayerId;
			}
		}

		public NetworkConnection connectionToServer
		{
			get
			{
				return this.m_ConnectionToServer;
			}
		}

		public NetworkConnection connectionToClient
		{
			get
			{
				return this.m_ConnectionToClient;
			}
		}

		public ReadOnlyCollection<NetworkConnection> observers
		{
			get
			{
				if (this.m_Observers == null)
				{
					return null;
				}
				return new ReadOnlyCollection<NetworkConnection>(this.m_Observers);
			}
		}

		internal void SetDynamicAssetId(NetworkHash128 newAssetId)
		{
			if (!this.m_AssetId.IsValid() || this.m_AssetId.Equals(newAssetId))
			{
				this.m_AssetId = newAssetId;
			}
			else if (LogFilter.logWarn)
			{
				Debug.LogWarning("SetDynamicAssetId object already has an assetId <" + this.m_AssetId + ">");
			}
		}

		internal void SetClientOwner(NetworkConnection conn)
		{
			if (this.m_ClientAuthorityOwner != null && LogFilter.logError)
			{
				Debug.LogError("SetClientOwner m_ClientAuthorityOwner already set!");
			}
			this.m_ClientAuthorityOwner = conn;
			this.m_ClientAuthorityOwner.AddOwnedObject(this);
		}

		internal void ClearClientOwner()
		{
			this.m_ClientAuthorityOwner = null;
		}

		internal void ForceAuthority(bool authority)
		{
			this.m_HasAuthority = authority;
			if (authority)
			{
				this.OnStartAuthority();
			}
			else
			{
				this.OnStopAuthority();
			}
		}

		internal static NetworkInstanceId GetNextNetworkId()
		{
			uint value = NetworkIdentity.s_NextNetworkId;
			NetworkIdentity.s_NextNetworkId += 1u;
			return new NetworkInstanceId(value);
		}

		private void CacheBehaviours()
		{
			if (this.m_NetworkBehaviours == null)
			{
				this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
			}
		}

		internal void SetNetworkInstanceId(NetworkInstanceId newNetId)
		{
			this.m_NetId = newNetId;
		}

		public void ForceSceneId(int newSceneId)
		{
			this.m_SceneId = new NetworkSceneId((uint)newSceneId);
		}

		internal void UpdateClientServer(bool isClientFlag, bool isServerFlag)
		{
			this.m_IsClient |= isClientFlag;
			this.m_IsServer |= isServerFlag;
		}

		internal void SetNoServer()
		{
			this.m_IsServer = false;
			this.SetNetworkInstanceId(NetworkInstanceId.Zero);
		}

		internal void SetNotLocalPlayer()
		{
			this.m_IsLocalPlayer = false;
			this.m_HasAuthority = false;
		}

		internal void RemoveObserverInternal(NetworkConnection conn)
		{
			if (this.m_Observers != null)
			{
				this.m_Observers.Remove(conn);
				this.m_ObserverConnections.Remove(conn.connectionId);
			}
		}

		private void OnDestroy()
		{
			if (this.m_IsServer)
			{
				NetworkServer.Destroy(base.gameObject);
			}
		}

		internal void OnStartServer()
		{
			if (this.m_IsServer)
			{
				return;
			}
			this.m_IsServer = true;
			if (this.m_LocalPlayerAuthority)
			{
				this.m_HasAuthority = false;
			}
			else
			{
				this.m_HasAuthority = true;
			}
			this.m_Observers = new List<NetworkConnection>();
			this.m_ObserverConnections = new HashSet<int>();
			this.CacheBehaviours();
			if (this.netId.IsEmpty())
			{
				this.m_NetId = NetworkIdentity.GetNextNetworkId();
				if (LogFilter.logDev)
				{
					Debug.Log(string.Concat(new object[]
					{
						"OnStartServer ",
						base.gameObject,
						" GUID:",
						this.netId
					}));
				}
				NetworkServer.instance.SetLocalObjectOnServer(this.netId, base.gameObject);
				for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
				{
					NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
					try
					{
						networkBehaviour.OnStartServer();
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in OnStartServer:" + ex.Message + " " + ex.StackTrace);
					}
				}
				if (NetworkClient.active && NetworkServer.localClientActive)
				{
					ClientScene.SetLocalObject(this.netId, base.gameObject);
					this.OnStartClient();
				}
				if (this.m_HasAuthority)
				{
					this.OnStartAuthority();
				}
				return;
			}
			if (LogFilter.logError)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Object has non-zero netId ",
					this.netId,
					" for ",
					base.gameObject,
					" !!1"
				}));
			}
		}

		internal void OnStartClient()
		{
			if (!this.m_IsClient)
			{
				this.m_IsClient = true;
			}
			this.CacheBehaviours();
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"OnStartClient ",
					base.gameObject,
					" GUID:",
					this.netId,
					" localPlayerAuthority:",
					this.localPlayerAuthority
				}));
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				try
				{
					networkBehaviour.PreStartClient();
					networkBehaviour.OnStartClient();
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnStartClient:" + ex.Message + " " + ex.StackTrace);
				}
			}
		}

		internal void OnStartAuthority()
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				try
				{
					networkBehaviour.OnStartAuthority();
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnStartAuthority:" + ex.Message + " " + ex.StackTrace);
				}
			}
		}

		internal void OnStopAuthority()
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				try
				{
					networkBehaviour.OnStopAuthority();
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnStopAuthority:" + ex.Message + " " + ex.StackTrace);
				}
			}
		}

		internal void OnSetLocalVisibility(bool vis)
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				try
				{
					networkBehaviour.OnSetLocalVisibility(vis);
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnSetLocalVisibility:" + ex.Message + " " + ex.StackTrace);
				}
			}
		}

		internal bool OnCheckObserver(NetworkConnection conn)
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				try
				{
					if (!networkBehaviour.OnCheckObserver(conn))
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnCheckObserver:" + ex.Message + " " + ex.StackTrace);
				}
			}
			return true;
		}

		internal void UNetSerializeAllVars(NetworkWriter writer)
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				networkBehaviour.OnSerialize(writer, true);
			}
		}

		internal void HandleSyncEvent(int cmdHash, NetworkReader reader)
		{
			if (base.gameObject == null)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"SyncEvent [",
						NetworkBehaviour.GetCmdHashHandlerName(cmdHash),
						"] received for deleted object ",
						this.netId
					}));
				}
				return;
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				if (networkBehaviour.InvokeSyncEvent(cmdHash, reader))
				{
					break;
				}
			}
		}

		internal void HandleClientAuthority(bool authority)
		{
			if (!this.localPlayerAuthority)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("HandleClientAuthority " + base.gameObject + " does not have localPlayerAuthority");
				}
				return;
			}
			this.ForceAuthority(authority);
		}

		internal void HandleSyncList(int cmdHash, NetworkReader reader)
		{
			if (base.gameObject == null)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"SyncList [",
						NetworkBehaviour.GetCmdHashHandlerName(cmdHash),
						"] received for deleted object ",
						this.netId
					}));
				}
				return;
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				if (networkBehaviour.InvokeSyncList(cmdHash, reader))
				{
					break;
				}
			}
		}

		internal void HandleCommand(int cmdHash, NetworkReader reader)
		{
			if (base.gameObject == null)
			{
				string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
				if (LogFilter.logWarn)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Command [",
						cmdHashHandlerName,
						"] received for deleted object [netId=",
						this.netId,
						"]"
					}));
				}
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				if (networkBehaviour.InvokeCommand(cmdHash, reader))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				string cmdHashHandlerName2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
				if (LogFilter.logError)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Found no receiver for incoming command [",
						cmdHashHandlerName2,
						"] on ",
						base.gameObject,
						",  the server and client should have the same NetworkBehaviour instances [netId=",
						this.netId,
						"]."
					}));
				}
			}
		}

		internal void HandleRPC(int cmdHash, NetworkReader reader)
		{
			if (base.gameObject == null)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"ClientRpc [",
						NetworkBehaviour.GetCmdHashHandlerName(cmdHash),
						"] received for deleted object ",
						this.netId
					}));
				}
				return;
			}
			if (this.m_NetworkBehaviours.Length == 0)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("No receiver found for ClientRpc [" + NetworkBehaviour.GetCmdHashHandlerName(cmdHash) + "]. Does the script with the function inherit NetworkBehaviour?");
				}
				return;
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				if (networkBehaviour.InvokeRPC(cmdHash, reader))
				{
					return;
				}
			}
			string text = NetworkBehaviour.GetInvoker(cmdHash);
			if (text == null)
			{
				text = "[unknown:" + cmdHash + "]";
			}
			if (LogFilter.logWarn)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Failed to invoke RPC ",
					text,
					"(",
					cmdHash,
					") on netID ",
					this.netId
				}));
			}
			NetworkBehaviour.DumpInvokers();
		}

		internal void UNetUpdate()
		{
			uint num = 0u;
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				int dirtyChannel = networkBehaviour.GetDirtyChannel();
				if (dirtyChannel != -1)
				{
					num |= 1u << dirtyChannel;
				}
			}
			if (num == 0u)
			{
				return;
			}
			for (int j = 0; j < NetworkServer.numChannels; j++)
			{
				if ((num & 1u << j) != 0u)
				{
					NetworkIdentity.s_UpdateWriter.StartMessage(8);
					NetworkIdentity.s_UpdateWriter.Write(this.netId);
					bool flag = false;
					for (int k = 0; k < this.m_NetworkBehaviours.Length; k++)
					{
						short position = NetworkIdentity.s_UpdateWriter.Position;
						NetworkBehaviour networkBehaviour2 = this.m_NetworkBehaviours[k];
						if (networkBehaviour2.GetDirtyChannel() != j)
						{
							networkBehaviour2.OnSerialize(NetworkIdentity.s_UpdateWriter, false);
						}
						else
						{
							if (networkBehaviour2.OnSerialize(NetworkIdentity.s_UpdateWriter, false))
							{
								networkBehaviour2.ClearAllDirtyBits();
								flag = true;
							}
							if (NetworkIdentity.s_UpdateWriter.Position - position > (short)NetworkServer.maxPacketSize)
							{
								Debug.LogWarning(string.Concat(new object[]
								{
									"Large state update of ",
									(int)(NetworkIdentity.s_UpdateWriter.Position - position),
									" bytes (max is ",
									NetworkServer.maxPacketSize,
									" for netId:",
									this.netId,
									" from script:",
									networkBehaviour2
								}));
							}
						}
					}
					if (flag)
					{
						NetworkIdentity.s_UpdateWriter.FinishMessage();
						NetworkServer.SendWriterToReady(base.gameObject, NetworkIdentity.s_UpdateWriter, j);
					}
				}
			}
		}

		internal void OnUpdateVars(NetworkReader reader, bool initialState)
		{
			if (initialState && this.m_NetworkBehaviours == null)
			{
				this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				networkBehaviour.OnDeserialize(reader, initialState);
			}
		}

		internal void SetLocalPlayer(short localPlayerControllerId)
		{
			this.m_IsLocalPlayer = true;
			this.m_PlayerId = localPlayerControllerId;
			if (this.localPlayerAuthority)
			{
				this.m_HasAuthority = true;
			}
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				networkBehaviour.OnStartLocalPlayer();
				if (this.localPlayerAuthority)
				{
					networkBehaviour.OnStartAuthority();
				}
			}
		}

		internal void SetConnectionToServer(NetworkConnection conn)
		{
			this.m_ConnectionToServer = conn;
		}

		internal void SetConnectionToClient(NetworkConnection conn, short newPlayerControllerId)
		{
			this.m_PlayerId = newPlayerControllerId;
			this.m_ConnectionToClient = conn;
		}

		internal void OnNetworkDestroy()
		{
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				networkBehaviour.OnNetworkDestroy();
			}
			this.m_IsServer = false;
		}

		internal void ClearObservers()
		{
			if (this.m_Observers != null)
			{
				int count = this.m_Observers.Count;
				for (int i = 0; i < count; i++)
				{
					NetworkConnection networkConnection = this.m_Observers[i];
					networkConnection.RemoveFromVisList(this, true);
				}
				this.m_Observers.Clear();
				this.m_ObserverConnections.Clear();
			}
		}

		internal void AddObserver(NetworkConnection conn)
		{
			if (this.m_Observers == null)
			{
				return;
			}
			if (this.m_ObserverConnections.Contains(conn.connectionId))
			{
				if (LogFilter.logDebug)
				{
					Debug.Log(string.Concat(new object[]
					{
						"Duplicate observer ",
						conn.address,
						" added for ",
						base.gameObject
					}));
				}
				return;
			}
			if (LogFilter.logDev)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Added observer ",
					conn.address,
					" added for ",
					base.gameObject
				}));
			}
			this.m_Observers.Add(conn);
			this.m_ObserverConnections.Add(conn.connectionId);
			conn.AddToVisList(this);
		}

		internal void RemoveObserver(NetworkConnection conn)
		{
			if (this.m_Observers == null)
			{
				return;
			}
			this.m_Observers.Remove(conn);
			this.m_ObserverConnections.Remove(conn.connectionId);
			conn.RemoveFromVisList(this, false);
		}

		public void RebuildObservers(bool initialize)
		{
			if (this.m_Observers == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			HashSet<NetworkConnection> hashSet = new HashSet<NetworkConnection>();
			HashSet<NetworkConnection> hashSet2 = new HashSet<NetworkConnection>(this.m_Observers);
			for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = this.m_NetworkBehaviours[i];
				flag2 |= networkBehaviour.OnRebuildObservers(hashSet, initialize);
			}
			if (!flag2)
			{
				if (initialize)
				{
					foreach (NetworkConnection current in NetworkServer.connections)
					{
						if (current != null)
						{
							if (current.isReady)
							{
								this.AddObserver(current);
							}
						}
					}
					foreach (NetworkConnection current2 in NetworkServer.localConnections)
					{
						if (current2 != null)
						{
							if (current2.isReady)
							{
								this.AddObserver(current2);
							}
						}
					}
				}
				return;
			}
			foreach (NetworkConnection current3 in hashSet)
			{
				if (current3 != null)
				{
					if (!current3.isReady)
					{
						if (LogFilter.logWarn)
						{
							Debug.LogWarning(string.Concat(new object[]
							{
								"Observer is not ready for ",
								base.gameObject,
								" ",
								current3
							}));
						}
					}
					else if (initialize || !hashSet2.Contains(current3))
					{
						current3.AddToVisList(this);
						if (LogFilter.logDebug)
						{
							Debug.Log(string.Concat(new object[]
							{
								"New Observer for ",
								base.gameObject,
								" ",
								current3
							}));
						}
						flag = true;
					}
				}
			}
			foreach (NetworkConnection current4 in hashSet2)
			{
				if (!hashSet.Contains(current4))
				{
					current4.RemoveFromVisList(this, false);
					if (LogFilter.logDebug)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Removed Observer for ",
							base.gameObject,
							" ",
							current4
						}));
					}
					flag = true;
				}
			}
			if (initialize)
			{
				foreach (NetworkConnection current5 in NetworkServer.localConnections)
				{
					if (!hashSet.Contains(current5))
					{
						this.OnSetLocalVisibility(false);
					}
				}
			}
			if (!flag)
			{
				return;
			}
			this.m_Observers = new List<NetworkConnection>(hashSet);
			this.m_ObserverConnections.Clear();
			foreach (NetworkConnection current6 in this.m_Observers)
			{
				this.m_ObserverConnections.Add(current6.connectionId);
			}
		}

		public bool RemoveClientAuthority(NetworkConnection conn)
		{
			if (!this.isServer)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("RemoveClientAuthority can only be call on the server for spawned objects.");
				}
				return false;
			}
			if (this.connectionToClient != null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("RemoveClientAuthority cannot remove authority for a player object");
				}
				return false;
			}
			if (this.m_ClientAuthorityOwner == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has no clientAuthority owner.");
				}
				return false;
			}
			if (this.m_ClientAuthorityOwner != conn)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has different owner.");
				}
				return false;
			}
			this.m_ClientAuthorityOwner.RemoveOwnedObject(this);
			this.m_ClientAuthorityOwner = null;
			this.ForceAuthority(true);
			conn.Send(15, new ClientAuthorityMessage
			{
				netId = this.netId,
				authority = false
			});
			return true;
		}

		public bool AssignClientAuthority(NetworkConnection conn)
		{
			if (!this.isServer)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AssignClientAuthority can only be call on the server for spawned objects.");
				}
				return false;
			}
			if (!this.localPlayerAuthority)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AssignClientAuthority can only be used for NetworkIdentity component with LocalPlayerAuthority set.");
				}
				return false;
			}
			if (this.m_ClientAuthorityOwner != null && conn != this.m_ClientAuthorityOwner)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AssignClientAuthority for " + base.gameObject + " already has an owner. Use RemoveClientAuthority() first.");
				}
				return false;
			}
			if (conn == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("AssignClientAuthority for " + base.gameObject + " owner cannot be null. Use RemoveClientAuthority() instead.");
				}
				return false;
			}
			this.m_ClientAuthorityOwner = conn;
			this.m_ClientAuthorityOwner.AddOwnedObject(this);
			this.ForceAuthority(false);
			conn.Send(15, new ClientAuthorityMessage
			{
				netId = this.netId,
				authority = true
			});
			return true;
		}

		internal static void UNetStaticUpdate()
		{
			NetworkServer.Update();
			NetworkClient.UpdateClients();
			NetworkManager.UpdateScene();
		}
	}
}
