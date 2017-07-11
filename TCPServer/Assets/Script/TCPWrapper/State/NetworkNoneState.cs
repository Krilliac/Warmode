
namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/08 Hyeon
    /// NetworkConnectedState
    /// 네트워크 무 상태
    /// </summary>
    public class NetworkNoneState : INetworkState
    {
        private NetworkManager mNetworkManager;

        public void Enter(NetworkManager networkManager)
        {
            this.mNetworkManager = networkManager;
        }

        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
            mNetworkManager.SettingConnector(host, port, protocolResolver);

            TCPConnector connector = new TCPConnector();
            connector.connectHandler = mNetworkManager.OnConnnectComplete;
            connector.failHandler = mNetworkManager.OnConnectFail;
            connector.Connect(host, port, protocolResolver);
        }

        public void Disconnect()
        {

        }

        public void Reconnect()
        {
        }

        public void Send(PacketStream packet)
        {
        }


    }
}
