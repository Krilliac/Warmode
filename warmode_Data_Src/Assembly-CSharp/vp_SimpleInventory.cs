using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_SimpleInventory : MonoBehaviour
{
	[Serializable]
	public class InventoryItemStatus
	{
		public string Name = "Unnamed";

		public int Have;

		public int CanHave = 1;

		public bool ClearOnDeath = true;
	}

	[Serializable]
	public class InventoryWeaponStatus : vp_SimpleInventory.InventoryItemStatus
	{
		public string ClipType = string.Empty;

		public int LoadedAmmo;

		public int MaxAmmo = 10;
	}

	protected vp_FPPlayerEventHandler m_Player;

	[SerializeField]
	protected List<vp_SimpleInventory.InventoryItemStatus> m_ItemTypes;

	[SerializeField]
	protected List<vp_SimpleInventory.InventoryWeaponStatus> m_WeaponTypes;

	protected Dictionary<string, vp_SimpleInventory.InventoryItemStatus> m_ItemStatusDictionary;

	protected vp_SimpleInventory.InventoryWeaponStatus m_CurrentWeaponStatus;

	protected int m_RefreshWeaponStatusIterations;

	protected Dictionary<string, vp_SimpleInventory.InventoryItemStatus> ItemStatusDictionary
	{
		get
		{
			if (this.m_ItemStatusDictionary == null)
			{
				this.m_ItemStatusDictionary = new Dictionary<string, vp_SimpleInventory.InventoryItemStatus>();
				for (int i = this.m_ItemTypes.Count - 1; i > -1; i--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_ItemTypes[i].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_ItemTypes[i].Name, this.m_ItemTypes[i]);
					}
					else
					{
						this.m_ItemTypes.Remove(this.m_ItemTypes[i]);
					}
				}
				for (int j = this.m_WeaponTypes.Count - 1; j > -1; j--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_WeaponTypes[j].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_WeaponTypes[j].Name, this.m_WeaponTypes[j]);
					}
					else
					{
						this.m_WeaponTypes.Remove(this.m_WeaponTypes[j]);
					}
				}
			}
			return this.m_ItemStatusDictionary;
		}
	}

	protected virtual int OnValue_CurrentWeaponAmmoCount
	{
		get
		{
			return (this.m_CurrentWeaponStatus == null) ? 0 : this.m_CurrentWeaponStatus.LoadedAmmo;
		}
		set
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return;
			}
			this.m_CurrentWeaponStatus.LoadedAmmo = value;
		}
	}

	protected virtual int OnValue_CurrentWeaponClipCount
	{
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
			if (!this.ItemStatusDictionary.TryGetValue(this.m_CurrentWeaponStatus.ClipType, out inventoryItemStatus))
			{
				return 0;
			}
			return inventoryItemStatus.Have;
		}
	}

	protected virtual string OnValue_CurrentWeaponClipType
	{
		get
		{
			return (this.m_CurrentWeaponStatus == null) ? string.Empty : this.m_CurrentWeaponStatus.ClipType;
		}
	}

	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	private void Awake()
	{
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
	}

	public bool HaveItem(object name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		return this.ItemStatusDictionary.TryGetValue((string)name, out inventoryItemStatus) && inventoryItemStatus.Have >= 1;
	}

	private vp_SimpleInventory.InventoryItemStatus GetItemStatus(string name)
	{
		vp_SimpleInventory.InventoryItemStatus result;
		if (!this.ItemStatusDictionary.TryGetValue(name, out result))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Unknown item type: '",
				name,
				"'."
			}));
		}
		return result;
	}

	private vp_SimpleInventory.InventoryWeaponStatus GetWeaponStatus(string name)
	{
		if (name == null)
		{
			return null;
		}
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Unknown item type: '",
				name,
				"'."
			}));
			return null;
		}
		if (inventoryItemStatus.GetType() != typeof(vp_SimpleInventory.InventoryWeaponStatus))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Item is not a weapon: '",
				name,
				"'."
			}));
			return null;
		}
		return (vp_SimpleInventory.InventoryWeaponStatus)inventoryItemStatus;
	}

	protected void RefreshWeaponStatus()
	{
		if (!this.m_Player.CurrentWeaponWielded.Get() && this.m_RefreshWeaponStatusIterations < 50)
		{
			this.m_RefreshWeaponStatusIterations++;
			vp_Timer.In(0.1f, new vp_Timer.Callback(this.RefreshWeaponStatus), null);
			return;
		}
		this.m_RefreshWeaponStatusIterations = 0;
		string text = this.m_Player.CurrentWeaponName.Get();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.m_CurrentWeaponStatus = this.GetWeaponStatus(text);
	}

	protected virtual int OnMessage_GetItemCount(string name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			return 0;
		}
		return inventoryItemStatus.Have;
	}

	protected virtual bool OnAttempt_DepleteAmmo()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return false;
		}
		if (this.m_CurrentWeaponStatus.LoadedAmmo < 1)
		{
			return this.m_CurrentWeaponStatus.MaxAmmo == 0;
		}
		this.m_CurrentWeaponStatus.LoadedAmmo--;
		return true;
	}

	protected virtual bool OnAttempt_AddAmmo(object arg)
	{
		object[] array = (object[])arg;
		string name = (string)array[0];
		int num = (array.Length != 2) ? -1 : ((int)array[1]);
		vp_SimpleInventory.InventoryWeaponStatus weaponStatus = this.GetWeaponStatus(name);
		if (weaponStatus == null)
		{
			return false;
		}
		if (num == -1)
		{
			weaponStatus.LoadedAmmo = weaponStatus.MaxAmmo;
		}
		else
		{
			weaponStatus.LoadedAmmo = Mathf.Min(num, weaponStatus.MaxAmmo);
		}
		return true;
	}

	protected virtual bool OnAttempt_AddItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length != 2) ? 1 : ((int)array[1]);
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		itemStatus.CanHave = Mathf.Max(1, itemStatus.CanHave);
		if (itemStatus.Have >= itemStatus.CanHave)
		{
			return false;
		}
		itemStatus.Have = Mathf.Min(itemStatus.Have + num, itemStatus.CanHave);
		return true;
	}

	protected virtual bool OnAttempt_RemoveItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length != 2) ? 1 : ((int)array[1]);
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		if (itemStatus.Have <= 0)
		{
			return false;
		}
		itemStatus.Have = Mathf.Max(itemStatus.Have - num, 0);
		return true;
	}

	protected virtual bool OnAttempt_RemoveClip()
	{
		return this.m_CurrentWeaponStatus != null && this.GetItemStatus(this.m_CurrentWeaponStatus.ClipType) != null && this.m_CurrentWeaponStatus.LoadedAmmo < this.m_CurrentWeaponStatus.MaxAmmo;
	}

	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num == 0 || (num >= 0 && num <= this.m_WeaponTypes.Count && this.HaveItem(this.m_WeaponTypes[num - 1].Name));
	}

	protected virtual void OnStop_SetWeapon()
	{
		this.RefreshWeaponStatus();
	}

	protected virtual void OnStart_Dead()
	{
		foreach (vp_SimpleInventory.InventoryItemStatus current in this.m_ItemStatusDictionary.Values)
		{
			if (current.ClearOnDeath)
			{
				current.Have = 0;
				if (current.GetType() == typeof(vp_SimpleInventory.InventoryWeaponStatus))
				{
					((vp_SimpleInventory.InventoryWeaponStatus)current).LoadedAmmo = 0;
				}
			}
		}
	}
}
