using System;
using UnityEngine;

public class vp_Earthquake : MonoBehaviour
{
	protected Vector3 m_EarthQuakeForce = default(Vector3);

	protected float m_Endtime;

	protected Vector2 m_Magnitude = Vector2.zero;

	protected vp_EventHandler EventHandler;

	private vp_FPPlayerEventHandler m_Player;

	private vp_FPPlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && this.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)this.EventHandler;
			}
			return this.m_Player;
		}
	}

	protected virtual Vector3 OnValue_EarthQuakeForce
	{
		get
		{
			return this.m_EarthQuakeForce;
		}
		set
		{
			this.m_EarthQuakeForce = value;
		}
	}

	protected virtual void Awake()
	{
		this.EventHandler = (vp_EventHandler)UnityEngine.Object.FindObjectOfType(typeof(vp_EventHandler));
	}

	protected virtual void OnEnable()
	{
		if (this.EventHandler != null)
		{
			this.EventHandler.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.EventHandler != null)
		{
			this.EventHandler.Unregister(this);
		}
	}

	protected void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			this.UpdateEarthQuake();
		}
	}

	protected void UpdateEarthQuake()
	{
		if (!this.Player.Earthquake.Active)
		{
			this.m_EarthQuakeForce = Vector3.zero;
			return;
		}
		this.m_EarthQuakeForce = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(1f), this.m_Magnitude.x * (Vector3.right + Vector3.forward) * Mathf.Min(this.m_Endtime - Time.time, 1f) * Time.timeScale);
		this.m_EarthQuakeForce.y = 0f;
		if (UnityEngine.Random.value < 0.3f * Time.timeScale)
		{
			this.m_EarthQuakeForce.y = UnityEngine.Random.Range(0f, this.m_Magnitude.y * 0.35f) * Mathf.Min(this.m_Endtime - Time.time, 1f);
		}
	}

	protected virtual void OnStart_Earthquake()
	{
		Vector3 vector = (Vector3)this.Player.Earthquake.Argument;
		this.m_Magnitude.x = vector.x;
		this.m_Magnitude.y = vector.y;
		this.m_Endtime = Time.time + vector.z;
		this.Player.Earthquake.AutoDuration = vector.z;
	}

	protected virtual void OnMessage_BombShake(float impact)
	{
		this.Player.Earthquake.TryStart<Vector3>(new Vector3(impact * 0.5f, impact * 0.5f, 1f));
	}
}
