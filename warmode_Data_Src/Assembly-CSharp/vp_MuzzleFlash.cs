using System;
using UnityEngine;

public class vp_MuzzleFlash : MonoBehaviour
{
	protected float m_FadeSpeed = 0.075f;

	protected bool m_ForceShow;

	protected Color m_Color = new Color(1f, 1f, 1f, 0f);

	protected Transform m_Transform;

	public Material[] mat;

	public Renderer r;

	public float FadeSpeed
	{
		get
		{
			return this.m_FadeSpeed;
		}
		set
		{
			this.m_FadeSpeed = value;
		}
	}

	public bool ForceShow
	{
		get
		{
			return this.m_ForceShow;
		}
		set
		{
			this.m_ForceShow = value;
		}
	}

	private void Awake()
	{
		this.r = base.GetComponent<Renderer>();
		this.r.material = this.mat[UnityEngine.Random.Range(0, 3)];
		this.m_Transform = base.transform;
		this.m_Color = this.r.material.GetColor("_TintColor");
		this.m_Color.a = 0f;
		this.m_ForceShow = false;
	}

	private void Start()
	{
		if (this.m_Transform.root.gameObject.layer == 30)
		{
			base.gameObject.layer = 31;
		}
	}

	private void Update()
	{
		if (this.m_ForceShow)
		{
			this.Show();
		}
		else if (this.m_Color.a > 0f)
		{
			this.m_Color.a = this.m_Color.a - this.m_FadeSpeed * (Time.deltaTime * 60f);
		}
		else
		{
			this.r.enabled = false;
		}
		this.r.material.SetColor("_TintColor", this.m_Color);
	}

	public void Show()
	{
		this.m_Color.a = 0.5f;
		this.r.enabled = true;
	}

	public void Shoot()
	{
		this.r.sharedMaterial = this.mat[UnityEngine.Random.Range(0, 3)];
		this.m_Transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
		this.m_Color.a = 0.5f;
		this.r.enabled = true;
	}
}
