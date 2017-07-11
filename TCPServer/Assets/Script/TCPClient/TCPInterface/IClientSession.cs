using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UNITY_TCPCLIENT
{

    /// <summary>
    /// 2015/12/05 Hyeon
    /// IClientSession
    /// 어플리케이션 레벨에서 사용될 세션
    /// </summary>
    public interface IClientSession
    {
        TCPSession TcpSession { get; }
        void OnRead(PacketStream packet);
        void OnConnected();
        void OnDisconnected();
        void Send(PacketStream packet);
        void Disconnect();
    }
}
