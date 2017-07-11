using System;
using UnityEngine;

public class RegularSend : MonoBehaviour
{
	private Transform trPlayer;

	private Transform trCamera;

	private vp_FPPlayerEventHandler csUPlayer;

	private static float sendtime;

	private float rot_x;

	private float rot_y;

	private Vector3 oldpos = Vector3.zero;

	private void Awake()
	{
		this.trPlayer = GameObject.Find("LocalPlayer").transform;
		this.trCamera = Camera.main.transform;
		this.csUPlayer = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
		RegularSend.sendtime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (!Client.Auth)
		{
			return;
		}
		if (BasePlayer.deadflag > 0)
		{
			return;
		}
		float num = Vector3.Distance(this.oldpos, this.trPlayer.position);
		if (num > 0.8f)
		{
			this.SendPos();
			return;
		}
		if (Time.realtimeSinceStartup > RegularSend.sendtime)
		{
			this.SendPos();
			return;
		}
	}

	private void SendPos()
	{
		RegularSend.sendtime = Time.realtimeSinceStartup + 0.1f;
		float num = Mathf.Sqrt(Mathf.Abs(this.trPlayer.position.x - this.oldpos.x) * Mathf.Abs(this.trPlayer.position.z - this.oldpos.z));
		this.oldpos.x = this.trPlayer.position.x;
		this.oldpos.y = this.trPlayer.position.y;
		this.oldpos.z = this.trPlayer.position.z;
		this.rot_x = this.trCamera.eulerAngles.x;
		this.rot_y = this.trPlayer.eulerAngles.y;
		int state = 0;
		int zoom = 0;
		if (vp_FPInput.speed > 0.1f)
		{
			if (num > 0.01f)
			{
				state = 2;
			}
			if (this.csUPlayer.Run.Active)
			{
				state = 3;
			}
			else if (this.csUPlayer.Crouch.Active)
			{
				state = 5;
			}
			else if (this.csUPlayer.Zoom.Active)
			{
				state = 1;
			}
			if (vp_FPInput.speed < 6f)
			{
				state = 1;
			}
		}
		else if (this.csUPlayer.Crouch.Active)
		{
			state = 4;
		}
		if (this.csUPlayer.Zoom.Active)
		{
			zoom = 1;
		}
		Client.cs.send_pos(this.oldpos.x, this.oldpos.y, this.oldpos.z, this.rot_x, this.rot_y, state, zoom, vp_FPController.cs.GetMoveVector().y, vp_FPController.cs.GetMoveVector().x);
	}

	public void SendPosOnce()
	{
		float num = Mathf.Sqrt(Mathf.Abs(this.trPlayer.position.x - this.oldpos.x) * Mathf.Abs(this.trPlayer.position.z - this.oldpos.z));
		this.oldpos.x = this.trPlayer.position.x;
		this.oldpos.y = this.trPlayer.position.y;
		this.oldpos.z = this.trPlayer.position.z;
		this.rot_x = this.trCamera.eulerAngles.x;
		this.rot_y = this.trPlayer.eulerAngles.y;
		int state = 0;
		int zoom = 0;
		if (vp_FPInput.speed > 0.1f)
		{
			if (num > 0.01f)
			{
				state = 2;
			}
			if (this.csUPlayer.Run.Active)
			{
				state = 3;
			}
			else if (this.csUPlayer.Crouch.Active)
			{
				state = 5;
			}
			else if (this.csUPlayer.Zoom.Active)
			{
				state = 1;
			}
			if (vp_FPInput.speed < 6f)
			{
				state = 1;
			}
		}
		else if (this.csUPlayer.Crouch.Active)
		{
			state = 4;
		}
		if (this.csUPlayer.Zoom.Active)
		{
			zoom = 1;
		}
		Client.cs.send_pos(this.oldpos.x, this.oldpos.y, this.oldpos.z, this.rot_x, this.rot_y, state, zoom, 0f, 0f);
	}
}
