using System;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
	public bool isGrounded;

	public bool sendPackets;

	private int ticcounter;

	private int ptimer;

	public int uid;

	public int ownerid;

	public Vector3 PrevPos;

	public Vector3 PrevRot;

	public Vector3 NewPos;

	public Vector3 NewRot;

	private float pickupradius = 1.1f;

	private float freezeradius = 0.1f;

	private Transform trPlayer;

	public int wid;

	public int customskin;

	public int ammocount;

	public int clip;

	public bool playerCollision;

	private void Start()
	{
		this.trPlayer = GameObject.Find("LocalPlayer").transform;
		this.ptimer = 0;
	}

	private void FixedUpdate()
	{
		if (this.trPlayer != null)
		{
			if (Mathf.Abs(this.trPlayer.position.x - base.transform.position.x) < this.pickupradius && Mathf.Abs(this.trPlayer.position.y + 0.8f - base.transform.position.y) < this.pickupradius && Mathf.Abs(this.trPlayer.position.z - base.transform.position.z) < this.pickupradius)
			{
				this.playerCollision = true;
				if ((this.wid != 49 && this.uid != BasePlayer.lastdroppeduid && BasePlayer.weapon[WeaponData.GetData(this.wid).slot] == null) || (this.wid == 49 && BasePlayer.team == 1 && BasePlayer.defuse == 0))
				{
					if (this.ptimer == 0)
					{
						this.ptimer = 60;
						Client.cs.send_weapon_pickup(this.uid);
					}
					else
					{
						this.ptimer--;
						if (this.ptimer < 0)
						{
							this.ptimer = 0;
						}
					}
				}
			}
			else
			{
				if (this.uid == BasePlayer.lastdroppeduid)
				{
					BasePlayer.lastdroppeduid = -1;
				}
				this.playerCollision = false;
			}
		}
		if (this.isGrounded || this.ownerid != Client.ID)
		{
			return;
		}
		if (this.ticcounter < 70)
		{
			this.ticcounter++;
			return;
		}
		this.ticcounter = 0;
		if (Mathf.Abs(base.transform.position.x - this.PrevPos.x) < this.freezeradius && Mathf.Abs(base.transform.position.y - this.PrevPos.y) < this.freezeradius && Mathf.Abs(base.transform.position.z - this.PrevPos.z) < this.freezeradius)
		{
			this.isGrounded = true;
			UnityEngine.Object.Destroy(base.GetComponent<Rigidbody>());
			base.GetComponent<BoxCollider>().enabled = false;
			Client.cs.send_weapon_pos(1, this.uid, base.transform.position, base.transform.eulerAngles);
		}
		this.PrevPos = base.transform.position;
		if (!this.sendPackets)
		{
			return;
		}
	}
}
