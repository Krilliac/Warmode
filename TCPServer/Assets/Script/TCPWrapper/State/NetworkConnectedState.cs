
namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/08 Hyeon
    /// NetworkConnectedState
    /// 네트워크 연결 상태
    /// </summary>
    public class NetworkConnectedState : INetworkState
    {
        private IClientSession mClientSession = null;

        public NetworkConnectedState(IClientSession clientSession)
        {
            this.mClientSession = clientSession;
        }

        public void Enter(NetworkManager networkManager)
        {
            networkManager.OnConnectEventFire();
        }

        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
            
        }

        public void Disconnect()
        {
            mClientSession.Disconnect();
        }

        public void Reconnect()
        {
        }

        public void Send(PacketStream packet)
        {
            if (mClientSession == null)
                return;

            mClientSession.Send(packet);
        }


    }
}
