using BestHTTP.WebSocket.Frames;
using System;

namespace BestHTTP.WebSocket
{
	public sealed class WebSocket
	{
		public OnWebSocketOpenDelegate OnOpen;

		public OnWebSocketMessageDelegate OnMessage;

		public OnWebSocketBinaryDelegate OnBinary;

		public OnWebSocketClosedDelegate OnClosed;

		public OnWebSocketErrorDelegate OnError;

		public OnWebSocketIncompleteFrameDelegate OnIncompleteFrame;

		private bool requestSent;

		private WebSocketResponse webSocket;

		public HTTPRequest InternalRequest
		{
			get;
			private set;
		}

		public bool IsOpen
		{
			get
			{
				return this.webSocket != null && !this.webSocket.IsClosed;
			}
		}

		public bool StartPingThread
		{
			get;
			set;
		}

		public int PingFrequency
		{
			get;
			set;
		}

		public WebSocket(Uri uri) : this(uri, string.Empty, string.Empty)
		{
		}

		public WebSocket(Uri uri, string origin, string protocol = "")
		{
			this.PingFrequency = 1000;
			if (uri.Port == -1)
			{
				uri = new Uri(string.Concat(new string[]
				{
					uri.Scheme,
					"://",
					uri.Host,
					":",
					(!uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase)) ? "80" : "443",
					uri.PathAndQuery
				}));
			}
			this.InternalRequest = new HTTPRequest(uri, delegate(HTTPRequest req, HTTPResponse resp)
			{
				if (((resp == null || req.Exception != null) && this.OnError != null) || (resp != null && resp.StatusCode != 101))
				{
					this.OnError(this, req.Exception);
				}
			});
			this.InternalRequest.SetHeader("Host", uri.Host + ":" + uri.Port);
			this.InternalRequest.SetHeader("Upgrade", "websocket");
			this.InternalRequest.SetHeader("Connection", "keep-alive, Upgrade");
			this.InternalRequest.SetHeader("Sec-WebSocket-Key", this.GetSecKey(new object[]
			{
				this,
				this.InternalRequest,
				uri,
				new object()
			}));
			if (!string.IsNullOrEmpty(origin))
			{
				this.InternalRequest.SetHeader("Origin", origin);
			}
			this.InternalRequest.SetHeader("Sec-WebSocket-Version", "13");
			if (!string.IsNullOrEmpty(protocol))
			{
				this.InternalRequest.SetHeader("Sec-WebSocket-Protocol", protocol);
			}
			this.InternalRequest.SetHeader("Cache-Control", "no-cache");
			this.InternalRequest.SetHeader("Pragma", "no-cache");
			this.InternalRequest.DisableCache = true;
			this.InternalRequest.OnUpgraded = delegate(HTTPRequest req, HTTPResponse resp)
			{
				this.webSocket = (resp as WebSocketResponse);
				if (this.webSocket == null)
				{
					if (this.OnError != null)
					{
						this.OnError(this, req.Exception);
					}
					return;
				}
				if (this.OnOpen != null)
				{
					this.OnOpen(this);
				}
				this.webSocket.OnText = delegate(WebSocketResponse ws, string msg)
				{
					if (this.OnMessage != null)
					{
						this.OnMessage(this, msg);
					}
				};
				this.webSocket.OnBinary = delegate(WebSocketResponse ws, byte[] bin)
				{
					if (this.OnBinary != null)
					{
						this.OnBinary(this, bin);
					}
				};
				this.webSocket.OnClosed = delegate(WebSocketResponse ws, ushort code, string msg)
				{
					if (this.OnClosed != null)
					{
						this.OnClosed(this, code, msg);
					}
				};
				if (this.OnIncompleteFrame != null)
				{
					this.webSocket.OnIncompleteFrame = delegate(WebSocketResponse ws, WebSocketFrameReader frame)
					{
						if (this.OnIncompleteFrame != null)
						{
							this.OnIncompleteFrame(this, frame);
						}
					};
				}
				if (this.StartPingThread)
				{
					this.webSocket.StartPinging(Math.Min(this.PingFrequency, 100));
				}
			};
		}

		public void Open()
		{
			if (this.requestSent || this.InternalRequest == null)
			{
				return;
			}
			this.InternalRequest.Send();
			this.requestSent = true;
		}

		public void Send(string message)
		{
			if (this.IsOpen)
			{
				this.webSocket.Send(message);
			}
		}

		public void Send(byte[] buffer)
		{
			if (this.IsOpen)
			{
				this.webSocket.Send(buffer);
			}
		}

		public void Send(byte[] buffer, ulong offset, ulong count)
		{
			if (this.IsOpen)
			{
				this.webSocket.Send(buffer, offset, count);
			}
		}

		public void Send(IWebSocketFrameWriter frame)
		{
			if (this.IsOpen)
			{
				this.webSocket.Send(frame);
			}
		}

		public void Close()
		{
			if (this.IsOpen)
			{
				this.webSocket.Close();
			}
		}

		public void Close(ushort code, string message)
		{
			if (this.IsOpen)
			{
				this.webSocket.Close(code, message);
			}
		}

		private string GetSecKey(object[] from)
		{
			byte[] array = new byte[16];
			int num = 0;
			for (int i = 0; i < from.Length; i++)
			{
				byte[] bytes = BitConverter.GetBytes(from[i].GetHashCode());
				int num2 = 0;
				while (num2 < bytes.Length && num < array.Length)
				{
					array[num++] = bytes[num2];
					num2++;
				}
			}
			return Convert.ToBase64String(array);
		}
	}
}
