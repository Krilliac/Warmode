using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class vp_StateEventHandler : vp_EventHandler
{
	private List<vp_Component> m_StateTargets = new List<vp_Component>();

	protected override void Awake()
	{
		base.Awake();
		vp_Component[] components = base.transform.root.GetComponents<vp_Component>();
		for (int i = 0; i < components.Length; i++)
		{
			vp_Component item = components[i];
			this.m_StateTargets.Add(item);
		}
	}

	protected void BindStateToActivity(vp_Activity a)
	{
		this.BindStateToActivityOnStart(a);
		this.BindStateToActivityOnStop(a);
	}

	protected void BindStateToActivityOnStart(vp_Activity a)
	{
		if (!this.ActivityInitialized(a))
		{
			return;
		}
		string s = a.EventName;
		a.StartCallbacks = (vp_Activity.Callback)Delegate.Combine(a.StartCallbacks, new vp_Activity.Callback(delegate
		{
			foreach (vp_Component current in this.m_StateTargets)
			{
				current.SetState(s, true, true, false);
			}
		}));
	}

	protected void BindStateToActivityOnStop(vp_Activity a)
	{
		if (!this.ActivityInitialized(a))
		{
			return;
		}
		string s = a.EventName;
		a.StopCallbacks = (vp_Activity.Callback)Delegate.Combine(a.StopCallbacks, new vp_Activity.Callback(delegate
		{
			foreach (vp_Component current in this.m_StateTargets)
			{
				current.SetState(s, false, true, false);
			}
		}));
	}

	public void RefreshActivityStates()
	{
		foreach (vp_Event current in this.m_HandlerEvents.Values)
		{
			if (current is vp_Activity || current.GetType().BaseType == typeof(vp_Activity))
			{
				foreach (vp_Component current2 in this.m_StateTargets)
				{
					current2.SetState(current.EventName, ((vp_Activity)current).Active, true, false);
				}
			}
		}
	}

	public void ResetActivityStates()
	{
		foreach (vp_Component current in this.m_StateTargets)
		{
			current.ResetState();
		}
	}

	public void SetState(string state, bool setActive = true, bool recursive = true, bool includeDisabled = false)
	{
		foreach (vp_Component current in this.m_StateTargets)
		{
			current.SetState(state, setActive, recursive, includeDisabled);
		}
	}

	private bool ActivityInitialized(vp_Activity a)
	{
		if (a == null)
		{
			Debug.LogError("Error: (" + this + ") Activity is null.");
			return false;
		}
		if (string.IsNullOrEmpty(a.EventName))
		{
			Debug.LogError("Error: (" + this + ") Activity not initialized. Make sure the event handler has run its Awake call before binding layers.");
			return false;
		}
		return true;
	}
}
