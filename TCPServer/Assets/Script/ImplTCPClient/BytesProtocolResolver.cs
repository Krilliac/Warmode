using System;
using UNITY_TCPCLIENT;

/// <summary>
/// 2015/12/05 Hyeon
/// BytesProtocolResolver
/// 바이트 프로토콜
/// </summary>
public class BytesProtocolResolver : IProtocolResolver
{
    PacketStream IProtocolResolver.PacketProtocolResolve(ArraySegment<byte> segmentBytes, out int packetSize)
    {
        packetSize = 0;

        // 헤더 사이즈 체크
        if (segmentBytes.Count < ClientCommon.HEADER_SIZE)
        {
            return null;
        }

        // Body 사이즈
        UInt16 bodyLen = BitConverter.ToUInt16(segmentBytes.Array, segmentBytes.Offset);
        // 패킷 사이즈
        packetSize = bodyLen + ClientCommon.HEADER_SIZE;

        // Body 사이즈 체크
        if (segmentBytes.Count < packetSize)
        {
            return null;
        }

        // 헤더의 Len을 제외한 만큼 패킷을 복사해서 넘겨주자.
        byte[] packetData = new byte[packetSize - ClientCommon.BODY_LENGTH_SIZE];
        int copyOffset = segmentBytes.Offset + ClientCommon.BODY_LENGTH_SIZE;
        int copyLength = packetSize - ClientCommon.BODY_LENGTH_SIZE;
        Array.Copy(segmentBytes.Array, copyOffset, packetData, 0, copyLength);
        PacketStream packet = new PacketStream(packetData, packetData.Length);

        return packet;
    }
}

