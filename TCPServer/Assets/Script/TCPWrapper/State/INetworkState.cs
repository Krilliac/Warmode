
namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// INetworkState
    /// 2015/12/07 Hyeon
    /// 네트워크 상태 인터페이스
    /// </summary>
    public interface INetworkState
    {
        void Enter(NetworkManager networkManager);
        void Connect(string host, int port, IProtocolResolver protocolResolver);
        void Disconnect();
        void Reconnect();
        void Send(PacketStream packet);

    }
}
