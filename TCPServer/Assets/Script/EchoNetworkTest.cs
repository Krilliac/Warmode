using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UNITY_TCPCLIENT;

/// <summary>
/// Echo 패킷 테스트 컴포넌트
/// </summary>
public class EchoNetworkTest : MonoBehaviour {

    public String IP = "127.0.0.1";
    public int PORT = 9001;
    public Text StateText;

    private NetworkManager mNetworkManager;
    
    void Start()
    {
        mNetworkManager = NetworkManager.getInstance();
        
        mNetworkManager.mReceiveEventCallback = TestRecv;
        mNetworkManager.mConnectCompleteEvent += () => { StateText.text = "Connect";  Debug.Log("Connect!"); };
        mNetworkManager.mDisconnectedCompleteEvent += () => { StateText.text = "Disconnect"; Debug.Log("Disconnect!"); };
        mNetworkManager.mConnectFailEvent += () => { StateText.text = "Connect Fail"; Debug.Log("Connect Fail!"); };
        mNetworkManager.mReconnectFailEvent += () => { StateText.text = "Reconnect Fail"; Debug.Log("Reconnect Fail!"); };
    }

    public void TestConnect()
    {
        mNetworkManager.Connect(IP, PORT, new BytesProtocolResolver());
    }

    public void TestReconnect()
    {
        mNetworkManager.Reconnect();
    }

    public void TestDisconnect()
    {
        mNetworkManager.Disconnect();
    }

    public void TestSend()
    {
        var origin = new EchoPacket
        {
            Arg1 = UnityEngine.Random.Range(1000, 10000),
            Arg2 = Guid.NewGuid().
                        ToString(),
            Args = new List<int>
                    {
                        UnityEngine.Random.Range(10,100),
                        UnityEngine.Random.Range(101,200),
                    }
 
        };


        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, origin);

                UInt16 size = (ushort)((ushort)stream.Length - (ushort)ClientCommon.BODY_LENGTH_SIZE);
                PacketStream packet = new PacketStream((int)(ClientCommon.BODY_LENGTH_SIZE + (int)stream.Length));
                packet.Push(size);
                packet.Push(stream.GetBuffer(), (int)stream.Length);

                mNetworkManager.Send(packet);
            }
        }
    }


    private void TestRecv(PacketStream packet)
    {
        using (var stream = new MemoryStream(packet.Buffer))
        {
            var formatter = new BinaryFormatter();
            var decode = (EchoPacket)formatter.Deserialize(stream);
            Debug.Log("ID : " + decode.Id);
            Debug.Log("Arg1 : " + decode.Arg1);
            Debug.Log("Arg2 : " + decode.Arg2);
            Debug.Log("Args[0] : " + decode.Args[0]);
            Debug.Log("Args[1] : " + decode.Args[1]);
        }
    }


}
