using System;

public class vp_ExampleAmmoPickup : vp_Pickup
{
	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		if (base.TryGive(player))
		{
			this.TryReloadIfEmpty(player);
			return true;
		}
		if (this.TryReloadIfEmpty(player))
		{
			base.TryGive(player);
			return true;
		}
		return false;
	}

	private bool TryReloadIfEmpty(vp_FPPlayerEventHandler player)
	{
		return !(player.CurrentWeaponClipType.Get() != this.InventoryName) && player.Reload.TryStart();
	}
}
