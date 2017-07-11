using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
	public class RecvData
	{
		public byte[] Buffer;

		public int Len;

		public RecvData(byte[] _buffer, int _len)
		{
			this.Buffer = new byte[_len];
			for (int i = 0; i < _len; i++)
			{
				this.Buffer[i] = _buffer[i];
			}
			this.Len = _len;
		}

		~RecvData()
		{
			this.Buffer = null;
		}
	}

	public class UdpState
	{
		public IPEndPoint e;

		public UdpClient u;
	}

	public static UDPClient cs = null;

	private static List<UDPClient.RecvData> Tlist = new List<UDPClient.RecvData>();

	public static void AddMsg(byte[] buffer, int len)
	{
		List<UDPClient.RecvData> tlist = UDPClient.Tlist;
		lock (tlist)
		{
			UDPClient.Tlist.Add(new UDPClient.RecvData(buffer, len));
		}
	}

	public void PostAwake()
	{
		UDPClient.cs = this;
	}

	private void Update()
	{
		List<UDPClient.RecvData> tlist = UDPClient.Tlist;
		lock (tlist)
		{
			for (int i = 0; i < UDPClient.Tlist.Count; i++)
			{
				this.ProcessData(UDPClient.Tlist[i].Buffer, UDPClient.Tlist[i].Len);
			}
			UDPClient.Tlist.Clear();
		}
	}

	public void send_serverinfo(string ip, int port)
	{
		UdpClient udpClient = new UdpClient();
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		UDPClient.UdpState udpState = new UDPClient.UdpState();
		udpState.e = iPEndPoint;
		udpState.u = udpClient;
		try
		{
			NET.BEGIN_WRITE();
			NET.WRITE_BYTE(244);
			NET.WRITE_BYTE(0);
			NET.WRITE_BYTE(0);
			NET.WRITE_BYTE(0);
			NET.WRITE_FLOAT(Time.time);
			NET.END_WRITE();
			udpClient.Send(NET.WRITE_DATA(), NET.WRITE_LEN(), iPEndPoint);
		}
		catch
		{
			UnityEngine.Debug.LogWarning("[udp] send error");
		}
		udpClient.BeginReceive(new AsyncCallback(UDPClient.ReceiveCallback), udpState);
		base.StartCoroutine(this.forceclose(udpClient));
	}

	public static void ReceiveCallback(IAsyncResult ar)
	{
		UdpClient u = ((UDPClient.UdpState)ar.AsyncState).u;
		IPEndPoint e = ((UDPClient.UdpState)ar.AsyncState).e;
		byte[] array = u.EndReceive(ar, ref e);
		UDPClient.AddMsg(array, array.Length);
		u.Close();
	}

	[DebuggerHidden]
	private IEnumerator forceclose(UdpClient u)
	{
		UDPClient.<forceclose>c__IteratorD <forceclose>c__IteratorD = new UDPClient.<forceclose>c__IteratorD();
		<forceclose>c__IteratorD.u = u;
		<forceclose>c__IteratorD.<$>u = u;
		return <forceclose>c__IteratorD;
	}

	private void ProcessData(byte[] buffer, int len)
	{
		if (len < 4)
		{
			return;
		}
		if (buffer[0] != 244)
		{
			return;
		}
		byte b = buffer[1];
		if (b == 0)
		{
			this.recv_serverinfo(buffer, len);
		}
	}

	private void recv_serverinfo(byte[] buffer, int len)
	{
		NET.BEGIN_READ(buffer, len, 4);
		int port = NET.READ_SHORT();
		int players = NET.READ_BYTE();
		int gamemode = NET.READ_BYTE();
		string mapname = NET.READ_STRING();
		int rate = NET.READ_BYTE();
		float num = NET.READ_FLOAT();
		float pingTime = Time.time - num;
		MenuServers.UpdateServer(port, players, gamemode, mapname, rate, pingTime);
	}
}
