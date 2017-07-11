using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Steam : MonoBehaviour
{
	public static bool active;

	public static bool logged;

	public static string username = string.Empty;

	public static ulong steamid;

	private static float timer;

	[DllImport("ws")]
	private static extern bool s_init();

	[DllImport("ws")]
	private static extern void s_user_init();

	[DllImport("ws")]
	private static extern bool s_user_loggedon();

	[DllImport("ws")]
	private static extern IntPtr s_user_name();

	[DllImport("ws")]
	private static extern ulong s_get_id();

	[DllImport("ws")]
	private static extern IntPtr s_get_session_ticket();

	[DllImport("ws")]
	public static extern bool html_open();

	[DllImport("ws")]
	public static extern bool GetMediumAvatar(byte[] img, int size);

	[DllImport("ws")]
	public static extern ulong s_get_tnx_orderid();

	[DllImport("ws")]
	private static extern void s_runcb();

	[DllImport("ws")]
	public static extern void s_a(uint a, uint b);

	[DllImport("ws")]
	public static extern void s_b();

	[DllImport("ws")]
	public static extern void s_c();

	[DllImport("ws")]
	public static extern bool GetFriend(ref IntPtr name, ref bool ingame, byte[] img, ref int state);

	public static void Init()
	{
		Steam.active = Steam.s_init();
		if (!Steam.active)
		{
			return;
		}
		Steam.logged = Steam.s_user_loggedon();
		if (!Steam.logged)
		{
			return;
		}
		Steam.s_user_init();
		Steam.username = Marshal.PtrToStringAnsi(Steam.s_user_name());
		BaseData.Name = Steam.username;
		Steam.steamid = Steam.s_get_id();
	}

	public static string GetTicket()
	{
		return Marshal.PtrToStringAnsi(Steam.s_get_session_ticket());
	}

	public static void RunCallBacks()
	{
		if (Time.time < Steam.timer + 0.1f)
		{
			return;
		}
		Steam.timer = Time.time;
		Steam.s_runcb();
	}
}
