using System;

namespace BestHTTP.SocketIO
{
	public enum SocketIOErrors
	{
		UnknownTransport,
		UnknownSid,
		BadHandshakeMethod,
		BadRequest,
		Internal,
		User
	}
}
