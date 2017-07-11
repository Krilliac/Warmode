using System;
using System.Collections.Generic;
using UNITY_TCPCLIENT;

[Serializable]
public class EchoPacket : IProtocol
{
    public ushort Id
    {
        get
        {
            return 1;
        }
    }
    public int Arg1 { get; set; }
    public string Arg2 { get; set; }
    public IList<int>  Args{ get; set; }
}