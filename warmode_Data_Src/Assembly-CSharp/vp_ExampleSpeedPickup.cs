using System;

public class vp_ExampleSpeedPickup : vp_Pickup
{
	protected vp_Timer.Handle m_Timer = new vp_Timer.Handle();

	protected override void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted && !this.m_Audio.isPlaying)
		{
			this.Remove();
		}
	}

	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (this.m_Timer.Active)
		{
			return false;
		}
		player.SetState("MegaSpeed", true, true, false);
		vp_Timer.In(this.RespawnDuration, delegate
		{
			player.SetState("MegaSpeed", false, true, false);
		}, this.m_Timer);
		return true;
	}
}
