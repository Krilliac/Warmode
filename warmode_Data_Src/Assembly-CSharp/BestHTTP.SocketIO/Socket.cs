using BestHTTP.JSON;
using BestHTTP.SocketIO.Events;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO
{
	public sealed class Socket : ISocket
	{
		private Dictionary<int, SocketIOAckCallback> AckCallbacks;

		private EventTable EventCallbacks;

		private List<object> arguments = new List<object>();

		public SocketManager Manager
		{
			get;
			private set;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public bool IsOpen
		{
			get;
			private set;
		}

		public bool AutoDecodePayload
		{
			get;
			set;
		}

		internal Socket(string nsp, SocketManager manager)
		{
			this.Namespace = nsp;
			this.Manager = manager;
			this.IsOpen = false;
			this.AutoDecodePayload = true;
			this.EventCallbacks = new EventTable(this);
		}

		void ISocket.Open()
		{
			if (this.Manager.State == SocketManager.States.Open)
			{
				this.OnTransportOpen(this.Manager.Socket, null, new object[0]);
			}
			else
			{
				this.Manager.Socket.Off("connect", new SocketIOCallback(this.OnTransportOpen));
				this.Manager.Socket.On("connect", new SocketIOCallback(this.OnTransportOpen));
				if (this.Manager.Options.AutoConnect && this.Manager.State == SocketManager.States.Initial)
				{
					this.Manager.Open();
				}
			}
		}

		void ISocket.Disconnect(bool remove)
		{
			if (this.IsOpen)
			{
				Packet packet = new Packet(TransportEventTypes.Message, SocketIOEventTypes.Disconnect, this.Namespace, string.Empty, 0, 0);
				((IManager)this.Manager).SendPacket(packet);
				this.IsOpen = false;
				((ISocket)this).OnPacket(packet);
			}
			if (this.AckCallbacks != null)
			{
				this.AckCallbacks.Clear();
			}
			if (remove)
			{
				this.EventCallbacks.Clear();
				((IManager)this.Manager).Remove(this);
			}
		}

		void ISocket.OnPacket(Packet packet)
		{
			switch (packet.SocketIOEvent)
			{
			case SocketIOEventTypes.Disconnect:
				if (this.IsOpen)
				{
					this.IsOpen = false;
					this.Disconnect();
				}
				break;
			case SocketIOEventTypes.Error:
			{
				bool flag = false;
				Dictionary<string, object> dictionary = Json.Decode(packet.Payload, ref flag) as Dictionary<string, object>;
				if (flag)
				{
					Error error = new Error((SocketIOErrors)Convert.ToInt32(dictionary["code"]), dictionary["message"] as string);
					this.EventCallbacks.Call(EventNames.GetNameFor(SocketIOEventTypes.Error), packet, new object[]
					{
						error
					});
					return;
				}
				break;
			}
			}
			this.EventCallbacks.Call(packet);
			if ((packet.SocketIOEvent == SocketIOEventTypes.Ack || packet.SocketIOEvent == SocketIOEventTypes.BinaryAck) && this.AckCallbacks != null)
			{
				SocketIOAckCallback socketIOAckCallback = null;
				if (this.AckCallbacks.TryGetValue(packet.Id, out socketIOAckCallback) && socketIOAckCallback != null)
				{
					try
					{
						socketIOAckCallback(this, packet, packet.Decode(this.Manager.Encoder));
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("Socket", "ackCallback", ex);
					}
				}
				this.AckCallbacks.Remove(packet.Id);
			}
		}

		void ISocket.EmitEvent(SocketIOEventTypes type, params object[] args)
		{
			((ISocket)this).EmitEvent(EventNames.GetNameFor(type), args);
		}

		void ISocket.EmitEvent(string eventName, params object[] args)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				this.EventCallbacks.Call(eventName, null, args);
			}
		}

		void ISocket.EmitError(SocketIOErrors errCode, string msg)
		{
			((ISocket)this).EmitEvent(SocketIOEventTypes.Error, new object[]
			{
				new Error(errCode, msg)
			});
		}

		public void Disconnect()
		{
			((ISocket)this).Disconnect(true);
		}

		public Socket Emit(string eventName, params object[] args)
		{
			return this.Emit(eventName, null, args);
		}

		public Socket Emit(string eventName, SocketIOAckCallback callback, params object[] args)
		{
			bool flag = EventNames.IsBlacklisted(eventName);
			if (flag)
			{
				throw new ArgumentException("Blacklisted event: " + eventName);
			}
			this.arguments.Clear();
			this.arguments.Add(eventName);
			List<byte[]> list = null;
			if (args != null && args.Length > 0)
			{
				int num = 0;
				for (int i = 0; i < args.Length; i++)
				{
					byte[] array = args[i] as byte[];
					if (array != null)
					{
						if (list == null)
						{
							list = new List<byte[]>();
						}
						this.arguments.Add(string.Format("{{\"_placeholder\":true,\"num\":{0}}}", num++.ToString()));
						list.Add(array);
					}
					else
					{
						this.arguments.Add(args[i]);
					}
				}
			}
			string text = null;
			try
			{
				text = this.Manager.Encoder.Encode(this.arguments);
			}
			catch (Exception ex)
			{
				((ISocket)this).EmitError(SocketIOErrors.Internal, "Error while encoding payload: " + ex.Message + " " + ex.StackTrace);
				return this;
			}
			this.arguments.Clear();
			if (text == null)
			{
				throw new ArgumentException("Encoding the arguments to JSON failed!");
			}
			int num2 = 0;
			if (callback != null)
			{
				num2 = this.Manager.NextAckId;
				if (this.AckCallbacks == null)
				{
					this.AckCallbacks = new Dictionary<int, SocketIOAckCallback>();
				}
				this.AckCallbacks[num2] = callback;
			}
			Packet packet = new Packet(TransportEventTypes.Message, (list != null) ? SocketIOEventTypes.BinaryEvent : SocketIOEventTypes.Event, this.Namespace, text, 0, num2);
			if (list != null)
			{
				packet.Attachments = list;
			}
			((IManager)this.Manager).SendPacket(packet);
			return this;
		}

		public Socket EmitAck(Packet originalPacket, params object[] args)
		{
			if (originalPacket == null)
			{
				throw new ArgumentNullException("originalPacket == null!");
			}
			if (originalPacket.SocketIOEvent != SocketIOEventTypes.Event && originalPacket.SocketIOEvent != SocketIOEventTypes.BinaryEvent)
			{
				throw new ArgumentException("Wrong packet - you can't send an Ack for a packet with id == 0 and SocketIOEvent != Event or SocketIOEvent != BinaryEvent!");
			}
			this.arguments.Clear();
			if (args != null && args.Length > 0)
			{
				this.arguments.AddRange(args);
			}
			string text = null;
			try
			{
				text = this.Manager.Encoder.Encode(this.arguments);
			}
			catch (Exception ex)
			{
				((ISocket)this).EmitError(SocketIOErrors.Internal, "Error while encoding payload: " + ex.Message + " " + ex.StackTrace);
				return this;
			}
			if (text == null)
			{
				throw new ArgumentException("Encoding the arguments to JSON failed!");
			}
			Packet packet = new Packet(TransportEventTypes.Message, (originalPacket.SocketIOEvent != SocketIOEventTypes.Event) ? SocketIOEventTypes.BinaryAck : SocketIOEventTypes.Ack, this.Namespace, text, 0, originalPacket.Id);
			((IManager)this.Manager).SendPacket(packet);
			return this;
		}

		public void On(string eventName, SocketIOCallback callback)
		{
			this.EventCallbacks.Register(eventName, callback, false, this.AutoDecodePayload);
		}

		public void On(SocketIOEventTypes type, SocketIOCallback callback)
		{
			string nameFor = EventNames.GetNameFor(type);
			this.EventCallbacks.Register(nameFor, callback, false, this.AutoDecodePayload);
		}

		public void On(string eventName, SocketIOCallback callback, bool autoDecodePayload)
		{
			this.EventCallbacks.Register(eventName, callback, false, autoDecodePayload);
		}

		public void On(SocketIOEventTypes type, SocketIOCallback callback, bool autoDecodePayload)
		{
			string nameFor = EventNames.GetNameFor(type);
			this.EventCallbacks.Register(nameFor, callback, false, autoDecodePayload);
		}

		public void Once(string eventName, SocketIOCallback callback)
		{
			this.EventCallbacks.Register(eventName, callback, true, this.AutoDecodePayload);
		}

		public void Once(SocketIOEventTypes type, SocketIOCallback callback)
		{
			this.EventCallbacks.Register(EventNames.GetNameFor(type), callback, true, this.AutoDecodePayload);
		}

		public void Once(string eventName, SocketIOCallback callback, bool autoDecodePayload)
		{
			this.EventCallbacks.Register(eventName, callback, true, autoDecodePayload);
		}

		public void Once(SocketIOEventTypes type, SocketIOCallback callback, bool autoDecodePayload)
		{
			this.EventCallbacks.Register(EventNames.GetNameFor(type), callback, true, autoDecodePayload);
		}

		public void Off()
		{
			this.EventCallbacks.Clear();
		}

		public void Off(string eventName)
		{
			this.EventCallbacks.Unregister(eventName);
		}

		public void Off(SocketIOEventTypes type)
		{
			this.Off(EventNames.GetNameFor(type));
		}

		public void Off(string eventName, SocketIOCallback callback)
		{
			this.EventCallbacks.Unregister(eventName, callback);
		}

		public void Off(SocketIOEventTypes type, SocketIOCallback callback)
		{
			this.EventCallbacks.Unregister(EventNames.GetNameFor(type), callback);
		}

		private void OnTransportOpen(Socket socket, Packet packet, params object[] args)
		{
			if (this.Namespace != "/")
			{
				((IManager)this.Manager).SendPacket(new Packet(TransportEventTypes.Message, SocketIOEventTypes.Connect, this.Namespace, string.Empty, 0, 0));
			}
			this.IsOpen = true;
		}
	}
}
