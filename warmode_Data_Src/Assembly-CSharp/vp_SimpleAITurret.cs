using System;
using UnityEngine;

[RequireComponent(typeof(vp_Shooter))]
public class vp_SimpleAITurret : MonoBehaviour
{
	public float ViewRange = 10f;

	public float AimSpeed = 50f;

	public float WakeInterval = 2f;

	protected vp_Shooter m_Shooter;

	protected Transform m_Transform;

	protected Transform m_Target;

	protected vp_Timer.Handle m_Timer = new vp_Timer.Handle();

	private void Start()
	{
		this.m_Shooter = base.GetComponent<vp_Shooter>();
		this.m_Transform = base.transform;
	}

	private void Update()
	{
		if (!this.m_Timer.Active)
		{
			vp_Timer.In(this.WakeInterval, delegate
			{
				if (this.m_Target == null)
				{
					this.m_Target = this.ScanForLocalPlayer();
				}
				else
				{
					this.m_Target = null;
				}
			}, this.m_Timer);
		}
		if (this.m_Target != null)
		{
			this.AttackTarget();
		}
	}

	private Transform ScanForLocalPlayer()
	{
		Collider[] array = Physics.OverlapSphere(this.m_Transform.position, this.ViewRange, 1073741824);
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Collider collider = array2[i];
			RaycastHit raycastHit;
			Physics.Linecast(this.m_Transform.position, collider.transform.position + Vector3.up, out raycastHit);
			if (!(raycastHit.collider != null) || !(raycastHit.collider != collider))
			{
				return collider.transform;
			}
		}
		return null;
	}

	private void AttackTarget()
	{
		Vector3 forward = this.m_Target.position - this.m_Transform.position;
		Quaternion to = Quaternion.LookRotation(forward);
		this.m_Transform.rotation = Quaternion.RotateTowards(this.m_Transform.rotation, to, Time.deltaTime * this.AimSpeed);
		this.m_Shooter.TryFire();
	}
}
