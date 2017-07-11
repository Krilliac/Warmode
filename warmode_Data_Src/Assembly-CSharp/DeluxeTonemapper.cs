using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class DeluxeTonemapper : MonoBehaviour
{
	public enum Mode
	{
		Color,
		Luminance,
		ExtendedLuminance
	}

	[SerializeField]
	public FilmicCurve m_MainCurve = new FilmicCurve();

	[SerializeField]
	public Color m_Tint = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	public DeluxeTonemapper.Mode m_Mode;

	private DeluxeTonemapper.Mode m_LastMode;

	private Material m_Material;

	private Shader m_Shader;

	public Shader TonemappingShader
	{
		get
		{
			return this.m_Shader;
		}
	}

	private void DestroyMaterial(Material mat)
	{
		if (mat)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	private void CreateMaterials()
	{
		if (this.m_Shader == null)
		{
			if (this.m_Mode == DeluxeTonemapper.Mode.Color)
			{
				this.m_Shader = Shader.Find("Hidden/Deluxe/TonemapperColor");
			}
			if (this.m_Mode == DeluxeTonemapper.Mode.Luminance)
			{
				this.m_Shader = Shader.Find("Hidden/Deluxe/TonemapperLuminosity");
			}
			if (this.m_Mode == DeluxeTonemapper.Mode.ExtendedLuminance)
			{
				this.m_Shader = Shader.Find("Hidden/Deluxe/TonemapperLuminosityExtended");
			}
		}
		if (this.m_Material == null && this.m_Shader != null && this.m_Shader.isSupported)
		{
			this.m_Material = this.CreateMaterial(this.m_Shader);
		}
	}

	private Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			return null;
		}
		return new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	private void OnDisable()
	{
		this.DestroyMaterial(this.m_Material);
		this.m_Material = null;
		this.m_Shader = null;
	}

	public void StoreK()
	{
		this.m_MainCurve.StoreK();
	}

	public void UpdateCoefficients()
	{
		this.StoreK();
		this.m_MainCurve.UpdateCoefficients();
		if (this.m_Material == null)
		{
			return;
		}
		this.m_Material.SetFloat("_K", this.m_MainCurve.m_k);
		this.m_Material.SetFloat("_Crossover", this.m_MainCurve.m_CrossOverPoint);
		this.m_Material.SetVector("_Toe", this.m_MainCurve.m_ToeCoef);
		this.m_Material.SetVector("_Shoulder", this.m_MainCurve.m_ShoulderCoef);
		this.m_Material.SetVector("_Tint", this.m_Tint);
		this.m_Material.SetFloat("_LuminosityWhite", this.m_MainCurve.m_LuminositySaturationPoint * this.m_MainCurve.m_LuminositySaturationPoint);
	}

	public void ReloadShaders()
	{
		this.OnDisable();
	}

	private void OnEnable()
	{
		if (this.m_MainCurve == null)
		{
			this.m_MainCurve = new FilmicCurve();
		}
		this.CreateMaterials();
		this.UpdateCoefficients();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.m_Mode != this.m_LastMode)
		{
			this.ReloadShaders();
		}
		this.m_LastMode = this.m_Mode;
		this.CreateMaterials();
		this.UpdateCoefficients();
		Graphics.Blit(source, destination, this.m_Material);
	}
}
