using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Transports
{
	public interface ITransport
	{
		TransportStates State
		{
			get;
		}

		SocketManager Manager
		{
			get;
		}

		bool IsRequestInProgress
		{
			get;
		}

		void Open();

		void Poll();

		void Send(Packet packet);

		void Send(List<Packet> packets);

		void Close();
	}
}
