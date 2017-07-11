using System;
using UnityEngine;

public class RotateModelAB : MonoBehaviour
{
	public bool fullrotate;

	private MeshRenderer r;

	private float speed = -5f;

	private void Start()
	{
		this.r = base.GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		if (this.fullrotate)
		{
			this.RotateXY();
		}
		else
		{
			this.RotateX();
		}
	}

	private void RotateX()
	{
		if (Input.GetMouseButton(0) && Input.mousePosition.y > (float)Screen.height * 0.2f)
		{
			if (this.r != null)
			{
				base.gameObject.transform.RotateAround(this.r.bounds.center, Vector3.up, Input.GetAxis("Mouse X") * this.speed);
			}
			else
			{
				base.gameObject.transform.RotateAround(base.gameObject.transform.position, Vector3.up, Input.GetAxis("Mouse X") * this.speed);
			}
		}
	}

	private void RotateXY()
	{
		if (Input.GetMouseButton(0) && Input.mousePosition.y > (float)Screen.height * 0.2f)
		{
			if (this.r != null)
			{
				base.gameObject.transform.RotateAround(this.r.bounds.center, Vector3.up, Input.GetAxis("Mouse X") * this.speed);
				base.gameObject.transform.RotateAround(this.r.bounds.center, Vector3.right, -Input.GetAxis("Mouse Y") * this.speed);
			}
			else
			{
				base.gameObject.transform.RotateAround(base.gameObject.transform.position, Vector3.up, Input.GetAxis("Mouse X") * this.speed);
				base.gameObject.transform.RotateAround(base.gameObject.transform.position, Vector3.right, -Input.GetAxis("Mouse Y") * this.speed);
			}
		}
	}
}
