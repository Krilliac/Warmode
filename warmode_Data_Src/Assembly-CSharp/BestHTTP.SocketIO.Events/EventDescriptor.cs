using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Events
{
	internal sealed class EventDescriptor
	{
		private SocketIOCallback[] CallbackArray;

		public List<SocketIOCallback> Callbacks
		{
			get;
			private set;
		}

		public bool OnlyOnce
		{
			get;
			private set;
		}

		public bool AutoDecodePayload
		{
			get;
			private set;
		}

		public EventDescriptor(bool onlyOnce, bool autoDecodePayload, SocketIOCallback callback)
		{
			this.OnlyOnce = onlyOnce;
			this.AutoDecodePayload = autoDecodePayload;
			this.Callbacks = new List<SocketIOCallback>(1);
			if (callback != null)
			{
				this.Callbacks.Add(callback);
			}
		}

		public void Call(Socket socket, Packet packet, params object[] args)
		{
			if (this.CallbackArray == null || this.CallbackArray.Length < this.Callbacks.Count)
			{
				Array.Resize<SocketIOCallback>(ref this.CallbackArray, this.Callbacks.Count);
			}
			this.Callbacks.CopyTo(this.CallbackArray);
			for (int i = 0; i < this.CallbackArray.Length; i++)
			{
				try
				{
					this.CallbackArray[i](socket, packet, args);
				}
				catch (Exception ex)
				{
					((ISocket)socket).EmitError(SocketIOErrors.User, ex.Message + " " + ex.StackTrace);
					HTTPManager.Logger.Exception("EventDescriptor", "Call", ex);
				}
				if (this.OnlyOnce)
				{
					this.Callbacks.Remove(this.CallbackArray[i]);
				}
				this.CallbackArray[i] = null;
			}
		}
	}
}
