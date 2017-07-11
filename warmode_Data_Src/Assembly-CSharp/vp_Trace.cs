using System;
using UnityEngine;

public class vp_Trace : MonoBehaviour
{
	private float m_RemoveTime;

	private float m_Speed = 300f;

	private float m_Length = 3f;

	private LineRenderer lr;

	public Vector3 e = Vector3.zero;

	private void Start()
	{
		this.m_RemoveTime = Time.time + this.m_Speed;
		this.lr = base.GetComponent<LineRenderer>();
		float a = UnityEngine.Random.Range(0.1f, 1f);
		this.lr.SetColors(new Color(1f, 1f, 1f, a), new Color(1f, 1f, 1f, a));
		base.transform.LookAt(this.e);
		this.m_Length = UnityEngine.Random.Range(3f, 6f);
	}

	private void Update()
	{
		base.transform.position = base.transform.position + base.transform.forward * this.m_Speed * Time.deltaTime;
		this.lr.SetPosition(0, base.transform.position);
		this.lr.SetPosition(1, base.transform.position + base.transform.forward * this.m_Length);
		if (Time.time > this.m_RemoveTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (Vector3.Distance(base.transform.position, this.e) < this.m_Length)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
