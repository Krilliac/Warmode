using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class HitReactionCharacter : MonoBehaviour
	{
		private string colliderName;

		[SerializeField]
		private string mixingAnim;

		[SerializeField]
		private Transform recursiveMixingTransform;

		[SerializeField]
		private Camera cam;

		[SerializeField]
		private HitReaction hitReaction;

		[SerializeField]
		private float hitForce = 1f;

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit = default(RaycastHit);
				if (Physics.Raycast(ray, out raycastHit, 100f))
				{
					this.hitReaction.Hit(raycastHit.collider, ray.direction * this.hitForce, raycastHit.point);
					this.colliderName = raycastHit.collider.name;
				}
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("LMB to shoot the Dummy, RMB to rotate the camera.", new GUILayoutOption[0]);
			if (this.colliderName != string.Empty)
			{
				GUILayout.Label("Last Bone Hit: " + this.colliderName, new GUILayoutOption[0]);
			}
		}

		private void Start()
		{
			if (this.mixingAnim != string.Empty)
			{
				base.GetComponent<Animation>()[this.mixingAnim].layer = 1;
				base.GetComponent<Animation>()[this.mixingAnim].AddMixingTransform(this.recursiveMixingTransform, true);
				base.GetComponent<Animation>().Play(this.mixingAnim);
			}
		}
	}
}
