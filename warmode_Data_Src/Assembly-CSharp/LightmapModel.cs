using System;
using UnityEngine;

public class LightmapModel : MonoBehaviour
{
	public GameObject go;

	private float timelast = -1f;

	private float timeint = 0.05f;

	private void OnEnable()
	{
		this.timelast = -1f;
		this.Update();
	}

	private void Update()
	{
		if (Options.xLightmapShadow == 0)
		{
			return;
		}
		if (this.timelast + this.timeint > Time.time)
		{
			return;
		}
		this.timelast = Time.time;
		if (this.go == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		int layerMask = 1;
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(position + Vector3.up * 0.1f, Vector3.down), out raycastHit, 0.2f, layerMask))
		{
			return;
		}
		Renderer component = raycastHit.collider.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		if (component.lightmapIndex > 253)
		{
			return;
		}
		LightmapData lightmapData = LightmapSettings.lightmaps[component.lightmapIndex];
		Texture2D lightmapFar = lightmapData.lightmapFar;
		if (lightmapFar == null)
		{
			return;
		}
		Vector2 lightmapCoord = raycastHit.lightmapCoord;
		Color pixelBilinear;
		try
		{
			pixelBilinear = lightmapFar.GetPixelBilinear(lightmapCoord.x, lightmapCoord.y);
		}
		catch (UnityException var_8_F5)
		{
			return;
		}
		Color color = new Color(pixelBilinear.r * pixelBilinear.a + 0.125f, pixelBilinear.g * pixelBilinear.a + 0.125f, pixelBilinear.b * pixelBilinear.a + 0.125f);
		Renderer component2 = this.go.GetComponent<Renderer>();
		if (component2 == null)
		{
			return;
		}
		Material[] materials = component2.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			Material material = materials[i];
			material.color = color;
		}
	}
}
