using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking.NetworkSystem;

namespace UnityEngine.Networking
{
	public class NetworkCRC
	{
		internal static NetworkCRC s_Singleton;

		private Dictionary<string, int> m_Scripts = new Dictionary<string, int>();

		private bool m_ScriptCRCCheck;

		internal static NetworkCRC singleton
		{
			get
			{
				if (NetworkCRC.s_Singleton == null)
				{
					NetworkCRC.s_Singleton = new NetworkCRC();
				}
				return NetworkCRC.s_Singleton;
			}
		}

		public Dictionary<string, int> scripts
		{
			get
			{
				return this.m_Scripts;
			}
		}

		public static bool scriptCRCCheck
		{
			get
			{
				return NetworkCRC.singleton.m_ScriptCRCCheck;
			}
			set
			{
				NetworkCRC.singleton.m_ScriptCRCCheck = value;
			}
		}

		public static void ReinitializeScriptCRCs(Assembly callingAssembly)
		{
			NetworkCRC.singleton.m_Scripts.Clear();
			Type[] types = callingAssembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.BaseType == typeof(NetworkBehaviour))
				{
					MethodInfo method = type.GetMethod(".cctor", BindingFlags.Static);
					if (method != null)
					{
						method.Invoke(null, new object[0]);
					}
				}
			}
		}

		public static void RegisterBehaviour(string name, int channel)
		{
			NetworkCRC.singleton.m_Scripts[name] = channel;
		}

		internal static bool Validate(CRCMessageEntry[] scripts, int numChannels)
		{
			return NetworkCRC.singleton.ValidateInternal(scripts, numChannels);
		}

		private bool ValidateInternal(CRCMessageEntry[] remoteScripts, int numChannels)
		{
			if (this.m_Scripts.Count != remoteScripts.Length)
			{
				if (LogFilter.logWarn)
				{
					Debug.LogWarning("Network configuration mismatch detected. The number of networked scripts on the client does not match the number of networked scripts on the server. This could be caused by lazy loading of scripts on the client. This warning can be disabled by the checkbox NetworkManager Script CRC Check.");
				}
				this.Dump(remoteScripts);
				return false;
			}
			for (int i = 0; i < remoteScripts.Length; i++)
			{
				CRCMessageEntry cRCMessageEntry = remoteScripts[i];
				if (LogFilter.logDebug)
				{
					Debug.Log(string.Concat(new object[]
					{
						"Script: ",
						cRCMessageEntry.name,
						" Channel: ",
						cRCMessageEntry.channel
					}));
				}
				if (this.m_Scripts.ContainsKey(cRCMessageEntry.name))
				{
					int num = this.m_Scripts[cRCMessageEntry.name];
					if (num != (int)cRCMessageEntry.channel)
					{
						if (LogFilter.logError)
						{
							Debug.LogError(string.Concat(new object[]
							{
								"HLAPI CRC Channel Mismatch. Script: ",
								cRCMessageEntry.name,
								" LocalChannel: ",
								num,
								" RemoteChannel: ",
								cRCMessageEntry.channel
							}));
						}
						this.Dump(remoteScripts);
						return false;
					}
				}
				if ((int)cRCMessageEntry.channel >= numChannels)
				{
					if (LogFilter.logError)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"HLAPI CRC channel out of range! Script: ",
							cRCMessageEntry.name,
							" Channel: ",
							cRCMessageEntry.channel
						}));
					}
					this.Dump(remoteScripts);
					return false;
				}
			}
			return true;
		}

		private void Dump(CRCMessageEntry[] remoteScripts)
		{
			foreach (string current in this.m_Scripts.Keys)
			{
				Debug.Log(string.Concat(new object[]
				{
					"CRC Local Dump ",
					current,
					" : ",
					this.m_Scripts[current]
				}));
			}
			for (int i = 0; i < remoteScripts.Length; i++)
			{
				CRCMessageEntry cRCMessageEntry = remoteScripts[i];
				Debug.Log(string.Concat(new object[]
				{
					"CRC Remote Dump ",
					cRCMessageEntry.name,
					" : ",
					cRCMessageEntry.channel
				}));
			}
		}
	}
}
