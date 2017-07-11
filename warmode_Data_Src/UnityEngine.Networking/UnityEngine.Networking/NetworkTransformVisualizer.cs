using System;
using System.ComponentModel;

namespace UnityEngine.Networking
{
	[EditorBrowsable(EditorBrowsableState.Never), AddComponentMenu("Network/NetworkTransformVisualizer"), DisallowMultipleComponent, RequireComponent(typeof(NetworkTransform))]
	public class NetworkTransformVisualizer : NetworkBehaviour
	{
		[SerializeField]
		private GameObject m_VisualizerPrefab;

		private NetworkTransform m_NetworkTransform;

		private GameObject m_Visualizer;

		private static Material s_LineMaterial;

		public GameObject visualizerPrefab
		{
			get
			{
				return this.m_VisualizerPrefab;
			}
			set
			{
				this.m_VisualizerPrefab = value;
			}
		}

		public override void OnStartClient()
		{
			if (this.m_VisualizerPrefab != null)
			{
				this.m_NetworkTransform = base.GetComponent<NetworkTransform>();
				NetworkTransformVisualizer.CreateLineMaterial();
				this.m_Visualizer = (GameObject)UnityEngine.Object.Instantiate(this.m_VisualizerPrefab, base.transform.position, Quaternion.identity);
			}
		}

		public override void OnStartLocalPlayer()
		{
			if (this.m_Visualizer == null)
			{
				return;
			}
			if (this.m_NetworkTransform.localPlayerAuthority || base.isServer)
			{
				UnityEngine.Object.Destroy(this.m_Visualizer);
			}
		}

		private void OnDestroy()
		{
			if (this.m_Visualizer != null)
			{
				UnityEngine.Object.Destroy(this.m_Visualizer);
			}
		}

		[ClientCallback]
		private void FixedUpdate()
		{
			if (this.m_Visualizer == null)
			{
				return;
			}
			if (!NetworkServer.active && !NetworkClient.active)
			{
				return;
			}
			if (!base.isServer && !base.isClient)
			{
				return;
			}
			if (base.hasAuthority && this.m_NetworkTransform.localPlayerAuthority)
			{
				return;
			}
			this.m_Visualizer.transform.position = this.m_NetworkTransform.targetSyncPosition;
			if (this.m_NetworkTransform.rigidbody3D != null && this.m_Visualizer.GetComponent<Rigidbody>() != null)
			{
				this.m_Visualizer.GetComponent<Rigidbody>().velocity = this.m_NetworkTransform.targetSyncVelocity;
			}
			if (this.m_NetworkTransform.rigidbody2D != null && this.m_Visualizer.GetComponent<Rigidbody2D>() != null)
			{
				this.m_Visualizer.GetComponent<Rigidbody2D>().velocity = this.m_NetworkTransform.targetSyncVelocity;
			}
			Quaternion rotation = Quaternion.identity;
			if (this.m_NetworkTransform.rigidbody3D != null)
			{
				rotation = this.m_NetworkTransform.targetSyncRotation3D;
			}
			if (this.m_NetworkTransform.rigidbody2D != null)
			{
				rotation = Quaternion.Euler(0f, 0f, this.m_NetworkTransform.targetSyncRotation2D);
			}
			this.m_Visualizer.transform.rotation = rotation;
		}

		private void OnRenderObject()
		{
			if (this.m_Visualizer == null)
			{
				return;
			}
			if (this.m_NetworkTransform.localPlayerAuthority && base.hasAuthority)
			{
				return;
			}
			if (this.m_NetworkTransform.lastSyncTime == 0f)
			{
				return;
			}
			NetworkTransformVisualizer.s_LineMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(Color.white);
			GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			GL.Vertex3(this.m_NetworkTransform.targetSyncPosition.x, this.m_NetworkTransform.targetSyncPosition.y, this.m_NetworkTransform.targetSyncPosition.z);
			GL.End();
			this.DrawRotationInterpolation();
		}

		private void DrawRotationInterpolation()
		{
			Quaternion quaternion = Quaternion.identity;
			if (this.m_NetworkTransform.rigidbody3D != null)
			{
				quaternion = this.m_NetworkTransform.targetSyncRotation3D;
			}
			if (this.m_NetworkTransform.rigidbody2D != null)
			{
				quaternion = Quaternion.Euler(0f, 0f, this.m_NetworkTransform.targetSyncRotation2D);
			}
			if (quaternion == Quaternion.identity)
			{
				return;
			}
			GL.Begin(1);
			GL.Color(Color.yellow);
			GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			Vector3 vector = base.transform.position + base.transform.right;
			GL.Vertex3(vector.x, vector.y, vector.z);
			GL.End();
			GL.Begin(1);
			GL.Color(Color.green);
			GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			Vector3 b = quaternion * Vector3.right;
			Vector3 vector2 = base.transform.position + b;
			GL.Vertex3(vector2.x, vector2.y, vector2.z);
			GL.End();
		}

		private static void CreateLineMaterial()
		{
			if (NetworkTransformVisualizer.s_LineMaterial)
			{
				return;
			}
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (!shader)
			{
				Debug.LogWarning("Could not find Colored builtin shader");
				return;
			}
			NetworkTransformVisualizer.s_LineMaterial = new Material(shader);
			NetworkTransformVisualizer.s_LineMaterial.hideFlags = HideFlags.HideAndDontSave;
			NetworkTransformVisualizer.s_LineMaterial.SetInt("_ZWrite", 0);
		}
	}
}
