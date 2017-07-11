using System;
using UnityEngine;

public class act : MonoBehaviour
{
	public static void Potracheno(byte ctype)
	{
		Client.subversion = ctype;
	}

	private void Start()
	{
		int num = UnityEngine.Random.Range(10, 20);
		string text = string.Empty;
		for (int i = 0; i < num; i++)
		{
			text += this.GetRandomLetter();
		}
		base.gameObject.name = text;
	}

	public static void sp1()
	{
		act.Potracheno(1);
	}

	public static void sp2()
	{
		act.Potracheno(2);
	}

	public static void sp3()
	{
		act.Potracheno(3);
	}

	public static void sp4()
	{
		act.Potracheno(4);
	}

	public void p1()
	{
		act.sp1();
	}

	public void p2()
	{
		act.sp2();
	}

	public void p3()
	{
		act.sp3();
	}

	public void p4()
	{
		act.sp4();
	}

	private string GetRandomLetter()
	{
		string text = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";
		return text[UnityEngine.Random.Range(0, text.Length - 1)].ToString();
	}
}
