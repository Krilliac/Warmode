using System;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// IProtocolResolver
    /// 프로토콜 메세지 파싱
    /// 프로토콜에 따라서 패킷을 쪼개서 리턴
    /// 범용성을 위해서 인터페이스로 나눔
    /// </summary>
    public interface IProtocolResolver
    {
        /// <summary>
        /// 패킷 파싱
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="bytes"></param>
        /// <param name="packet_size"></param>
        /// <returns></returns>
        PacketStream PacketProtocolResolve(ArraySegment<byte> segmentBytes, out int packet_size);

    }
}


