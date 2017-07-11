using System;
using UnityEngine;

public sealed class vp_Layer
{
	public static class Mask
	{
		public const int BulletBlockers = -1843396629;

		public const int BulletBlockersPeople = -1876951061;

		public const int BulletBlockersZombie = -1843658773;

		public const int ExternalBlockers = -1753219094;

		public const int ExternalBlockersZombie = -1787035670;

		public const int PhysicsBlockers = 1342177280;

		public const int IgnoreWalkThru = -746586133;
	}

	public const int Default = 0;

	public const int TransparentFX = 1;

	public const int IgnoreRaycast = 2;

	public const int Water = 4;

	public const int Zombie = 18;

	public const int MapShadows = 20;

	public const int Player3rd = 21;

	public const int WeaponDrop = 22;

	public const int PlayerRagDoll = 23;

	public const int PlayerClip = 24;

	public const int Enemy = 25;

	public const int Pickup = 26;

	public const int Trigger = 27;

	public const int MovableObject = 28;

	public const int Debris = 29;

	public const int LocalPlayer = 30;

	public const int Weapon = 31;

	public static readonly vp_Layer instance;

	private vp_Layer()
	{
	}

	static vp_Layer()
	{
		vp_Layer.instance = new vp_Layer();
		Physics.IgnoreLayerCollision(30, 29);
		Physics.IgnoreLayerCollision(30, 23);
		Physics.IgnoreLayerCollision(29, 29);
		Physics.IgnoreLayerCollision(29, 21);
		Physics.IgnoreLayerCollision(29, 24);
		Physics.IgnoreLayerCollision(29, 25);
		Physics.IgnoreLayerCollision(29, 18);
		Physics.IgnoreLayerCollision(29, 20);
		Physics.IgnoreLayerCollision(30, 22);
		Physics.IgnoreLayerCollision(21, 22);
		Physics.IgnoreLayerCollision(30, 25);
		Physics.IgnoreLayerCollision(30, 18);
		Physics.IgnoreLayerCollision(30, 20);
	}

	public static void Set(GameObject obj, int layer, bool recursive = false)
	{
		if (layer < 0 || layer > 31)
		{
			Debug.LogError("vp_Layer: Attempted to set layer id out of range [0-31].");
			return;
		}
		obj.layer = layer;
		if (recursive)
		{
			foreach (Transform transform in obj.transform)
			{
				vp_Layer.Set(transform.gameObject, layer, true);
			}
		}
	}

	public static int GetBulletBlockers()
	{
		if (ScoreBoard.gamemode != 3)
		{
			return -1843396629;
		}
		if (BasePlayer.team == 0)
		{
			return -1843658773;
		}
		return -1876951061;
	}

	public static int GetExternalBlockers()
	{
		if (ScoreBoard.gamemode != 3)
		{
			return -1753219094;
		}
		return -1787035670;
	}
}
