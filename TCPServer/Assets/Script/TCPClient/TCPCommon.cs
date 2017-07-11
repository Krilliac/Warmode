namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// TCPCommon
    /// TCP Common 상수 정의
    /// </summary>
    public class TCPCommon
    {
        public enum NETWORK_STATE : byte
        {
            CONNECT,
            DISCONNECT,
            NONE
        };

        public static readonly int SEND_BUFFERQUEUE_POWER = 10;

        public static readonly int RECV_BUFFERQUEUE_POWER = 10;

        /***
         * Send 버퍼 크기
         */
        public static readonly int SEND_BUFFER_SIZE = 4096;

        /***
         * Recv 버퍼 크기
         */
        public static readonly int RECV_BUFFER_SIZE = 4096;

        /***
         * Packet Strema 버퍼 크기
         ***/
        public static readonly int PACKET_BUFFER_SIZE = 1024;
    }
}
