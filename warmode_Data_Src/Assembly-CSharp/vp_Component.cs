using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_Component : MonoBehaviour
{
	public bool Persist;

	protected vp_StateManager m_StateManager;

	public vp_EventHandler EventHandler;

	[NonSerialized]
	protected vp_State m_DefaultState;

	protected bool m_Initialized;

	protected Transform m_Transform;

	protected Transform m_Parent;

	protected Transform m_Root;

	public List<vp_State> States = new List<vp_State>();

	public List<vp_Component> Children = new List<vp_Component>();

	public List<vp_Component> Siblings = new List<vp_Component>();

	public List<vp_Component> Family = new List<vp_Component>();

	public List<Renderer> Renderers = new List<Renderer>();

	public List<AudioSource> AudioSources = new List<AudioSource>();

	protected AudioSource m_Audio;

	protected vp_Timer.Handle m_DeactivationTimer = new vp_Timer.Handle();

	public vp_StateManager StateManager
	{
		get
		{
			return this.m_StateManager;
		}
	}

	public vp_State DefaultState
	{
		get
		{
			return this.m_DefaultState;
		}
	}

	public float Delta
	{
		get
		{
			return Time.deltaTime * 60f;
		}
	}

	public float SDelta
	{
		get
		{
			return Time.smoothDeltaTime * 60f;
		}
	}

	public Transform Transform
	{
		get
		{
			return this.m_Transform;
		}
	}

	public Transform Parent
	{
		get
		{
			return this.m_Parent;
		}
	}

	public Transform Root
	{
		get
		{
			return this.m_Root;
		}
	}

	public AudioSource Audio
	{
		get
		{
			return this.m_Audio;
		}
	}

	public bool Rendering
	{
		get
		{
			return this.Renderers.Count > 0 && this.Renderers[0].enabled;
		}
		set
		{
			foreach (Renderer current in this.Renderers)
			{
				if (!(current == null))
				{
					current.enabled = value;
				}
			}
		}
	}

	protected virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_Parent = base.transform.parent;
		this.m_Root = base.transform.root;
		this.m_Audio = base.GetComponent<AudioSource>();
		this.EventHandler = (vp_EventHandler)this.m_Transform.root.GetComponentInChildren(typeof(vp_EventHandler));
		this.CacheChildren();
		this.CacheSiblings();
		this.CacheFamily();
		this.CacheRenderers();
		this.CacheAudioSources();
		this.m_StateManager = new vp_StateManager(this, this.States);
		this.StateManager.SetState("Default", base.enabled);
	}

	protected virtual void Start()
	{
		this.ResetState();
	}

	protected virtual void Init()
	{
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

	protected virtual void Update()
	{
		if (!this.m_Initialized)
		{
			this.Init();
			this.m_Initialized = true;
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void LateUpdate()
	{
	}

	public void SetState(string state, bool enabled = true, bool recursive = false, bool includeDisabled = false)
	{
		this.m_StateManager.SetState(state, enabled);
		if (recursive)
		{
			foreach (vp_Component current in this.Children)
			{
				if (includeDisabled || (vp_Utility.IsActive(current.gameObject) && current.enabled))
				{
					current.SetState(state, enabled, true, includeDisabled);
				}
			}
		}
	}

	public void ActivateGameObject(bool setActive = true)
	{
		if (setActive)
		{
			this.Activate();
			foreach (vp_Component current in this.Siblings)
			{
				current.Activate();
			}
		}
		else
		{
			this.DeactivateWhenSilent();
			foreach (vp_Component current2 in this.Siblings)
			{
				current2.DeactivateWhenSilent();
			}
		}
	}

	public void ResetState()
	{
		this.m_StateManager.Reset();
		this.Refresh();
	}

	public bool StateEnabled(string stateName)
	{
		return this.m_StateManager.IsEnabled(stateName);
	}

	public void RefreshDefaultState()
	{
		vp_State vp_State = null;
		if (this.States.Count == 0)
		{
			vp_State = new vp_State(base.GetType().Name, "Default", null, null);
			this.States.Add(vp_State);
		}
		else
		{
			for (int i = this.States.Count - 1; i > -1; i--)
			{
				if (this.States[i].Name == "Default")
				{
					vp_State = this.States[i];
					this.States.Remove(vp_State);
					this.States.Add(vp_State);
				}
			}
			if (vp_State == null)
			{
				vp_State = new vp_State(base.GetType().Name, "Default", null, null);
				this.States.Add(vp_State);
			}
		}
		if (vp_State.Preset == null || vp_State.Preset.ComponentType == null)
		{
			vp_State.Preset = new vp_ComponentPreset();
		}
		if (vp_State.TextAsset == null)
		{
			vp_State.Preset.InitFromComponent(this);
		}
		vp_State.Enabled = true;
		this.m_DefaultState = vp_State;
	}

	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		this.RefreshDefaultState();
		this.Refresh();
	}

	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromResources(this, path);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	public void CacheChildren()
	{
		this.Children.Clear();
		vp_Component[] componentsInChildren = base.GetComponentsInChildren<vp_Component>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			vp_Component vp_Component = componentsInChildren[i];
			if (vp_Component.transform.parent == base.transform)
			{
				this.Children.Add(vp_Component);
			}
		}
	}

	public void CacheSiblings()
	{
		this.Siblings.Clear();
		vp_Component[] components = base.GetComponents<vp_Component>();
		for (int i = 0; i < components.Length; i++)
		{
			vp_Component vp_Component = components[i];
			if (vp_Component != this)
			{
				this.Siblings.Add(vp_Component);
			}
		}
	}

	public void CacheFamily()
	{
		this.Family.Clear();
		vp_Component[] componentsInChildren = base.transform.root.GetComponentsInChildren<vp_Component>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			vp_Component vp_Component = componentsInChildren[i];
			if (vp_Component != this)
			{
				this.Family.Add(vp_Component);
			}
		}
	}

	public void CacheRenderers()
	{
		this.Renderers.Clear();
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer item = componentsInChildren[i];
			this.Renderers.Add(item);
		}
	}

	public void CacheAudioSources()
	{
		this.AudioSources.Clear();
		AudioSource[] componentsInChildren = base.GetComponentsInChildren<AudioSource>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			AudioSource item = componentsInChildren[i];
			this.AudioSources.Add(item);
		}
	}

	public virtual void Activate()
	{
		this.m_DeactivationTimer.Cancel();
		vp_Utility.Activate(base.gameObject, true);
	}

	public virtual void Deactivate()
	{
		vp_Utility.Activate(base.gameObject, false);
	}

	public void DeactivateWhenSilent()
	{
		if (this == null)
		{
			return;
		}
		if (vp_Utility.IsActive(base.gameObject))
		{
			foreach (AudioSource current in this.AudioSources)
			{
				if (current.isPlaying && !current.loop)
				{
					this.Rendering = false;
					vp_Timer.In(0.1f, delegate
					{
						this.DeactivateWhenSilent();
					}, this.m_DeactivationTimer);
					return;
				}
			}
		}
		this.Deactivate();
	}

	public virtual void Refresh()
	{
	}
}
