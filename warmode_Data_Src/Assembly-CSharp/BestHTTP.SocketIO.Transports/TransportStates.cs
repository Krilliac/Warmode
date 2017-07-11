using System;

namespace BestHTTP.SocketIO.Transports
{
	public enum TransportStates
	{
		Connecting,
		Opening,
		Open,
		Closed,
		Paused
	}
}
