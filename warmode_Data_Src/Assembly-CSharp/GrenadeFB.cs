using System;
using UnityEngine;

public class GrenadeFB : MonoBehaviour
{
	private float handletime;

	private float explodetime;

	private GameObject handle;

	private int uid;

	private void Start()
	{
		this.handletime = Time.time + 0.1f;
		this.explodetime = Time.time + 2f;
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
		if (this.explodetime != 0f && Time.time > this.explodetime)
		{
			this.explodetime = 0f;
			if (this.handle)
			{
				UnityEngine.Object.Destroy(this.handle);
			}
			UnityEngine.Object.Destroy(base.gameObject);
			if (base.gameObject.GetComponent<BaseEnt>().ownerid == Client.ID)
			{
				Client.cs.send_ent_destroy(this.uid, base.gameObject.GetComponent<BaseEnt>().type, base.gameObject.transform.position);
			}
		}
	}
}
