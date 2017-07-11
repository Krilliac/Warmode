using System;

namespace BestHTTP.SocketIO.Events
{
	public delegate void SocketIOCallback(Socket socket, Packet packet, params object[] args);
}
