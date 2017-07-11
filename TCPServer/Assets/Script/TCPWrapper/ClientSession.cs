using System;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// ClientSession
    /// 2015/12/05 Hyeon
    /// 클라이언트 세션
    /// </summary>
    public class ClientSession : IClientSession
    {
        #region FIELD
        private TCPSession mTcpSession;
        private WeakReference mTcpNetworkQueue;
        #endregion

        public ClientSession(TCPSession tcpSession, NetworkSyncQueue networkSyncQueue, IProtocolResolver protocolResolver)
        {
            this.mTcpNetworkQueue = new WeakReference(networkSyncQueue);
            this.mTcpSession = tcpSession;

            mTcpSession.SetClientSession(this);
            mTcpSession.SetProtocolResolver(protocolResolver);
        }

        TCPSession IClientSession.TcpSession { get { return mTcpSession; } }

        void IClientSession.OnRead(PacketStream packet)
        {
            if (mTcpNetworkQueue.IsAlive)
            {
                (mTcpNetworkQueue.Target as NetworkSyncQueue).PushReceivePacket(packet);
            }
        }

        void IClientSession.OnConnected()
        {
            if (mTcpNetworkQueue.IsAlive)
            {
                (mTcpNetworkQueue.Target as NetworkSyncQueue).PushNetworkStateEvent(new Const<TCPCommon.NETWORK_STATE>(TCPCommon.NETWORK_STATE.CONNECT));
            }
        }

        void IClientSession.OnDisconnected()
        {
            if (mTcpNetworkQueue.IsAlive)
            {
                (mTcpNetworkQueue.Target as NetworkSyncQueue).PushNetworkStateEvent(new Const<TCPCommon.NETWORK_STATE>(TCPCommon.NETWORK_STATE.DISCONNECT));
            }
        }

        void IClientSession.Send(PacketStream packet)
        {
            mTcpSession.SendRequest(packet);
        }

        void IClientSession.Disconnect()
        {
            mTcpSession.Disconnect();
        }
    }
}
