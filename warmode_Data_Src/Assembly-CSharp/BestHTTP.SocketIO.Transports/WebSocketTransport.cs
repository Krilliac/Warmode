using BestHTTP.Logger;
using BestHTTP.WebSocket;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Transports
{
	internal sealed class WebSocketTransport : ITransport
	{
		private Packet PacketWithAttachment;

		private byte[] Buffer;

		public TransportStates State
		{
			get;
			private set;
		}

		public SocketManager Manager
		{
			get;
			private set;
		}

		public bool IsRequestInProgress
		{
			get;
			private set;
		}

		public WebSocket Implementation
		{
			get;
			private set;
		}

		public WebSocketTransport(SocketManager manager)
		{
			this.State = TransportStates.Closed;
			this.Manager = manager;
		}

		public void Open()
		{
			if (this.State != TransportStates.Closed)
			{
				return;
			}
			Uri uri = new Uri(string.Format("{0}?transport=websocket&sid={1}{2}", new UriBuilder("ws", this.Manager.Uri.Host, this.Manager.Uri.Port, this.Manager.Uri.PathAndQuery).Uri.ToString(), this.Manager.Handshake.Sid, this.Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : this.Manager.Options.BuildQueryParams()));
			this.Implementation = new WebSocket(uri);
			this.Implementation.OnOpen = new OnWebSocketOpenDelegate(this.OnOpen);
			this.Implementation.OnMessage = new OnWebSocketMessageDelegate(this.OnMessage);
			this.Implementation.OnBinary = new OnWebSocketBinaryDelegate(this.OnBinary);
			this.Implementation.OnError = new OnWebSocketErrorDelegate(this.OnError);
			this.Implementation.OnClosed = new OnWebSocketClosedDelegate(this.OnClosed);
			if (HTTPManager.Proxy != null)
			{
				this.Implementation.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
			}
			this.Implementation.Open();
			this.State = TransportStates.Connecting;
		}

		public void Close()
		{
			if (this.State == TransportStates.Closed)
			{
				return;
			}
			this.State = TransportStates.Closed;
			this.Implementation.Close();
			this.Implementation = null;
		}

		public void Poll()
		{
		}

		private void OnOpen(WebSocket ws)
		{
			HTTPManager.Logger.Information("WebSocketTransport", "OnOpen");
			this.State = TransportStates.Opening;
			this.Send(new Packet(TransportEventTypes.Ping, SocketIOEventTypes.Unknown, "/", "probe", 0, 0));
		}

		private void OnMessage(WebSocket ws, string message)
		{
			if (HTTPManager.Logger.Level <= Loglevels.All)
			{
				HTTPManager.Logger.Verbose("WebSocketTransport", "OnMessage: " + message);
			}
			try
			{
				Packet packet = new Packet(message);
				if (packet.AttachmentCount == 0)
				{
					this.OnPacket(packet);
				}
				else
				{
					this.PacketWithAttachment = packet;
				}
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("WebSocketTransport", "OnMessage", ex);
			}
		}

		private void OnBinary(WebSocket ws, byte[] data)
		{
			HTTPManager.Logger.Verbose("WebSocketTransport", "OnBinary");
			if (this.PacketWithAttachment != null)
			{
				this.PacketWithAttachment.AddAttachmentFromServer(data, false);
				if (this.PacketWithAttachment.HasAllAttachment)
				{
					try
					{
						this.OnPacket(this.PacketWithAttachment);
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("WebSocketTransport", "OnBinary", ex);
					}
					finally
					{
						this.PacketWithAttachment = null;
					}
				}
			}
		}

		private void OnError(WebSocket ws, Exception ex)
		{
			HTTPManager.Logger.Exception("WebSocketTransport", "OnError", ex);
			((IManager)this.Manager).OnTransportError(this, (ex == null) ? string.Empty : (ex.Message + " " + ex.StackTrace));
		}

		private void OnClosed(WebSocket ws, ushort code, string message)
		{
			HTTPManager.Logger.Information("WebSocketTransport", "OnClosed");
			this.Close();
			((IManager)this.Manager).TryToReconnect();
		}

		public void Send(Packet packet)
		{
			if (this.State == TransportStates.Closed || this.State == TransportStates.Paused)
			{
				return;
			}
			string text = packet.Encode();
			if (HTTPManager.Logger.Level <= Loglevels.All)
			{
				HTTPManager.Logger.Verbose("WebSocketTransport", "Send: " + text);
			}
			if (packet.AttachmentCount != 0 || (packet.Attachments != null && packet.Attachments.Count != 0))
			{
				if (packet.Attachments == null)
				{
					throw new ArgumentException("packet.Attachments are null!");
				}
				if (packet.AttachmentCount != packet.Attachments.Count)
				{
					throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
				}
			}
			this.Implementation.Send(text);
			if (packet.AttachmentCount != 0)
			{
				int num = packet.Attachments[0].Length + 1;
				for (int i = 1; i < packet.Attachments.Count; i++)
				{
					if (packet.Attachments[i].Length + 1 > num)
					{
						num = packet.Attachments[i].Length + 1;
					}
				}
				if (this.Buffer == null || this.Buffer.Length < num)
				{
					Array.Resize<byte>(ref this.Buffer, num);
				}
				for (int j = 0; j < packet.AttachmentCount; j++)
				{
					this.Buffer[0] = 4;
					Array.Copy(packet.Attachments[j], 0, this.Buffer, 1, packet.Attachments[j].Length);
					this.Implementation.Send(this.Buffer, 0uL, (ulong)((long)packet.Attachments[j].Length + 1L));
				}
			}
		}

		public void Send(List<Packet> packets)
		{
			for (int i = 0; i < packets.Count; i++)
			{
				this.Send(packets[i]);
			}
			packets.Clear();
		}

		private void OnPacket(Packet packet)
		{
			TransportEventTypes transportEvent = packet.TransportEvent;
			if (transportEvent == TransportEventTypes.Pong)
			{
				if (packet.Payload == "probe")
				{
					HTTPManager.Logger.Information("WebSocketTransport", "\"probe\" packet received, sending Upgrade packet");
					this.Send(new Packet(TransportEventTypes.Upgrade, SocketIOEventTypes.Event, "/", string.Empty, 0, 0));
					this.State = TransportStates.Open;
					if (((IManager)this.Manager).OnTransportConnected(this))
					{
						this.OnPacket(new Packet(TransportEventTypes.Message, SocketIOEventTypes.Connect, "/", string.Empty, 0, 0));
						return;
					}
				}
			}
			((IManager)this.Manager).OnPacket(packet);
		}
	}
}
