using BestHTTP.Authentication;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using BestHTTP.Forms;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace BestHTTP
{
	public sealed class HTTPRequest : IEnumerator
	{
		internal static readonly byte[] EOL = new byte[]
		{
			13,
			10
		};

		public static int UploadChunkSize = 1024;

		public OnUploadProgressDelegate OnUploadProgress;

		public OnDownloadProgressDelegate OnProgress;

		public OnRequestFinishedDelegate OnUpgraded;

		private List<Cookie> customCookies;

		private OnBeforeRedirectionDelegate onBeforeRedirection;

		private bool isKeepAlive;

		private bool disableCache;

		private int streamFragmentSize;

		private bool useStreaming;

		private HTTPFormBase FieldCollector;

		private HTTPFormBase FormImpl;

		public event Func<HTTPRequest, X509Certificate, X509Chain, bool> CustomCertificationValidator
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.CustomCertificationValidator = (Func<HTTPRequest, X509Certificate, X509Chain, bool>)Delegate.Combine(this.CustomCertificationValidator, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.CustomCertificationValidator = (Func<HTTPRequest, X509Certificate, X509Chain, bool>)Delegate.Remove(this.CustomCertificationValidator, value);
			}
		}

		public event OnBeforeRedirectionDelegate OnBeforeRedirection
		{
			add
			{
				this.onBeforeRedirection = (OnBeforeRedirectionDelegate)Delegate.Combine(this.onBeforeRedirection, value);
			}
			remove
			{
				this.onBeforeRedirection = (OnBeforeRedirectionDelegate)Delegate.Remove(this.onBeforeRedirection, value);
			}
		}

		public Uri Uri
		{
			get;
			private set;
		}

		public HTTPMethods MethodType
		{
			get;
			set;
		}

		public byte[] RawData
		{
			get;
			set;
		}

		public Stream UploadStream
		{
			get;
			set;
		}

		public bool DisposeUploadStream
		{
			get;
			set;
		}

		public bool UseUploadStreamLength
		{
			get;
			set;
		}

		public bool IsKeepAlive
		{
			get
			{
				return this.isKeepAlive;
			}
			set
			{
				if (this.State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the IsKeepAlive property while processing the request is not supported.");
				}
				this.isKeepAlive = value;
			}
		}

		public bool DisableCache
		{
			get
			{
				return this.disableCache;
			}
			set
			{
				if (this.State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the DisableCache property while processing the request is not supported.");
				}
				this.disableCache = value;
			}
		}

		public bool UseStreaming
		{
			get
			{
				return this.useStreaming;
			}
			set
			{
				if (this.State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the UseStreaming property while processing the request is not supported.");
				}
				this.useStreaming = value;
			}
		}

		public int StreamFragmentSize
		{
			get
			{
				return this.streamFragmentSize;
			}
			set
			{
				if (this.State == HTTPRequestStates.Processing)
				{
					throw new NotSupportedException("Changing the StreamFragmentSize property while processing the request is not supported.");
				}
				if (value < 1)
				{
					throw new ArgumentException("StreamFragmentSize must be at least 1.");
				}
				this.streamFragmentSize = value;
			}
		}

		public OnRequestFinishedDelegate Callback
		{
			get;
			set;
		}

		public bool DisableRetry
		{
			get;
			set;
		}

		public bool IsRedirected
		{
			get;
			internal set;
		}

		public Uri RedirectUri
		{
			get;
			internal set;
		}

		public Uri CurrentUri
		{
			get
			{
				return (!this.IsRedirected) ? this.Uri : this.RedirectUri;
			}
		}

		public HTTPResponse Response
		{
			get;
			internal set;
		}

		public HTTPResponse ProxyResponse
		{
			get;
			internal set;
		}

		public Exception Exception
		{
			get;
			internal set;
		}

		public object Tag
		{
			get;
			set;
		}

		public Credentials Credentials
		{
			get;
			set;
		}

		public bool HasProxy
		{
			get
			{
				return this.Proxy != null;
			}
		}

		public HTTPProxy Proxy
		{
			get;
			set;
		}

		public int MaxRedirects
		{
			get;
			set;
		}

		public bool UseAlternateSSL
		{
			get;
			set;
		}

		public bool IsCookiesEnabled
		{
			get;
			set;
		}

		public List<Cookie> Cookies
		{
			get
			{
				if (this.customCookies == null)
				{
					this.customCookies = new List<Cookie>();
				}
				return this.customCookies;
			}
			set
			{
				this.customCookies = value;
			}
		}

		public HTTPFormUsage FormUsage
		{
			get;
			set;
		}

		public HTTPRequestStates State
		{
			get;
			internal set;
		}

		public int RedirectCount
		{
			get;
			internal set;
		}

		public TimeSpan ConnectTimeout
		{
			get;
			set;
		}

		public TimeSpan Timeout
		{
			get;
			set;
		}

		public bool EnableTimoutForStreaming
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public ICertificateVerifyer CustomCertificateVerifyer
		{
			get;
			set;
		}

		internal int Downloaded
		{
			get;
			set;
		}

		internal int DownloadLength
		{
			get;
			set;
		}

		internal bool DownloadProgressChanged
		{
			get;
			set;
		}

		internal long UploadStreamLength
		{
			get
			{
				if (this.UploadStream == null || !this.UseUploadStreamLength)
				{
					return -1L;
				}
				long result;
				try
				{
					result = this.UploadStream.Length;
				}
				catch
				{
					result = -1L;
				}
				return result;
			}
		}

		internal long Uploaded
		{
			get;
			private set;
		}

		internal long UploadLength
		{
			get;
			private set;
		}

		internal bool UploadProgressChanged
		{
			get;
			set;
		}

		private Dictionary<string, List<string>> Headers
		{
			get;
			set;
		}

		public object Current
		{
			get
			{
				return null;
			}
		}

		public HTTPRequest(Uri uri) : this(uri, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, HTTPManager.IsCachingDisabled, null)
		{
		}

		public HTTPRequest(Uri uri, OnRequestFinishedDelegate callback) : this(uri, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, HTTPManager.IsCachingDisabled, callback)
		{
		}

		public HTTPRequest(Uri uri, bool isKeepAlive, OnRequestFinishedDelegate callback) : this(uri, HTTPMethods.Get, isKeepAlive, HTTPManager.IsCachingDisabled, callback)
		{
		}

		public HTTPRequest(Uri uri, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback) : this(uri, HTTPMethods.Get, isKeepAlive, disableCache, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType) : this(uri, methodType, HTTPManager.KeepAliveDefaultValue, HTTPManager.IsCachingDisabled || methodType != HTTPMethods.Get, null)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, OnRequestFinishedDelegate callback) : this(uri, methodType, HTTPManager.KeepAliveDefaultValue, HTTPManager.IsCachingDisabled || methodType != HTTPMethods.Get, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, bool isKeepAlive, OnRequestFinishedDelegate callback) : this(uri, methodType, isKeepAlive, HTTPManager.IsCachingDisabled || methodType != HTTPMethods.Get, callback)
		{
		}

		public HTTPRequest(Uri uri, HTTPMethods methodType, bool isKeepAlive, bool disableCache, OnRequestFinishedDelegate callback)
		{
			this.Uri = uri;
			this.MethodType = methodType;
			this.IsKeepAlive = isKeepAlive;
			this.DisableCache = disableCache;
			this.Callback = callback;
			this.StreamFragmentSize = 4096;
			this.DisableRetry = (methodType == HTTPMethods.Post);
			this.MaxRedirects = 2147483647;
			this.RedirectCount = 0;
			this.IsCookiesEnabled = HTTPManager.IsCookiesEnabled;
			int num = 0;
			this.DownloadLength = num;
			this.Downloaded = num;
			this.DownloadProgressChanged = false;
			this.State = HTTPRequestStates.Initial;
			this.ConnectTimeout = HTTPManager.ConnectTimeout;
			this.Timeout = HTTPManager.RequestTimeout;
			this.EnableTimoutForStreaming = false;
			this.Proxy = HTTPManager.Proxy;
			this.UseUploadStreamLength = true;
			this.DisposeUploadStream = true;
			this.CustomCertificateVerifyer = HTTPManager.DefaultCertificateVerifyer;
		}

		public void AddField(string fieldName, string value)
		{
			this.AddField(fieldName, value, Encoding.UTF8);
		}

		public void AddField(string fieldName, string value, Encoding e)
		{
			if (this.FieldCollector == null)
			{
				this.FieldCollector = new HTTPFormBase();
			}
			this.FieldCollector.AddField(fieldName, value, e);
		}

		public void AddBinaryData(string fieldName, byte[] content)
		{
			this.AddBinaryData(fieldName, content, null, null);
		}

		public void AddBinaryData(string fieldName, byte[] content, string fileName)
		{
			this.AddBinaryData(fieldName, content, fileName, null);
		}

		public void AddBinaryData(string fieldName, byte[] content, string fileName, string mimeType)
		{
			if (this.FieldCollector == null)
			{
				this.FieldCollector = new HTTPFormBase();
			}
			this.FieldCollector.AddBinaryData(fieldName, content, fileName, mimeType);
		}

		public void SetFields(WWWForm wwwForm)
		{
			this.FormUsage = HTTPFormUsage.Unity;
			this.FormImpl = new UnityForm(wwwForm);
		}

		public void SetForm(HTTPFormBase form)
		{
			this.FormImpl = form;
		}

		public void ClearForm()
		{
			this.FormImpl = null;
			this.FieldCollector = null;
		}

		private HTTPFormBase SelectFormImplementation()
		{
			if (this.FormImpl != null)
			{
				return this.FormImpl;
			}
			if (this.FieldCollector == null)
			{
				return null;
			}
			switch (this.FormUsage)
			{
			case HTTPFormUsage.Automatic:
				if (this.FieldCollector.HasBinary || this.FieldCollector.HasLongValue)
				{
					goto IL_7B;
				}
				break;
			case HTTPFormUsage.UrlEncoded:
				break;
			case HTTPFormUsage.Multipart:
				goto IL_7B;
			case HTTPFormUsage.Unity:
				this.FormImpl = new UnityForm();
				goto IL_9B;
			default:
				goto IL_9B;
			}
			this.FormImpl = new HTTPUrlEncodedForm();
			goto IL_9B;
			IL_7B:
			this.FormImpl = new HTTPMultiPartForm();
			IL_9B:
			this.FormImpl.CopyFrom(this.FieldCollector);
			return this.FormImpl;
		}

		public void AddHeader(string name, string value)
		{
			if (this.Headers == null)
			{
				this.Headers = new Dictionary<string, List<string>>();
			}
			List<string> list;
			if (!this.Headers.TryGetValue(name, out list))
			{
				this.Headers.Add(name, list = new List<string>(1));
			}
			list.Add(value);
		}

		public void SetHeader(string name, string value)
		{
			if (this.Headers == null)
			{
				this.Headers = new Dictionary<string, List<string>>();
			}
			List<string> list;
			if (!this.Headers.TryGetValue(name, out list))
			{
				this.Headers.Add(name, list = new List<string>(1));
			}
			list.Clear();
			list.Add(value);
		}

		public bool RemoveHeader(string name)
		{
			return this.Headers != null && this.Headers.Remove(name);
		}

		public bool HasHeader(string name)
		{
			return this.Headers != null && this.Headers.ContainsKey(name);
		}

		public string GetFirstHeaderValue(string name)
		{
			if (this.Headers == null)
			{
				return null;
			}
			List<string> list = null;
			if (this.Headers.TryGetValue(name, out list) && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		public List<string> GetHeaderValues(string name)
		{
			if (this.Headers == null)
			{
				return null;
			}
			List<string> list = null;
			if (this.Headers.TryGetValue(name, out list) && list.Count > 0)
			{
				return list;
			}
			return null;
		}

		public void RemoveHeaders()
		{
			if (this.Headers == null)
			{
				return;
			}
			this.Headers.Clear();
		}

		public void SetRangeHeader(int firstBytePos)
		{
			this.SetHeader("Range", string.Format("bytes={0}-", firstBytePos));
		}

		public void SetRangeHeader(int firstBytePos, int lastBytePos)
		{
			this.SetHeader("Range", string.Format("bytes={0}-{1}", firstBytePos, lastBytePos));
		}

		private void SendHeaders(BinaryWriter stream)
		{
			if (!this.HasHeader("Host"))
			{
				this.SetHeader("Host", this.CurrentUri.Authority);
			}
			if (this.IsRedirected && !this.HasHeader("Referer"))
			{
				this.AddHeader("Referer", this.Uri.ToString());
			}
			if (!this.HasHeader("Accept-Encoding"))
			{
				this.AddHeader("Accept-Encoding", "gzip, identity");
			}
			if (this.HasProxy && !this.HasHeader("Proxy-Connection"))
			{
				this.AddHeader("Proxy-Connection", (!this.IsKeepAlive) ? "Close" : "Keep-Alive");
			}
			if (!this.HasHeader("Connection"))
			{
				this.AddHeader("Connection", (!this.IsKeepAlive) ? "Close, TE" : "Keep-Alive, TE");
			}
			if (!this.HasHeader("TE"))
			{
				this.AddHeader("TE", "identity");
			}
			if (!this.HasHeader("User-Agent"))
			{
				this.AddHeader("User-Agent", "BestHTTP");
			}
			long num;
			if (this.UploadStream == null)
			{
				byte[] entityBody = this.GetEntityBody();
				num = (long)((entityBody == null) ? 0 : entityBody.Length);
				if (this.RawData == null && (this.FormImpl != null || (this.FieldCollector != null && !this.FieldCollector.IsEmpty)))
				{
					this.SelectFormImplementation();
					if (this.FormImpl != null)
					{
						this.FormImpl.PrepareRequest(this);
					}
				}
			}
			else
			{
				num = this.UploadStreamLength;
				if (num == -1L)
				{
					this.SetHeader("Transfer-Encoding", "Chunked");
				}
				if (!this.HasHeader("Content-Type"))
				{
					this.SetHeader("Content-Type", "application/octet-stream");
				}
			}
			if (num != -1L && !this.HasHeader("Content-Length"))
			{
				this.SetHeader("Content-Length", num.ToString());
			}
			if (this.HasProxy && this.Proxy.Credentials != null)
			{
				switch (this.Proxy.Credentials.Type)
				{
				case AuthenticationTypes.Unknown:
				case AuthenticationTypes.Digest:
				{
					Digest digest = DigestStore.Get(this.Proxy.Address);
					if (digest != null)
					{
						string value = digest.GenerateResponseHeader(this, this.Proxy.Credentials);
						if (!string.IsNullOrEmpty(value))
						{
							this.SetHeader("Proxy-Authorization", value);
						}
					}
					break;
				}
				case AuthenticationTypes.Basic:
					this.SetHeader("Proxy-Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Credentials.UserName + ":" + this.Credentials.Password)));
					break;
				}
			}
			if (this.Credentials != null)
			{
				switch (this.Credentials.Type)
				{
				case AuthenticationTypes.Unknown:
				case AuthenticationTypes.Digest:
				{
					Digest digest2 = DigestStore.Get(this.CurrentUri);
					if (digest2 != null)
					{
						string value2 = digest2.GenerateResponseHeader(this, this.Credentials);
						if (!string.IsNullOrEmpty(value2))
						{
							this.SetHeader("Authorization", value2);
						}
					}
					break;
				}
				case AuthenticationTypes.Basic:
					this.SetHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Credentials.UserName + ":" + this.Credentials.Password)));
					break;
				}
			}
			List<Cookie> list = (!this.IsCookiesEnabled) ? null : CookieJar.Get(this.CurrentUri);
			if (list == null)
			{
				list = this.customCookies;
			}
			else if (this.customCookies != null)
			{
				list.AddRange(this.customCookies);
			}
			if (list != null && list.Count > 0)
			{
				bool flag = true;
				string text = string.Empty;
				bool flag2 = HTTPProtocolFactory.IsSecureProtocol(this.CurrentUri);
				SupportedProtocols protocolFromUri = HTTPProtocolFactory.GetProtocolFromUri(this.CurrentUri);
				foreach (Cookie current in list)
				{
					if ((!current.IsSecure || (current.IsSecure && flag2)) && (!current.IsHttpOnly || (current.IsHttpOnly && protocolFromUri == SupportedProtocols.HTTP)))
					{
						if (!flag)
						{
							text += "; ";
						}
						else
						{
							flag = false;
						}
						text += current.ToString();
						current.LastAccess = DateTime.UtcNow;
					}
				}
				this.SetHeader("Cookie", text);
			}
			foreach (KeyValuePair<string, List<string>> current2 in this.Headers)
			{
				byte[] aSCIIBytes = (current2.Key + ": ").GetASCIIBytes();
				for (int i = 0; i < current2.Value.Count; i++)
				{
					stream.Write(aSCIIBytes);
					stream.Write(current2.Value[i].GetASCIIBytes());
					stream.Write(HTTPRequest.EOL);
				}
			}
		}

		public string DumpHeaders()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.SendHeaders(binaryWriter);
					result = memoryStream.ToArray().AsciiToString();
				}
			}
			return result;
		}

		internal byte[] GetEntityBody()
		{
			if (this.RawData != null)
			{
				return this.RawData;
			}
			if (this.FormImpl != null || (this.FieldCollector != null && !this.FieldCollector.IsEmpty))
			{
				this.SelectFormImplementation();
				if (this.FormImpl != null)
				{
					return this.FormImpl.GetData();
				}
			}
			return null;
		}

		internal bool SendOutTo(Stream stream)
		{
			bool result = false;
			try
			{
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				string arg = (!this.HasProxy || !this.Proxy.SendWholeUri) ? this.CurrentUri.PathAndQuery : this.CurrentUri.OriginalString;
				HTTPManager.Logger.Information("HTTPConnection", string.Format("Sending {0} request to {1}", this.MethodType.ToString().ToUpper(), arg));
				binaryWriter.Write(string.Format("{0} {1} HTTP/1.1", this.MethodType.ToString().ToUpper(), arg).GetASCIIBytes());
				binaryWriter.Write(HTTPRequest.EOL);
				this.SendHeaders(binaryWriter);
				binaryWriter.Write(HTTPRequest.EOL);
				binaryWriter.Flush();
				byte[] array = this.RawData;
				if (array == null && this.FormImpl != null)
				{
					array = this.FormImpl.GetData();
				}
				if (array != null || this.UploadStream != null)
				{
					Stream stream2 = this.UploadStream;
					if (stream2 == null)
					{
						stream2 = new MemoryStream(array, 0, array.Length);
						this.UploadLength = (long)array.Length;
					}
					else
					{
						this.UploadLength = ((!this.UseUploadStreamLength) ? -1L : this.UploadStreamLength);
					}
					this.Uploaded = 0L;
					byte[] array2 = new byte[HTTPRequest.UploadChunkSize];
					int num;
					while ((num = stream2.Read(array2, 0, array2.Length)) > 0)
					{
						if (!this.UseUploadStreamLength)
						{
							binaryWriter.Write(num.ToString("X").GetASCIIBytes());
							binaryWriter.Write(HTTPRequest.EOL);
						}
						binaryWriter.Write(array2, 0, num);
						if (!this.UseUploadStreamLength)
						{
							binaryWriter.Write(HTTPRequest.EOL);
						}
						binaryWriter.Flush();
						this.Uploaded += (long)num;
						this.UploadProgressChanged = true;
					}
					if (!this.UseUploadStreamLength)
					{
						binaryWriter.Write("0".GetASCIIBytes());
						binaryWriter.Write(HTTPRequest.EOL);
						binaryWriter.Write(HTTPRequest.EOL);
					}
					binaryWriter.Flush();
					if (this.UploadStream == null && stream2 != null)
					{
						stream2.Dispose();
					}
				}
				result = true;
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPRequest", "SendOutTo", ex);
			}
			finally
			{
				if (this.UploadStream != null && this.DisposeUploadStream)
				{
					this.UploadStream.Dispose();
				}
			}
			return result;
		}

		internal void UpgradeCallback()
		{
			if (this.Response == null || !this.Response.IsUpgraded)
			{
				return;
			}
			try
			{
				if (this.OnUpgraded != null)
				{
					this.OnUpgraded(this, this.Response);
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPRequest", "UpgradeCallback", ex);
			}
		}

		internal void CallCallback()
		{
			try
			{
				if (this.Callback != null)
				{
					this.Callback(this, this.Response);
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPRequest", "CallCallback", ex);
			}
		}

		internal bool CallOnBeforeRedirection(Uri redirectUri)
		{
			return this.onBeforeRedirection == null || this.onBeforeRedirection(this, this.Response, redirectUri);
		}

		internal void FinishStreaming()
		{
			if (this.Response != null && this.UseStreaming)
			{
				this.Response.FinishStreaming();
			}
		}

		internal void Prepare()
		{
			if (this.FormUsage == HTTPFormUsage.Unity)
			{
				this.SelectFormImplementation();
			}
		}

		internal bool CallCustomCertificationValidator(X509Certificate cert, X509Chain chain)
		{
			return this.CustomCertificationValidator == null || this.CustomCertificationValidator(this, cert, chain);
		}

		public HTTPRequest Send()
		{
			return HTTPManager.SendRequest(this);
		}

		public void Abort()
		{
			object locker = HTTPManager.Locker;
			lock (locker)
			{
				HTTPConnection connectionWith = HTTPManager.GetConnectionWith(this);
				if (connectionWith == null)
				{
					if (!HTTPManager.RemoveFromQueue(this))
					{
						HTTPManager.Logger.Warning("HTTPRequest", "Abort - No active connection found with this request! (The request may already finished?)");
					}
					this.State = HTTPRequestStates.Aborted;
				}
				else
				{
					if (this.Response != null && this.Response.IsStreamed)
					{
						this.Response.Dispose();
					}
					connectionWith.Abort(HTTPConnectionStates.AbortRequested);
				}
			}
		}

		public void Clear()
		{
			this.ClearForm();
			this.RemoveHeaders();
		}

		public bool MoveNext()
		{
			object locker = HTTPManager.Locker;
			bool result;
			lock (locker)
			{
				HTTPConnection connectionWith = HTTPManager.GetConnectionWith(this);
				result = (connectionWith != null && connectionWith.State <= HTTPConnectionStates.WaitForRecycle);
			}
			return result;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}
