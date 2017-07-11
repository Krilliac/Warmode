using System;
using System.Reflection;
using UnityEngine;

public class CC : MonoBehaviour
{
	public static void CheckOnce()
	{
		CC.Check1();
		CC.Check2();
	}

	private static void Check1()
	{
		Type type = Type.GetType("MemeHacks");
		if (type == null)
		{
			return;
		}
		if (type.GetMethod("SimpleESP", BindingFlags.Instance | BindingFlags.NonPublic) == null)
		{
			return;
		}
		if (Client.cs != null)
		{
			Client.cs.send_update();
		}
	}

	private static void Check2()
	{
		Type type = Type.GetType("OoohShiiitBaby");
		if (type == null)
		{
			return;
		}
		if (type.GetMethod("SpreadAssCumIsOnFloor", BindingFlags.Instance | BindingFlags.NonPublic) == null)
		{
			return;
		}
		if (Client.cs != null)
		{
			Client.cs.send_update();
		}
	}

	public static void CheckTest()
	{
		Type type = Type.GetType("Client");
		if (type == null)
		{
			return;
		}
		if (type.GetMethod("DoRead", BindingFlags.Instance | BindingFlags.NonPublic) == null)
		{
			return;
		}
	}
}
