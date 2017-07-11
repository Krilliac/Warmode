using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class vp_State
{
	public vp_StateManager StateManager;

	public string TypeName;

	public string Name;

	public TextAsset TextAsset;

	public vp_ComponentPreset Preset;

	public List<int> StatesToBlock;

	[NonSerialized]
	protected bool m_Enabled;

	[NonSerialized]
	protected List<vp_State> m_CurrentlyBlockedBy;

	public bool Enabled
	{
		get
		{
			return this.m_Enabled;
		}
		set
		{
			this.m_Enabled = value;
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.StateManager == null)
			{
				return;
			}
			if (this.m_Enabled)
			{
				this.StateManager.ImposeBlockingList(this);
			}
			else
			{
				this.StateManager.RelaxBlockingList(this);
			}
		}
	}

	public bool Blocked
	{
		get
		{
			return this.CurrentlyBlockedBy.Count > 0;
		}
	}

	public int BlockCount
	{
		get
		{
			return this.CurrentlyBlockedBy.Count;
		}
	}

	protected List<vp_State> CurrentlyBlockedBy
	{
		get
		{
			if (this.m_CurrentlyBlockedBy == null)
			{
				this.m_CurrentlyBlockedBy = new List<vp_State>();
			}
			return this.m_CurrentlyBlockedBy;
		}
	}

	public vp_State(string typeName, string name = "Untitled", string path = null, TextAsset asset = null)
	{
		this.TypeName = typeName;
		this.Name = name;
		this.TextAsset = asset;
	}

	public void AddBlocker(vp_State blocker)
	{
		if (!this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Add(blocker);
		}
	}

	public void RemoveBlocker(vp_State blocker)
	{
		if (this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Remove(blocker);
		}
	}
}
