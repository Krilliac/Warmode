using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.Statistics;
using BestHTTP.WebSocket;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BestHTTP
{
	public static class HTTPManager
	{
		private static byte maxConnectionPerServer;

		private static HeartbeatManager heartbeats;

		private static ILogger logger;

		private static Dictionary<string, List<HTTPConnection>> Connections;

		private static List<HTTPConnection> ActiveConnections;

		private static List<HTTPConnection> FreeConnections;

		private static List<HTTPConnection> RecycledConnections;

		private static List<HTTPRequest> RequestQueue;

		private static bool IsCallingCallbacks;

		internal static object Locker;

		public static byte MaxConnectionPerServer
		{
			get
			{
				return HTTPManager.maxConnectionPerServer;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MaxConnectionPerServer must be greater than 0!");
				}
				HTTPManager.maxConnectionPerServer = value;
			}
		}

		public static bool KeepAliveDefaultValue
		{
			get;
			set;
		}

		public static bool IsCachingDisabled
		{
			get;
			set;
		}

		public static TimeSpan MaxConnectionIdleTime
		{
			get;
			set;
		}

		public static bool IsCookiesEnabled
		{
			get;
			set;
		}

		public static uint CookieJarSize
		{
			get;
			set;
		}

		public static bool EnablePrivateBrowsing
		{
			get;
			set;
		}

		public static TimeSpan ConnectTimeout
		{
			get;
			set;
		}

		public static TimeSpan RequestTimeout
		{
			get;
			set;
		}

		public static Func<string> RootCacheFolderProvider
		{
			get;
			set;
		}

		public static HTTPProxy Proxy
		{
			get;
			set;
		}

		public static HeartbeatManager Heartbeats
		{
			get
			{
				if (HTTPManager.heartbeats == null)
				{
					HTTPManager.heartbeats = new HeartbeatManager();
				}
				return HTTPManager.heartbeats;
			}
		}

		public static ILogger Logger
		{
			get
			{
				if (HTTPManager.logger == null)
				{
					HTTPManager.logger = new DefaultLogger();
					HTTPManager.logger.Level = Loglevels.None;
				}
				return HTTPManager.logger;
			}
			set
			{
				HTTPManager.logger = value;
			}
		}

		public static ICertificateVerifyer DefaultCertificateVerifyer
		{
			get;
			set;
		}

		internal static int MaxPathLength
		{
			get;
			set;
		}

		static HTTPManager()
		{
			HTTPManager.Connections = new Dictionary<string, List<HTTPConnection>>();
			HTTPManager.ActiveConnections = new List<HTTPConnection>();
			HTTPManager.FreeConnections = new List<HTTPConnection>();
			HTTPManager.RecycledConnections = new List<HTTPConnection>();
			HTTPManager.RequestQueue = new List<HTTPRequest>();
			HTTPManager.Locker = new object();
			HTTPManager.MaxConnectionPerServer = 4;
			HTTPManager.KeepAliveDefaultValue = true;
			HTTPManager.MaxPathLength = 255;
			HTTPManager.MaxConnectionIdleTime = TimeSpan.FromSeconds(30.0);
			HTTPManager.IsCookiesEnabled = true;
			HTTPManager.CookieJarSize = 10485760u;
			HTTPManager.EnablePrivateBrowsing = false;
			HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(20.0);
			HTTPManager.RequestTimeout = TimeSpan.FromSeconds(60.0);
			HTTPManager.logger = new DefaultLogger();
			HTTPManager.DefaultCertificateVerifyer = null;
		}

		public static void Setup()
		{
			HTTPUpdateDelegator.CheckInstance();
			HTTPCacheService.CheckSetup();
			CookieJar.SetupFolder();
		}

		public static HTTPRequest SendRequest(string url, OnRequestFinishedDelegate callback)
		{
			return HTTPManager.SendRequest(new HTTPRequest(new Uri(url), HTTPMethods.Get, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, OnRequestFinishedDelegate callback)
		{
			return HTTPManager.SendRequest(new HTTPRequest(new Uri(url), methodType, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, bool isKeepAlive, OnRequestFinishedDelegate callback)
		{
			return HTTPManager.SendRequest(new HTTPRequest(new Uri(url), methodType, isKeepAlive, callback));
		}

		public static HTTPRequest SendRequest(string url, HTTPMethods methodType, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
		{
			return HTTPManager.SendRequest(new HTTPRequest(new Uri(url), methodType, isKeepAlive, disableCache, callback));
		}

		public static HTTPRequest SendRequest(HTTPRequest request)
		{
			object locker = HTTPManager.Locker;
			lock (locker)
			{
				HTTPManager.Setup();
				if (HTTPManager.IsCallingCallbacks)
				{
					request.State = HTTPRequestStates.Queued;
					HTTPManager.RequestQueue.Add(request);
				}
				else
				{
					HTTPManager.SendRequestImpl(request);
				}
			}
			return request;
		}

		public static GeneralStatistics GetGeneralStatistics(StatisticsQueryFlags queryFlags)
		{
			GeneralStatistics result = default(GeneralStatistics);
			result.QueryFlags = queryFlags;
			if ((byte)(queryFlags & StatisticsQueryFlags.Connections) != 0)
			{
				int num = 0;
				foreach (KeyValuePair<string, List<HTTPConnection>> current in HTTPManager.Connections)
				{
					if (current.Value != null)
					{
						num += current.Value.Count;
					}
				}
				result.Connections = num;
				result.ActiveConnections = HTTPManager.ActiveConnections.Count;
				result.FreeConnections = HTTPManager.FreeConnections.Count;
				result.RecycledConnections = HTTPManager.RecycledConnections.Count;
				result.RequestsInQueue = HTTPManager.RequestQueue.Count;
			}
			if ((byte)(queryFlags & StatisticsQueryFlags.Cache) != 0)
			{
				result.CacheEntityCount = HTTPCacheService.GetCacheEntityCount();
				result.CacheSize = HTTPCacheService.GetCacheSize();
			}
			if ((byte)(queryFlags & StatisticsQueryFlags.Cookies) != 0)
			{
				List<Cookie> all = CookieJar.GetAll();
				result.CookieCount = all.Count;
				uint num2 = 0u;
				for (int i = 0; i < all.Count; i++)
				{
					num2 += all[i].GuessSize();
				}
				result.CookieJarSize = num2;
			}
			return result;
		}

		private static void SendRequestImpl(HTTPRequest request)
		{
			HTTPConnection conn = HTTPManager.FindOrCreateFreeConnection(request);
			if (conn != null)
			{
				if (HTTPManager.ActiveConnections.Find((HTTPConnection c) => c == conn) == null)
				{
					HTTPManager.ActiveConnections.Add(conn);
				}
				HTTPManager.FreeConnections.Remove(conn);
				request.State = HTTPRequestStates.Processing;
				request.Prepare();
				conn.Process(request);
			}
			else
			{
				request.State = HTTPRequestStates.Queued;
				HTTPManager.RequestQueue.Add(request);
			}
		}

		private static string GetKeyForRequest(HTTPRequest request)
		{
			return ((request.Proxy == null) ? string.Empty : new UriBuilder(request.Proxy.Address.Scheme, request.Proxy.Address.Host, request.Proxy.Address.Port).Uri.ToString()) + new UriBuilder(request.CurrentUri.Scheme, request.CurrentUri.Host, request.CurrentUri.Port).Uri.ToString();
		}

		private static HTTPConnection FindOrCreateFreeConnection(HTTPRequest request)
		{
			HTTPConnection hTTPConnection = null;
			string keyForRequest = HTTPManager.GetKeyForRequest(request);
			List<HTTPConnection> list;
			if (HTTPManager.Connections.TryGetValue(keyForRequest, out list))
			{
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].State < HTTPConnectionStates.Free)
					{
						num++;
					}
				}
				if (num <= (int)HTTPManager.MaxConnectionPerServer)
				{
					int num2 = 0;
					while (num2 < list.Count && hTTPConnection == null)
					{
						HTTPConnection hTTPConnection2 = list[num2];
						if (hTTPConnection2 != null && hTTPConnection2.IsFree && (!hTTPConnection2.HasProxy || hTTPConnection2.LastProcessedUri == null || hTTPConnection2.LastProcessedUri.Host.Equals(request.CurrentUri.Host, StringComparison.OrdinalIgnoreCase)))
						{
							hTTPConnection = hTTPConnection2;
						}
						num2++;
					}
				}
			}
			else
			{
				HTTPManager.Connections.Add(keyForRequest, list = new List<HTTPConnection>((int)HTTPManager.MaxConnectionPerServer));
			}
			if (hTTPConnection == null)
			{
				if (list.Count >= (int)HTTPManager.MaxConnectionPerServer)
				{
					return null;
				}
				list.Add(hTTPConnection = new HTTPConnection(keyForRequest));
			}
			return hTTPConnection;
		}

		private static bool CanProcessFromQueue()
		{
			for (int i = 0; i < HTTPManager.RequestQueue.Count; i++)
			{
				if (HTTPManager.FindOrCreateFreeConnection(HTTPManager.RequestQueue[i]) != null)
				{
					return true;
				}
			}
			return false;
		}

		private static void RecycleConnection(HTTPConnection conn)
		{
			conn.Recycle();
			HTTPManager.RecycledConnections.Add(conn);
		}

		internal static HTTPConnection GetConnectionWith(HTTPRequest request)
		{
			object locker = HTTPManager.Locker;
			HTTPConnection result;
			lock (locker)
			{
				for (int i = 0; i < HTTPManager.ActiveConnections.Count; i++)
				{
					HTTPConnection hTTPConnection = HTTPManager.ActiveConnections[i];
					if (hTTPConnection.CurrentRequest == request)
					{
						result = hTTPConnection;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static bool RemoveFromQueue(HTTPRequest request)
		{
			return HTTPManager.RequestQueue.Remove(request);
		}

		internal static string GetRootCacheFolder()
		{
			try
			{
				if (HTTPManager.RootCacheFolderProvider != null)
				{
					return HTTPManager.RootCacheFolderProvider();
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPManager", "GetRootCacheFolder", ex);
			}
			return Application.persistentDataPath;
		}

		public static void OnUpdate()
		{
			object locker = HTTPManager.Locker;
			lock (locker)
			{
				HTTPManager.IsCallingCallbacks = true;
				try
				{
					for (int i = 0; i < HTTPManager.ActiveConnections.Count; i++)
					{
						HTTPConnection hTTPConnection = HTTPManager.ActiveConnections[i];
						switch (hTTPConnection.State)
						{
						case HTTPConnectionStates.Processing:
							hTTPConnection.HandleProgressCallback();
							if (hTTPConnection.CurrentRequest.UseStreaming && hTTPConnection.CurrentRequest.Response != null && hTTPConnection.CurrentRequest.Response.HasStreamedFragments())
							{
								hTTPConnection.HandleCallback();
							}
							if (((!hTTPConnection.CurrentRequest.UseStreaming && hTTPConnection.CurrentRequest.UploadStream == null) || hTTPConnection.CurrentRequest.EnableTimoutForStreaming) && DateTime.UtcNow - hTTPConnection.StartTime > hTTPConnection.CurrentRequest.Timeout)
							{
								hTTPConnection.Abort(HTTPConnectionStates.TimedOut);
							}
							break;
						case HTTPConnectionStates.Redirected:
							HTTPManager.SendRequest(hTTPConnection.CurrentRequest);
							HTTPManager.RecycleConnection(hTTPConnection);
							break;
						case HTTPConnectionStates.Upgraded:
							hTTPConnection.HandleCallback();
							break;
						case HTTPConnectionStates.WaitForProtocolShutdown:
						{
							WebSocketResponse webSocketResponse = hTTPConnection.CurrentRequest.Response as WebSocketResponse;
							if (webSocketResponse != null)
							{
								webSocketResponse.HandleEvents();
							}
							if (webSocketResponse == null || webSocketResponse.IsClosed)
							{
								hTTPConnection.HandleCallback();
								hTTPConnection.Dispose();
								HTTPManager.RecycleConnection(hTTPConnection);
							}
							break;
						}
						case HTTPConnectionStates.WaitForRecycle:
							hTTPConnection.CurrentRequest.FinishStreaming();
							hTTPConnection.HandleCallback();
							HTTPManager.RecycleConnection(hTTPConnection);
							break;
						case HTTPConnectionStates.AbortRequested:
						{
							WebSocketResponse webSocketResponse = hTTPConnection.CurrentRequest.Response as WebSocketResponse;
							if (webSocketResponse != null)
							{
								webSocketResponse.HandleEvents();
								if (webSocketResponse.IsClosed)
								{
									hTTPConnection.HandleCallback();
									hTTPConnection.Dispose();
									HTTPManager.RecycleConnection(hTTPConnection);
								}
							}
							break;
						}
						case HTTPConnectionStates.TimedOut:
							if (DateTime.UtcNow - hTTPConnection.TimedOutStart > TimeSpan.FromMilliseconds(500.0))
							{
								HTTPManager.Logger.Information("HTTPManager", "Hard aborting connection becouse of a long waiting TimedOut state");
								hTTPConnection.CurrentRequest.Response = null;
								hTTPConnection.CurrentRequest.State = HTTPRequestStates.TimedOut;
								hTTPConnection.HandleCallback();
								HTTPManager.RecycleConnection(hTTPConnection);
							}
							break;
						case HTTPConnectionStates.Closed:
							hTTPConnection.CurrentRequest.FinishStreaming();
							hTTPConnection.HandleCallback();
							HTTPManager.RecycleConnection(hTTPConnection);
							break;
						}
					}
				}
				finally
				{
					HTTPManager.IsCallingCallbacks = false;
				}
				if (HTTPManager.RecycledConnections.Count > 0)
				{
					for (int j = 0; j < HTTPManager.RecycledConnections.Count; j++)
					{
						HTTPConnection hTTPConnection2 = HTTPManager.RecycledConnections[j];
						if (hTTPConnection2.IsFree)
						{
							HTTPManager.ActiveConnections.Remove(hTTPConnection2);
							HTTPManager.FreeConnections.Add(hTTPConnection2);
						}
					}
					HTTPManager.RecycledConnections.Clear();
				}
				if (HTTPManager.FreeConnections.Count > 0)
				{
					for (int k = 0; k < HTTPManager.FreeConnections.Count; k++)
					{
						HTTPConnection hTTPConnection3 = HTTPManager.FreeConnections[k];
						if (hTTPConnection3.IsRemovable)
						{
							List<HTTPConnection> list = null;
							if (HTTPManager.Connections.TryGetValue(hTTPConnection3.ServerAddress, out list))
							{
								list.Remove(hTTPConnection3);
							}
							hTTPConnection3.Dispose();
							HTTPManager.FreeConnections.RemoveAt(k);
							k--;
						}
					}
				}
				if (HTTPManager.CanProcessFromQueue())
				{
					if (HTTPManager.RequestQueue.Find((HTTPRequest req) => req.Priority != 0) != null)
					{
						HTTPManager.RequestQueue.Sort((HTTPRequest req1, HTTPRequest req2) => req1.Priority - req2.Priority);
					}
					HTTPRequest[] array = HTTPManager.RequestQueue.ToArray();
					HTTPManager.RequestQueue.Clear();
					for (int l = 0; l < array.Length; l++)
					{
						HTTPManager.SendRequest(array[l]);
					}
				}
			}
			if (HTTPManager.heartbeats != null)
			{
				HTTPManager.heartbeats.Update();
			}
		}

		internal static void OnQuit()
		{
			object locker = HTTPManager.Locker;
			lock (locker)
			{
				HTTPCacheService.SaveLibrary();
				foreach (KeyValuePair<string, List<HTTPConnection>> current in HTTPManager.Connections)
				{
					foreach (HTTPConnection current2 in current.Value)
					{
						current2.Dispose();
					}
					current.Value.Clear();
				}
				HTTPManager.Connections.Clear();
			}
		}
	}
}
