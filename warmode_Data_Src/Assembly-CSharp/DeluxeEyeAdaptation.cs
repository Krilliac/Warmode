using System;
using UnityEngine;

[ExecuteInEditMode]
public class DeluxeEyeAdaptation : MonoBehaviour
{
	[SerializeField]
	public DeluxeEyeAdaptationLogic m_Logic;

	[SerializeField]
	public bool m_LowResolution;

	[SerializeField]
	public bool m_ShowHistogram;

	[SerializeField]
	public float m_HistogramSize = 0.3f;

	private Material m_HistogramMaterial;

	private Shader m_HistogramShader;

	private Material m_BrightnessMaterial;

	private Shader m_BrightnessShader;

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

	private void DestroyMaterial(Material mat)
	{
		if (mat)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	public void OnDisable()
	{
		if (this.m_Logic != null)
		{
			this.m_Logic.ClearMeshes();
		}
		this.DestroyMaterial(this.m_HistogramMaterial);
		this.m_HistogramMaterial = null;
		this.m_HistogramShader = null;
		this.DestroyMaterial(this.m_BrightnessMaterial);
		this.m_BrightnessMaterial = null;
		this.m_BrightnessShader = null;
	}

	private void CreateMaterials()
	{
		if (this.m_HistogramShader == null)
		{
			this.m_HistogramShader = Shader.Find("Hidden/Deluxe/EyeAdaptation");
		}
		if (this.m_HistogramMaterial == null && this.m_HistogramShader != null && this.m_HistogramShader.isSupported)
		{
			this.m_HistogramMaterial = this.CreateMaterial(this.m_HistogramShader);
		}
		if (this.m_BrightnessShader == null)
		{
			this.m_BrightnessShader = Shader.Find("Hidden/Deluxe/EyeAdaptationBright");
		}
		if (this.m_BrightnessMaterial == null && this.m_BrightnessShader != null && this.m_BrightnessShader.isSupported)
		{
			this.m_BrightnessMaterial = this.CreateMaterial(this.m_BrightnessShader);
		}
		else if (this.m_BrightnessShader == null)
		{
			Debug.LogError("Cant find brightness shader");
		}
		else if (!this.m_BrightnessShader.isSupported)
		{
			Debug.LogError("Brightness shader unsupported");
		}
	}

	public void OnEnable()
	{
		this.CreateMaterials();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		if (this.m_Logic == null)
		{
			this.m_Logic = new DeluxeEyeAdaptationLogic();
		}
		DeluxeTonemapper component = base.GetComponent<DeluxeTonemapper>();
		if (component != null)
		{
			this.m_Logic.m_Range = component.m_MainCurve.m_WhitePoint;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, source.format);
		RenderTexture temporary2 = RenderTexture.GetTemporary(temporary.width / 2, temporary.height / 2, 0, source.format);
		if (this.m_BrightnessMaterial == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.m_BrightnessMaterial.SetTexture("_UpTex", source);
		source.filterMode = FilterMode.Bilinear;
		this.m_BrightnessMaterial.SetVector("_PixelSize", new Vector4(1f / (float)source.width * 0.5f, 1f / (float)source.height * 0.5f));
		Graphics.Blit(source, temporary, this.m_BrightnessMaterial, 1);
		this.m_BrightnessMaterial.SetTexture("_UpTex", temporary);
		temporary.filterMode = FilterMode.Bilinear;
		this.m_BrightnessMaterial.SetVector("_PixelSize", new Vector4(1f / (float)temporary.width * 0.5f, 1f / (float)temporary.height * 0.5f));
		Graphics.Blit(temporary, temporary2, this.m_BrightnessMaterial, 1);
		source.filterMode = FilterMode.Point;
		if (this.m_LowResolution)
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(temporary2.width / 2, temporary2.height / 2, 0, source.format);
			this.m_BrightnessMaterial.SetTexture("_UpTex", temporary2);
			temporary2.filterMode = FilterMode.Bilinear;
			this.m_BrightnessMaterial.SetVector("_PixelSize", new Vector4(1f / (float)temporary2.width * 0.5f, 1f / (float)temporary2.height * 0.5f));
			Graphics.Blit(temporary2, temporary3, this.m_BrightnessMaterial, 1);
			this.m_HistogramMaterial.SetTexture("_FrameTex", temporary3);
			this.m_Logic.ComputeExposure(temporary3.width, temporary3.height, this.m_HistogramMaterial);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		else
		{
			this.m_HistogramMaterial.SetTexture("_FrameTex", temporary2);
			this.m_Logic.ComputeExposure(temporary2.width, temporary2.height, this.m_HistogramMaterial);
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		this.m_BrightnessMaterial.SetFloat("_BrightnessMultiplier", this.m_Logic.m_BrightnessMultiplier);
		this.m_BrightnessMaterial.SetTexture("_BrightnessTex", this.m_Logic.m_BrightnessRT);
		this.m_BrightnessMaterial.SetTexture("_ColorTex", source);
		if (this.m_ShowHistogram)
		{
			RenderTexture temporary4 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
			Graphics.Blit(source, temporary4, this.m_BrightnessMaterial, 0);
			this.m_BrightnessMaterial.SetTexture("_Histogram", this.m_Logic.m_HistogramList[0]);
			this.m_BrightnessMaterial.SetTexture("_ColorTex", temporary4);
			this.m_BrightnessMaterial.SetFloat("_LuminanceRange", this.m_Logic.m_Range);
			this.m_BrightnessMaterial.SetVector("_HistogramCoefs", this.m_Logic.m_HistCoefs);
			this.m_BrightnessMaterial.SetVector("_MinMax", new Vector4(0.01f, this.m_HistogramSize, 0.01f, this.m_HistogramSize));
			this.m_BrightnessMaterial.SetFloat("_TotalPixelNumber", (float)(this.m_Logic.m_CurrentWidth * this.m_Logic.m_CurrentHeight));
			Graphics.Blit(temporary4, destination, this.m_BrightnessMaterial, 2);
			RenderTexture.ReleaseTemporary(temporary4);
		}
		else
		{
			Graphics.Blit(source, destination, this.m_BrightnessMaterial, 0);
		}
	}
}
