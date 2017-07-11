using System;

public class vp_ExampleWeaponPickup : vp_Pickup
{
	public int AmmoIncluded;

	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		if (!base.TryGive(player))
		{
			return false;
		}
		player.SetWeaponByName.Try(this.InventoryName);
		return true;
	}
}
