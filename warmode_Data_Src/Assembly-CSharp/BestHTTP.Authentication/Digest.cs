using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.Authentication
{
	internal sealed class Digest
	{
		public Uri Uri
		{
			get;
			private set;
		}

		public AuthenticationTypes Type
		{
			get;
			private set;
		}

		public string Realm
		{
			get;
			private set;
		}

		public bool Stale
		{
			get;
			private set;
		}

		private string Nonce
		{
			get;
			set;
		}

		private string Opaque
		{
			get;
			set;
		}

		private string Algorithm
		{
			get;
			set;
		}

		public List<string> ProtectedUris
		{
			get;
			private set;
		}

		private string QualityOfProtections
		{
			get;
			set;
		}

		private int NonceCount
		{
			get;
			set;
		}

		private string HA1Sess
		{
			get;
			set;
		}

		internal Digest(Uri uri)
		{
			this.Uri = uri;
			this.Algorithm = "md5";
		}

		public void ParseChallange(string header)
		{
			this.Type = AuthenticationTypes.Unknown;
			this.Stale = false;
			this.Opaque = null;
			this.HA1Sess = null;
			this.NonceCount = 0;
			this.QualityOfProtections = null;
			if (this.ProtectedUris != null)
			{
				this.ProtectedUris.Clear();
			}
			WWWAuthenticateHeaderParser wWWAuthenticateHeaderParser = new WWWAuthenticateHeaderParser(header);
			foreach (KeyValuePair current in wWWAuthenticateHeaderParser.Values)
			{
				string key = current.Key;
				switch (key)
				{
				case "basic":
					this.Type = AuthenticationTypes.Basic;
					break;
				case "digest":
					this.Type = AuthenticationTypes.Digest;
					break;
				case "realm":
					this.Realm = current.Value;
					break;
				case "domain":
					if (!string.IsNullOrEmpty(current.Value) && current.Value.Length != 0)
					{
						if (this.ProtectedUris == null)
						{
							this.ProtectedUris = new List<string>();
						}
						int num2 = 0;
						string item = current.Value.Read(ref num2, ' ', true);
						do
						{
							this.ProtectedUris.Add(item);
							item = current.Value.Read(ref num2, ' ', true);
						}
						while (num2 < current.Value.Length);
					}
					break;
				case "nonce":
					this.Nonce = current.Value;
					break;
				case "qop":
					this.QualityOfProtections = current.Value;
					break;
				case "stale":
					this.Stale = bool.Parse(current.Value);
					break;
				case "opaque":
					this.Opaque = current.Value;
					break;
				case "algorithm":
					this.Algorithm = current.Value;
					break;
				}
			}
		}

		public string GenerateResponseHeader(HTTPRequest request, Credentials credentials)
		{
			try
			{
				AuthenticationTypes type = this.Type;
				if (type == AuthenticationTypes.Basic)
				{
					string result = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", credentials.UserName, credentials.Password)));
					return result;
				}
				if (type == AuthenticationTypes.Digest)
				{
					this.NonceCount++;
					string text = string.Empty;
					string text2 = new Random(request.GetHashCode()).Next(-2147483648, 2147483647).ToString("X8");
					string text3 = this.NonceCount.ToString("X8");
					string text4 = this.Algorithm.TrimAndLower();
					string result;
					if (text4 != null)
					{
						if (Digest.<>f__switch$map1 == null)
						{
							Digest.<>f__switch$map1 = new Dictionary<string, int>(2)
							{
								{
									"md5",
									0
								},
								{
									"md5-sess",
									1
								}
							};
						}
						int num;
						if (Digest.<>f__switch$map1.TryGetValue(text4, out num))
						{
							if (num != 0)
							{
								if (num != 1)
								{
									goto IL_199;
								}
								if (string.IsNullOrEmpty(this.HA1Sess))
								{
									this.HA1Sess = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										credentials.UserName,
										this.Realm,
										credentials.Password,
										this.Nonce,
										text3
									}).CalculateMD5Hash();
								}
								text = this.HA1Sess;
							}
							else
							{
								text = string.Format("{0}:{1}:{2}", credentials.UserName, this.Realm, credentials.Password).CalculateMD5Hash();
							}
							string text5 = string.Empty;
							string text6 = (this.QualityOfProtections == null) ? null : this.QualityOfProtections.TrimAndLower();
							if (text6 == null)
							{
								string arg = (request.MethodType.ToString().ToUpper() + ":" + request.CurrentUri.PathAndQuery).CalculateMD5Hash();
								text5 = string.Format("{0}:{1}:{2}", text, this.Nonce, arg).CalculateMD5Hash();
							}
							else if (text6.Contains("auth-int"))
							{
								text6 = "auth-int";
								byte[] array = request.GetEntityBody();
								if (array == null)
								{
									array = string.Empty.GetASCIIBytes();
								}
								string text7 = string.Format("{0}:{1}:{2}", request.MethodType.ToString().ToUpper(), request.CurrentUri.PathAndQuery, array.CalculateMD5Hash()).CalculateMD5Hash();
								text5 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									text,
									this.Nonce,
									text3,
									text2,
									text6,
									text7
								}).CalculateMD5Hash();
							}
							else
							{
								if (!text6.Contains("auth"))
								{
									result = string.Empty;
									return result;
								}
								text6 = "auth";
								string text8 = (request.MethodType.ToString().ToUpper() + ":" + request.CurrentUri.PathAndQuery).CalculateMD5Hash();
								text5 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									text,
									this.Nonce,
									text3,
									text2,
									text6,
									text8
								}).CalculateMD5Hash();
							}
							string text9 = string.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", cnonce=\"{4}\", response=\"{5}\"", new object[]
							{
								credentials.UserName,
								this.Realm,
								this.Nonce,
								request.Uri.PathAndQuery,
								text2,
								text5
							});
							if (text6 != null)
							{
								text9 += ", qop=\"" + text6 + "\", nc=" + text3;
							}
							if (!string.IsNullOrEmpty(this.Opaque))
							{
								text9 = text9 + ", opaque=\"" + this.Opaque + "\"";
							}
							result = text9;
							return result;
						}
					}
					IL_199:
					result = string.Empty;
					return result;
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		public bool IsUriProtected(Uri uri)
		{
			if (string.CompareOrdinal(uri.Host, this.Uri.Host) != 0)
			{
				return false;
			}
			string text = uri.ToString();
			if (this.ProtectedUris != null && this.ProtectedUris.Count > 0)
			{
				for (int i = 0; i < this.ProtectedUris.Count; i++)
				{
					if (text.Contains(this.ProtectedUris[i]))
					{
						return true;
					}
				}
			}
			return true;
		}
	}
}
