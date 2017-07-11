using System;
using UnityEngine;

public class CWeaponData
{
	public int wid;

	public string wName;

	public int maxClip;

	public int maxBackpack;

	public int ammoType;

	public int weaponClass;

	public int cost;

	public int fragMoney;

	public float drawTime;

	public int slot;

	public bool canDrop;

	public float walkAcceleration;

	public float runAcceleration;

	public int zoomMode;

	public int buyMenuSlot;

	public int crosshairDynamic;

	public int hand;

	public bool customSkin;

	public AudioClip fire1;

	public Texture2D icon;

	public Texture2D icon2;

	public Texture2D icon2_inverted;

	public string selectName;

	public string sCost;

	public CWeaponData(int wid, string wName, int maxClip, int maxBackpack, int ammoType, int weaponClass, int cost, int fragMoney, float drawTime, int slot, bool canDrop, float walkAcceleration, float runAcceleration, int zoomMode, int buyMenuSlot, int crosshairDynamic, int hand, bool customSkin)
	{
		this.wid = wid;
		this.wName = wName;
		this.maxClip = maxClip;
		this.maxBackpack = maxBackpack;
		this.ammoType = ammoType;
		this.weaponClass = weaponClass;
		this.cost = cost;
		this.fragMoney = fragMoney;
		this.drawTime = drawTime;
		this.slot = slot;
		this.canDrop = canDrop;
		this.walkAcceleration = walkAcceleration;
		this.runAcceleration = runAcceleration;
		this.zoomMode = zoomMode;
		this.buyMenuSlot = buyMenuSlot;
		this.crosshairDynamic = crosshairDynamic;
		this.hand = hand;
		this.customSkin = customSkin;
		this.selectName = wid.ToString() + wName.ToUpper();
		this.sCost = cost.ToString();
	}

	~CWeaponData()
	{
	}
}
