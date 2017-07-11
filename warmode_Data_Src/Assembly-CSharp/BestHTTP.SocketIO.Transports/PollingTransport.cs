using BestHTTP.Logger;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Transports
{
	internal sealed class PollingTransport : ITransport
	{
		private HTTPRequest LastRequest;

		private HTTPRequest PollRequest;

		private Packet PacketWithAttachment;

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
			get
			{
				return this.LastRequest != null;
			}
		}

		public PollingTransport(SocketManager manager)
		{
			this.Manager = manager;
		}

		public void Open()
		{
			string arg_A4_0 = "{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true";
			object[] expr_0B = new object[6];
			expr_0B[0] = this.Manager.Uri.ToString();
			expr_0B[1] = 4;
			expr_0B[2] = this.Manager.Timestamp.ToString();
			int arg_5E_1 = 3;
			SocketManager expr_45 = this.Manager;
			ulong num;
			expr_45.RequestCounter = (num = expr_45.RequestCounter) + 1uL;
			num = num;
			expr_0B[arg_5E_1] = num.ToString();
			expr_0B[4] = this.Manager.Handshake.Sid;
			expr_0B[5] = (this.Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : this.Manager.Options.BuildQueryParams());
			new HTTPRequest(new Uri(string.Format(arg_A4_0, expr_0B)), new OnRequestFinishedDelegate(this.OnRequestFinished))
			{
				DisableCache = true,
				DisableRetry = true
			}.Send();
			this.State = TransportStates.Opening;
		}

		public void Close()
		{
			if (this.State == TransportStates.Closed)
			{
				return;
			}
			this.State = TransportStates.Closed;
		}

		public void Send(Packet packet)
		{
			this.Send(new List<Packet>
			{
				packet
			});
		}

		public void Send(List<Packet> packets)
		{
			if (this.State != TransportStates.Open)
			{
				throw new Exception("Transport is not in Open state!");
			}
			if (this.IsRequestInProgress)
			{
				throw new Exception("Sending packets are still in progress!");
			}
			byte[] array = null;
			try
			{
				array = packets[0].EncodeBinary();
				for (int i = 1; i < packets.Count; i++)
				{
					byte[] array2 = packets[i].EncodeBinary();
					Array.Resize<byte>(ref array, array.Length + array2.Length);
					Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
				}
				packets.Clear();
			}
			catch (Exception ex)
			{
				((IManager)this.Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				return;
			}
			string arg_161_0 = "{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true";
			object[] expr_C4 = new object[6];
			expr_C4[0] = this.Manager.Uri.ToString();
			expr_C4[1] = 4;
			expr_C4[2] = this.Manager.Timestamp.ToString();
			int arg_11B_1 = 3;
			SocketManager expr_FF = this.Manager;
			ulong num;
			expr_FF.RequestCounter = (num = expr_FF.RequestCounter) + 1uL;
			num = num;
			expr_C4[arg_11B_1] = num.ToString();
			expr_C4[4] = this.Manager.Handshake.Sid;
			expr_C4[5] = (this.Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : this.Manager.Options.BuildQueryParams());
			this.LastRequest = new HTTPRequest(new Uri(string.Format(arg_161_0, expr_C4)), HTTPMethods.Post, new OnRequestFinishedDelegate(this.OnRequestFinished));
			this.LastRequest.DisableCache = true;
			this.LastRequest.SetHeader("Content-Type", "application/octet-stream");
			this.LastRequest.RawData = array;
			this.LastRequest.Send();
		}

		private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			this.LastRequest = null;
			if (this.State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					this.ParseResponse(resp);
				}
				else
				{
					text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", new object[]
					{
						resp.StatusCode,
						resp.Message,
						resp.DataAsText,
						req.CurrentUri
					});
				}
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)this.Manager).OnTransportError(this, text);
			}
		}

		public void Poll()
		{
			if (this.PollRequest != null)
			{
				return;
			}
			string arg_B1_0 = "{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true";
			object[] expr_18 = new object[6];
			expr_18[0] = this.Manager.Uri.ToString();
			expr_18[1] = 4;
			expr_18[2] = this.Manager.Timestamp.ToString();
			int arg_6B_1 = 3;
			SocketManager expr_52 = this.Manager;
			ulong num;
			expr_52.RequestCounter = (num = expr_52.RequestCounter) + 1uL;
			num = num;
			expr_18[arg_6B_1] = num.ToString();
			expr_18[4] = this.Manager.Handshake.Sid;
			expr_18[5] = (this.Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : this.Manager.Options.BuildQueryParams());
			this.PollRequest = new HTTPRequest(new Uri(string.Format(arg_B1_0, expr_18)), HTTPMethods.Get, new OnRequestFinishedDelegate(this.OnPollRequestFinished));
			this.PollRequest.DisableCache = true;
			this.PollRequest.DisableRetry = true;
			this.PollRequest.Send();
		}

		private void OnPollRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			this.PollRequest = null;
			if (this.State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					this.ParseResponse(resp);
				}
				else
				{
					text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", new object[]
					{
						resp.StatusCode,
						resp.Message,
						resp.DataAsText,
						req.CurrentUri
					});
				}
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)this.Manager).OnTransportError(this, text);
			}
		}

		private void OnPacket(Packet packet)
		{
			if (packet.AttachmentCount != 0 && !packet.HasAllAttachment)
			{
				this.PacketWithAttachment = packet;
				return;
			}
			TransportEventTypes transportEvent = packet.TransportEvent;
			if (transportEvent == TransportEventTypes.Message)
			{
				if (packet.SocketIOEvent == SocketIOEventTypes.Connect && this.State == TransportStates.Opening)
				{
					this.State = TransportStates.Open;
					if (!((IManager)this.Manager).OnTransportConnected(this))
					{
						return;
					}
				}
			}
			((IManager)this.Manager).OnPacket(packet);
		}

		private void ParseResponse(HTTPResponse resp)
		{
			try
			{
				if (resp != null && resp.Data != null && resp.Data.Length >= 1)
				{
					string dataAsText = resp.DataAsText;
					if (!(dataAsText == "ok"))
					{
						int num = dataAsText.IndexOf(':', 0);
						int num2 = 0;
						while (num >= 0 && num < dataAsText.Length)
						{
							int num3 = int.Parse(dataAsText.Substring(num2, num - num2));
							string text = dataAsText.Substring(++num, num3);
							if (text.Length > 2 && text[0] == 'b' && text[1] == '4')
							{
								byte[] data = Convert.FromBase64String(text.Substring(2));
								if (this.PacketWithAttachment != null)
								{
									this.PacketWithAttachment.AddAttachmentFromServer(data, true);
									if (this.PacketWithAttachment.HasAllAttachment)
									{
										try
										{
											this.OnPacket(this.PacketWithAttachment);
										}
										catch (Exception ex)
										{
											HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket with attachment", ex);
											((IManager)this.Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
										}
										finally
										{
											this.PacketWithAttachment = null;
										}
									}
								}
							}
							else
							{
								try
								{
									Packet packet = new Packet(text);
									this.OnPacket(packet);
								}
								catch (Exception ex2)
								{
									HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket", ex2);
									((IManager)this.Manager).EmitError(SocketIOErrors.Internal, ex2.Message + " " + ex2.StackTrace);
								}
							}
							num2 = num + num3;
							num = dataAsText.IndexOf(':', num2);
						}
					}
				}
			}
			catch (Exception ex3)
			{
				((IManager)this.Manager).EmitError(SocketIOErrors.Internal, ex3.Message + " " + ex3.StackTrace);
				HTTPManager.Logger.Exception("PollingTransport", "ParseResponse", ex3);
			}
		}
	}
}
