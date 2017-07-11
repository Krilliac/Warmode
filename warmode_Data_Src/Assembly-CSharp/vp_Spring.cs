using System;
using UnityEngine;

public class vp_Spring
{
	public enum UpdateMode
	{
		Position,
		PositionAdditive,
		Rotation,
		RotationAdditive,
		Scale,
		ScaleAdditive
	}

	protected delegate void UpdateDelegate();

	protected vp_Spring.UpdateMode Mode;

	protected bool m_AutoUpdate = true;

	protected vp_Spring.UpdateDelegate m_UpdateFunc;

	public Vector3 State = Vector3.zero;

	protected Vector3 m_Velocity = Vector3.zero;

	public Vector3 RestState = Vector3.zero;

	public Vector3 Stiffness = new Vector3(0.5f, 0.5f, 0.5f);

	public Vector3 Damping = new Vector3(0.75f, 0.75f, 0.75f);

	protected float m_VelocityFadeInCap = 1f;

	protected float m_VelocityFadeInEndTime;

	protected float m_VelocityFadeInLength;

	protected Vector3[] m_SoftForceFrame = new Vector3[120];

	public float MaxVelocity = 10000f;

	public float MinVelocity = 1E-07f;

	public Vector3 MaxState = new Vector3(10000f, 10000f, 10000f);

	public Vector3 MinState = new Vector3(-10000f, -10000f, -10000f);

	protected Transform m_Transform;

	public Transform Transform
	{
		set
		{
			this.m_Transform = value;
			this.RefreshUpdateMode();
		}
	}

	public vp_Spring(Transform transform, vp_Spring.UpdateMode mode, bool autoUpdate = true)
	{
		this.Mode = mode;
		this.Transform = transform;
		this.m_AutoUpdate = autoUpdate;
	}

	public void FixedUpdate()
	{
		if (this.m_VelocityFadeInEndTime > Time.time)
		{
			this.m_VelocityFadeInCap = Mathf.Clamp01(1f - (this.m_VelocityFadeInEndTime - Time.time) / this.m_VelocityFadeInLength);
		}
		else
		{
			this.m_VelocityFadeInCap = 1f;
		}
		if (this.m_SoftForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SoftForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SoftForceFrame[i] = ((i >= 119) ? Vector3.zero : this.m_SoftForceFrame[i + 1]);
				if (this.m_SoftForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.Calculate();
		this.m_UpdateFunc();
	}

	private void Position()
	{
		this.m_Transform.localPosition = this.State;
	}

	private void Rotation()
	{
		this.m_Transform.localEulerAngles = this.State;
	}

	private void Scale()
	{
		this.m_Transform.localScale = this.State;
	}

	private void PositionAdditive()
	{
		this.m_Transform.localPosition += this.State;
	}

	private void RotationAdditive()
	{
		this.m_Transform.localEulerAngles += this.State;
	}

	private void ScaleAdditive()
	{
		this.m_Transform.localScale += this.State;
	}

	private void None()
	{
	}

	protected void RefreshUpdateMode()
	{
		this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.None);
		switch (this.Mode)
		{
		case vp_Spring.UpdateMode.Position:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Position);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditive:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditive);
			}
			break;
		case vp_Spring.UpdateMode.Rotation:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Rotation);
			}
			break;
		case vp_Spring.UpdateMode.RotationAdditive:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.RotationAdditive);
			}
			break;
		case vp_Spring.UpdateMode.Scale:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Scale);
			}
			break;
		case vp_Spring.UpdateMode.ScaleAdditive:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.ScaleAdditive);
			}
			break;
		}
		this.RestState = this.State;
	}

	protected void Calculate()
	{
		if (this.State == this.RestState)
		{
			return;
		}
		this.m_Velocity += Vector3.Scale(this.RestState - this.State, this.Stiffness);
		this.m_Velocity = Vector3.Scale(this.m_Velocity, this.Damping);
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		if (this.m_Velocity.sqrMagnitude > this.MinVelocity * this.MinVelocity)
		{
			this.Move();
		}
		else
		{
			this.Reset();
		}
	}

	private void AddForceInternal(Vector3 force)
	{
		if (force != Vector3.zero && (force.x > 10f || force.y > 10f || force.z > 10f))
		{
			Debug.Log(string.Concat(new string[]
			{
				"Addforce: ",
				force.x.ToString(),
				" ",
				force.y.ToString(),
				" ",
				force.z.ToString()
			}));
		}
		force *= this.m_VelocityFadeInCap;
		this.m_Velocity += force;
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		this.Move();
	}

	public void AddForce(Vector3 force)
	{
		if (Time.timeScale < 1f)
		{
			this.AddSoftForce(force, 1f);
		}
		else
		{
			this.AddForceInternal(force);
		}
	}

	public void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			this.m_SoftForceFrame[i] += force / frames;
		}
	}

	protected void Move()
	{
		this.State += this.m_Velocity * Time.timeScale;
		this.State.x = Mathf.Clamp(this.State.x, this.MinState.x, this.MaxState.x);
		this.State.y = Mathf.Clamp(this.State.y, this.MinState.y, this.MaxState.y);
		this.State.z = Mathf.Clamp(this.State.z, this.MinState.z, this.MaxState.z);
	}

	public void Reset()
	{
		this.m_Velocity = Vector3.zero;
		this.State = this.RestState;
	}

	public void Stop(bool includeSoftForce = false)
	{
		this.m_Velocity = Vector3.zero;
		if (includeSoftForce)
		{
			this.StopSoftForce();
		}
	}

	public void StopSoftForce()
	{
		for (int i = 0; i < 120; i++)
		{
			this.m_SoftForceFrame[i] = Vector3.zero;
		}
	}

	public void ForceVelocityFadeIn(float seconds)
	{
		this.m_VelocityFadeInLength = seconds;
		this.m_VelocityFadeInEndTime = Time.time + seconds;
		this.m_VelocityFadeInCap = 0f;
	}
}
