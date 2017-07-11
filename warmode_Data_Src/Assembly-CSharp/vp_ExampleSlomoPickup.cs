using System;
using UnityEngine;

public class vp_ExampleSlomoPickup : vp_Pickup
{
	private vp_FPPlayerEventHandler m_Player;

	protected override void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted)
		{
			if (this.m_Player != null && this.m_Player.Dead.Active && !this.m_RespawnTimer.Active)
			{
				this.Respawn();
				return;
			}
			if (Time.timeScale > 0.2f && !vp_TimeUtility.Paused)
			{
				vp_TimeUtility.FadeTimeScale(0.2f, 0.1f);
			}
			else if (!this.m_Audio.isPlaying)
			{
				this.Remove();
			}
		}
		else if (Time.timeScale < 1f && !vp_TimeUtility.Paused)
		{
			vp_TimeUtility.FadeTimeScale(1f, 0.05f);
		}
	}

	protected override bool TryGive(vp_FPPlayerEventHandler player)
	{
		this.m_Player = player;
		return !this.m_Depleted && Time.timeScale == 1f;
	}
}
