using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vp_FPWeaponHandler : MonoBehaviour
{
	protected class WeaponComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((vp_FPWeapon)x).gameObject.name, ((vp_FPWeapon)y).gameObject.name);
		}
	}

	public static vp_FPWeaponHandler cs;

	public int StartWeapon;

	public float AttackStateDisableDelay = 0.5f;

	public float SetWeaponRefreshStatesDelay = 0.5f;

	public float SetWeaponDuration = 0.1f;

	public float SetWeaponReloadSleepDuration = 0.3f;

	public float SetWeaponZoomSleepDuration = 0.3f;

	public float SetWeaponAttackSleepDuration = 0.3f;

	public float ReloadAttackSleepDuration = 0.3f;

	protected vp_FPPlayerEventHandler m_Player;

	protected List<vp_FPWeapon> m_Weapons = new List<vp_FPWeapon>();

	public int m_CurrentWeaponID = -1;

	protected vp_FPWeapon m_CurrentWeapon;

	protected vp_Timer.Handle m_SetWeaponTimer = new vp_Timer.Handle();

	protected vp_Timer.Handle m_SetWeaponRefreshTimer = new vp_Timer.Handle();

	protected vp_Timer.Handle m_DisableAttackStateTimer = new vp_Timer.Handle();

	protected vp_Timer.Handle m_DisableReloadStateTimer = new vp_Timer.Handle();

	public vp_FPWeapon CurrentWeapon
	{
		get
		{
			return this.m_CurrentWeapon;
		}
	}

	public int CurrentWeaponID
	{
		get
		{
			return this.m_CurrentWeaponID;
		}
	}

	protected virtual bool OnValue_CurrentWeaponWielded
	{
		get
		{
			return !(this.m_CurrentWeapon == null) && this.m_CurrentWeapon.Wielded;
		}
	}

	protected virtual string OnValue_CurrentWeaponName
	{
		get
		{
			if (this.m_CurrentWeapon == null || this.m_Weapons == null)
			{
				return string.Empty;
			}
			return this.m_CurrentWeapon.name;
		}
	}

	protected virtual int OnValue_CurrentWeaponID
	{
		get
		{
			return this.m_CurrentWeaponID;
		}
	}

	protected virtual void Awake()
	{
		vp_FPWeaponHandler.cs = this;
		if (base.GetComponent<vp_FPWeapon>())
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component should sit above any vp_FPWeapons in the gameobject hierarchy.");
			return;
		}
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		vp_FPWeapon[] componentsInChildren = base.GetComponentsInChildren<vp_FPWeapon>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			vp_FPWeapon item = componentsInChildren[i];
			this.m_Weapons.Insert(this.m_Weapons.Count, item);
		}
		if (this.m_Weapons.Count == 0)
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component must be added to a gameobject with vp_FPWeapon components in child gameobjects.");
			return;
		}
		IComparer @object = new vp_FPWeaponHandler.WeaponComparer();
		this.m_Weapons.Sort(new Comparison<vp_FPWeapon>(@object.Compare));
		this.StartWeapon = Mathf.Clamp(this.StartWeapon, 0, this.m_Weapons.Count);
	}

	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
		vp_Timer.In(0f, delegate
		{
			if (base.enabled)
			{
				this.SetWeapon(this.m_CurrentWeaponID);
			}
		}, null);
	}

	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	protected virtual void Update()
	{
		if (this.m_CurrentWeaponID == -1)
		{
			this.m_CurrentWeaponID = this.StartWeapon;
			this.SetWeapon(this.m_CurrentWeaponID);
		}
	}

	public virtual void SetWeapon(int i)
	{
		if (this.m_Weapons.Count < 1)
		{
			Debug.LogError("Error: (" + this + ") Tried to set weapon with an empty weapon list.");
			return;
		}
		if (i < 0 || i > this.m_Weapons.Count)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") Weapon list does not have a weapon with index: ",
				i
			}));
			return;
		}
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.ResetState();
		}
		foreach (vp_FPWeapon current in this.m_Weapons)
		{
			current.ActivateGameObject(false);
		}
		this.m_CurrentWeaponID = i;
		this.m_CurrentWeapon = null;
		if (this.m_CurrentWeaponID > 0)
		{
			this.m_CurrentWeapon = this.m_Weapons[this.m_CurrentWeaponID - 1];
			if (this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.ActivateGameObject(true);
			}
		}
	}

	public virtual void CancelTimers()
	{
		vp_Timer.CancelAll("EjectShell");
		this.m_DisableAttackStateTimer.Cancel();
		this.m_SetWeaponTimer.Cancel();
		this.m_SetWeaponRefreshTimer.Cancel();
	}

	public virtual void SetWeaponLayer(int layer)
	{
		if (this.m_CurrentWeaponID < 1 || this.m_CurrentWeaponID > this.m_Weapons.Count)
		{
			return;
		}
		vp_Layer.Set(this.m_Weapons[this.m_CurrentWeaponID - 1].gameObject, layer, true);
	}

	protected virtual void OnStart_Reload()
	{
		this.m_Player.Attack.Stop(this.m_Player.CurrentWeaponReloadDuration.Get() + this.ReloadAttackSleepDuration);
	}

	protected virtual void OnStart_SetWeapon()
	{
		this.CancelTimers();
		this.m_Player.Reload.Stop(this.SetWeaponDuration + this.SetWeaponReloadSleepDuration);
		this.m_Player.Zoom.Stop(this.SetWeaponDuration + this.SetWeaponZoomSleepDuration);
		this.m_Player.Attack.Stop(this.SetWeaponDuration + this.SetWeaponAttackSleepDuration);
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(false);
		}
		this.m_Player.SetWeapon.AutoDuration = this.SetWeaponDuration;
	}

	protected virtual void OnStop_SetWeapon()
	{
		int weapon = (int)this.m_Player.SetWeapon.Argument;
		this.SetWeapon(weapon);
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(true);
		}
		vp_Timer.In(this.SetWeaponRefreshStatesDelay, delegate
		{
			this.m_Player.RefreshActivityStates();
		}, this.m_SetWeaponRefreshTimer);
	}

	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num != this.m_CurrentWeaponID && num >= 0 && num <= this.m_Weapons.Count;
	}

	protected virtual bool CanStart_Attack()
	{
		return !(this.m_CurrentWeapon == null) && !this.m_Player.Attack.Active && !this.m_Player.SetWeapon.Active && !this.m_Player.Reload.Active;
	}

	protected virtual void OnStop_Attack()
	{
		vp_Timer.In(this.AttackStateDisableDelay, delegate
		{
			if (!this.m_Player.Attack.Active && this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.SetState("Attack", false, false, false);
			}
		}, this.m_DisableAttackStateTimer);
	}

	protected virtual bool OnAttempt_SetPrevWeapon()
	{
		int num = this.m_CurrentWeaponID - 1;
		if (num < 1)
		{
			num = this.m_Weapons.Count;
		}
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			num--;
			if (num < 1)
			{
				num = this.m_Weapons.Count;
			}
			num2++;
			if (num2 > this.m_Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual bool OnAttempt_SetNextWeapon()
	{
		int num = this.m_CurrentWeaponID + 1;
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			if (num > this.m_Weapons.Count + 1)
			{
				num = 0;
			}
			num++;
			num2++;
			if (num2 > this.m_Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual bool OnAttempt_SetWeaponByName(string name)
	{
		for (int i = 0; i < this.m_Weapons.Count; i++)
		{
			if (this.m_Weapons[i].name == name)
			{
				return this.m_Player.SetWeapon.TryStart<int>(i + 1);
			}
		}
		return false;
	}
}
