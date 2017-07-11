using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Rigidbody))]
public class vp_MovingPlatform : MonoBehaviour
{
	public enum PathMoveType
	{
		PingPong,
		Loop,
		Target
	}

	public enum Direction
	{
		Forward,
		Backwards,
		Direct
	}

	protected class WaypointComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((Transform)x).name, ((Transform)y).name);
		}
	}

	public enum MovementInterpolationMode
	{
		EaseInOut,
		EaseIn,
		EaseOut,
		EaseOut2,
		Slerp,
		Lerp
	}

	public enum RotateInterpolationMode
	{
		SyncToMovement,
		EaseOut,
		CustomEaseOut,
		CustomRotate
	}

	protected Transform m_Transform;

	public vp_MovingPlatform.PathMoveType PathType;

	public GameObject PathWaypoints;

	public vp_MovingPlatform.Direction PathDirection;

	protected List<Transform> m_Waypoints = new List<Transform>();

	protected int m_NextWaypoint;

	protected Vector3 m_CurrentTargetPosition = Vector3.zero;

	protected Vector3 m_CurrentTargetAngle = Vector3.zero;

	protected int m_TargetedWayPoint;

	protected float m_TravelDistance;

	protected Vector3 m_OriginalAngle = Vector3.zero;

	protected int m_CurrentWaypoint;

	public int MoveAutoStartTarget = 1000;

	public float MoveSpeed = 0.1f;

	public float MoveReturnDelay;

	public float MoveCooldown;

	protected bool m_Moving;

	protected float m_NextAllowedMoveTime;

	protected float m_MoveTime;

	protected vp_Timer.Handle m_ReturnDelayTimer = new vp_Timer.Handle();

	protected Vector3 m_PrevPos = Vector3.zero;

	public vp_MovingPlatform.MovementInterpolationMode MoveInterpolationMode;

	protected AnimationCurve m_EaseInOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	protected AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float RotationEaseAmount = 0.1f;

	public Vector3 RotationSpeed = Vector3.zero;

	protected Vector3 m_PrevAngle = Vector3.zero;

	public vp_MovingPlatform.RotateInterpolationMode RotationInterpolationMode;

	public AudioClip SoundStart;

	public AudioClip SoundStop;

	public AudioClip SoundMove;

	public AudioClip SoundWaypoint;

	protected AudioSource m_Audio;

	public bool PhysicsSnapPlayerToTopOnIntersect = true;

	protected Rigidbody m_RigidBody;

	protected Collider m_Collider;

	protected Collider m_PlayerCollider;

	protected vp_FPPlayerEventHandler m_PlayerToPush;

	public float m_PhysicsPushForce = 2f;

	protected float m_PhysicsCurrentMoveVelocity;

	protected float m_PhysicsCurrentRotationVelocity;

	protected Dictionary<Collider, vp_FPPlayerEventHandler> m_KnownPlayers = new Dictionary<Collider, vp_FPPlayerEventHandler>();

	private void Start()
	{
		this.m_Transform = base.transform;
		this.m_Collider = base.GetComponentInChildren<Collider>();
		this.m_RigidBody = base.GetComponent<Rigidbody>();
		this.m_RigidBody.useGravity = false;
		this.m_RigidBody.isKinematic = true;
		this.m_NextWaypoint = 0;
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Audio.loop = true;
		this.m_Audio.clip = this.SoundMove;
		if (this.PathWaypoints == null)
		{
			return;
		}
		base.gameObject.layer = 28;
		foreach (Transform transform in this.PathWaypoints.transform)
		{
			if (vp_Utility.IsActive(transform.gameObject))
			{
				this.m_Waypoints.Add(transform);
				transform.gameObject.layer = 28;
			}
			if (transform.GetComponent<Renderer>() != null)
			{
				transform.GetComponent<Renderer>().enabled = false;
			}
			if (transform.GetComponent<Collider>() != null)
			{
				transform.GetComponent<Collider>().enabled = false;
			}
		}
		IComparer @object = new vp_MovingPlatform.WaypointComparer();
		this.m_Waypoints.Sort(new Comparison<Transform>(@object.Compare));
		if (this.m_Waypoints.Count > 0)
		{
			this.m_CurrentTargetPosition = this.m_Waypoints[this.m_NextWaypoint].position;
			this.m_CurrentTargetAngle = this.m_Waypoints[this.m_NextWaypoint].eulerAngles;
			this.m_Transform.position = this.m_CurrentTargetPosition;
			this.m_Transform.eulerAngles = this.m_CurrentTargetAngle;
			if (this.MoveAutoStartTarget > this.m_Waypoints.Count - 1)
			{
				this.MoveAutoStartTarget = this.m_Waypoints.Count - 1;
			}
		}
	}

	private void FixedUpdate()
	{
		this.UpdatePath();
		this.UpdateMovement();
		this.UpdateRotation();
		this.UpdateVelocity();
	}

	private void UpdatePath()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		if (this.GetDistanceLeft() < 0.01f && Time.time >= this.m_NextAllowedMoveTime)
		{
			switch (this.PathType)
			{
			case vp_MovingPlatform.PathMoveType.PingPong:
				if (this.PathDirection == vp_MovingPlatform.Direction.Backwards)
				{
					if (this.m_NextWaypoint == 0)
					{
						this.PathDirection = vp_MovingPlatform.Direction.Forward;
					}
				}
				else if (this.m_NextWaypoint == this.m_Waypoints.Count - 1)
				{
					this.PathDirection = vp_MovingPlatform.Direction.Backwards;
				}
				this.OnArriveAtWaypoint();
				this.GoToNextWaypoint();
				break;
			case vp_MovingPlatform.PathMoveType.Loop:
				this.OnArriveAtWaypoint();
				this.GoToNextWaypoint();
				break;
			case vp_MovingPlatform.PathMoveType.Target:
				if (this.m_NextWaypoint == this.m_TargetedWayPoint)
				{
					if (this.m_Moving)
					{
						this.OnStop();
					}
					else if (this.m_NextWaypoint != 0)
					{
						this.OnArriveAtDestination();
					}
				}
				else
				{
					if (this.m_Moving)
					{
						if (this.m_PhysicsCurrentMoveVelocity == 0f)
						{
							this.OnStart();
						}
						else
						{
							this.OnArriveAtWaypoint();
						}
					}
					this.GoToNextWaypoint();
				}
				break;
			}
		}
	}

	private void OnStart()
	{
		if (this.SoundStart != null)
		{
			this.m_Audio.PlayOneShot(this.SoundStart);
		}
	}

	private void OnArriveAtWaypoint()
	{
		if (this.SoundWaypoint != null)
		{
			this.m_Audio.PlayOneShot(this.SoundWaypoint);
		}
	}

	private void OnArriveAtDestination()
	{
		if (this.MoveReturnDelay > 0f && !this.m_ReturnDelayTimer.Active)
		{
			vp_Timer.In(this.MoveReturnDelay, delegate
			{
				this.GoTo(0);
			}, this.m_ReturnDelayTimer);
		}
	}

	private void OnStop()
	{
		this.m_Audio.Stop();
		if (this.SoundStop != null)
		{
			this.m_Audio.PlayOneShot(this.SoundStop);
		}
		this.m_Transform.position = this.m_CurrentTargetPosition;
		this.m_Transform.eulerAngles = this.m_CurrentTargetAngle;
		this.m_Moving = false;
		if (this.m_NextWaypoint == 0)
		{
			this.m_NextAllowedMoveTime = Time.time + this.MoveCooldown;
		}
	}

	private void UpdateMovement()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		switch (this.MoveInterpolationMode)
		{
		case vp_MovingPlatform.MovementInterpolationMode.EaseInOut:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_EaseInOutCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			break;
		case vp_MovingPlatform.MovementInterpolationMode.EaseIn:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.MoveTowards(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_MoveTime), default(Vector3));
			break;
		case vp_MovingPlatform.MovementInterpolationMode.EaseOut:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_LinearCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			break;
		case vp_MovingPlatform.MovementInterpolationMode.EaseOut2:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.MoveSpeed * 0.25f), default(Vector3));
			break;
		case vp_MovingPlatform.MovementInterpolationMode.Slerp:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.Slerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_LinearCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			break;
		case vp_MovingPlatform.MovementInterpolationMode.Lerp:
			this.m_Transform.position = vp_Utility.NaNSafeVector3(Vector3.MoveTowards(this.m_Transform.position, this.m_CurrentTargetPosition, this.MoveSpeed), default(Vector3));
			break;
		}
	}

	private void UpdateRotation()
	{
		switch (this.RotationInterpolationMode)
		{
		case vp_MovingPlatform.RotateInterpolationMode.SyncToMovement:
			if (this.m_Moving)
			{
				this.m_Transform.eulerAngles = vp_Utility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_OriginalAngle.x, this.m_CurrentTargetAngle.x, 1f - this.GetDistanceLeft() / this.m_TravelDistance), Mathf.LerpAngle(this.m_OriginalAngle.y, this.m_CurrentTargetAngle.y, 1f - this.GetDistanceLeft() / this.m_TravelDistance), Mathf.LerpAngle(this.m_OriginalAngle.z, this.m_CurrentTargetAngle.z, 1f - this.GetDistanceLeft() / this.m_TravelDistance)), default(Vector3));
			}
			break;
		case vp_MovingPlatform.RotateInterpolationMode.EaseOut:
			this.m_Transform.eulerAngles = vp_Utility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_Transform.eulerAngles.x, this.m_CurrentTargetAngle.x, this.m_LinearCurve.Evaluate(this.m_MoveTime)), Mathf.LerpAngle(this.m_Transform.eulerAngles.y, this.m_CurrentTargetAngle.y, this.m_LinearCurve.Evaluate(this.m_MoveTime)), Mathf.LerpAngle(this.m_Transform.eulerAngles.z, this.m_CurrentTargetAngle.z, this.m_LinearCurve.Evaluate(this.m_MoveTime))), default(Vector3));
			break;
		case vp_MovingPlatform.RotateInterpolationMode.CustomEaseOut:
			this.m_Transform.eulerAngles = vp_Utility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_Transform.eulerAngles.x, this.m_CurrentTargetAngle.x, this.RotationEaseAmount), Mathf.LerpAngle(this.m_Transform.eulerAngles.y, this.m_CurrentTargetAngle.y, this.RotationEaseAmount), Mathf.LerpAngle(this.m_Transform.eulerAngles.z, this.m_CurrentTargetAngle.z, this.RotationEaseAmount)), default(Vector3));
			break;
		case vp_MovingPlatform.RotateInterpolationMode.CustomRotate:
			this.m_Transform.Rotate(this.RotationSpeed);
			break;
		}
	}

	private void UpdateVelocity()
	{
		this.m_MoveTime += this.MoveSpeed * 0.01f;
		this.m_PhysicsCurrentMoveVelocity = (this.m_Transform.position - this.m_PrevPos).magnitude;
		this.m_PhysicsCurrentRotationVelocity = (this.m_Transform.eulerAngles - this.m_PrevAngle).magnitude;
		this.m_PrevPos = this.m_Transform.position;
		this.m_PrevAngle = this.m_Transform.eulerAngles;
	}

	public void GoTo(int targetWayPoint)
	{
		if (Time.time < this.m_NextAllowedMoveTime)
		{
			return;
		}
		if (this.PathType != vp_MovingPlatform.PathMoveType.Target)
		{
			return;
		}
		this.m_TargetedWayPoint = this.GetValidWaypoint(targetWayPoint);
		if (targetWayPoint > this.m_NextWaypoint)
		{
			if (this.PathDirection != vp_MovingPlatform.Direction.Direct)
			{
				this.PathDirection = vp_MovingPlatform.Direction.Forward;
			}
		}
		else if (this.PathDirection != vp_MovingPlatform.Direction.Direct)
		{
			this.PathDirection = vp_MovingPlatform.Direction.Backwards;
		}
		this.m_Moving = true;
	}

	protected float GetDistanceLeft()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return 0f;
		}
		return Vector3.Distance(this.m_Transform.position, this.m_Waypoints[this.m_NextWaypoint].position);
	}

	protected void GoToNextWaypoint()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		this.m_MoveTime = 0f;
		if (!this.m_Audio.isPlaying)
		{
			this.m_Audio.Play();
		}
		this.m_CurrentWaypoint = this.m_NextWaypoint;
		switch (this.PathDirection)
		{
		case vp_MovingPlatform.Direction.Forward:
			this.m_NextWaypoint = this.GetValidWaypoint(this.m_NextWaypoint + 1);
			break;
		case vp_MovingPlatform.Direction.Backwards:
			this.m_NextWaypoint = this.GetValidWaypoint(this.m_NextWaypoint - 1);
			break;
		case vp_MovingPlatform.Direction.Direct:
			this.m_NextWaypoint = this.m_TargetedWayPoint;
			break;
		}
		this.m_OriginalAngle = this.m_CurrentTargetAngle;
		this.m_CurrentTargetPosition = this.m_Waypoints[this.m_NextWaypoint].position;
		this.m_CurrentTargetAngle = this.m_Waypoints[this.m_NextWaypoint].eulerAngles;
		this.m_TravelDistance = this.GetDistanceLeft();
		this.m_Moving = true;
	}

	protected int GetValidWaypoint(int wayPoint)
	{
		if (wayPoint < 0)
		{
			return this.m_Waypoints.Count - 1;
		}
		if (wayPoint > this.m_Waypoints.Count - 1)
		{
			return 0;
		}
		return wayPoint;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!this.GetPlayer(col))
		{
			return;
		}
		this.TryPushPlayer();
		this.TryAutoStart();
	}

	private void OnTriggerStay(Collider col)
	{
		if (!this.PhysicsSnapPlayerToTopOnIntersect)
		{
			return;
		}
		if (!this.GetPlayer(col))
		{
			return;
		}
		this.TrySnapPlayerToTop();
	}

	private bool GetPlayer(Collider col)
	{
		if (!this.m_KnownPlayers.ContainsKey(col))
		{
			if (col.gameObject.layer != 30)
			{
				return false;
			}
			vp_FPPlayerEventHandler component = col.transform.root.GetComponent<vp_FPPlayerEventHandler>();
			if (component == null)
			{
				return false;
			}
			this.m_KnownPlayers.Add(col, component);
		}
		if (!this.m_KnownPlayers.TryGetValue(col, out this.m_PlayerToPush))
		{
			return false;
		}
		this.m_PlayerCollider = col;
		return true;
	}

	private void TryPushPlayer()
	{
		if (this.m_PlayerToPush == null || this.m_PlayerToPush.Platform == null)
		{
			return;
		}
		if (this.m_PlayerToPush.Position.Get().y > this.m_Collider.bounds.max.y)
		{
			return;
		}
		if (this.m_PlayerToPush.Platform.Get() == this.m_Transform)
		{
			return;
		}
		float num = this.m_PhysicsCurrentMoveVelocity;
		if (num == 0f)
		{
			num = this.m_PhysicsCurrentRotationVelocity * 0.1f;
		}
		if (num > 0f)
		{
			this.m_PlayerToPush.ForceImpact.Send(vp_Utility.HorizontalVector(-(this.m_Transform.position - this.m_PlayerCollider.bounds.center).normalized * num * this.m_PhysicsPushForce));
		}
	}

	private void TrySnapPlayerToTop()
	{
		if (this.m_PlayerToPush == null || this.m_PlayerToPush.Platform == null)
		{
			return;
		}
		if (this.m_PlayerToPush.Position.Get().y > this.m_Collider.bounds.max.y)
		{
			return;
		}
		if (this.m_PlayerToPush.Platform.Get() == this.m_Transform)
		{
			return;
		}
		if (this.RotationSpeed.x != 0f || this.RotationSpeed.z != 0f || this.m_CurrentTargetAngle.x != 0f || this.m_CurrentTargetAngle.z != 0f)
		{
			return;
		}
		if (this.m_Collider.bounds.max.x < this.m_PlayerCollider.bounds.max.x || this.m_Collider.bounds.max.z < this.m_PlayerCollider.bounds.max.z || this.m_Collider.bounds.min.x > this.m_PlayerCollider.bounds.min.x || this.m_Collider.bounds.min.z > this.m_PlayerCollider.bounds.min.z)
		{
			return;
		}
		Vector3 o = this.m_PlayerToPush.Position.Get();
		o.y = this.m_Collider.bounds.max.y - 0.1f;
		this.m_PlayerToPush.Position.Set(o);
	}

	private void TryAutoStart()
	{
		if (this.MoveAutoStartTarget == 0)
		{
			return;
		}
		if (this.m_PhysicsCurrentMoveVelocity > 0f || this.m_Moving)
		{
			return;
		}
		this.GoTo(this.MoveAutoStartTarget);
	}
}
