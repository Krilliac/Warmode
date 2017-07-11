using System;

namespace BestHTTP.SocketIO.Events
{
	public static class EventNames
	{
		public const string Connect = "connect";

		public const string Disconnect = "disconnect";

		public const string Event = "event";

		public const string Ack = "ack";

		public const string Error = "error";

		public const string BinaryEvent = "binaryevent";

		public const string BinaryAck = "binaryack";

		private static string[] SocketIONames = new string[]
		{
			"unknown",
			"connect",
			"disconnect",
			"event",
			"ack",
			"error",
			"binaryevent",
			"binaryack"
		};

		private static string[] TransportNames = new string[]
		{
			"unknown",
			"open",
			"close",
			"ping",
			"pong",
			"message",
			"upgrade",
			"noop"
		};

		private static string[] BlacklistedEvents = new string[]
		{
			"connect",
			"connect_error",
			"connect_timeout",
			"disconnect",
			"error",
			"reconnect",
			"reconnect_attempt",
			"reconnect_failed",
			"reconnect_error",
			"reconnecting"
		};

		public static string GetNameFor(SocketIOEventTypes type)
		{
			return EventNames.SocketIONames[(int)(type + 1)];
		}

		public static string GetNameFor(TransportEventTypes transEvent)
		{
			return EventNames.TransportNames[(int)(transEvent + 1)];
		}

		public static bool IsBlacklisted(string eventName)
		{
			for (int i = 0; i < EventNames.BlacklistedEvents.Length; i++)
			{
				if (string.Compare(EventNames.BlacklistedEvents[i], eventName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
