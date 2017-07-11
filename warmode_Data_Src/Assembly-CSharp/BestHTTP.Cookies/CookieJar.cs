using System;
using System.Collections.Generic;
using System.IO;

namespace BestHTTP.Cookies
{
	public static class CookieJar
	{
		private const int Version = 1;

		private static List<Cookie> Cookies = new List<Cookie>();

		private static object Locker = new object();

		private static string CookieFolder
		{
			get;
			set;
		}

		private static string LibraryPath
		{
			get;
			set;
		}

		internal static void SetupFolder()
		{
			try
			{
				if (string.IsNullOrEmpty(CookieJar.CookieFolder) || string.IsNullOrEmpty(CookieJar.LibraryPath))
				{
					CookieJar.CookieFolder = Path.Combine(HTTPManager.GetRootCacheFolder(), "Cookies");
					CookieJar.LibraryPath = Path.Combine(CookieJar.CookieFolder, "Library");
				}
			}
			catch
			{
			}
		}

		internal static void Set(HTTPResponse response)
		{
			if (response == null)
			{
				return;
			}
			object locker = CookieJar.Locker;
			lock (locker)
			{
				try
				{
					CookieJar.Maintain();
					List<Cookie> list = new List<Cookie>();
					List<string> headerValues = response.GetHeaderValues("set-cookie");
					if (headerValues != null)
					{
						foreach (string current in headerValues)
						{
							try
							{
								Cookie cookie = Cookie.Parse(current, response.baseRequest.CurrentUri);
								if (cookie != null)
								{
									int num;
									Cookie cookie2 = CookieJar.Find(cookie, out num);
									if (!string.IsNullOrEmpty(cookie.Value) && cookie.WillExpireInTheFuture())
									{
										if (cookie2 == null)
										{
											CookieJar.Cookies.Add(cookie);
											list.Add(cookie);
										}
										else
										{
											cookie.Date = cookie2.Date;
											CookieJar.Cookies[num] = cookie;
											list.Add(cookie);
										}
									}
									else if (num != -1)
									{
										CookieJar.Cookies.RemoveAt(num);
									}
								}
							}
							catch
							{
							}
						}
						response.Cookies = list;
					}
				}
				catch
				{
				}
			}
		}

		internal static void Maintain()
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				try
				{
					uint num = 0u;
					TimeSpan t = TimeSpan.FromDays(7.0);
					int i = 0;
					while (i < CookieJar.Cookies.Count)
					{
						Cookie cookie = CookieJar.Cookies[i];
						if (!cookie.WillExpireInTheFuture() || cookie.LastAccess + t < DateTime.UtcNow)
						{
							CookieJar.Cookies.RemoveAt(i);
						}
						else
						{
							if (!cookie.IsSession)
							{
								num += cookie.GuessSize();
							}
							i++;
						}
					}
					if (num > HTTPManager.CookieJarSize)
					{
						CookieJar.Cookies.Sort();
						while (num > HTTPManager.CookieJarSize && CookieJar.Cookies.Count > 0)
						{
							Cookie cookie2 = CookieJar.Cookies[0];
							CookieJar.Cookies.RemoveAt(0);
							num -= cookie2.GuessSize();
						}
					}
				}
				catch
				{
				}
			}
		}

		internal static void Persist()
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				try
				{
					CookieJar.Maintain();
					if (!Directory.Exists(CookieJar.CookieFolder))
					{
						Directory.CreateDirectory(CookieJar.CookieFolder);
					}
					using (FileStream fileStream = new FileStream(CookieJar.LibraryPath, FileMode.Create))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							binaryWriter.Write(1);
							int num = 0;
							foreach (Cookie current in CookieJar.Cookies)
							{
								if (!current.IsSession)
								{
									num++;
								}
							}
							binaryWriter.Write(num);
							foreach (Cookie current2 in CookieJar.Cookies)
							{
								if (!current2.IsSession)
								{
									current2.SaveTo(binaryWriter);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		internal static void Load()
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				try
				{
					CookieJar.Cookies.Clear();
					if (!Directory.Exists(CookieJar.CookieFolder))
					{
						Directory.CreateDirectory(CookieJar.CookieFolder);
					}
					if (File.Exists(CookieJar.LibraryPath))
					{
						using (FileStream fileStream = new FileStream(CookieJar.LibraryPath, FileMode.Open))
						{
							using (BinaryReader binaryReader = new BinaryReader(fileStream))
							{
								binaryReader.ReadInt32();
								int num = binaryReader.ReadInt32();
								for (int i = 0; i < num; i++)
								{
									Cookie cookie = new Cookie();
									cookie.LoadFrom(binaryReader);
									if (cookie.WillExpireInTheFuture())
									{
										CookieJar.Cookies.Add(cookie);
									}
								}
							}
						}
					}
				}
				catch
				{
					CookieJar.Cookies.Clear();
				}
			}
		}

		public static List<Cookie> Get(Uri uri)
		{
			object locker = CookieJar.Locker;
			List<Cookie> result;
			lock (locker)
			{
				List<Cookie> list = new List<Cookie>();
				for (int i = 0; i < CookieJar.Cookies.Count; i++)
				{
					Cookie cookie = CookieJar.Cookies[i];
					if (cookie.WillExpireInTheFuture() && uri.Host.IndexOf(cookie.Domain) != -1 && uri.AbsolutePath.StartsWith(cookie.Path))
					{
						list.Add(cookie);
					}
				}
				result = list;
			}
			return result;
		}

		public static List<Cookie> GetAll()
		{
			return CookieJar.Cookies;
		}

		public static void Clear()
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				CookieJar.Cookies.Clear();
			}
		}

		public static void Clear(TimeSpan olderThan)
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				int i = 0;
				while (i < CookieJar.Cookies.Count)
				{
					Cookie cookie = CookieJar.Cookies[i];
					if (!cookie.WillExpireInTheFuture() || cookie.Date + olderThan < DateTime.UtcNow)
					{
						CookieJar.Cookies.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		public static void Clear(string domain)
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				int i = 0;
				while (i < CookieJar.Cookies.Count)
				{
					Cookie cookie = CookieJar.Cookies[i];
					if (!cookie.WillExpireInTheFuture() || cookie.Domain.IndexOf(domain) != -1)
					{
						CookieJar.Cookies.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		public static void Remove(Uri uri, string name)
		{
			object locker = CookieJar.Locker;
			lock (locker)
			{
				int i = 0;
				while (i < CookieJar.Cookies.Count)
				{
					Cookie cookie = CookieJar.Cookies[i];
					if (cookie.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && uri.Host.IndexOf(cookie.Domain) != -1)
					{
						CookieJar.Cookies.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		private static Cookie Find(Cookie cookie, out int idx)
		{
			for (int i = 0; i < CookieJar.Cookies.Count; i++)
			{
				Cookie cookie2 = CookieJar.Cookies[i];
				if (cookie2.Equals(cookie))
				{
					idx = i;
					return cookie2;
				}
			}
			idx = -1;
			return null;
		}
	}
}
