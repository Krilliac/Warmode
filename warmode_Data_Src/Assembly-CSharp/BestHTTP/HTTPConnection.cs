using BestHTTP.Authentication;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using SocketEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace BestHTTP
{
	internal sealed class HTTPConnection : IDisposable
	{
		private enum RetryCauses
		{
			None,
			Reconnect,
			Authenticate,
			ProxyAuthenticate
		}

		private TcpClient Client;

		private Stream Stream;

		private DateTime LastProcessTime;

		internal string ServerAddress
		{
			get;
			private set;
		}

		internal HTTPConnectionStates State
		{
			get;
			private set;
		}

		internal bool IsFree
		{
			get
			{
				return this.State == HTTPConnectionStates.Free;
			}
		}

		internal HTTPRequest CurrentRequest
		{
			get;
			private set;
		}

		internal bool IsRemovable
		{
			get
			{
				return this.IsFree && DateTime.UtcNow - this.LastProcessTime > HTTPManager.MaxConnectionIdleTime;
			}
		}

		internal DateTime StartTime
		{
			get;
			private set;
		}

		internal DateTime TimedOutStart
		{
			get;
			private set;
		}

		internal HTTPProxy Proxy
		{
			get;
			private set;
		}

		internal bool HasProxy
		{
			get
			{
				return this.Proxy != null;
			}
		}

		internal Uri LastProcessedUri
		{
			get;
			private set;
		}

		internal HTTPConnection(string serverAddress)
		{
			this.ServerAddress = serverAddress;
			this.State = HTTPConnectionStates.Initial;
			this.LastProcessTime = DateTime.UtcNow;
		}

		internal void Process(HTTPRequest request)
		{
			if (this.State == HTTPConnectionStates.Processing)
			{
				throw new Exception("Connection already processing a request!");
			}
			this.StartTime = DateTime.MaxValue;
			this.State = HTTPConnectionStates.Processing;
			this.CurrentRequest = request;
			new Thread(new ParameterizedThreadStart(this.ThreadFunc)).Start();
		}

		internal void Recycle()
		{
			if (this.State == HTTPConnectionStates.TimedOut)
			{
				this.LastProcessTime = DateTime.MinValue;
			}
			this.State = HTTPConnectionStates.Free;
			this.CurrentRequest = null;
		}

		private void ThreadFunc(object param)
		{
			bool flag = false;
			bool flag2 = false;
			HTTPConnection.RetryCauses retryCauses = HTTPConnection.RetryCauses.None;
			try
			{
				if (!this.HasProxy && this.CurrentRequest.HasProxy)
				{
					this.Proxy = this.CurrentRequest.Proxy;
				}
				if (!this.TryLoadAllFromCache())
				{
					if (this.Client != null && !this.Client.IsConnected())
					{
						this.Close();
					}
					while (true)
					{
						if (retryCauses == HTTPConnection.RetryCauses.Reconnect)
						{
							this.Close();
							Thread.Sleep(100);
						}
						this.LastProcessedUri = this.CurrentRequest.CurrentUri;
						retryCauses = HTTPConnection.RetryCauses.None;
						this.Connect();
						if (this.State == HTTPConnectionStates.AbortRequested)
						{
							break;
						}
						if (!this.CurrentRequest.DisableCache)
						{
							HTTPCacheService.SetHeaders(this.CurrentRequest);
						}
						bool flag3 = this.CurrentRequest.SendOutTo(this.Stream);
						if (!flag3)
						{
							this.Close();
							if (this.State == HTTPConnectionStates.TimedOut)
							{
								goto Block_13;
							}
							if (!flag)
							{
								flag = true;
								retryCauses = HTTPConnection.RetryCauses.Reconnect;
							}
						}
						if (flag3)
						{
							bool flag4 = this.Receive();
							if (this.State == HTTPConnectionStates.TimedOut)
							{
								goto Block_16;
							}
							if (!flag4 && !flag)
							{
								flag = true;
								retryCauses = HTTPConnection.RetryCauses.Reconnect;
							}
							if (this.CurrentRequest.Response != null)
							{
								int statusCode = this.CurrentRequest.Response.StatusCode;
								switch (statusCode)
								{
								case 301:
								case 302:
								case 307:
								case 308:
								{
									if (this.CurrentRequest.RedirectCount >= this.CurrentRequest.MaxRedirects)
									{
										goto IL_3F7;
									}
									this.CurrentRequest.RedirectCount++;
									string firstHeaderValue = this.CurrentRequest.Response.GetFirstHeaderValue("location");
									if (string.IsNullOrEmpty(firstHeaderValue))
									{
										goto IL_3C9;
									}
									Uri redirectUri = this.GetRedirectUri(firstHeaderValue);
									if (!this.CurrentRequest.CallOnBeforeRedirection(redirectUri))
									{
										HTTPManager.Logger.Information("HTTPConnection", "OnBeforeRedirection returned False");
										goto IL_3F7;
									}
									this.CurrentRequest.RemoveHeader("Host");
									this.CurrentRequest.SetHeader("Referer", this.CurrentRequest.CurrentUri.ToString());
									this.CurrentRequest.RedirectUri = redirectUri;
									this.CurrentRequest.Response = null;
									bool flag5 = true;
									this.CurrentRequest.IsRedirected = flag5;
									flag2 = flag5;
									goto IL_3F7;
								}
								case 303:
								case 304:
								case 305:
								case 306:
									IL_186:
									if (statusCode == 401)
									{
										string firstHeaderValue2 = this.CurrentRequest.Response.GetFirstHeaderValue("www-authenticate");
										if (!string.IsNullOrEmpty(firstHeaderValue2))
										{
											Digest orCreate = DigestStore.GetOrCreate(this.CurrentRequest.CurrentUri);
											orCreate.ParseChallange(firstHeaderValue2);
											if (this.CurrentRequest.Credentials != null && orCreate.IsUriProtected(this.CurrentRequest.CurrentUri) && (!this.CurrentRequest.HasHeader("Authorization") || orCreate.Stale))
											{
												retryCauses = HTTPConnection.RetryCauses.Authenticate;
											}
										}
										goto IL_3F7;
									}
									if (statusCode != 407)
									{
										goto IL_3F7;
									}
									if (this.CurrentRequest.HasProxy)
									{
										string firstHeaderValue3 = this.CurrentRequest.Response.GetFirstHeaderValue("proxy-authenticate");
										if (!string.IsNullOrEmpty(firstHeaderValue3))
										{
											Digest orCreate2 = DigestStore.GetOrCreate(this.CurrentRequest.Proxy.Address);
											orCreate2.ParseChallange(firstHeaderValue3);
											if (this.CurrentRequest.Proxy.Credentials != null && orCreate2.IsUriProtected(this.CurrentRequest.Proxy.Address) && (!this.CurrentRequest.HasHeader("Proxy-Authorization") || orCreate2.Stale))
											{
												retryCauses = HTTPConnection.RetryCauses.ProxyAuthenticate;
											}
										}
									}
									goto IL_3F7;
								}
								goto IL_186;
								IL_3F7:
								if (this.CurrentRequest.IsCookiesEnabled)
								{
									CookieJar.Set(this.CurrentRequest.Response);
								}
								this.TryStoreInCache();
								if (this.CurrentRequest.Response == null || this.CurrentRequest.Response.HasHeaderWithValue("connection", "close") || this.CurrentRequest.UseAlternateSSL)
								{
									this.Close();
								}
							}
						}
						if (retryCauses == HTTPConnection.RetryCauses.None)
						{
							goto Block_37;
						}
					}
					throw new Exception("AbortRequested");
					Block_13:
					throw new Exception("AbortRequested");
					Block_16:
					throw new Exception("AbortRequested");
					IL_3C9:
					throw new MissingFieldException(string.Format("Got redirect status({0}) without 'location' header!", this.CurrentRequest.Response.StatusCode.ToString()));
					Block_37:;
				}
			}
			catch (TimeoutException exception)
			{
				this.CurrentRequest.Response = null;
				this.CurrentRequest.Exception = exception;
				this.CurrentRequest.State = HTTPRequestStates.ConnectionTimedOut;
				this.Close();
			}
			catch (Exception exception2)
			{
				if (this.CurrentRequest != null)
				{
					if (this.CurrentRequest.UseStreaming)
					{
						HTTPCacheService.DeleteEntity(this.CurrentRequest.CurrentUri, true);
					}
					this.CurrentRequest.Response = null;
					HTTPConnectionStates state = this.State;
					if (state != HTTPConnectionStates.AbortRequested)
					{
						if (state != HTTPConnectionStates.TimedOut)
						{
							this.CurrentRequest.Exception = exception2;
							this.CurrentRequest.State = HTTPRequestStates.Error;
						}
						else
						{
							this.CurrentRequest.State = HTTPRequestStates.TimedOut;
						}
					}
					else
					{
						this.CurrentRequest.State = HTTPRequestStates.Aborted;
					}
				}
				this.Close();
			}
			finally
			{
				if (this.CurrentRequest != null)
				{
					object locker = HTTPManager.Locker;
					lock (locker)
					{
						if (this.CurrentRequest != null && this.CurrentRequest.Response != null && this.CurrentRequest.Response.IsUpgraded)
						{
							this.State = HTTPConnectionStates.Upgraded;
						}
						else
						{
							this.State = ((!flag2) ? ((this.Client != null) ? HTTPConnectionStates.WaitForRecycle : HTTPConnectionStates.Closed) : HTTPConnectionStates.Redirected);
						}
						if (this.CurrentRequest.State == HTTPRequestStates.Processing && (this.State == HTTPConnectionStates.Closed || this.State == HTTPConnectionStates.WaitForRecycle))
						{
							this.CurrentRequest.State = HTTPRequestStates.Finished;
						}
						if (this.CurrentRequest.State == HTTPRequestStates.ConnectionTimedOut)
						{
							this.State = HTTPConnectionStates.Closed;
						}
						this.LastProcessTime = DateTime.UtcNow;
					}
					HTTPCacheService.SaveLibrary();
					CookieJar.Persist();
				}
			}
		}

		private void Connect()
		{
			Uri uri = (!this.CurrentRequest.HasProxy) ? this.CurrentRequest.CurrentUri : this.CurrentRequest.Proxy.Address;
			if (this.Client == null)
			{
				this.Client = new TcpClient();
			}
			if (!this.Client.Connected)
			{
				this.Client.ConnectTimeout = this.CurrentRequest.ConnectTimeout;
				this.Client.Connect(uri.Host, uri.Port);
				HTTPManager.Logger.Information("HTTPConnection", "Connected to " + uri.Host);
			}
			else
			{
				HTTPManager.Logger.Information("HTTPConnection", "Already connected to " + uri.Host);
			}
			object locker = HTTPManager.Locker;
			lock (locker)
			{
				this.StartTime = DateTime.UtcNow;
			}
			if (this.Stream == null)
			{
				if (this.HasProxy && !this.Proxy.IsTransparent)
				{
					this.Stream = this.Client.GetStream();
					BinaryWriter binaryWriter = new BinaryWriter(this.Stream);
					binaryWriter.Write(string.Format("CONNECT {0}:{1} HTTP/1.1", this.CurrentRequest.CurrentUri.Host, this.CurrentRequest.CurrentUri.Port).GetASCIIBytes());
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Write(string.Format("Proxy-Connection: Keep-Alive", new object[0]));
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Write(string.Format("Connection: Keep-Alive", new object[0]));
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Write(string.Format("Host: {0}:{1}", this.CurrentRequest.CurrentUri.Host, this.CurrentRequest.CurrentUri.Port).GetASCIIBytes());
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Flush();
					this.CurrentRequest.ProxyResponse = new HTTPProxyResponse(this.CurrentRequest, this.Stream, false, false);
					if (!this.CurrentRequest.ProxyResponse.Receive(-1, true))
					{
						throw new Exception("Connection to the Proxy Server failed!");
					}
					HTTPManager.Logger.Information("HTTPConnection", string.Concat(new object[]
					{
						"Proxy returned - status code: ",
						this.CurrentRequest.ProxyResponse.StatusCode,
						" message: ",
						this.CurrentRequest.ProxyResponse.Message
					}));
				}
				if (HTTPProtocolFactory.IsSecureProtocol(this.CurrentRequest.CurrentUri))
				{
					if (this.CurrentRequest.UseAlternateSSL)
					{
						TlsClientProtocol tlsClientProtocol = new TlsClientProtocol(this.Client.GetStream(), new SecureRandom());
						List<string> list = new List<string>(1);
						list.Add(this.CurrentRequest.CurrentUri.Host);
						TlsClientProtocol arg_32B_0 = tlsClientProtocol;
						ICertificateVerifyer arg_326_0;
						if (this.CurrentRequest.CustomCertificateVerifyer == null)
						{
							ICertificateVerifyer certificateVerifyer = new AlwaysValidVerifyer();
							arg_326_0 = certificateVerifyer;
						}
						else
						{
							arg_326_0 = this.CurrentRequest.CustomCertificateVerifyer;
						}
						arg_32B_0.Connect(new LegacyTlsClient(arg_326_0, null, list));
						this.Stream = tlsClientProtocol.Stream;
					}
					else
					{
						SslStream sslStream = new SslStream(this.Client.GetStream(), false, (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => this.CurrentRequest.CallCustomCertificationValidator(cert, chain));
						if (!sslStream.IsAuthenticated)
						{
							sslStream.AuthenticateAsClient(uri.Host);
						}
						this.Stream = sslStream;
					}
				}
				else
				{
					this.Stream = this.Client.GetStream();
				}
			}
		}

		private bool Receive()
		{
			this.CurrentRequest.Response = HTTPProtocolFactory.Get(HTTPProtocolFactory.GetProtocolFromUri(this.CurrentRequest.CurrentUri), this.CurrentRequest, this.Stream, this.CurrentRequest.UseStreaming, false);
			if (!this.CurrentRequest.Response.Receive(-1, true))
			{
				this.CurrentRequest.Response = null;
				return false;
			}
			if (this.CurrentRequest.Response.StatusCode == 304)
			{
				int contentLength;
				using (Stream body = HTTPCacheService.GetBody(this.CurrentRequest.CurrentUri, out contentLength))
				{
					if (!this.CurrentRequest.Response.HasHeader("content-length"))
					{
						this.CurrentRequest.Response.Headers.Add("content-length", new List<string>(1)
						{
							contentLength.ToString()
						});
					}
					this.CurrentRequest.Response.IsFromCache = true;
					this.CurrentRequest.Response.ReadRaw(body, contentLength);
				}
			}
			return true;
		}

		private bool TryLoadAllFromCache()
		{
			if (this.CurrentRequest.DisableCache)
			{
				return false;
			}
			try
			{
				if (HTTPCacheService.IsCachedEntityExpiresInTheFuture(this.CurrentRequest))
				{
					this.CurrentRequest.Response = HTTPCacheService.GetFullResponse(this.CurrentRequest);
					if (this.CurrentRequest.Response != null)
					{
						return true;
					}
				}
			}
			catch
			{
				HTTPCacheService.DeleteEntity(this.CurrentRequest.CurrentUri, true);
			}
			return false;
		}

		private void TryStoreInCache()
		{
			if (!this.CurrentRequest.UseStreaming && !this.CurrentRequest.DisableCache && this.CurrentRequest.Response != null && HTTPCacheService.IsCacheble(this.CurrentRequest.CurrentUri, this.CurrentRequest.MethodType, this.CurrentRequest.Response))
			{
				HTTPCacheService.Store(this.CurrentRequest.CurrentUri, this.CurrentRequest.MethodType, this.CurrentRequest.Response);
			}
		}

		private Uri GetRedirectUri(string location)
		{
			Uri result = null;
			try
			{
				result = new Uri(location);
			}
			catch (UriFormatException)
			{
				Uri uri = this.CurrentRequest.Uri;
				UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, location);
				result = uriBuilder.Uri;
			}
			return result;
		}

		internal void HandleProgressCallback()
		{
			if (this.CurrentRequest.OnProgress != null && this.CurrentRequest.DownloadProgressChanged)
			{
				try
				{
					this.CurrentRequest.OnProgress(this.CurrentRequest, this.CurrentRequest.Downloaded, this.CurrentRequest.DownloadLength);
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("HTTPManager", "HandleProgressCallback - OnProgress", ex);
				}
				this.CurrentRequest.DownloadProgressChanged = false;
			}
			if (this.CurrentRequest.OnUploadProgress != null && this.CurrentRequest.UploadProgressChanged)
			{
				try
				{
					this.CurrentRequest.OnUploadProgress(this.CurrentRequest, this.CurrentRequest.Uploaded, this.CurrentRequest.UploadLength);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("HTTPManager", "HandleProgressCallback - OnUploadProgress", ex2);
				}
				this.CurrentRequest.UploadProgressChanged = false;
			}
		}

		internal void HandleCallback()
		{
			try
			{
				this.HandleProgressCallback();
				if (this.State == HTTPConnectionStates.Upgraded)
				{
					if (this.CurrentRequest != null && this.CurrentRequest.Response != null && this.CurrentRequest.Response.IsUpgraded)
					{
						this.CurrentRequest.UpgradeCallback();
					}
					this.State = HTTPConnectionStates.WaitForProtocolShutdown;
				}
				else
				{
					this.CurrentRequest.CallCallback();
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("HTTPManager", "HandleCallback", ex);
			}
		}

		internal void Abort(HTTPConnectionStates newState)
		{
			this.State = newState;
			HTTPConnectionStates state = this.State;
			if (state == HTTPConnectionStates.TimedOut)
			{
				this.TimedOutStart = DateTime.UtcNow;
			}
			if (this.Stream != null)
			{
				this.Stream.Dispose();
			}
		}

		private void Close()
		{
			this.LastProcessedUri = null;
			if (this.Client != null)
			{
				try
				{
					this.Client.Close();
				}
				catch
				{
				}
				finally
				{
					this.Stream = null;
					this.Client = null;
				}
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
