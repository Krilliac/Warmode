using System;
using UnityEngine;

public class GrenadeSG : MonoBehaviour
{
	private float handletime;

	private float destroytime;

	private GameObject handle;

	private int uid;

	private int ticcounter;

	private float freezeradius = 0.1f;

	public Vector3 PrevPos;

	public bool isGrounded;

	private void Start()
	{
		this.handletime = Time.time + 0.1f;
		this.destroytime = Time.time + 45f;
		string[] array = base.gameObject.name.Split(new char[]
		{
			'_'
		});
		int.TryParse(array[1], out this.uid);
	}

	private void Update()
	{
		if (this.handletime != 0f && Time.time > this.handletime)
		{
			this.handle = GameObject.Find(base.gameObject.name + "/handle");
			this.handle.transform.SetParent(null);
			this.handle.GetComponent<Rigidbody>().isKinematic = false;
			this.handle.GetComponent<Rigidbody>().AddForce(base.gameObject.GetComponent<Rigidbody>().velocity + Camera.main.transform.right * 75f);
			this.handle.GetComponent<Rigidbody>().AddTorque(this.handle.transform.forward * 500f);
			this.handletime = 0f;
		}
		if (this.destroytime != 0f && Time.time > this.destroytime)
		{
			this.destroytime = 0f;
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (this.isGrounded)
		{
			return;
		}
		if (this.ticcounter < 70)
		{
			this.ticcounter++;
			return;
		}
		this.ticcounter = 0;
		if (Mathf.Abs(base.transform.position.x - this.PrevPos.x) < this.freezeradius && Mathf.Abs(base.transform.position.y - this.PrevPos.y) < this.freezeradius && Mathf.Abs(base.transform.position.z - this.PrevPos.z) < this.freezeradius)
		{
			if (this.handle)
			{
				UnityEngine.Object.Destroy(this.handle);
			}
			this.isGrounded = true;
			if (base.gameObject.GetComponent<BaseEnt>().ownerid == Client.ID)
			{
				Client.cs.send_ent_destroy(this.uid, base.gameObject.GetComponent<BaseEnt>().type, base.gameObject.transform.position);
			}
		}
		this.PrevPos = base.transform.position;
	}
}
