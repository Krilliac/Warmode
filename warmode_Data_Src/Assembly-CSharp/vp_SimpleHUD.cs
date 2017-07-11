using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class vp_SimpleHUD : MonoBehaviour
{
	public Texture DamageFlashTexture;

	public bool ShowHUD = true;

	private Color m_MessageColor = new Color(2f, 2f, 0f, 2f);

	private Color m_InvisibleColor = new Color(1f, 1f, 0f, 0f);

	private Color m_DamageFlashColor = new Color(0.8f, 0f, 0f, 0f);

	private Color m_DamageFlashInvisibleColor = new Color(1f, 0f, 0f, 0f);

	private string m_PickupMessage = string.Empty;

	protected static GUIStyle m_MessageStyle;

	private vp_FPPlayerEventHandler m_Player;

	public static GUIStyle MessageStyle
	{
		get
		{
			if (vp_SimpleHUD.m_MessageStyle == null)
			{
				vp_SimpleHUD.m_MessageStyle = new GUIStyle("Label");
				vp_SimpleHUD.m_MessageStyle.alignment = TextAnchor.MiddleCenter;
			}
			return vp_SimpleHUD.m_MessageStyle;
		}
	}

	private void Awake()
	{
		this.m_Player = base.transform.GetComponent<vp_FPPlayerEventHandler>();
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

	protected virtual void OnGUI()
	{
	}

	protected virtual void OnMessage_HUDText(string message)
	{
		this.m_MessageColor = Color.white;
		this.m_PickupMessage = message;
	}

	protected virtual void OnMessage_HUDDamageFlash(float intensity)
	{
		if (this.DamageFlashTexture == null)
		{
			return;
		}
		if (intensity == 0f)
		{
			this.m_DamageFlashColor.a = 0f;
		}
		else
		{
			this.m_DamageFlashColor.a = this.m_DamageFlashColor.a + intensity;
		}
	}
}
