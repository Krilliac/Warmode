using System;
using UnityEngine;

[ExecuteInEditMode]
public class glareFxCheapSM20 : MonoBehaviour
{
	public float intensity = 5f;

	public float threshold = 0.5f;

	public int blurIteration = 2;

	public Texture2D lensDirt;

	public Shader shader;

	private Material rbMaterial;

	public Shader blurShader;

	private Material m_BlurMaterial;

	public Shader compositeShader;

	private Material m_CompositeMaterial;

	protected Material blurMaterial
	{
		get
		{
			if (this.m_BlurMaterial == null)
			{
				this.m_BlurMaterial = new Material(this.blurShader);
				this.m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.m_BlurMaterial;
		}
	}

	protected Material compositeMaterial
	{
		get
		{
			if (this.m_CompositeMaterial == null)
			{
				this.m_CompositeMaterial = new Material(this.compositeShader);
				this.m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.m_CompositeMaterial;
		}
	}

	private Material GetMaterial()
	{
		if (this.rbMaterial == null)
		{
			this.rbMaterial = new Material(this.shader);
			this.rbMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		return this.rbMaterial;
	}

	protected void OnDisable()
	{
		if (this.m_CompositeMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.m_CompositeMaterial);
		}
		if (this.m_BlurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.m_BlurMaterial);
		}
	}

	private void Start()
	{
		if (this.shader == null)
		{
			Debug.LogError("No glare shader assigned!", this);
			base.enabled = false;
		}
		if (this.blurShader == null)
		{
			Debug.LogError("No blur shader assigned!", this);
			base.enabled = false;
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest)
	{
		float num = 0.75f;
		Graphics.BlitMultiTap(source, dest, this.blurMaterial, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, this.blurMaterial, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		this.threshold = Mathf.Clamp01(this.threshold);
		this.intensity = Mathf.Clamp(this.intensity, 0f, 10f);
		this.GetMaterial().SetFloat("_int", this.intensity);
		this.GetMaterial().SetTexture("_OrgTex", source);
		this.GetMaterial().SetTexture("_lensDirt", this.lensDirt);
		this.GetMaterial().SetFloat("_threshold", this.threshold);
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		this.DownSample4x(source, temporary);
		for (int i = 0; i < this.blurIteration; i++)
		{
			this.FourTapCone(temporary, temporary);
		}
		Graphics.Blit(temporary, dest, this.GetMaterial());
		RenderTexture.ReleaseTemporary(temporary);
	}
}
