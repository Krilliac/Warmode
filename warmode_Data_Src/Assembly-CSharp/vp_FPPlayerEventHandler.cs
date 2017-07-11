using System;
using UnityEngine;

public class vp_FPPlayerEventHandler : vp_StateEventHandler
{
	public vp_Message<float> HUDDamageFlash;

	public vp_Message<string> HUDText;

	public vp_Value<Vector2> InputMoveVector;

	public vp_Value<bool> AllowGameplayInput;

	public vp_Value<float> Health;

	public vp_Value<Vector3> Position;

	public vp_Value<Vector2> Rotation;

	public vp_Value<Vector3> Forward;

	public vp_Activity Dead;

	public vp_Activity Run;

	public vp_Activity Jump;

	public vp_Activity Crouch;

	public vp_Activity Zoom;

	public vp_Activity Attack;

	public vp_Activity Reload;

	public vp_Activity<int> SetWeapon;

	public vp_Activity<Vector3> Earthquake;

	public vp_Attempt SetPrevWeapon;

	public vp_Attempt SetNextWeapon;

	public vp_Attempt<string> SetWeaponByName;

	public vp_Value<int> CurrentWeaponID;

	public vp_Value<string> CurrentWeaponName;

	public vp_Value<bool> CurrentWeaponWielded;

	public vp_Value<float> CurrentWeaponReloadDuration;

	public vp_Value<string> CurrentWeaponClipType;

	public vp_Message<string, int> GetItemCount;

	public vp_Message<float> FallImpact;

	public vp_Message<float> HeadImpact;

	public vp_Message<Vector3> ForceImpact;

	public vp_Message<float> GroundStomp;

	public vp_Message<float> BombShake;

	public vp_Value<Vector3> EarthQuakeForce;

	public vp_Message Stop;

	public vp_Value<Transform> Platform;

	public vp_Value<bool> Pause;

	protected override void Awake()
	{
		base.Awake();
		base.BindStateToActivity(this.Run);
		base.BindStateToActivity(this.Jump);
		base.BindStateToActivity(this.Crouch);
		base.BindStateToActivity(this.Zoom);
		base.BindStateToActivity(this.Reload);
		base.BindStateToActivity(this.Dead);
		base.BindStateToActivityOnStart(this.Attack);
		this.SetWeapon.AutoDuration = 1f;
		this.Reload.AutoDuration = 1f;
		this.Zoom.MinDuration = 0.2f;
		this.Crouch.MinDuration = 0f;
		this.Jump.MinPause = 0f;
		this.SetWeapon.MinPause = 0f;
	}
}
