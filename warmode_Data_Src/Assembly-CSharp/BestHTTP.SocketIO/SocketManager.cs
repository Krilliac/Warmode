using BestHTTP.Extensions;
using BestHTTP.SocketIO.Events;
using BestHTTP.SocketIO.JsonEncoders;
using BestHTTP.SocketIO.Transports;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BestHTTP.SocketIO
{
	public sealed class SocketManager : IHeartbeat, IManager
	{
		public enum States
		{
			Initial,
			Closed,
			Opening,
			Open,
			Reconnecting
		}

		public const int MinProtocolVersion = 4;

		public static IJsonEncoder DefaultEncoder = new DefaultJSonEncoder();

		private SocketManager.States state;

		private int nextAckId;

		private Dictionary<string, Socket> Namespaces = new Dictionary<string, Socket>();

		private List<Socket> Sockets = new List<Socket>();

		private List<Packet> OfflinePackets;

		private PollingTransport Poller;

		private DateTime LastHeartbeat = DateTime.MinValue;

		private DateTime LastPongReceived = DateTime.MinValue;

		private DateTime ReconnectAt;

		private DateTime ConnectionStarted;

		public SocketManager.States State
		{
			get
			{
				return this.state;
			}
			private set
			{
				this.PreviousState = this.state;
				this.state = value;
			}
		}

		public SocketOptions Options
		{
			get;
			private set;
		}

		public Uri Uri
		{
			get;
			private set;
		}

		public HandshakeData Handshake
		{
			get;
			private set;
		}

		public ITransport Transport
		{
			get;
			private set;
		}

		public ulong RequestCounter
		{
			get;
			internal set;
		}

		public Socket Socket
		{
			get
			{
				return this.GetSocket();
			}
		}

		public Socket this[string nsp]
		{
			get
			{
				return this.GetSocket(nsp);
			}
		}

		public int ReconnectAttempts
		{
			get;
			private set;
		}

		public IJsonEncoder Encoder
		{
			get;
			set;
		}

		internal uint Timestamp
		{
			get
			{
				return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
			}
		}

		internal int NextAckId
		{
			get
			{
				return Interlocked.Increment(ref this.nextAckId);
			}
		}

		internal SocketManager.States PreviousState
		{
			get;
			private set;
		}

		public SocketManager(Uri uri) : this(uri, new SocketOptions())
		{
		}

		public SocketManager(Uri uri, SocketOptions options)
		{
			this.Uri = uri;
			this.Options = options;
			this.State = SocketManager.States.Initial;
			this.PreviousState = SocketManager.States.Initial;
			this.Encoder = SocketManager.DefaultEncoder;
		}

		void IManager.Remove(Socket socket)
		{
			this.Namespaces.Remove(socket.Namespace);
			this.Sockets.Remove(socket);
		}

		void IManager.Close(bool removeSockets)
		{
			if (this.State == SocketManager.States.Closed)
			{
				return;
			}
			HTTPManager.Logger.Information("SocketManager", "Closing");
			HTTPManager.Heartbeats.Unsubscribe(this);
			if (removeSockets)
			{
				while (this.Sockets.Count > 0)
				{
					((ISocket)this.Sockets[this.Sockets.Count - 1]).Disconnect(removeSockets);
				}
			}
			else
			{
				for (int i = 0; i < this.Sockets.Count; i++)
				{
					((ISocket)this.Sockets[i]).Disconnect(removeSockets);
				}
			}
			this.State = SocketManager.States.Closed;
			this.LastHeartbeat = DateTime.MinValue;
			if (this.OfflinePackets != null)
			{
				this.OfflinePackets.Clear();
			}
			if (removeSockets)
			{
				this.Namespaces.Clear();
			}
			if (this.Handshake != null)
			{
				this.Handshake.Abort();
			}
			this.Handshake = null;
			if (this.Transport != null)
			{
				this.Transport.Close();
			}
			this.Transport = null;
			if (this.Poller != null)
			{
				this.Poller.Close();
			}
			this.Poller = null;
		}

		void IManager.TryToReconnect()
		{
			if (this.State == SocketManager.States.Reconnecting || this.State == SocketManager.States.Closed)
			{
				return;
			}
			if (!this.Options.Reconnection)
			{
				((IManager)this).EmitAll(EventNames.GetNameFor(SocketIOEventTypes.Disconnect), new object[0]);
				this.Close();
				return;
			}
			if (++this.ReconnectAttempts >= this.Options.ReconnectionAttempts)
			{
				((IManager)this).EmitEvent("reconnect_failed", new object[0]);
				this.Close();
				return;
			}
			Random random = new Random();
			int num = (int)this.Options.ReconnectionDelay.TotalMilliseconds * this.ReconnectAttempts;
			this.ReconnectAt = DateTime.UtcNow + TimeSpan.FromMilliseconds((double)Math.Min(random.Next((int)((float)num - (float)num * this.Options.RandomizationFactor), (int)((float)num + (float)num * this.Options.RandomizationFactor)), (int)this.Options.ReconnectionDelayMax.TotalMilliseconds));
			((IManager)this).Close(false);
			this.State = SocketManager.States.Reconnecting;
			for (int i = 0; i < this.Sockets.Count; i++)
			{
				((ISocket)this.Sockets[i]).Open();
			}
			HTTPManager.Heartbeats.Subscribe(this);
			HTTPManager.Logger.Information("SocketManager", "Reconnecting");
		}

		bool IManager.OnTransportConnected(ITransport transport)
		{
			if (this.State != SocketManager.States.Opening)
			{
				return false;
			}
			if (this.Poller != null && this.Poller.State == TransportStates.Open && (this.Transport == null || this.Transport.State == TransportStates.Open))
			{
				if (this.PreviousState == SocketManager.States.Reconnecting)
				{
					((IManager)this).EmitEvent("reconnect", new object[0]);
				}
				this.State = SocketManager.States.Open;
				this.LastPongReceived = DateTime.UtcNow;
				this.ReconnectAttempts = 0;
				this.SendOfflinePackets();
				HTTPManager.Logger.Information("SocketManager", "Open");
				return true;
			}
			return false;
		}

		void IManager.OnTransportError(ITransport trans, string err)
		{
			HTTPManager.Logger.Error("SocketManager", "OnTransportError - " + err);
			((IManager)this).EmitError(SocketIOErrors.Internal, err);
			if (trans.State == TransportStates.Connecting || trans.State == TransportStates.Opening)
			{
				trans.Close();
				if (trans == this.Transport && this.Poller != null && this.Poller.State == TransportStates.Open)
				{
					this.Transport = null;
					this.State = SocketManager.States.Open;
					this.LastPongReceived = DateTime.UtcNow;
					this.ReconnectAttempts = 0;
					HTTPManager.Logger.Information("SocketManager", "Open (Websocket transport failed to upgrade, fallback to polling)");
				}
				else
				{
					((IManager)this).TryToReconnect();
				}
			}
			else
			{
				trans.Close();
				((IManager)this).TryToReconnect();
			}
		}

		void IManager.SendPacket(Packet packet)
		{
			ITransport transport = this.SelectTransport();
			if (transport != null)
			{
				try
				{
					transport.Send(packet);
				}
				catch (Exception ex)
				{
					((IManager)this).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				}
			}
			else
			{
				if (this.OfflinePackets == null)
				{
					this.OfflinePackets = new List<Packet>();
				}
				this.OfflinePackets.Add(packet.Clone());
			}
		}

		void IManager.OnPacket(Packet packet)
		{
			if (this.State == SocketManager.States.Closed)
			{
				return;
			}
			TransportEventTypes transportEvent = packet.TransportEvent;
			if (transportEvent != TransportEventTypes.Ping)
			{
				if (transportEvent == TransportEventTypes.Pong)
				{
					this.LastPongReceived = DateTime.UtcNow;
				}
			}
			else
			{
				((IManager)this).SendPacket(new Packet(TransportEventTypes.Pong, SocketIOEventTypes.Unknown, "/", string.Empty, 0, 0));
			}
			Socket socket = null;
			if (this.Namespaces.TryGetValue(packet.Namespace, out socket))
			{
				((ISocket)socket).OnPacket(packet);
			}
			else
			{
				HTTPManager.Logger.Warning("SocketManager", "Namespace \"" + packet.Namespace + "\" not found!");
			}
		}

		void IManager.EmitEvent(string eventName, params object[] args)
		{
			Socket socket = null;
			if (this.Namespaces.TryGetValue("/", out socket))
			{
				((ISocket)socket).EmitEvent(eventName, args);
			}
		}

		void IManager.EmitEvent(SocketIOEventTypes type, params object[] args)
		{
			((IManager)this).EmitEvent(EventNames.GetNameFor(type), args);
		}

		void IManager.EmitError(SocketIOErrors errCode, string msg)
		{
			((IManager)this).EmitEvent(SocketIOEventTypes.Error, new object[]
			{
				new Error(errCode, msg)
			});
		}

		void IManager.EmitAll(string eventName, params object[] args)
		{
			for (int i = 0; i < this.Sockets.Count; i++)
			{
				((ISocket)this.Sockets[i]).EmitEvent(eventName, args);
			}
		}

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			switch (this.State)
			{
			case SocketManager.States.Opening:
				if (DateTime.UtcNow - this.ConnectionStarted >= this.Options.Timeout)
				{
					((IManager)this).EmitEvent("connect_error", new object[0]);
					((IManager)this).EmitEvent("connect_timeout", new object[0]);
					((IManager)this).TryToReconnect();
				}
				break;
			case SocketManager.States.Open:
			{
				ITransport transport;
				if (this.Transport != null && this.Transport.State == TransportStates.Open)
				{
					transport = this.Transport;
				}
				else
				{
					transport = this.Poller;
				}
				if (transport == null || transport.State != TransportStates.Open)
				{
					return;
				}
				transport.Poll();
				this.SendOfflinePackets();
				if (this.LastHeartbeat == DateTime.MinValue)
				{
					this.LastHeartbeat = DateTime.UtcNow;
					return;
				}
				if (DateTime.UtcNow - this.LastHeartbeat > this.Handshake.PingInterval)
				{
					((IManager)this).SendPacket(new Packet(TransportEventTypes.Ping, SocketIOEventTypes.Unknown, "/", string.Empty, 0, 0));
					this.LastHeartbeat = DateTime.UtcNow;
				}
				if (DateTime.UtcNow - this.LastPongReceived > this.Handshake.PingTimeout)
				{
					((IManager)this).EmitAll(EventNames.GetNameFor(SocketIOEventTypes.Disconnect), new object[0]);
					((IManager)this).TryToReconnect();
				}
				break;
			}
			case SocketManager.States.Reconnecting:
				if (this.ReconnectAt != DateTime.MinValue && DateTime.UtcNow >= this.ReconnectAt)
				{
					((IManager)this).EmitEvent("reconnect_attempt", new object[0]);
					((IManager)this).EmitEvent("reconnecting", new object[0]);
					this.Open();
				}
				break;
			}
		}

		public Socket GetSocket()
		{
			return this.GetSocket("/");
		}

		public Socket GetSocket(string nsp)
		{
			if (string.IsNullOrEmpty(nsp))
			{
				throw new ArgumentNullException("Namespace parameter is null or empty!");
			}
			Socket socket = null;
			if (!this.Namespaces.TryGetValue(nsp, out socket))
			{
				socket = new Socket(nsp, this);
				this.Namespaces.Add(nsp, socket);
				this.Sockets.Add(socket);
				((ISocket)socket).Open();
			}
			return socket;
		}

		public void Open()
		{
			if (this.State != SocketManager.States.Initial && this.State != SocketManager.States.Closed && this.State != SocketManager.States.Reconnecting)
			{
				return;
			}
			HTTPManager.Logger.Information("SocketManager", "Opening");
			this.ReconnectAt = DateTime.MinValue;
			this.Handshake = new HandshakeData(this);
			this.Handshake.OnReceived = delegate(HandshakeData hsd)
			{
				this.CreateTransports();
			};
			this.Handshake.OnError = delegate(HandshakeData hsd, string err)
			{
				((IManager)this).EmitError(SocketIOErrors.Internal, err);
				((IManager)this).TryToReconnect();
			};
			this.Handshake.Start();
			((IManager)this).EmitEvent("connecting", new object[0]);
			this.State = SocketManager.States.Opening;
			this.ConnectionStarted = DateTime.UtcNow;
			HTTPManager.Heartbeats.Subscribe(this);
			this.GetSocket("/");
		}

		public void Close()
		{
			((IManager)this).Close(true);
		}

		private void CreateTransports()
		{
			bool flag = this.Handshake.Upgrades.Contains("websocket");
			if (flag)
			{
				this.Transport = new WebSocketTransport(this);
			}
			this.Poller = new PollingTransport(this);
			this.Poller.Open();
			if (this.Transport == null)
			{
				this.Transport = this.Poller;
			}
			else
			{
				this.Transport.Open();
			}
		}

		private ITransport SelectTransport()
		{
			if (this.State != SocketManager.States.Open)
			{
				return null;
			}
			if (this.Transport != null && this.Transport != this.Poller)
			{
				if (this.Transport.State == TransportStates.Open)
				{
					if (this.Poller != null && this.Poller.State == TransportStates.Open && this.Poller.IsRequestInProgress)
					{
						return null;
					}
					return this.Transport;
				}
				else if (this.Transport.State == TransportStates.Opening)
				{
					return null;
				}
			}
			if (this.Poller != null && this.Poller.State == TransportStates.Open && !this.Poller.IsRequestInProgress)
			{
				return this.Poller;
			}
			return null;
		}

		private void SendOfflinePackets()
		{
			ITransport transport = this.SelectTransport();
			if (this.OfflinePackets != null && this.OfflinePackets.Count > 0 && transport != null)
			{
				transport.Send(this.OfflinePackets);
				this.OfflinePackets.Clear();
			}
		}

		public void EmitAll(string eventName, params object[] args)
		{
			for (int i = 0; i < this.Sockets.Count; i++)
			{
				this.Sockets[i].Emit(eventName, args);
			}
		}
	}
}
