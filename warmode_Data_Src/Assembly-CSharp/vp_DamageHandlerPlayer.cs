using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class vp_DamageHandlerPlayer : MonoBehaviour
{
	private byte id = 255;

	private byte hitzone;

	public void Set(byte _id, byte _hitzone)
	{
		this.id = _id;
		this.hitzone = _hitzone;
	}

	public virtual void Damage()
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (this.id == 255)
		{
			return;
		}
		Vector3 origin = vp_HitscanBullet.lastray.origin;
		Vector3 point = vp_HitscanBullet.lasthit.point;
		int num = 0;
		if (BasePlayer.weapon[0] != null)
		{
			num = BasePlayer.weapon[0].clip;
		}
		else if (BasePlayer.weapon[1] != null)
		{
			num = BasePlayer.weapon[1].clip;
		}
		Client.cs.send_takedamage(this.id, this.hitzone, (byte)num, origin.x, origin.y, origin.z, point.x, point.y, point.z);
		if (this.hitzone == 1)
		{
			this.NoDelayHitHS();
		}
		else
		{
			this.NoDelayHit();
		}
		Crosshair.SetHit();
		if (this.hitzone == 1)
		{
			PlayerControll.SetBlood(vp_HitscanBullet.lasthit.point.x, vp_HitscanBullet.lasthit.point.y, vp_HitscanBullet.lasthit.point.z, 1f);
		}
		else
		{
			PlayerControll.SetBlood(vp_HitscanBullet.lasthit.point.x, vp_HitscanBullet.lasthit.point.y, vp_HitscanBullet.lasthit.point.z, 0.5f);
		}
		PlayerControll.Player[(int)this.id].hitReaction.Hit(vp_HitscanBullet.lasthit.collider, vp_HitscanBullet.lastray.direction * 1f, vp_HitscanBullet.lasthit.point);
	}

	protected virtual void RemoveBulletHoles()
	{
		foreach (Transform transform in base.transform)
		{
			Component[] components = transform.GetComponents<vp_HitscanBullet>();
			if (components.Length != 0)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayHit()
	{
		return new vp_DamageHandlerPlayer.<DelayHit>c__IteratorF();
	}

	private void NoDelayHit()
	{
		BasePlayer.go.GetComponent<AudioSource>().PlayOneShot(SND.GetSoundByName("player/enemy_hit"));
	}

	private void NoDelayHitHS()
	{
		BasePlayer.go.GetComponent<AudioSource>().PlayOneShot(SND.GetSoundByName("player/enemy_headshot"));
	}
}
