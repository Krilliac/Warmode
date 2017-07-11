using BestHTTP.JSON;
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO
{
	public sealed class HandshakeData
	{
		public Action<HandshakeData> OnReceived;

		public Action<HandshakeData, string> OnError;

		private HTTPRequest HandshakeRequest;

		public string Sid
		{
			get;
			private set;
		}

		public List<string> Upgrades
		{
			get;
			private set;
		}

		public TimeSpan PingInterval
		{
			get;
			private set;
		}

		public TimeSpan PingTimeout
		{
			get;
			private set;
		}

		public SocketManager Manager
		{
			get;
			private set;
		}

		public HandshakeData(SocketManager manager)
		{
			this.Manager = manager;
		}

		internal void Start()
		{
			if (this.HandshakeRequest != null)
			{
				return;
			}
			string arg_79_0 = "{0}?EIO={1}&transport=polling&t={2}-{3}{4}&b64=true";
			object[] expr_18 = new object[5];
			expr_18[0] = this.Manager.Uri.ToString();
			expr_18[1] = 4;
			expr_18[2] = this.Manager.Timestamp;
			int arg_65_1 = 3;
			SocketManager expr_4F = this.Manager;
			ulong requestCounter;
			expr_4F.RequestCounter = (requestCounter = expr_4F.RequestCounter) + 1uL;
			expr_18[arg_65_1] = requestCounter;
			expr_18[4] = this.Manager.Options.BuildQueryParams();
			this.HandshakeRequest = new HTTPRequest(new Uri(string.Format(arg_79_0, expr_18)), new OnRequestFinishedDelegate(this.OnHandshakeCallback));
			this.HandshakeRequest.DisableCache = true;
			this.HandshakeRequest.Send();
			HTTPManager.Logger.Information("HandshakeData", "Handshake request sent");
		}

		internal void Abort()
		{
			if (this.HandshakeRequest != null)
			{
				this.HandshakeRequest.Abort();
			}
			this.HandshakeRequest = null;
			this.OnReceived = null;
			this.OnError = null;
		}

		private void OnHandshakeCallback(HTTPRequest req, HTTPResponse resp)
		{
			this.HandshakeRequest = null;
			HTTPRequestStates state = req.State;
			if (state != HTTPRequestStates.Finished)
			{
				if (state != HTTPRequestStates.Error)
				{
					this.RaiseOnError(req.State.ToString());
				}
				else
				{
					this.RaiseOnError((req.Exception == null) ? string.Empty : (req.Exception.Message + " " + req.Exception.StackTrace));
				}
			}
			else if (resp.IsSuccess)
			{
				HTTPManager.Logger.Information("HandshakeData", "Handshake data arrived: " + resp.DataAsText);
				int num = resp.DataAsText.IndexOf("{");
				if (num < 0)
				{
					this.RaiseOnError("Invalid handshake text: " + resp.DataAsText);
					return;
				}
				if (this.Parse(resp.DataAsText.Substring(num)) == null)
				{
					this.RaiseOnError("Parsing Handshake data failed: " + resp.DataAsText);
					return;
				}
				if (this.OnReceived != null)
				{
					this.OnReceived(this);
					this.OnReceived = null;
				}
			}
			else
			{
				this.RaiseOnError(string.Format("Handshake request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", new object[]
				{
					resp.StatusCode,
					resp.Message,
					resp.DataAsText,
					req.CurrentUri
				}));
			}
		}

		private void RaiseOnError(string err)
		{
			HTTPManager.Logger.Error("HandshakeData", "Handshake request failed with error: " + err);
			if (this.OnError != null)
			{
				this.OnError(this, err);
				this.OnError = null;
			}
		}

		private HandshakeData Parse(string str)
		{
			bool flag = false;
			Dictionary<string, object> from = Json.Decode(str, ref flag) as Dictionary<string, object>;
			if (!flag)
			{
				return null;
			}
			try
			{
				this.Sid = HandshakeData.GetString(from, "sid");
				this.Upgrades = HandshakeData.GetStringList(from, "upgrades");
				this.PingInterval = TimeSpan.FromMilliseconds((double)HandshakeData.GetInt(from, "pingInterval"));
				this.PingTimeout = TimeSpan.FromMilliseconds((double)HandshakeData.GetInt(from, "pingTimeout"));
			}
			catch
			{
				return null;
			}
			return this;
		}

		private static object Get(Dictionary<string, object> from, string key)
		{
			object result;
			if (!from.TryGetValue(key, out result))
			{
				throw new Exception(string.Format("Can't get {0} from Handshake data!", key));
			}
			return result;
		}

		private static string GetString(Dictionary<string, object> from, string key)
		{
			return HandshakeData.Get(from, key) as string;
		}

		private static List<string> GetStringList(Dictionary<string, object> from, string key)
		{
			List<object> list = HandshakeData.Get(from, key) as List<object>;
			List<string> list2 = new List<string>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i] as string;
				if (text != null)
				{
					list2.Add(text);
				}
			}
			return list2;
		}

		private static int GetInt(Dictionary<string, object> from, string key)
		{
			return (int)((double)HandshakeData.Get(from, key));
		}
	}
}
