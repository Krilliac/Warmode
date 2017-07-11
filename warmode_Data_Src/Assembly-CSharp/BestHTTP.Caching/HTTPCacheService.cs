using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BestHTTP.Caching
{
	public static class HTTPCacheService
	{
		private const int LibraryVersion = 1;

		private static Dictionary<Uri, HTTPCacheFileInfo> library;

		private static bool InClearThread;

		private static bool InMaintainenceThread;

		private static Dictionary<Uri, HTTPCacheFileInfo> Library
		{
			get
			{
				HTTPCacheService.LoadLibrary();
				return HTTPCacheService.library;
			}
		}

		internal static string CacheFolder
		{
			get;
			set;
		}

		private static string LibraryPath
		{
			get;
			set;
		}

		private static string GetFileNameFromUri(Uri uri)
		{
			return Convert.ToBase64String(uri.ToString().GetASCIIBytes()).Replace('/', '-');
		}

		private static Uri GetUriFromFileName(string fileName)
		{
			byte[] bytes = Convert.FromBase64String(fileName.Replace('-', '/'));
			string uriString = bytes.AsciiToString();
			return new Uri(uriString);
		}

		internal static void CheckSetup()
		{
			try
			{
				HTTPCacheService.SetupCacheFolder();
				HTTPCacheService.LoadLibrary();
			}
			catch
			{
			}
		}

		internal static void SetupCacheFolder()
		{
			try
			{
				if (string.IsNullOrEmpty(HTTPCacheService.CacheFolder) || string.IsNullOrEmpty(HTTPCacheService.LibraryPath))
				{
					HTTPCacheService.CacheFolder = Path.Combine(HTTPManager.GetRootCacheFolder(), "HTTPCache");
					if (!Directory.Exists(HTTPCacheService.CacheFolder))
					{
						Directory.CreateDirectory(HTTPCacheService.CacheFolder);
					}
					HTTPCacheService.LibraryPath = Path.Combine(HTTPManager.GetRootCacheFolder(), "Library");
				}
			}
			catch
			{
			}
		}

		internal static bool HasEntity(Uri uri)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			bool result;
			lock (obj)
			{
				result = HTTPCacheService.Library.ContainsKey(uri);
			}
			return result;
		}

		internal static bool DeleteEntity(Uri uri, bool removeFromLibrary = true)
		{
			object obj = HTTPCacheFileLock.Acquire(uri);
			object obj2 = obj;
			bool result;
			lock (obj2)
			{
				try
				{
					Dictionary<Uri, HTTPCacheFileInfo> obj3 = HTTPCacheService.Library;
					lock (obj3)
					{
						HTTPCacheFileInfo hTTPCacheFileInfo;
						bool flag = HTTPCacheService.Library.TryGetValue(uri, out hTTPCacheFileInfo);
						if (flag)
						{
							hTTPCacheFileInfo.Delete();
						}
						if (flag && removeFromLibrary)
						{
							HTTPCacheService.Library.Remove(uri);
						}
						result = true;
					}
				}
				finally
				{
				}
			}
			return result;
		}

		internal static bool IsCachedEntityExpiresInTheFuture(HTTPRequest request)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (HTTPCacheService.Library.TryGetValue(request.CurrentUri, out hTTPCacheFileInfo))
				{
					return hTTPCacheFileInfo.WillExpireInTheFuture();
				}
			}
			return false;
		}

		internal static void SetHeaders(HTTPRequest request)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (HTTPCacheService.Library.TryGetValue(request.CurrentUri, out hTTPCacheFileInfo))
				{
					hTTPCacheFileInfo.SetUpRevalidationHeaders(request);
				}
			}
		}

		internal static Stream GetBody(Uri uri, out int length)
		{
			length = 0;
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (HTTPCacheService.Library.TryGetValue(uri, out hTTPCacheFileInfo))
				{
					return hTTPCacheFileInfo.GetBodyStream(out length);
				}
			}
			return null;
		}

		internal static HTTPResponse GetFullResponse(HTTPRequest request)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (HTTPCacheService.Library.TryGetValue(request.CurrentUri, out hTTPCacheFileInfo))
				{
					return hTTPCacheFileInfo.ReadResponseTo(request);
				}
			}
			return null;
		}

		internal static bool IsCacheble(Uri uri, HTTPMethods method, HTTPResponse response)
		{
			if (method != HTTPMethods.Get)
			{
				return false;
			}
			if (response == null)
			{
				return false;
			}
			if (response.StatusCode == 304)
			{
				return false;
			}
			if (response.StatusCode < 200 || response.StatusCode >= 400)
			{
				return false;
			}
			List<string> headerValues = response.GetHeaderValues("cache-control");
			if (headerValues != null)
			{
				if (headerValues.Exists(delegate(string headerValue)
				{
					string text = headerValue.ToLower();
					return text.Contains("no-store") || text.Contains("no-cache");
				}))
				{
					return false;
				}
			}
			List<string> headerValues2 = response.GetHeaderValues("pragma");
			if (headerValues2 != null)
			{
				if (headerValues2.Exists(delegate(string headerValue)
				{
					string text = headerValue.ToLower();
					return text.Contains("no-store") || text.Contains("no-cache");
				}))
				{
					return false;
				}
			}
			List<string> headerValues3 = response.GetHeaderValues("content-range");
			return headerValues3 == null;
		}

		internal static HTTPCacheFileInfo Store(Uri uri, HTTPMethods method, HTTPResponse response)
		{
			if (response == null || response.Data == null || response.Data.Length == 0)
			{
				return null;
			}
			HTTPCacheFileInfo hTTPCacheFileInfo = null;
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				if (!HTTPCacheService.Library.TryGetValue(uri, out hTTPCacheFileInfo))
				{
					HTTPCacheService.Library.Add(uri, hTTPCacheFileInfo = new HTTPCacheFileInfo(uri));
				}
				try
				{
					hTTPCacheFileInfo.Store(response);
				}
				catch
				{
					HTTPCacheService.DeleteEntity(uri, true);
					throw;
				}
			}
			return hTTPCacheFileInfo;
		}

		internal static Stream PrepareStreamed(Uri uri, HTTPResponse response)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			Stream saveStream;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (!HTTPCacheService.Library.TryGetValue(uri, out hTTPCacheFileInfo))
				{
					HTTPCacheService.Library.Add(uri, hTTPCacheFileInfo = new HTTPCacheFileInfo(uri));
				}
				try
				{
					saveStream = hTTPCacheFileInfo.GetSaveStream(response);
				}
				catch
				{
					HTTPCacheService.DeleteEntity(uri, true);
					throw;
				}
			}
			return saveStream;
		}

		public static void BeginClear()
		{
			if (HTTPCacheService.InClearThread)
			{
				return;
			}
			HTTPCacheService.InClearThread = true;
			HTTPCacheService.SetupCacheFolder();
			new Thread(new ParameterizedThreadStart(HTTPCacheService.ClearImpl)).Start();
		}

		private static void ClearImpl(object param)
		{
			try
			{
				string[] files = Directory.GetFiles(HTTPCacheService.CacheFolder);
				for (int i = 0; i < files.Length; i++)
				{
					try
					{
						string fileName = Path.GetFileName(files[i]);
						HTTPCacheService.DeleteEntity(HTTPCacheService.GetUriFromFileName(fileName), true);
					}
					catch
					{
					}
				}
			}
			finally
			{
				HTTPCacheService.SaveLibrary();
				HTTPCacheService.InClearThread = false;
			}
		}

		public static void BeginMaintainence(HTTPCacheMaintananceParams maintananceParam)
		{
			if (maintananceParam == null)
			{
				throw new ArgumentNullException("maintananceParams == null");
			}
			if (HTTPCacheService.InMaintainenceThread)
			{
				return;
			}
			HTTPCacheService.InMaintainenceThread = true;
			HTTPCacheService.SetupCacheFolder();
			new Thread(delegate(object param)
			{
				try
				{
					Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
					lock (obj)
					{
						DateTime t = DateTime.UtcNow - maintananceParam.DeleteOlder;
						List<Uri> list = new List<Uri>();
						foreach (KeyValuePair<Uri, HTTPCacheFileInfo> current in HTTPCacheService.Library)
						{
							if (current.Value.LastAccess < t && HTTPCacheService.DeleteEntity(current.Key, false))
							{
								list.Add(current.Key);
							}
						}
						for (int i = 0; i < list.Count; i++)
						{
							HTTPCacheService.Library.Remove(list[i]);
						}
						list.Clear();
						ulong num = HTTPCacheService.GetCacheSize();
						if (num > maintananceParam.MaxCacheSize)
						{
							List<HTTPCacheFileInfo> list2 = new List<HTTPCacheFileInfo>(HTTPCacheService.library.Count);
							foreach (KeyValuePair<Uri, HTTPCacheFileInfo> current2 in HTTPCacheService.library)
							{
								list2.Add(current2.Value);
							}
							list2.Sort();
							int num2 = 0;
							while (num >= maintananceParam.MaxCacheSize && num2 < list2.Count)
							{
								try
								{
									HTTPCacheFileInfo hTTPCacheFileInfo = list2[num2];
									ulong num3 = (ulong)((long)hTTPCacheFileInfo.BodyLength);
									HTTPCacheService.DeleteEntity(hTTPCacheFileInfo.Uri, true);
									num -= num3;
								}
								catch
								{
								}
								finally
								{
									num2++;
								}
							}
						}
					}
				}
				finally
				{
					HTTPCacheService.SaveLibrary();
					HTTPCacheService.InMaintainenceThread = false;
				}
			}).Start();
		}

		public static int GetCacheEntityCount()
		{
			HTTPCacheService.CheckSetup();
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			int count;
			lock (obj)
			{
				count = HTTPCacheService.Library.Count;
			}
			return count;
		}

		public static ulong GetCacheSize()
		{
			HTTPCacheService.CheckSetup();
			ulong num = 0uL;
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				foreach (KeyValuePair<Uri, HTTPCacheFileInfo> current in HTTPCacheService.Library)
				{
					if (current.Value.BodyLength > 0)
					{
						num += (ulong)((long)current.Value.BodyLength);
					}
				}
			}
			return num;
		}

		private static void LoadLibrary()
		{
			if (HTTPCacheService.library != null)
			{
				return;
			}
			HTTPCacheService.library = new Dictionary<Uri, HTTPCacheFileInfo>();
			if (!File.Exists(HTTPCacheService.LibraryPath))
			{
				HTTPCacheService.DeleteUnusedFiles();
				return;
			}
			try
			{
				Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.library;
				lock (obj)
				{
					using (FileStream fileStream = new FileStream(HTTPCacheService.LibraryPath, FileMode.Open))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream))
						{
							int version = binaryReader.ReadInt32();
							int num = binaryReader.ReadInt32();
							for (int i = 0; i < num; i++)
							{
								Uri uri = new Uri(binaryReader.ReadString());
								bool flag = File.Exists(Path.Combine(HTTPCacheService.CacheFolder, HTTPCacheService.GetFileNameFromUri(uri)));
								if (flag)
								{
									HTTPCacheService.library.Add(uri, new HTTPCacheFileInfo(uri, binaryReader, version));
								}
							}
						}
					}
				}
				HTTPCacheService.DeleteUnusedFiles();
			}
			catch
			{
			}
		}

		internal static void SaveLibrary()
		{
			if (HTTPCacheService.library == null)
			{
				return;
			}
			try
			{
				Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
				lock (obj)
				{
					using (FileStream fileStream = new FileStream(HTTPCacheService.LibraryPath, FileMode.Create))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							binaryWriter.Write(1);
							binaryWriter.Write(HTTPCacheService.Library.Count);
							foreach (KeyValuePair<Uri, HTTPCacheFileInfo> current in HTTPCacheService.Library)
							{
								binaryWriter.Write(current.Key.ToString());
								current.Value.SaveTo(binaryWriter);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		internal static void SetBodyLength(Uri uri, int bodyLength)
		{
			Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
			lock (obj)
			{
				HTTPCacheFileInfo hTTPCacheFileInfo;
				if (HTTPCacheService.Library.TryGetValue(uri, out hTTPCacheFileInfo))
				{
					hTTPCacheFileInfo.BodyLength = bodyLength;
				}
				else
				{
					HTTPCacheService.Library.Add(uri, hTTPCacheFileInfo = new HTTPCacheFileInfo(uri, DateTime.UtcNow, bodyLength));
				}
			}
		}

		private static void DeleteUnusedFiles()
		{
			HTTPCacheService.CheckSetup();
			string[] files = Directory.GetFiles(HTTPCacheService.CacheFolder);
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					string fileName = Path.GetFileName(files[i]);
					Uri uriFromFileName = HTTPCacheService.GetUriFromFileName(fileName);
					Dictionary<Uri, HTTPCacheFileInfo> obj = HTTPCacheService.Library;
					lock (obj)
					{
						if (!HTTPCacheService.Library.ContainsKey(uriFromFileName))
						{
							File.Delete(files[i]);
						}
					}
				}
				catch
				{
				}
			}
		}
	}
}
