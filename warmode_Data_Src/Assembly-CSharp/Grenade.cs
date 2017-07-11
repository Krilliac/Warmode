using System;
using UnityEngine;

public class Grenade : MonoBehaviour
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
			GameObject original = ContentLoader_.LoadGameObject("Detonator");
			UnityEngine.Object.Instantiate(original, base.gameObject.transform.position + Vector3.up * 0.1f, base.gameObject.transform.rotation);
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
			if (vp_FPController.cs == null || vp_FPCamera.cs == null)
			{
				return;
			}
			float num = Vector3.Distance(vp_FPController.cs.SmoothPosition, base.gameObject.transform.position);
			if (num > 20f)
			{
				return;
			}
			float num2 = 0.001f;
			if (num < 5f)
			{
				num2 = 0.005f;
			}
			else if (num < 10f)
			{
				num2 = 0.003f;
			}
			num = 20f - num;
			vp_FPCamera.cs.AddForce2(new Vector3(2f, -10f, 2f) * num * num2);
			if (UnityEngine.Random.value > 0.5f)
			{
				num2 = -num2;
			}
			vp_FPCamera.cs.AddRollForce(num2 * 200f);
		}
	}
}
