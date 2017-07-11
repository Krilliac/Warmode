using System;
using UnityEngine;

public class vp_ExampleHealthPickup : vp_Pickup
{
	public float Health = 0.1f;

	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Health.Get() < 0f)
		{
			return false;
		}
		if (player.Health.Get() >= 1f)
		{
			return false;
		}
		player.Health.Set(Mathf.Min(1f, player.Health.Get() + this.Health));
		return true;
	}
}
