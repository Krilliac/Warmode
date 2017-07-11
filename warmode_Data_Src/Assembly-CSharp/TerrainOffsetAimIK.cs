using RootMotion.FinalIK;
using System;
using UnityEngine;

public class TerrainOffsetAimIK : MonoBehaviour
{
	public AimIK aimIK;

	public Vector3 offset;

	public Transform target;

	private void LateUpdate()
	{
		this.aimIK.solver.transform.LookAt(this.target.position);
		this.aimIK.solver.IKPosition = this.target.position + this.offset;
	}
}
