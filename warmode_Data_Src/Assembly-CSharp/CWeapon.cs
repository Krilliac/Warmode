using System;

public class CWeapon
{
	public CWeaponData data;

	public int clip;

	public string sClip;

	public CWeapon(CWeaponData _data)
	{
		this.data = _data;
		this.clip = _data.maxClip;
		this.sClip = this.clip.ToString();
	}

	~CWeapon()
	{
	}
}
