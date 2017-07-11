using BestHTTP.Extensions;
using BestHTTP.WebSocket.Frames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BestHTTP.WebSocket
{
	public sealed class WebSocketResponse : HTTPResponse, IHeartbeat
	{
		public Action<WebSocketResponse, string> OnText;

		public Action<WebSocketResponse, byte[]> OnBinary;

		public Action<WebSocketResponse, WebSocketFrameReader> OnIncompleteFrame;

		public Action<WebSocketResponse, ushort, string> OnClosed;

		private List<WebSocketFrameReader> IncompleteFrames = new List<WebSocketFrameReader>();

		private List<WebSocketFrameReader> CompletedFrames = new List<WebSocketFrameReader>();

		private WebSocketFrameReader CloseFrame;

		private Thread ReceiverThread;

		private object FrameLock = new object();

		private object SendLock = new object();

		private bool closeSent;

		private bool closed;

		private DateTime lastPing = DateTime.MinValue;

		public bool IsClosed
		{
			get
			{
				return this.closed;
			}
		}

		public TimeSpan PingFrequnecy
		{
			get;
			private set;
		}

		public ushort MaxFragmentSize
		{
			get;
			private set;
		}

		internal WebSocketResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache) : base(request, stream, isStreamed, isFromCache)
		{
			this.closed = false;
			this.MaxFragmentSize = 32767;
		}

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			if (this.lastPing == DateTime.MinValue)
			{
				this.lastPing = DateTime.UtcNow;
				return;
			}
			if (DateTime.UtcNow - this.lastPing >= this.PingFrequnecy)
			{
				this.Send(new WebSocketPing(string.Empty));
				this.lastPing = DateTime.UtcNow;
			}
		}

		internal override bool Receive(int forceReadRawContentLength = -1, bool readPayloadData = true)
		{
			bool flag = base.Receive(forceReadRawContentLength, readPayloadData);
			if (flag && base.IsUpgraded)
			{
				this.ReceiverThread = new Thread(new ThreadStart(this.ReceiveThreadFunc));
				this.ReceiverThread.Name = "WebSocket Receiver Thread";
				this.ReceiverThread.IsBackground = true;
				this.ReceiverThread.Start();
			}
			return flag;
		}

		public void Send(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message must not be null!");
			}
			this.Send(new WebSocketTextFrame(message));
		}

		public void Send(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data must not be null!");
			}
			if ((long)data.Length > (long)this.MaxFragmentSize)
			{
				object sendLock = this.SendLock;
				lock (sendLock)
				{
					this.Send(new WebSocketBinaryFrame(data, 0uL, (ulong)this.MaxFragmentSize, false));
					ulong num2;
					for (ulong num = (ulong)this.MaxFragmentSize; num < (ulong)((long)data.Length); num += num2)
					{
						num2 = Math.Min((ulong)this.MaxFragmentSize, (ulong)((long)data.Length - (long)num));
						this.Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= (ulong)((long)data.Length)));
					}
				}
			}
			else
			{
				this.Send(new WebSocketBinaryFrame(data));
			}
		}

		public void Send(byte[] data, ulong offset, ulong count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data must not be null!");
			}
			if (offset + count > (ulong)((long)data.Length))
			{
				throw new ArgumentOutOfRangeException("offset + count >= data.Length");
			}
			if (count > (ulong)((long)this.MaxFragmentSize))
			{
				object sendLock = this.SendLock;
				lock (sendLock)
				{
					this.Send(new WebSocketBinaryFrame(data, offset, (ulong)this.MaxFragmentSize, false));
					ulong num2;
					for (ulong num = offset + (ulong)this.MaxFragmentSize; num < count; num += num2)
					{
						num2 = Math.Min((ulong)this.MaxFragmentSize, count - num);
						this.Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= count));
					}
				}
			}
			else
			{
				this.Send(new WebSocketBinaryFrame(data, offset, count, true));
			}
		}

		public void Send(IWebSocketFrameWriter frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame is null!");
			}
			if (this.closed)
			{
				return;
			}
			byte[] array = frame.Get();
			object sendLock = this.SendLock;
			lock (sendLock)
			{
				this.Stream.Write(array, 0, array.Length);
				this.Stream.Flush();
			}
			if (frame.Type == WebSocketFrameTypes.ConnectionClose)
			{
				this.closeSent = true;
			}
		}

		public void Close()
		{
			this.Close(1000, "Bye!");
		}

		public void Close(ushort code, string msg)
		{
			if (this.closed)
			{
				return;
			}
			this.Send(new WebSocketClose(code, msg));
		}

		public void StartPinging(int frequency)
		{
			if (frequency < 100)
			{
				throw new ArgumentException("frequency must be at least 100 millisec!");
			}
			this.PingFrequnecy = TimeSpan.FromMilliseconds((double)frequency);
			HTTPManager.Heartbeats.Subscribe(this);
		}

		private void ReceiveThreadFunc()
		{
			while (!this.closed)
			{
				try
				{
					WebSocketFrameReader webSocketFrameReader = new WebSocketFrameReader();
					webSocketFrameReader.Read(this.Stream);
					if (webSocketFrameReader.HasMask)
					{
						this.Close(1002, "Protocol Error: masked frame received from server!");
					}
					else if (!webSocketFrameReader.IsFinal)
					{
						if (this.OnIncompleteFrame == null)
						{
							this.IncompleteFrames.Add(webSocketFrameReader);
						}
						else
						{
							object frameLock = this.FrameLock;
							lock (frameLock)
							{
								this.CompletedFrames.Add(webSocketFrameReader);
							}
						}
					}
					else
					{
						switch (webSocketFrameReader.Type)
						{
						case WebSocketFrameTypes.Continuation:
							if (this.OnIncompleteFrame != null)
							{
								object frameLock2 = this.FrameLock;
								lock (frameLock2)
								{
									this.CompletedFrames.Add(webSocketFrameReader);
								}
								continue;
							}
							webSocketFrameReader.Assemble(this.IncompleteFrames);
							this.IncompleteFrames.Clear();
							break;
						case WebSocketFrameTypes.Text:
						case WebSocketFrameTypes.Binary:
							break;
						case (WebSocketFrameTypes)3:
						case (WebSocketFrameTypes)4:
						case (WebSocketFrameTypes)5:
						case (WebSocketFrameTypes)6:
						case (WebSocketFrameTypes)7:
							continue;
						case WebSocketFrameTypes.ConnectionClose:
							this.CloseFrame = webSocketFrameReader;
							if (!this.closeSent)
							{
								this.Send(new WebSocketClose());
							}
							this.closed = this.closeSent;
							continue;
						case WebSocketFrameTypes.Ping:
							if (!this.closeSent && !this.closed)
							{
								this.Send(new WebSocketPong(webSocketFrameReader));
							}
							continue;
						default:
							continue;
						}
						if (this.OnText != null)
						{
							object frameLock3 = this.FrameLock;
							lock (frameLock3)
							{
								this.CompletedFrames.Add(webSocketFrameReader);
							}
						}
					}
				}
				catch (ThreadAbortException)
				{
					this.IncompleteFrames.Clear();
					this.closed = true;
				}
				catch (Exception exception)
				{
					this.baseRequest.Exception = exception;
					this.closed = true;
				}
			}
			HTTPManager.Heartbeats.Unsubscribe(this);
		}

		internal void HandleEvents()
		{
			object frameLock = this.FrameLock;
			lock (frameLock)
			{
				for (int i = 0; i < this.CompletedFrames.Count; i++)
				{
					WebSocketFrameReader webSocketFrameReader = this.CompletedFrames[i];
					try
					{
						switch (webSocketFrameReader.Type)
						{
						case WebSocketFrameTypes.Continuation:
							break;
						case WebSocketFrameTypes.Text:
							if (webSocketFrameReader.IsFinal)
							{
								if (this.OnText != null)
								{
									this.OnText(this, Encoding.UTF8.GetString(webSocketFrameReader.Data, 0, webSocketFrameReader.Data.Length));
								}
								goto IL_D5;
							}
							break;
						case WebSocketFrameTypes.Binary:
							if (webSocketFrameReader.IsFinal)
							{
								if (this.OnBinary != null)
								{
									this.OnBinary(this, webSocketFrameReader.Data);
								}
								goto IL_D5;
							}
							break;
						default:
							goto IL_D5;
						}
						if (this.OnIncompleteFrame != null)
						{
							this.OnIncompleteFrame(this, webSocketFrameReader);
						}
						IL_D5:;
					}
					catch
					{
					}
				}
				this.CompletedFrames.Clear();
			}
			if (this.IsClosed && this.OnClosed != null)
			{
				try
				{
					ushort arg = 0;
					string arg2 = string.Empty;
					if (this.CloseFrame != null && this.CloseFrame.Data != null && this.CloseFrame.Data.Length >= 2)
					{
						if (BitConverter.IsLittleEndian)
						{
							Array.Reverse(this.CloseFrame.Data, 0, 2);
						}
						arg = BitConverter.ToUInt16(this.CloseFrame.Data, 0);
						if (this.CloseFrame.Data.Length > 2)
						{
							arg2 = Encoding.UTF8.GetString(this.CloseFrame.Data, 2, this.CloseFrame.Data.Length - 2);
						}
					}
					this.OnClosed(this, arg, arg2);
				}
				catch
				{
				}
			}
		}
	}
}
