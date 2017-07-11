using System;
using UnityEngine;

public class SH : MonoBehaviour
{
	private DateTime olddt;

	private long oldTick;

	private int errorCount;

	private void Start()
	{
		this.olddt = DateTime.Now;
		this.oldTick = (long)Environment.TickCount;
		base.InvokeRepeating("invSpeedHackX", 5f, 5f);
	}

	private void invSpeedHackX()
	{
		TimeSpan timeSpan = DateTime.Now - this.olddt;
		this.olddt = DateTime.Now;
		long num = (long)Environment.TickCount - this.oldTick;
		this.oldTick = (long)Environment.TickCount;
		if (timeSpan.TotalMilliseconds * 1.2999999523162842 < (double)num)
		{
			this.errorCount++;
		}
		if (this.errorCount > 5)
		{
			global::Console.cs.Command("disconnect");
		}
	}
}
