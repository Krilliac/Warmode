using BestHTTP.Logger;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Events
{
	internal sealed class EventTable
	{
		private Dictionary<string, List<EventDescriptor>> Table = new Dictionary<string, List<EventDescriptor>>();

		private Socket Socket
		{
			get;
			set;
		}

		public EventTable(Socket socket)
		{
			this.Socket = socket;
		}

		public void Register(string eventName, SocketIOCallback callback, bool onlyOnce, bool autoDecodePayload)
		{
			List<EventDescriptor> list;
			if (!this.Table.TryGetValue(eventName, out list))
			{
				this.Table.Add(eventName, list = new List<EventDescriptor>(1));
			}
			EventDescriptor eventDescriptor = list.Find((EventDescriptor d) => d.OnlyOnce == onlyOnce && d.AutoDecodePayload == autoDecodePayload);
			if (eventDescriptor == null)
			{
				list.Add(new EventDescriptor(onlyOnce, autoDecodePayload, callback));
			}
			else
			{
				eventDescriptor.Callbacks.Add(callback);
			}
		}

		public void Unregister(string eventName)
		{
			this.Table.Remove(eventName);
		}

		public void Unregister(string eventName, SocketIOCallback callback)
		{
			List<EventDescriptor> list;
			if (this.Table.TryGetValue(eventName, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Callbacks.Remove(callback);
				}
			}
		}

		public void Call(string eventName, Packet packet, params object[] args)
		{
			if (HTTPManager.Logger.Level <= Loglevels.All)
			{
				HTTPManager.Logger.Verbose("EventTable", "Call - " + eventName);
			}
			List<EventDescriptor> list;
			if (this.Table.TryGetValue(eventName, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Call(this.Socket, packet, args);
				}
			}
		}

		public void Call(Packet packet)
		{
			string text = packet.DecodeEventName();
			string text2 = (packet.SocketIOEvent == SocketIOEventTypes.Unknown) ? EventNames.GetNameFor(packet.TransportEvent) : EventNames.GetNameFor(packet.SocketIOEvent);
			object[] args = null;
			if (!this.HasSubsciber(text) && !this.HasSubsciber(text2))
			{
				return;
			}
			if (packet.TransportEvent == TransportEventTypes.Message && (packet.SocketIOEvent == SocketIOEventTypes.Event || packet.SocketIOEvent == SocketIOEventTypes.BinaryEvent) && this.ShouldDecodePayload(text))
			{
				args = packet.Decode(this.Socket.Manager.Encoder);
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.Call(text, packet, args);
			}
			if (!packet.IsDecoded && this.ShouldDecodePayload(text2))
			{
				args = packet.Decode(this.Socket.Manager.Encoder);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.Call(text2, packet, args);
			}
		}

		public void Clear()
		{
			this.Table.Clear();
		}

		private bool ShouldDecodePayload(string eventName)
		{
			List<EventDescriptor> list;
			if (this.Table.TryGetValue(eventName, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].AutoDecodePayload && list[i].Callbacks.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasSubsciber(string eventName)
		{
			return this.Table.ContainsKey(eventName);
		}
	}
}
