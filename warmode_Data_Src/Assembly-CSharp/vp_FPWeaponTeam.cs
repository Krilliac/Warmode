using System;
using UnityEngine;

[RequireComponent(typeof(vp_FPWeapon))]
public class vp_FPWeaponTeam : MonoBehaviour
{
	protected vp_FPWeapon m_Weapon;

	protected vp_FPPlayerEventHandler m_Player;

	protected virtual void Awake()
	{
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
	}

	protected virtual void Start()
	{
		this.m_Weapon = base.transform.GetComponent<vp_FPWeapon>();
	}

	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
		if (this.m_Weapon && this.m_Weapon.m_WeaponModel)
		{
			GameObject gameObject = GameObject.Find(this.m_Weapon.m_WeaponModel.name + "/Paramilitary2");
			if (gameObject)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}
}
