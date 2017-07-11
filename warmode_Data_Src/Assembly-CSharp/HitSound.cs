using System;
using UnityEngine;

public class HitSound : MonoBehaviour
{
	public static AudioClip[] hit = new AudioClip[3];

	public static AudioClip[] hitZombie = new AudioClip[3];

	public static AudioClip[] death = new AudioClip[4];

	public static AudioClip[] deathZombie = new AudioClip[2];

	public static AudioSource a = null;

	public void PostAwake()
	{
		HitSound.hit[0] = SND.GetSoundByName("player/player_hit0");
		HitSound.hit[1] = SND.GetSoundByName("player/player_hit1");
		HitSound.hit[2] = SND.GetSoundByName("player/player_hit2");
		HitSound.hitZombie[0] = SND.GetSoundByName("zombie_hit01");
		HitSound.hitZombie[1] = SND.GetSoundByName("zombie_hit02");
		HitSound.hitZombie[2] = SND.GetSoundByName("zombie_hit03");
		HitSound.death[0] = SND.GetSoundByName("player/player_die1");
		HitSound.death[1] = SND.GetSoundByName("player/player_die2");
		HitSound.death[2] = SND.GetSoundByName("player/player_die3");
		HitSound.death[3] = SND.GetSoundByName("player/player_die4");
		HitSound.deathZombie[0] = SND.GetSoundByName("zombie_die01");
		HitSound.deathZombie[1] = SND.GetSoundByName("zombie_die02");
		HitSound.a = base.gameObject.AddComponent<AudioSource>();
		HitSound.a.loop = false;
		HitSound.a.playOnAwake = false;
	}

	public static void SetHit()
	{
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			int num = UnityEngine.Random.Range(0, 3);
			HitSound.a.PlayOneShot(HitSound.hitZombie[num]);
		}
		else
		{
			int num2 = UnityEngine.Random.Range(0, 3);
			HitSound.a.PlayOneShot(HitSound.hit[num2]);
		}
	}

	public static void SetDeath()
	{
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			int num = UnityEngine.Random.Range(0, HitSound.deathZombie.Length);
			HitSound.a.PlayOneShot(HitSound.deathZombie[num]);
		}
		else
		{
			int num2 = UnityEngine.Random.Range(0, 4);
			HitSound.a.PlayOneShot(HitSound.death[num2]);
		}
	}
}
