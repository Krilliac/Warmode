using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace BestHTTP.Cookies
{
	public sealed class Cookie : IComparable<Cookie>
	{
		private const int Version = 1;

		public string Name
		{
			get;
			private set;
		}

		public string Value
		{
			get;
			private set;
		}

		public DateTime Date
		{
			get;
			internal set;
		}

		public DateTime LastAccess
		{
			get;
			set;
		}

		public DateTime Expires
		{
			get;
			private set;
		}

		public long MaxAge
		{
			get;
			private set;
		}

		public bool IsSession
		{
			get;
			private set;
		}

		public string Domain
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			private set;
		}

		public bool IsSecure
		{
			get;
			private set;
		}

		public bool IsHttpOnly
		{
			get;
			private set;
		}

		public Cookie(string name, string value) : this(name, value, string.Empty, string.Empty)
		{
		}

		public Cookie(string name, string value, string path) : this(name, value, path, string.Empty)
		{
		}

		public Cookie(string name, string value, string path, string domain)
		{
			this.Name = name;
			this.Value = value;
			this.Path = path;
			this.Domain = domain;
		}

		internal Cookie()
		{
			this.IsSession = true;
			this.MaxAge = -1L;
		}

		public bool WillExpireInTheFuture()
		{
			return this.IsSession || ((this.MaxAge == -1L) ? (this.Expires > DateTime.UtcNow) : (Math.Max(0L, (long)(DateTime.UtcNow - this.Date).TotalSeconds) < this.MaxAge));
		}

		public uint GuessSize()
		{
			return (uint)(((this.Name == null) ? 0 : (this.Name.Length * 2)) + ((this.Value == null) ? 0 : (this.Value.Length * 2)) + ((this.Domain == null) ? 0 : (this.Domain.Length * 2)) + ((this.Path == null) ? 0 : (this.Path.Length * 2)) + 32 + 3);
		}

		public static Cookie Parse(string header, Uri defaultDomain)
		{
			Cookie cookie = new Cookie();
			try
			{
				List<KeyValuePair> list = Cookie.ParseCookieHeader(header);
				foreach (KeyValuePair current in list)
				{
					string text = current.Key.ToLowerInvariant();
					switch (text)
					{
					case "path":
					{
						Cookie arg_115_0 = cookie;
						string arg_115_1;
						if (string.IsNullOrEmpty(current.Value) || !current.Value.StartsWith("/"))
						{
							arg_115_1 = "/";
						}
						else
						{
							string value = current.Value;
							cookie.Path = value;
							arg_115_1 = value;
						}
						arg_115_0.Path = arg_115_1;
						continue;
					}
					case "domain":
						if (string.IsNullOrEmpty(current.Value))
						{
							return null;
						}
						cookie.Domain = ((!current.Value.StartsWith(".")) ? current.Value : current.Value.Substring(1));
						continue;
					case "expires":
						cookie.Expires = current.Value.ToDateTime(DateTime.FromBinary(0L));
						cookie.IsSession = false;
						continue;
					case "max-age":
						cookie.MaxAge = current.Value.ToInt64(-1L);
						cookie.IsSession = false;
						continue;
					case "secure":
						cookie.IsSecure = true;
						continue;
					case "httponly":
						cookie.IsHttpOnly = true;
						continue;
					}
					cookie.Name = current.Key;
					cookie.Value = current.Value;
				}
				if (HTTPManager.EnablePrivateBrowsing)
				{
					cookie.IsSession = true;
				}
				if (string.IsNullOrEmpty(cookie.Domain))
				{
					cookie.Domain = defaultDomain.Host;
				}
				if (string.IsNullOrEmpty(cookie.Path))
				{
					cookie.Path = defaultDomain.AbsolutePath;
				}
				Cookie arg_25E_0 = cookie;
				DateTime utcNow = DateTime.UtcNow;
				cookie.LastAccess = utcNow;
				arg_25E_0.Date = utcNow;
			}
			catch
			{
			}
			return cookie;
		}

		internal void SaveTo(BinaryWriter stream)
		{
			stream.Write(1);
			stream.Write(this.Name ?? string.Empty);
			stream.Write(this.Value ?? string.Empty);
			stream.Write(this.Date.ToBinary());
			stream.Write(this.LastAccess.ToBinary());
			stream.Write(this.Expires.ToBinary());
			stream.Write(this.MaxAge);
			stream.Write(this.IsSession);
			stream.Write(this.Domain ?? string.Empty);
			stream.Write(this.Path ?? string.Empty);
			stream.Write(this.IsSecure);
			stream.Write(this.IsHttpOnly);
		}

		internal void LoadFrom(BinaryReader stream)
		{
			stream.ReadInt32();
			this.Name = stream.ReadString();
			this.Value = stream.ReadString();
			this.Date = DateTime.FromBinary(stream.ReadInt64());
			this.LastAccess = DateTime.FromBinary(stream.ReadInt64());
			this.Expires = DateTime.FromBinary(stream.ReadInt64());
			this.MaxAge = stream.ReadInt64();
			this.IsSession = stream.ReadBoolean();
			this.Domain = stream.ReadString();
			this.Path = stream.ReadString();
			this.IsSecure = stream.ReadBoolean();
			this.IsHttpOnly = stream.ReadBoolean();
		}

		public override string ToString()
		{
			return this.Name + "=" + this.Value;
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.Equals(obj as Cookie);
		}

		public bool Equals(Cookie cookie)
		{
			return cookie != null && (object.ReferenceEquals(this, cookie) || (this.Name == cookie.Name && this.Domain == cookie.Domain && this.Path == cookie.Path));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static string ReadValue(string str, ref int pos)
		{
			string empty = string.Empty;
			if (str == null)
			{
				return empty;
			}
			return str.Read(ref pos, ';', true);
		}

		private static List<KeyValuePair> ParseCookieHeader(string str)
		{
			List<KeyValuePair> list = new List<KeyValuePair>();
			if (str == null)
			{
				return list;
			}
			int i = 0;
			while (i < str.Length)
			{
				string key = str.Read(ref i, (char ch) => ch != '=' && ch != ';', true).Trim();
				KeyValuePair keyValuePair = new KeyValuePair(key);
				if (i < str.Length && str[i - 1] == '=')
				{
					keyValuePair.Value = Cookie.ReadValue(str, ref i);
				}
				list.Add(keyValuePair);
			}
			return list;
		}

		public int CompareTo(Cookie other)
		{
			return this.LastAccess.CompareTo(other.LastAccess);
		}
	}
}
