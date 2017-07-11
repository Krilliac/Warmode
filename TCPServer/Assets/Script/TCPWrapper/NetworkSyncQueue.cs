using UNITY_TCPCLIENT;


namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// NetworkSyncQueue
    /// IO Thread와 Unity Main Thread를 연결해주는 큐
    /// </summary>
    public class NetworkSyncQueue
    {
        /***
         * 패킷 큐
         * Push - IO Thread
         * Pop - Unity Main Thread
         */
        private LockFreeQueue<PacketStream> mReceivePacketQueue;

        /***
         * 네트워크 이벤트 큐
         * Push - IO Thread
         * Pop - Unity Main Thread
         */
        private LockFreeQueue<Const<TCPCommon.NETWORK_STATE>> mStateEventQueue;

        public NetworkSyncQueue()
        {
            mReceivePacketQueue = new LockFreeQueue<PacketStream>(7); // 128 Size
            mStateEventQueue = new LockFreeQueue<Const<TCPCommon.NETWORK_STATE>>(2); // 4 Size
        }

        public void PushNetworkStateEvent(Const<TCPCommon.NETWORK_STATE> stateEvent)
        {
            mStateEventQueue.Push(stateEvent);
        }

        public Const<TCPCommon.NETWORK_STATE> PopNetworkStateEvent()
        {
            return mStateEventQueue.Pop();
        }

        public bool HasNetworkStateEvent()
        {
            if (mStateEventQueue.getSize() <= 0)
                return false;

            return true;
        }

        public void PushReceivePacket(PacketStream packet)
        {
            mReceivePacketQueue.Push(packet);
        }

        public PacketStream PopReceivePacket()
        {
            return mReceivePacketQueue.Pop();
        }

        public bool HasReceivePacket()
        {
            if (mReceivePacketQueue.getSize() <= 0)
                return false;

            return true;
        }
    }
}
