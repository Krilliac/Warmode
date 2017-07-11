using System;

namespace BestHTTP.SocketIO.Events
{
	public delegate void SocketIOAckCallback(Socket socket, Packet packet, params object[] args);
}
